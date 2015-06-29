#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.IO;
using System.Linq;
using BTF2Schemas;
using Be.Stateless.BizTalk.BizMock;
using Be.Stateless.BizTalk.BizMock.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.Factory;
using Be.Stateless.BizTalk.Pipelines;
using Be.Stateless.BizTalk.Unit.Process;
using BizMock;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking.MessagingActivity
{
	[TestFixture]
	public class MessagingActivityTrackingFixture : ProcessFixture
	{
		#region Setup/Teardown

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_application = BizTalkServerGroup.Applications["BizTalk.Factory"];
			_passthruReceiveLocation = _application
				.CreateReceiveLocation<PassThruReceive>("BizTalk.Factory.RL1.Generic.BizMock.Any", 60);
			_xmlReceiveLocation = _application
				.CreateReceiveLocation<XmlReceive>("BizTalk.Factory.RL1.Generic.BizMock.XML", 60);
			_failSendPort = _application
				.CreateSendPort<AbsorbingTransmit>("BizTalk.Factory.SP1.Asbsorb.BizMock.Failed", 60)
				.SubscribesTo(ErrorReportProperties.ErrorType);
			_xmlSendPort = _application
				.CreateSendPort<XmlTransmit>("BizTalk.Factory.SP1.Generic.BizMock.XML", 60)
				.SubscribesTo(_passthruReceiveLocation, _xmlReceiveLocation);
			_application.ApplyChanges();
		}

		[SetUp]
		public void SetUp()
		{
			BizMockery.Reset(true);
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			_passthruReceiveLocation.Delete();
			_xmlReceiveLocation.Delete();
			_failSendPort.Delete();
			_xmlSendPort.Delete();
			_application.ApplyChanges();
		}

		#endregion

		[Test]
		public void ActivityTrackingSanityCheck()
		{
			var interchangeID = Activity.NewActivityId();
			var message = new BizMockMessage(@"Messages\btf2_services_header.xml");
			message.Promote(BtsProperties.InterchangeID, interchangeID);

			Submit.Request(message).To(_xmlReceiveLocation);
			Expect.One.Request.At(_xmlSendPort).Verify(new XmlMessageVerifier<btf2_services_header>());

			var process = TrackingDatabase.SingleProcess(p => p.InterchangeID == interchangeID && p.Name == DefaultProcesses.Unidentified);
			Assert.That(process.MessagingSteps.Count, Is.EqualTo(2));

			var inboundMessagingStep = process.MessagingSteps.Single(ms => ms.Status == TrackingStatus.Received);
			Assert.That(inboundMessagingStep.Name, Is.EqualTo(_xmlReceiveLocation.Name));
			Assert.That(
				inboundMessagingStep.Context.Properties.Any(
					p => p.Name == TrackingProperties.MessagingStepActivityId.Name
						&& p.Namespace == TrackingProperties.MessagingStepActivityId.Namespace
						&& p.Value == inboundMessagingStep.ActivityID));
			Assert.That(inboundMessagingStep.Message.Body, Is.EqualTo(message.ReadBodyAsString()));

			var outboundMessagingStep = process.MessagingSteps.Single(ms => ms.Status == TrackingStatus.Sent);
			Assert.That(outboundMessagingStep.Name, Is.EqualTo(_xmlSendPort.Name));
			Assert.That(
				outboundMessagingStep.Context.Properties.Any(
					p => p.Name == TrackingProperties.MessagingStepActivityId.Name
						&& p.Namespace == TrackingProperties.MessagingStepActivityId.Namespace
						&& p.Value == outboundMessagingStep.ActivityID));
			Assert.That(outboundMessagingStep.Message.Body, Is.EqualTo(message.ReadBodyAsString()));
		}

		[Test]
		[Ignore("TODO")]
		public void BusinessKeyValuesAreCapturedByTracking()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void MessageFailureUponReceiving()
		{
			var interchangeID = Activity.NewActivityId();
			var message = new BizMockMessage(@"Messages\unknown.xml");
			message.Promote(BtsProperties.InterchangeID, interchangeID);

			Submit.Request(message).To(_xmlReceiveLocation);
			Expect.One.Request.At(_failSendPort);

			var messagingStep = TrackingDatabase.SingleMessagingStep(ms => ms.InterchangeID == interchangeID);
			Assert.That(messagingStep.ErrorCode, Is.Not.Empty);
			Assert.That(messagingStep.ErrorDescription, Is.Not.Empty);
			Assert.That(messagingStep.Name, Is.EqualTo(_xmlReceiveLocation.Name));
			Assert.That(messagingStep.Processes.Count, Is.EqualTo(0));
			Assert.That(messagingStep.Status, Is.EqualTo(TrackingStatus.FailedMessage));

			Assert.That(messagingStep.Message.Body, Is.EqualTo(File.ReadAllText(@"Messages\unknown.xml")));
			Assert.That(messagingStep.Context.GetProperty(BtsProperties.SendPortName).Value, Is.EqualTo(_failSendPort.Name));
		}

		[Test]
		[Ignore("TODO")]
		public void MessageFailureUponSending()
		{
			throw new NotImplementedException();

			//var interchangeID = Activity.NewActivityId();
			//var message = new BizMockMessage(@"Messages\invalid.xml");
			//message.Promote(BtsProperties.InterchangeID, interchangeID);

			//Submit.Request(message).To(_passthruReceiveLocation);
			////Expect.One.Request.At(_xmlSendPort).Verify(".+");
			//Expect.One.Request.At(_failSendPort).Verify(".+");

			//var process = TrackingDatabase.SingleProcess(p => p.InterchangeID == interchangeID && p.Name == Processes.Failed, true);
			//Assert.That(process.MessagingSteps.Count, Is.EqualTo(2));
		}

		[Test]
		public void MonitoringModelSupportsLazyLoad()
		{
			var interchangeID = Activity.NewActivityId();
			var message = new BizMockMessage(@"Messages\btf2_services_header.xml");
			message.Promote(BtsProperties.InterchangeID, interchangeID);

			Submit.Request(message).To(_xmlReceiveLocation);
			Expect.One.Request.At(_xmlSendPort).Verify(new XmlMessageVerifier<btf2_services_header>());

			var process = TrackingDatabase.SingleProcess(p => p.InterchangeID == interchangeID && p.Name == DefaultProcesses.Unidentified, true);
			Assert.That(process.MessagingSteps.Count, Is.EqualTo(2));

			var inboundMessagingStep = process.MessagingSteps.Single(ms => ms.Status == TrackingStatus.Received);
			Assert.That(inboundMessagingStep.Name, Is.EqualTo(_xmlReceiveLocation.Name));
			Assert.That(inboundMessagingStep.Context.Properties.Any());
			Assert.That(inboundMessagingStep.Message.Body, Is.Not.Empty);

			var outboundMessagingStep = process.MessagingSteps.Single(ms => ms.Status == TrackingStatus.Sent);
			Assert.That(outboundMessagingStep.Name, Is.EqualTo(_xmlSendPort.Name));
			Assert.That(outboundMessagingStep.Context.Properties.Any());
			Assert.That(outboundMessagingStep.Message.Body, Is.Not.Empty);
		}

		[Test]
		[Ignore("TODO")]
		public void ProcessBatchOfMessages()
		{
			throw new NotImplementedException();
		}

		[Test]
		public void ProcessSingleMessage()
		{
			const string filePath = @"Messages\btf2_services_header.xml";

			var interchangeID = Activity.NewActivityId();
			var message = new BizMockMessage(filePath);
			message.Promote(BtsProperties.InterchangeID, interchangeID);

			Submit.Request(message).To(_xmlReceiveLocation);
			Expect.One.Request.At(_xmlSendPort).Verify(new XmlMessageVerifier<btf2_services_header>());

			var process = TrackingDatabase.SingleProcess(p => p.InterchangeID == interchangeID && p.Name == DefaultProcesses.Unidentified);
			Assert.That(process.MessagingSteps.Count, Is.EqualTo(2));

			var inboundMessagingStep = process.MessagingSteps.Single(ms => ms.Status == TrackingStatus.Received);
			Assert.That(inboundMessagingStep.Name, Is.EqualTo(_xmlReceiveLocation.Name));
			Assert.That(inboundMessagingStep.MessageSize, Is.EqualTo(new FileInfo(filePath).Length));
			Assert.That(inboundMessagingStep.MessageType, Is.EqualTo(Schema<btf2_services_header>.MessageType));

			var outboundMessagingStep = process.MessagingSteps.Single(ms => ms.Status == TrackingStatus.Sent);
			Assert.That(outboundMessagingStep.Name, Is.EqualTo(_xmlSendPort.Name));
			Assert.That(outboundMessagingStep.MessageSize, Is.EqualTo(new FileInfo(filePath).Length));
			Assert.That(outboundMessagingStep.MessageType, Is.EqualTo(Schema<btf2_services_header>.MessageType));
		}

		private Application _application;
		private SendPort _failSendPort;
		private ReceiveLocation _passthruReceiveLocation;
		private ReceiveLocation _xmlReceiveLocation;
		private SendPort _xmlSendPort;
	}
}

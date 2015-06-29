#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Unit.ServiceModel.Extensions;
using Be.Stateless.IO;
using BTF2Schemas;
using Microsoft.BizTalk.Component.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	[StubServiceHostActivator]
	[TestFixture]
	public class StubServiceFixture
	{
		[Test]
		public void CannotSetupExpectationAgainstNonServiceContract()
		{
			Assert.That(
				() => StubServiceHost.FindDefaultService<IDisposable>()
					.Setup(s => s.Dispose())
					.Aborts(),
				Throws.TypeOf<ArgumentException>()
					.With.Message.EqualTo("TContract type parameter 'IDisposable' is not a service contract."));
		}

		[Test]
		public void SetupCallbackExpectationAgainstVoidOperation()
		{
			var calledBack = false;
			StubServiceHost.FindDefaultService<IWork>()
				.Setup(s => s.Execute(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Callback(() => calledBack = true);

			var client = StubServiceClient<IWork>.Create();
			try
			{
				client.Execute(
					System.ServiceModel.Channels.Message.CreateMessage(
						MessageVersion.Soap11,
						"urn:services.stateless.be:unit:work:execute:request",
						XmlReader.Create(new StringReader("<request />"))));
				client.Close();
			}
			catch (Exception)
			{
				client.Abort();
				throw;
			}
			Assert.That(calledBack, Is.True);
		}

		[Test]
		public void SetupFailureExpectationAgainstAction()
		{
			StubServiceHost.FindDefaultService<IWork>()
				.Setup(s => s.Perform(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Aborts();

			var client = StubServiceClient<IWork>.Create();
			Assert.That(
				() => client.Perform(
					System.ServiceModel.Channels.Message.CreateMessage(
						MessageVersion.Soap11,
						"urn:services.stateless.be:unit:work:perform:request",
						XmlReader.Create(new StringReader("<request />")))),
				Throws.TypeOf<CommunicationException>());
			client.Abort();
		}

		[Test]
		public void SetupFailureExpectationAgainstMessageType()
		{
			StubServiceHost.DefaultService
				.Setup(s => s.Request(new SchemaMetadata<btf2_services_header>().DocumentSpec))
				.Aborts();

			var client = StubServiceClient.Create();
			Assert.That(
				() => client.Request(
					System.ServiceModel.Channels.Message.CreateMessage(
						MessageVersion.Soap11,
						"urn:services.stateless.be:unit:work:request",
						XmlReader.Create(new StringReader(MessageFactory.CreateMessage<btf2_services_header>().OuterXml)))),
				Throws.TypeOf<CommunicationException>());
			client.Abort();
		}

		[Test]
		public void SetupFailureExpectationAgainstVoidOperation()
		{
			StubServiceHost.FindDefaultService<IWork>()
				.Setup(s => s.Execute(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Aborts();

			var client = StubServiceClient<IWork>.Create();
			Assert.That(
				() => client.Execute(
					System.ServiceModel.Channels.Message.CreateMessage(
						MessageVersion.Soap11,
						"urn:services.stateless.be:unit:work:execute:request",
						XmlReader.Create(new StringReader("<request />")))),
				Throws.TypeOf<CommunicationException>());
			client.Abort();
		}

		[Test]
		public void SetupResponseExpectationAgainstAction()
		{
			StubServiceHost.FindDefaultService<IWork>()
				.Setup(s => s.Perform(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Returns(new StringStream("<response />"));

			var client = StubServiceClient<IWork>.Create();
			try
			{
				var result = client.Perform(
					System.ServiceModel.Channels.Message.CreateMessage(
						MessageVersion.Soap11,
						"urn:services.stateless.be:unit:work:perform:request",
						XmlReader.Create(new StringReader("<request />"))));

				var reader = result.GetReaderAtBodyContents();
				reader.MoveToContent();
				var outerXml = reader.ReadOuterXml();
				Assert.That(outerXml, Is.EqualTo("<response />"));

				client.Close();
			}
			catch (Exception)
			{
				client.Abort();
				throw;
			}
		}

		[Test]
		public void SetupResponseExpectationAgainstAnyMessageType()
		{
			Assert.That(
				() => StubServiceHost.DefaultService
					.Setup(s => s.Request(It.IsAny<DocumentSpec>()))
					.Returns(new StringStream("<response />")),
				Throws.ArgumentException.With.Message.EqualTo("Can only setup a response for a valid and non-null DocumentSpec."));
		}

		[Test]
		public void SetupResponseExpectationAgainstSpecificMessageType()
		{
			StubServiceHost.DefaultService
				.Setup(s => s.Request(new SchemaMetadata<btf2_services_header>().DocumentSpec))
				.Returns(new StringStream("<response />"));

			var client = StubServiceClient.Create();
			try
			{
				var response = client.Request(
					System.ServiceModel.Channels.Message.CreateMessage(
						MessageVersion.Soap11,
						"urn:services.stateless.be:unit:work:request",
						XmlReader.Create(new StringReader(MessageFactory.CreateMessage<btf2_services_header>().OuterXml))));

				// ReSharper disable PossibleNullReferenceException
				var reader = response.GetReaderAtBodyContents();
				// ReSharper restore PossibleNullReferenceException
				reader.MoveToContent();
				var outerXml = reader.ReadOuterXml();
				Assert.That(outerXml, Is.EqualTo("<response />"));

				client.Close();
			}
			catch (Exception)
			{
				client.Abort();
				throw;
			}
		}

		// ReSharper disable once UnusedMember.Local
		private void EnsureSetupExpressionsCompile()
		{
			StubServiceHost.FindDefaultService<IWork>()
				.Setup(s => s.Execute(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Callback(null)
				.Aborts();

			StubServiceHost.FindDefaultService<IWork>()
				.Setup(s => s.Perform(It.IsAny<System.ServiceModel.Channels.Message>()))
				.Callback(null)
				.Returns("file");

			StubServiceHost.DefaultService
				.Setup(s => s.Request(new DocumentSpec("s", "a")))
				.Callback(null)
				.Aborts();

			StubServiceHost.DefaultService
				.Setup(s => s.Request(new DocumentSpec("s", "a")))
				.Callback(null)
				.Returns("file");
		}

		[ServiceContract]
		internal interface IWork
		{
			[OperationContract(Action = "urn:services.stateless.be:unit:work:execute:request", ReplyAction = "urn:services.stateless.be:unit:work:execute:request/response")]
			void Execute(System.ServiceModel.Channels.Message message);

			[OperationContract(Action = "urn:services.stateless.be:unit:work:perform:request", ReplyAction = "urn:services.stateless.be:unit:work:perform:request/response")]
			System.ServiceModel.Channels.Message Perform(System.ServiceModel.Channels.Message message);
		}
	}
}

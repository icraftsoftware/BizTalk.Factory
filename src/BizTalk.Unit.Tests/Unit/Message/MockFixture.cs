#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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

using System.IO;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Message
{
	[TestFixture]
	public class MockFixture
	{
		[Test]
		public void MockBodyPartDataAndOriginalDataStream()
		{
			const string content = "<s1:letter xmlns:s1='urn-one'>" +
				"<s1:headers><s1:subject>inquiry</s1:subject></s1:headers>" +
				"<s1:body><s1:paragraph>paragraph-one</s1:paragraph></s1:body>" +
				"</s1:letter>";
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
			{
				var msg = new Mock<IBaseMessage> { DefaultValue = DefaultValue.Mock };
				msg.Object.BodyPart.Data = inputStream;

				Assert.That(msg.Object.BodyPart.GetOriginalDataStream(), Is.SameAs(inputStream));
			}
		}

		[Test]
		public void MockEnforcesDefaultValueEmptyForNestedContextMock()
		{
			var errorType = ErrorReportProperties.ErrorType;
			var inboundTransportLocation = BtsProperties.InboundTransportLocation;

			var msg = new Mock<IBaseMessage> { DefaultValue = DefaultValue.Mock };

			Assert.That(Mock.Get(msg.Object.Context).DefaultValue, Is.EqualTo(DefaultValue.Empty));
			Assert.That(msg.Object.Context.Read(errorType.Name, errorType.Namespace), Is.Null);
			Assert.That(msg.Object.GetProperty(errorType), Is.Null);

			msg.Setup(m => m.GetProperty(inboundTransportLocation)).Returns("inbound-transport-location");

			Assert.That(Mock.Get(msg.Object.Context).DefaultValue, Is.EqualTo(DefaultValue.Empty));
			Assert.That(msg.Object.Context.Read(errorType.Name, errorType.Namespace), Is.Null);
			Assert.That(msg.Object.GetProperty(errorType), Is.Null);
			Assert.That(msg.Object.Context.Read(inboundTransportLocation.Name, inboundTransportLocation.Namespace), Is.EqualTo("inbound-transport-location"));
			Assert.That(msg.Object.GetProperty(inboundTransportLocation), Is.EqualTo("inbound-transport-location"));

			msg.Verify(m => m.GetProperty(inboundTransportLocation));

			Assert.That(Mock.Get(msg.Object.Context).DefaultValue, Is.EqualTo(DefaultValue.Empty));
		}

		[Test]
		public void MockPartiallySupportsSetupOfIBaseMessageIsPromotedExtensionMethod()
		{
			var message = new Mock<IBaseMessage>();
			message
				.Setup(m => m.IsPromoted(BtsProperties.ActualRetryCount))
				.Returns(true)
				.Verifiable();
			message
				.Setup(m => m.IsPromoted(BtsProperties.AckRequired))
				.Returns(true)
				.Verifiable();
			message
				.Setup(m => m.IsPromoted(BtsProperties.SendPortName))
				.Returns(true)
				.Verifiable();

			// IsPromoted returns false unless actual values have been promoted, see also MockSupportsSetupOfIBaseMessagePromoteExtensionMethod
			Assert.That(message.Object.IsPromoted(BtsProperties.ActualRetryCount), Is.False);
			Assert.That(message.Object.IsPromoted(BtsProperties.AckRequired), Is.False);
			Assert.That(message.Object.IsPromoted(BtsProperties.SendPortName), Is.False);

			var contextMock = Mock.Get(message.Object.Context);
			Assert.That(contextMock.Object.IsPromoted(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace), Is.True);
			Assert.That(contextMock.Object.IsPromoted(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace), Is.True);
			Assert.That(contextMock.Object.IsPromoted(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace), Is.True);

			message.Verify();
		}

		[Test]
		public void MockSupportsDefaultValueEmpty()
		{
			var message = new Mock<IBaseMessage> { DefaultValue = DefaultValue.Empty };
			message.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			Assert.That(message.Object.Direction().IsInbound());
		}

		[Test]
		public void MockSupportsMissingSetupOfIBaseMessageGetPropertyExtensionMethod()
		{
			var message = new Mock<IBaseMessage>();

			Assert.That(message.Object.GetProperty(ErrorReportProperties.ErrorType), Is.Null);
			Assert.That(message.Object.HasFailed(), Is.False);
		}

		[Test]
		public void MockSupportsSetupOfIBaseMessageGetPropertyExtensionMethod()
		{
			var message = new Mock<IBaseMessage>();
			message
				.Setup(m => m.GetProperty(BtsProperties.ActualRetryCount))
				.Returns(10);
			message
				.Setup(m => m.GetProperty(BtsProperties.AckRequired))
				.Returns(true);
			message
				.Setup(m => m.GetProperty(BtsProperties.SendPortName))
				.Returns("send-port-name");

			Assert.That(message.Object.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(message.Object.GetProperty(BtsProperties.AckRequired), Is.EqualTo(true));
			Assert.That(message.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));

			Assert.That(message.Object.GetProperty(ErrorReportProperties.ErrorType), Is.Null);
		}

		[Test]
		public void MockSupportsSetupOfIBaseMessagePromoteExtensionMethod()
		{
			var message = new Mock<IBaseMessage>();
			message
				.Setup(m => m.Promote(BtsProperties.ActualRetryCount, 10))
				.Verifiable();
			message
				.Setup(m => m.Promote(BtsProperties.AckRequired, true))
				.Verifiable();
			message
				.Setup(m => m.Promote(BtsProperties.SendPortName, "send-port-name"))
				.Verifiable();

			message.Object.Promote(BtsProperties.ActualRetryCount, 10);
			message.Object.Promote(BtsProperties.AckRequired, true);
			message.Object.Promote(BtsProperties.SendPortName, "send-port-name");

			message.Verify();

			// IsPromoted returns true as well because actual values have been promoted, see also MockPartiallySupportsSetupOfIBaseMessageIsPromotedExtensionMethod
			Assert.That(message.Object.IsPromoted(BtsProperties.ActualRetryCount), Is.True);
			Assert.That(message.Object.IsPromoted(BtsProperties.AckRequired), Is.True);
			Assert.That(message.Object.IsPromoted(BtsProperties.SendPortName), Is.True);
		}

		[Test]
		public void MockSupportsSetupOfIBaseMessageRegularMethodsAndProperties()
		{
			var message = new Mock<IBaseMessage>();
			message
				.Setup(m => m.BodyPart.Data)
				.Returns(new MemoryStream());
			message
				.Setup(m => m.GetPart("part"))
				.Returns((IBaseMessagePart) null);

			Assert.That(message.Object.BodyPart.Data, Is.TypeOf<MemoryStream>());
			Assert.That(message.Object.GetPart("part"), Is.Null);
		}

		[Test]
		public void MockSupportsSetupOfIBaseMessageSetPropertyExtensionMethod()
		{
			var message = new Mock<IBaseMessage>();
			message
				.Setup(m => m.SetProperty(BtsProperties.ActualRetryCount, 10))
				.Verifiable();
			message
				.Setup(m => m.SetProperty(BtsProperties.AckRequired, true))
				.Verifiable();
			message
				.Setup(m => m.SetProperty(BtsProperties.SendPortName, "send-port-name"))
				.Verifiable();

			message.Object.SetProperty(BtsProperties.ActualRetryCount, 10);
			message.Object.SetProperty(BtsProperties.AckRequired, true);
			message.Object.SetProperty(BtsProperties.SendPortName, "send-port-name");

			message.Verify();
		}

		[Test]
		public void MockSupportsVerifiableSetupOnContext()
		{
			var message = new Mock<IBaseMessage>(MockBehavior.Strict);
			message
				.Setup(m => m.GetProperty(TrackingProperties.ProcessActivityId))
				.Returns(It.IsAny<string>())
				.Verifiable();
			message
				.Setup(m => m.GetProperty(TrackingProperties.ProcessingStepActivityId))
				.Returns(It.IsAny<string>())
				.Verifiable();

			message.Object.GetProperty(TrackingProperties.ProcessActivityId);

			Assert.That(
				() => message.Verify(),
				Throws.InstanceOf<MockException>().With.Message.EqualTo(
					string.Format(
						"The following setups were not matched:\n" + "IBaseMessage m => m.Context.Read(\"{0}\", \"{1}\")\r\n",
						TrackingProperties.ProcessingStepActivityId.Name,
						TrackingProperties.ProcessingStepActivityId.Namespace)));
		}

		[Test]
		public void MockSupportsVerifyOfIBaseMessagePromoteExtensionMethod()
		{
			var message = new Mock<IBaseMessage>();

			message.Object.Context.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			message.Object.Context.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			message.Object.Context.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			message.Verify(m => m.Promote(BtsProperties.ActualRetryCount, 10));
			message.Verify(m => m.Promote(BtsProperties.AckRequired, true));
			message.Verify(m => m.Promote(BtsProperties.SendPortName, "send-port-name"));
		}

		[Test]
		public void MockSupportsVerifyOfIBaseMessageSetPropertyExtensionMethod()
		{
			var message = new Mock<IBaseMessage>();

			message.Object.Context.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			message.Object.Context.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			message.Object.Context.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			message.Verify(m => m.SetProperty(BtsProperties.ActualRetryCount, 10));
			message.Verify(m => m.SetProperty(BtsProperties.AckRequired, true));
			message.Verify(m => m.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}
	}
}

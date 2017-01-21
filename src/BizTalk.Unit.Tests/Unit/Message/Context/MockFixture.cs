#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Message.Context
{
	[TestFixture]
	public class MockFixture
	{
		[Test]
		public void MockDefaultsContextPropertyValueToNull()
		{
			Assert.That(new Mock<IBaseMessageContext>().Object.GetProperty(BtsProperties.OutboundTransportLocation), Is.Null);
		}

		[Test]
		public void MockSupportsSetupOfIBaseMessageContextGetPropertyExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>();
			context
				.Setup(m => m.GetProperty(BtsProperties.ActualRetryCount))
				.Returns(10);
			context
				.Setup(m => m.GetProperty(BtsProperties.AckRequired))
				.Returns(true);
			context
				.Setup(m => m.GetProperty(BtsProperties.SendPortName))
				.Returns("send-port-name");

			Assert.That(context.Object.GetProperty(BtsProperties.ActualRetryCount), Is.EqualTo(10));
			Assert.That(context.Object.GetProperty(BtsProperties.AckRequired), Is.EqualTo(true));
			Assert.That(context.Object.GetProperty(BtsProperties.SendPortName), Is.EqualTo("send-port-name"));

			Assert.That(context.Object.GetProperty(ErrorReportProperties.ErrorType), Is.Null);
		}

		[Test]
		public void MockSupportsSetupOfIBaseMessageContextPromoteExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>();
			context
				.Setup(m => m.Promote(BtsProperties.ActualRetryCount, 10))
				.Verifiable();
			context
				.Setup(m => m.Promote(BtsProperties.AckRequired, true))
				.Verifiable();
			context
				.Setup(m => m.Promote(BtsProperties.SendPortName, "send-port-name"))
				.Verifiable();

			context.Object.Promote(BtsProperties.ActualRetryCount, 10);
			context.Object.Promote(BtsProperties.AckRequired, true);
			context.Object.Promote(BtsProperties.SendPortName, "send-port-name");

			context.Verify();

			// IsPromoted returns true as well because actual values have been promoted
			Assert.That(context.Object.IsPromoted(BtsProperties.ActualRetryCount), Is.True);
			Assert.That(context.Object.IsPromoted(BtsProperties.AckRequired), Is.True);
			Assert.That(context.Object.IsPromoted(BtsProperties.SendPortName), Is.True);
		}

		[Test]
		public void MockSupportsSetupOfIBaseMessageContextSetPropertyExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>();
			context
				.Setup(m => m.SetProperty(BtsProperties.ActualRetryCount, 10))
				.Verifiable();
			context
				.Setup(m => m.SetProperty(BtsProperties.AckRequired, true))
				.Verifiable();
			context
				.Setup(m => m.SetProperty(BtsProperties.SendPortName, "send-port-name"))
				.Verifiable();

			context.Object.SetProperty(BtsProperties.ActualRetryCount, 10);
			context.Object.SetProperty(BtsProperties.AckRequired, true);
			context.Object.SetProperty(BtsProperties.SendPortName, "send-port-name");

			context.Verify();
		}

		[Test]
		public void MockSupportsVerifiableSetupOnContext()
		{
			var context = new Mock<IBaseMessageContext>(MockBehavior.Strict);
			context
				.Setup(m => m.GetProperty(TrackingProperties.ProcessActivityId))
				.Returns(It.IsAny<string>())
				.Verifiable();
			context
				.Setup(m => m.GetProperty(TrackingProperties.ProcessingStepActivityId))
				.Returns(It.IsAny<string>())
				.Verifiable();

			context.Object.GetProperty(TrackingProperties.ProcessActivityId);

			Assert.That(
				() => context.Verify(),
				Throws.InstanceOf<MockException>().With.Message.EqualTo(
					string.Format(
						"The following setups were not matched:\n" + "IBaseMessageContext ctxt => ctxt.Read(\"{0}\", \"{1}\")\r\n",
						TrackingProperties.ProcessingStepActivityId.Name,
						TrackingProperties.ProcessingStepActivityId.Namespace)));
		}

		[Test]
		public void MockSupportsVerifyOfIBaseMessageContextPromoteExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>();

			context.Object.Promote(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			context.Object.Promote(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			context.Object.Promote(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			context.Verify(m => m.Promote(BtsProperties.ActualRetryCount, 10));
			context.Verify(m => m.Promote(BtsProperties.AckRequired, true));
			context.Verify(m => m.Promote(BtsProperties.SendPortName, "send-port-name"));
		}

		[Test]
		public void MockSupportsVerifyOfIBaseMessageContextSetPropertyExtensionMethod()
		{
			var context = new Mock<IBaseMessageContext>();

			context.Object.Write(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 10);
			context.Object.Write(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace, true);
			context.Object.Write(BtsProperties.SendPortName.Name, BtsProperties.SendPortName.Namespace, "send-port-name");

			context.Verify(m => m.SetProperty(BtsProperties.ActualRetryCount, 10));
			context.Verify(m => m.SetProperty(BtsProperties.AckRequired, true));
			context.Verify(m => m.SetProperty(BtsProperties.SendPortName, "send-port-name"));
		}
	}
}

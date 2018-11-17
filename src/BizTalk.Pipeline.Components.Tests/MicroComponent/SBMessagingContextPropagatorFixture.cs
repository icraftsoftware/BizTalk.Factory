#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class SBMessagingContextPropagatorFixture : MicroPipelineComponentFixture
	{
		[Test]
		public void BizTalkPropertiesAreOnlyPropagatedOutward()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.CorrelationToken)).Returns(Guid.NewGuid().ToString);
			MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.CorrelationId, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.Label, It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void CorrelationIdIsNotPromotedInwardWhenEmpty()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Promote(BizTalkFactoryProperties.CorrelationToken, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.CorrelationToken, It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void CorrelationIdIsPromotedInward()
		{
			var token = Guid.NewGuid().ToString();
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(SBMessagingProperties.CorrelationId)).Returns(token);

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Promote(BizTalkFactoryProperties.CorrelationToken, token), Times.Once);
			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.CorrelationToken, It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void CorrelationTokenIsNotPropagatedOutwardWhenEmpty()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.CorrelationId, It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void CorrelationTokenIsPropagatedOutward()
		{
			var token = Guid.NewGuid().ToString();
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.CorrelationToken)).Returns(token);

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.CorrelationId, token), Times.Once);
		}

		[Test]
		public void LabelIsNotPromotedInwardWhenEmpty()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.SetProperty(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void LabelIsPromotedInward()
		{
			var messageType = "urn:ns#root";
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(SBMessagingProperties.Label)).Returns(messageType);

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, messageType), Times.Once);
			MessageMock.Verify(m => m.SetProperty(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void MessageTypeIsNotPropagatedOutwardWhenEmpty()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.Label, It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void MessageTypeIsPropagatedOutward()
		{
			var messageType = "urn:ns#root";
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns(messageType);

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(SBMessagingProperties.Label, messageType), Times.Once);
		}

		[Test]
		public void SBMessagingPropertiesAreOnlyPromotedInward()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(SBMessagingProperties.CorrelationId)).Returns(Guid.NewGuid().ToString);
			MessageMock.Setup(m => m.GetProperty(SBMessagingProperties.Label)).Returns("urn:ns#root");

			var sut = new SBMessagingContextPropagator();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.CorrelationToken, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.Promote(BizTalkFactoryProperties.CorrelationToken, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.SetProperty(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, It.IsAny<string>()), Times.Never);
		}
	}
}

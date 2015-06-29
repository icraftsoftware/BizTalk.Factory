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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Runtime.Caching;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Unit.Component;
using Be.Stateless.Reflection;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class ActivityContinuatorComponentFixture : PipelineComponentFixture<ActivityContinuatorComponent>
	{
		#region Setup/Teardown

		[SetUp]
		public new void SetUp()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			CacheMock = new Mock<TrackingContextCache>();
			Reflector.SetField<TrackingContextCache>("_instance", CacheMock.Object);
		}

		#endregion

		[Test]
		public void ContinuationIsCachedForSolicitResponseOutboundMessage()
		{
			var correlationId = Guid.NewGuid().ToString();

			var trackingContext = new TrackingContext {
				ProcessActivityId = Activity.NewActivityId(),
				ProcessingStepActivityId = Activity.NewActivityId(),
				MessagingStepActivityId = Activity.NewActivityId()
			};

			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.IsSolicitResponse))
				.Returns(true);
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns("file://some-folder");
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.TransmitWorkId))
				.Returns(correlationId);
			MessageMock
				.Setup(m => m.GetProperty(TrackingProperties.ProcessActivityId))
				.Returns(trackingContext.ProcessActivityId)
				.Verifiable();
			MessageMock
				.Setup(m => m.GetProperty(TrackingProperties.ProcessingStepActivityId))
				.Returns(trackingContext.ProcessingStepActivityId)
				.Verifiable();
			MessageMock
				.Setup(m => m.GetProperty(TrackingProperties.MessagingStepActivityId))
				.Returns(trackingContext.MessagingStepActivityId)
				.Verifiable();

			var sut = new ActivityContinuatorComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.Verify(
				c => c.Add(
					correlationId,
					trackingContext,
					60),
				Times.Once());
			CacheMock.Verify(
				c => c.Remove(It.IsAny<string>()),
				Times.Never());

			// verifies that TrackingContext fields have been read from message.Context
			MessageMock.Verify();
		}

		[Test]
		public void ContinuationIsRestoredForSolicitResponseInboundMessage()
		{
			var correlationId = Guid.NewGuid().ToString();

			var trackingContext = new TrackingContext {
				ProcessActivityId = Activity.NewActivityId(),
				ProcessingStepActivityId = Activity.NewActivityId(),
				MessagingStepActivityId = Activity.NewActivityId()
			};

			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.IsSolicitResponse))
				.Returns(true);
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation))
				.Returns(@"c:\filedrop\in");
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.TransmitWorkId))
				.Returns(correlationId);

			CacheMock
				.Setup(c => c.Remove(correlationId))
				.Returns(trackingContext)
				.Verifiable();

			var sut = new ActivityContinuatorComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.Verify();

			// verifies that TrackingContext fields have been restored in message.Context
			MessageMock
				.Verify(m => m.SetProperty(TrackingProperties.ProcessActivityId, trackingContext.ProcessActivityId));
			MessageMock
				.Verify(m => m.SetProperty(TrackingProperties.ProcessingStepActivityId, trackingContext.ProcessingStepActivityId));
			MessageMock
				.Verify(m => m.SetProperty(TrackingProperties.MessagingStepActivityId, trackingContext.MessagingStepActivityId));
		}

		[Test]
		public void NoTrackingContextContinuationForOneWayPort()
		{
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation))
				.Returns(@"c:\filedrop\out");

			var sut = new ActivityContinuatorComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.Verify(
				c => c.Add(
					It.IsAny<string>(),
					It.IsAny<TrackingContext>(),
					60),
				Times.Never());
			CacheMock.Verify(
				c => c.Remove(It.IsAny<string>()),
				Times.Never());
		}

		[Test]
		public void NoTrackingContextContinuationForRequestResponsePort()
		{
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.IsRequestResponse))
				.Returns(true);
			MessageMock
				.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation))
				.Returns(@"c:\filedrop\in");

			var sut = new ActivityContinuatorComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.Verify(
				c => c.Add(
					It.IsAny<string>(),
					It.IsAny<TrackingContext>(),
					60),
				Times.Never());
			CacheMock.Verify(
				c => c.Remove(It.IsAny<string>()),
				Times.Never());
		}

		private Mock<TrackingContextCache> CacheMock { get; set; }
	}
}

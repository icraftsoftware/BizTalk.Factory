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
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Runtime.Caching;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Tracking.Messaging;
using Be.Stateless.BizTalk.Unit.Component;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class ActivityTrackerComponentFixture : ActivityTrackerComponentFixtureBase<ActivityTrackerComponent> { }

	public abstract class ActivityTrackerComponentFixtureBase<T> : PipelineComponentFixture<T> where T : ActivityTrackerComponent, new()
	{
		#region Setup/Teardown

		[SetUp]
		public new void SetUp()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			var activityTrackerComponentContext = new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Body, null);

			_activityTrackerFactory = ActivityTracker.Factory;
			ActivityTrackerMock = new Mock<ActivityTracker>(activityTrackerComponentContext);
			ActivityTracker.Factory = context => ActivityTrackerMock.Object;

			_messageBodyTrackerFactory = MessageBodyTracker.Factory;
			MessageBodyTrackerMock = new Mock<MessageBodyTracker>(activityTrackerComponentContext);
			MessageBodyTracker.Factory = context => MessageBodyTrackerMock.Object;

			_trackingContextCacheInstance = TrackingContextCache.Instance;
			CacheMock = new Mock<TrackingContextCache>(MockBehavior.Strict);
			TrackingContextCache.Instance = CacheMock.Object;

			_trackingResolverFactory = TrackingResolver.Factory;
			TrackingResolverMock = new Mock<TrackingResolver>(null, MessageMock.Object);
			TrackingResolver.Factory = (policyName, message) => TrackingResolverMock.Object;
		}

		[TearDown]
		public void TearDown()
		{
			ActivityTracker.Factory = _activityTrackerFactory;
			MessageBodyTracker.Factory = _messageBodyTrackerFactory;
			TrackingContextCache.Instance = _trackingContextCacheInstance;
			TrackingResolver.Factory = _trackingResolverFactory;
		}

		#endregion

		private Mock<ActivityTracker> ActivityTrackerMock { get; set; }

		private Mock<TrackingContextCache> CacheMock { get; set; }

		private Mock<MessageBodyTracker> MessageBodyTrackerMock { get; set; }

		private Mock<TrackingResolver> TrackingResolverMock { get; set; }

		[Test]
		public void TrackingContextIsCachedForSolicitResponseOutboundMessage()
		{
			var transmitWorkId = Guid.NewGuid().ToString();

			var trackingContext = new TrackingContext {
				ProcessActivityId = ActivityId.NewActivityId(),
				ProcessingStepActivityId = ActivityId.NewActivityId(),
				MessagingStepActivityId = ActivityId.NewActivityId()
			};

			MessageMock.Setup(m => m.GetProperty(BtsProperties.IsSolicitResponse)).Returns(true);
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BtsProperties.TransmitWorkId)).Returns(transmitWorkId);
			MessageMock.Setup(m => m.GetProperty(TrackingProperties.ProcessActivityId)).Returns(trackingContext.ProcessActivityId);
			MessageMock.Setup(m => m.GetProperty(TrackingProperties.ProcessingStepActivityId)).Returns(trackingContext.ProcessingStepActivityId);
			MessageMock.Setup(m => m.GetProperty(TrackingProperties.MessagingStepActivityId)).Returns(trackingContext.MessagingStepActivityId);

			CacheMock.Setup(c => c.Add(transmitWorkId, trackingContext, 60));

			var sut = CreatePipelineComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.VerifyAll();
		}

		[Test]
		public void TrackingContextIsCachedForSolicitResponseOutboundMessageUnlessNegativeCacheDuration()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.IsSolicitResponse)).Returns(true);
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(TrackingProperties.ProcessActivityId)).Returns(ActivityId.NewActivityId());

			var sut = CreatePipelineComponent();
			sut.TrackingContextRetentionDuration = -1;
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.VerifyAll();
		}

		[Test]
		public void TrackingContextIsNotPropagatedForOneWayPort()
		{
			var sut = CreatePipelineComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.VerifyAll();
		}

		[Test]
		public void TrackingContextIsNotPropagatedForRequestResponsePort()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.IsRequestResponse)).Returns(true);

			var sut = CreatePipelineComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.VerifyAll();
		}

		[Test]
		public void TrackingContextIsRestoredForSolicitResponseInboundMessage()
		{
			var transmitWorkId = Guid.NewGuid().ToString();

			var trackingContext = new TrackingContext {
				ProcessActivityId = ActivityId.NewActivityId(),
				ProcessingStepActivityId = ActivityId.NewActivityId(),
				MessagingStepActivityId = ActivityId.NewActivityId()
			};

			MessageMock.Setup(m => m.GetProperty(BtsProperties.IsSolicitResponse)).Returns(true);
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BtsProperties.TransmitWorkId)).Returns(transmitWorkId);

			CacheMock.Setup(c => c.Remove(transmitWorkId)).Returns(trackingContext);

			var sut = CreatePipelineComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.VerifyAll();

			// verifies that TrackingContext fields have been restored in message.Context
			MessageMock.Verify(m => m.SetProperty(TrackingProperties.ProcessActivityId, trackingContext.ProcessActivityId));
			MessageMock.Verify(m => m.SetProperty(TrackingProperties.ProcessingStepActivityId, trackingContext.ProcessingStepActivityId));
			MessageMock.Verify(m => m.SetProperty(TrackingProperties.MessagingStepActivityId, trackingContext.MessagingStepActivityId));
		}

		[Test]
		public void TrackingContextIsRestoredForSolicitResponseInboundMessageUnlessNegativeCacheDuration()
		{
			var transmitWorkId = Guid.NewGuid().ToString();

			var trackingContext = new TrackingContext {
				ProcessActivityId = ActivityId.NewActivityId(),
				ProcessingStepActivityId = ActivityId.NewActivityId(),
				MessagingStepActivityId = ActivityId.NewActivityId()
			};

			MessageMock.Setup(m => m.GetProperty(BtsProperties.IsSolicitResponse)).Returns(true);
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(BtsProperties.TransmitWorkId)).Returns(transmitWorkId);

			var sut = CreatePipelineComponent();
			sut.TrackingContextRetentionDuration = -1;
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.VerifyAll();

			// verifies that TrackingContext fields have been restored in message.Context
			MessageMock.Verify(m => m.SetProperty(TrackingProperties.ProcessActivityId, trackingContext.ProcessActivityId), Times.Never());
			MessageMock.Verify(m => m.SetProperty(TrackingProperties.ProcessingStepActivityId, trackingContext.ProcessingStepActivityId), Times.Never());
			MessageMock.Verify(m => m.SetProperty(TrackingProperties.MessagingStepActivityId, trackingContext.MessagingStepActivityId), Times.Never());
		}

		[Test]
		public void TrackingModesDefaultsToBody()
		{
			var sut = CreatePipelineComponent();
			Assert.That(sut.TrackingModes, Is.EqualTo(ActivityTrackingModes.Body));
		}

		[Test]
		public void TrackingModesIsAnythingButNone()
		{
			// MockBehavior must be Strict for following test
			var activityTrackerComponentContext = new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Body, null);
			ActivityTrackerMock = new Mock<ActivityTracker>(MockBehavior.Strict, activityTrackerComponentContext);
			MessageBodyTrackerMock = new Mock<MessageBodyTracker>(MockBehavior.Strict, activityTrackerComponentContext);

			// method call ordering is important as only one of the first two methods, i.e. either TryCheckOutMessageBody()
			// or SetupCapture(), ensures a TrackingStream is setup
			MessageBodyTrackerMock.Setup(mbt => mbt.TryCheckOutMessageBody())
				.Callback(
					() => MessageBodyTrackerMock.Setup(mbt => mbt.SetupTracking())
						.Callback(
							() => ActivityTrackerMock.Setup(at => at.TrackActivity())
								.Callback(
									() => MessageBodyTrackerMock.Setup(mbt => mbt.TryCheckInMessageBody()).Verifiable())
								.Verifiable())
						.Verifiable())
				.Verifiable();

			var sut = CreatePipelineComponent();
			sut.TrackingModes = ActivityTrackingModes.Step;
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			ActivityTrackerMock.VerifyAll();
			MessageBodyTrackerMock.VerifyAll();
		}

		[Test]
		public void TrackingModesIsNone()
		{
			var sut = CreatePipelineComponent();
			sut.TrackingModes = ActivityTrackingModes.None;
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageBodyTrackerMock.Verify(mbt => mbt.SetupTracking(), Times.Never());
			MessageBodyTrackerMock.Verify(mbt => mbt.TryCheckInMessageBody(), Times.Never());
			MessageBodyTrackerMock.Verify(mbt => mbt.TryCheckOutMessageBody(), Times.Never());
		}

		private Func<ActivityTrackerComponent.Context, ActivityTracker> _activityTrackerFactory;
		private Func<ActivityTrackerComponent.Context, MessageBodyTracker> _messageBodyTrackerFactory;
		private TrackingContextCache _trackingContextCacheInstance;
		private Func<PolicyName, IBaseMessage, TrackingResolver> _trackingResolverFactory;
	}
}

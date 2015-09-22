#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.IO;
using Be.Stateless.IO.Extensions;
using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	[TestFixture]
	public class ActivityTrackerFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			MessageMock = new Unit.Message.Mock<IBaseMessage>();
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			PipelineContextMock = new Mock<IPipelineContext>();

			ProcessMock = new Mock<Process>("pseudo-process-activity-id", new Mock<EventStream>().Object);
			InitiatingMessagingStepMock = new Mock<MessagingStep>("pseudo-initiating-activity-id", new Mock<EventStream>().Object);
			MessagingStepMock = new Mock<MessagingStep>("pseudo-activity-id", new Mock<EventStream>().Object);

			ActivityFactory = new Mock<IActivityFactory>();
			ActivityFactory.Setup(af => af.CreateProcess(It.IsAny<IBaseMessage>(), It.IsAny<string>())).Returns(ProcessMock.Object);
			ActivityFactory.Setup(af => af.FindProcess(It.IsAny<TrackingContext>())).Returns(ProcessMock.Object);
			ActivityFactory.Setup(af => af.CreateMessagingStep(It.IsAny<IBaseMessage>())).Returns(MessagingStepMock.Object);
			ActivityFactory.Setup(af => af.FindMessagingStep(It.IsAny<TrackingContext>())).Returns(InitiatingMessagingStepMock.Object);

			_activityFactoryFactory = PipelineContextExtensions.ActivityFactoryFactory;
			PipelineContextExtensions.ActivityFactoryFactory = pipelineContext => ActivityFactory.Object;

			_trackingResolverFactory = TrackingResolver.Factory;
			TrackingResolverMock = new Mock<TrackingResolver>(null, MessageMock.Object);
			TrackingResolver.Factory = (policyName, message) => TrackingResolverMock.Object;
		}

		[TearDown]
		public void TearDown()
		{
			PipelineContextExtensions.ActivityFactoryFactory = _activityFactoryFactory;
			TrackingResolver.Factory = _trackingResolverFactory;
		}

		#endregion

		[Test]
		public void CompleteTrackingOfInboundMessageWithoutProcessAffiliationAfterStreamLastReadEvent()
		{
			using (var stream = new TrackingStream(new StringStream("some-content")))
			{
				MessageMock.Object.BodyPart.Data = stream;

				var sut = ActivityTracker.Create(new MicroComponent.ActivityTracker.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Step, null));
				sut.TrackActivity();
				MessageMock.Object.BodyPart.Data.Drain();

				ProcessMock.Verify(
					p => p.TrackActivity(),
					Times.Never());
				InitiatingMessagingStepMock.Verify(
					ms => ms.TrackActivity(It.IsAny<ActivityTrackingModes>(), It.IsAny<TrackingStream>()),
					Times.Never());
				ProcessMock.Verify(
					p => p.AddStep(InitiatingMessagingStepMock.Object),
					Times.Never());
				MessagingStepMock.Verify(
					ms => ms.TrackActivity(It.IsAny<ActivityTrackingModes>(), It.IsAny<TrackingStream>()),
					Times.Once());
				ProcessMock.Verify(
					p => p.AddStep(MessagingStepMock.Object),
					Times.Never());
			}
		}

		[Test]
		public void CompleteTrackingOfInboundMessageWithProcessAffiliationAfterStreamLastReadEvent()
		{
			using (var stream = new TrackingStream(new StringStream("some-content")))
			{
				MessageMock.Object.BodyPart.Data = stream;
				MessageMock.Setup(m => m.GetProperty(TrackingProperties.ProcessActivityId)).Returns(ActivityId.NewActivityId());

				var sut = ActivityTracker.Create(new MicroComponent.ActivityTracker.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Step, null));
				sut.TrackActivity();
				MessageMock.Object.BodyPart.Data.Drain();

				ProcessMock.Verify(
					p => p.TrackActivity(),
					Times.Never());
				InitiatingMessagingStepMock.Verify(
					ms => ms.TrackActivity(It.IsAny<ActivityTrackingModes>(), It.IsAny<TrackingStream>()),
					Times.Never());
				ProcessMock.Verify(
					p => p.AddStep(InitiatingMessagingStepMock.Object),
					Times.Never());
				MessagingStepMock.Verify(
					ms => ms.TrackActivity(It.IsAny<ActivityTrackingModes>(), It.IsAny<TrackingStream>()),
					Times.Once());
				ProcessMock.Verify(
					p => p.AddStep(MessagingStepMock.Object),
					Times.Once());
			}
		}

		[Test]
		public void CompleteTrackingOfOutboundMessageWithoutProcessAffiliationAfterStreamLastReadEvent()
		{
			using (var stream = new TrackingStream(new StringStream("some-content")))
			{
				MessageMock.Object.BodyPart.Data = stream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
				MessageMock.Setup(m => m.GetProperty(TrackingProperties.MessagingStepActivityId)).Returns(ActivityId.NewActivityId()); // InitiatingMessagingStep

				var sut = ActivityTracker.Create(new MicroComponent.ActivityTracker.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Step, null));
				sut.TrackActivity();
				MessageMock.Object.BodyPart.Data.Drain();

				// entails complete messaging process
				ProcessMock.Verify(
					p => p.TrackActivity(),
					Times.Once());
				InitiatingMessagingStepMock.Verify(
					ms => ms.TrackActivity(It.IsAny<ActivityTrackingModes>(), It.IsAny<TrackingStream>()),
					Times.Never());
				ProcessMock.Verify(
					p => p.AddStep(InitiatingMessagingStepMock.Object),
					Times.Once());
				MessagingStepMock.Verify(
					ms => ms.TrackActivity(It.IsAny<ActivityTrackingModes>(), It.IsAny<TrackingStream>()),
					Times.Once());
				ProcessMock.Verify(
					p => p.AddStep(MessagingStepMock.Object),
					Times.Once());
			}
		}

		[Test]
		public void CompleteTrackingOfOutboundMessageWithoutProcessAffiliationAndInitiatingMessagingStepAfterStreamLastReadEvent()
		{
			using (var stream = new TrackingStream(new StringStream("some-content")))
			{
				MessageMock.Object.BodyPart.Data = stream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");

				var sut = ActivityTracker.Create(new MicroComponent.ActivityTracker.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Step, null));
				sut.TrackActivity();
				MessageMock.Object.BodyPart.Data.Drain();

				ProcessMock.Verify(
					p => p.TrackActivity(),
					Times.Never());
				InitiatingMessagingStepMock.Verify(
					ms => ms.TrackActivity(It.IsAny<ActivityTrackingModes>(), It.IsAny<TrackingStream>()),
					Times.Never());
				ProcessMock.Verify(
					p => p.AddStep(InitiatingMessagingStepMock.Object),
					Times.Never());
				MessagingStepMock.Verify(
					ms => ms.TrackActivity(It.IsAny<ActivityTrackingModes>(), It.IsAny<TrackingStream>()),
					Times.Once());
				// entails orphan messaging step
				ProcessMock.Verify(
					p => p.AddStep(MessagingStepMock.Object),
					Times.Never());
			}
		}

		[Test]
		public void CompleteTrackingOfOutboundMessageWithProcessAffiliationAfterStreamLastReadEvent()
		{
			using (var stream = new TrackingStream(new StringStream("some-content")))
			{
				MessageMock.Object.BodyPart.Data = stream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
				MessageMock.Setup(m => m.GetProperty(TrackingProperties.ProcessActivityId)).Returns(ActivityId.NewActivityId());

				var sut = ActivityTracker.Create(new MicroComponent.ActivityTracker.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Step, null));
				sut.TrackActivity();
				MessageMock.Object.BodyPart.Data.Drain();

				ProcessMock.Verify(
					p => p.TrackActivity(),
					Times.Never());
				InitiatingMessagingStepMock.Verify(
					ms => ms.TrackActivity(It.IsAny<ActivityTrackingModes>(), It.IsAny<TrackingStream>()),
					Times.Never());
				ProcessMock.Verify(
					p => p.AddStep(InitiatingMessagingStepMock.Object),
					Times.Never());
				MessagingStepMock.Verify(
					ms => ms.TrackActivity(It.IsAny<ActivityTrackingModes>(), It.IsAny<TrackingStream>()),
					Times.Once());
				ProcessMock.Verify(
					p => p.AddStep(MessagingStepMock.Object),
					Times.Once());
			}
		}

		[Test]
		public void CreateProcessAndMessagingStepAndFindPreviousMessagingStepBeforeStreamFirstReadEvent()
		{
			var trackingContext = new TrackingContext {
				MessagingStepActivityId = ActivityId.NewActivityId()
			};

			using (var stream = new TrackingStream(new StringStream("some-content")))
			{
				MessageMock.Object.BodyPart.Data = stream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
				MessageMock.Setup(m => m.GetProperty(TrackingProperties.MessagingStepActivityId)).Returns(trackingContext.MessagingStepActivityId);

				var sut = ActivityTracker.Create(new MicroComponent.ActivityTracker.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Step, null));
				sut.TrackActivity();

				ActivityFactory.Verify(
					af => af.CreateProcess(It.IsAny<IBaseMessage>(), It.IsAny<string>()),
					Times.Once());
				ActivityFactory.Verify(
					af => af.FindProcess(It.IsAny<TrackingContext>()),
					Times.Never());
				ActivityFactory.Verify(
					af => af.FindMessagingStep(It.Is<TrackingContext>(c => c.MessagingStepActivityId == trackingContext.MessagingStepActivityId)),
					Times.Once());
				ActivityFactory.Verify(
					af => af.CreateMessagingStep(It.IsAny<IBaseMessage>()),
					Times.Once());
			}
		}

		[Test]
		public void FindProcessAndCreateMessagingStepBeforeStreamFirstReadEvent()
		{
			var trackingContext = new TrackingContext {
				ProcessActivityId = ActivityId.NewActivityId(),
			};

			using (var stream = new TrackingStream(new StringStream("some-content")))
			{
				MessageMock.Object.BodyPart.Data = stream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
				MessageMock.Setup(m => m.GetProperty(TrackingProperties.ProcessActivityId)).Returns(trackingContext.ProcessActivityId);
				MessageMock.Setup(m => m.GetProperty(TrackingProperties.MessagingStepActivityId)).Returns(trackingContext.MessagingStepActivityId);

				var sut = ActivityTracker.Create(new MicroComponent.ActivityTracker.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Step, null));
				sut.TrackActivity();

				ActivityFactory.Verify(
					af => af.CreateProcess(It.IsAny<IBaseMessage>(), It.IsAny<string>()),
					Times.Never());
				ActivityFactory.Verify(
					af => af.FindProcess(It.Is<TrackingContext>(c => c.ProcessActivityId == trackingContext.ProcessActivityId)),
					Times.Once());
				ActivityFactory.Verify(
					af => af.CreateMessagingStep(It.IsAny<IBaseMessage>()),
					Times.Once());
				ActivityFactory.Verify(
					af => af.FindMessagingStep(It.IsAny<TrackingContext>()),
					Times.Never());
			}
		}

		[Test]
		public void ResolveProcessNameBeforeStreamFirstReadEvent()
		{
			using (var stream = new TrackingStream(new StringStream("some-content")))
			{
				MessageMock.Object.BodyPart.Data = stream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");

				var sut = ActivityTracker.Create(new MicroComponent.ActivityTracker.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Step, null));
				sut.TrackActivity();

				TrackingResolverMock.Verify(tr => tr.ResolveProcessName(), Times.Once());
			}
		}

		private Mock<IActivityFactory> ActivityFactory { get; set; }

		private Mock<MessagingStep> InitiatingMessagingStepMock { get; set; }

		private Unit.Message.Mock<IBaseMessage> MessageMock { get; set; }

		private Mock<MessagingStep> MessagingStepMock { get; set; }

		private Mock<IPipelineContext> PipelineContextMock { get; set; }

		private Mock<Process> ProcessMock { get; set; }

		private Mock<TrackingResolver> TrackingResolverMock { get; set; }

		private Func<IPipelineContext, IActivityFactory> _activityFactoryFactory;
		private Func<PolicyName, IBaseMessage, TrackingResolver> _trackingResolverFactory;
	}
}

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
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Runtime.Caching;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Tracking.Messaging;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.IO;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class ActivityTrackerFixture : MicroPipelineComponentFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			var activityTrackerContext = new ActivityTracker.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Body, null);

			_activityTrackerFactory = Tracking.Messaging.ActivityTracker.Factory;
			ActivityTrackerMock = new Mock<Tracking.Messaging.ActivityTracker>(activityTrackerContext);
			Tracking.Messaging.ActivityTracker.Factory = context => ActivityTrackerMock.Object;

			_messageBodyTrackerFactory = MessageBodyTracker.Factory;
			MessageBodyTrackerMock = new Mock<MessageBodyTracker>(activityTrackerContext);
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
			Tracking.Messaging.ActivityTracker.Factory = _activityTrackerFactory;
			MessageBodyTracker.Factory = _messageBodyTrackerFactory;
			TrackingContextCache.Instance = _trackingContextCacheInstance;
			TrackingResolver.Factory = _trackingResolverFactory;
		}

		#endregion

		[Test]
		public void Deserialize()
		{
			var microPipelineComponentType = typeof(ActivityTracker);
			var xml = string.Format(
				"<mComponent name=\"{0}\"><TrackingContextCacheDuration>00:02:00</TrackingContextCacheDuration></mComponent>",
				microPipelineComponentType.AssemblyQualifiedName);
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				var microPipelineComponent = reader.DeserializeMicroPipelineComponent();
				Assert.That(((ActivityTracker) microPipelineComponent).TrackingContextCacheDuration, Is.EqualTo(TimeSpan.FromMinutes(2)));
				Assert.That(reader.EOF);
			}
		}

		[Test]
		public void Serialize()
		{
			var component = new ActivityTracker();
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				component.Serialize(writer);
			}
			Assert.That(
				builder.ToString(),
				Is.EqualTo(
					string.Format(
						"<mComponent name=\"{0}\"><TrackingContextCacheDuration>00:01:00</TrackingContextCacheDuration><TrackingModes>Body</TrackingModes><TrackingResolutionPolicy /></mComponent>",
						component.GetType().AssemblyQualifiedName)));
		}

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

			CacheMock.Setup(c => c.Set(transmitWorkId, trackingContext, 60 + 1));

			var sut = CreateActivityTracker();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.VerifyAll();
		}

		[Test]
		public void TrackingContextIsCachedForSolicitResponseOutboundMessageUnlessNegativeCacheDuration()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.IsSolicitResponse)).Returns(true);
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			MessageMock.Setup(m => m.GetProperty(TrackingProperties.ProcessActivityId)).Returns(ActivityId.NewActivityId());

			var sut = CreateActivityTracker();
			sut.TrackingContextCacheDuration = TimeSpan.FromSeconds(-1);

			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.VerifyAll();
		}

		[Test]
		public void TrackingContextIsNotPropagatedForOneWayPort()
		{
			var sut = CreateActivityTracker();

			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			CacheMock.VerifyAll();
		}

		[Test]
		public void TrackingContextIsNotPropagatedForRequestResponsePort()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.IsRequestResponse)).Returns(true);

			var sut = CreateActivityTracker();

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

			CacheMock.Setup(c => c.Get(transmitWorkId)).Returns(trackingContext);

			var sut = CreateActivityTracker();
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

			var sut = CreateActivityTracker();
			sut.TrackingContextCacheDuration = TimeSpan.FromSeconds(-1);

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
			Assert.That(CreateActivityTracker().TrackingModes, Is.EqualTo(ActivityTrackingModes.Body));
		}

		[Test]
		public void TrackingModesIsAnythingButNone()
		{
			// MockBehavior must be Strict for following test
			var activityTrackerComponentContext = new ActivityTracker.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Body, null);
			ActivityTrackerMock = new Mock<Tracking.Messaging.ActivityTracker>(MockBehavior.Strict, activityTrackerComponentContext);
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

			var sut = CreateActivityTracker();
			sut.TrackingModes = ActivityTrackingModes.Step;

			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			ActivityTrackerMock.VerifyAll();
			MessageBodyTrackerMock.VerifyAll();
		}

		[Test]
		public void TrackingModesIsNone()
		{
			var sut = CreateActivityTracker();
			sut.TrackingModes = ActivityTrackingModes.None;

			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageBodyTrackerMock.Verify(mbt => mbt.SetupTracking(), Times.Never());
			MessageBodyTrackerMock.Verify(mbt => mbt.TryCheckInMessageBody(), Times.Never());
			MessageBodyTrackerMock.Verify(mbt => mbt.TryCheckOutMessageBody(), Times.Never());
		}

		protected virtual ActivityTracker CreateActivityTracker()
		{
			return new ActivityTracker();
		}

		private Mock<Tracking.Messaging.ActivityTracker> ActivityTrackerMock { get; set; }

		private Mock<TrackingContextCache> CacheMock { get; set; }

		private Mock<MessageBodyTracker> MessageBodyTrackerMock { get; set; }

		private Mock<TrackingResolver> TrackingResolverMock { get; set; }

		private Func<ActivityTracker.Context, Tracking.Messaging.ActivityTracker> _activityTrackerFactory;
		private Func<ActivityTracker.Context, MessageBodyTracker> _messageBodyTrackerFactory;
		private TrackingContextCache _trackingContextCacheInstance;
		private Func<PolicyName, IBaseMessage, TrackingResolver> _trackingResolverFactory;
	}
}

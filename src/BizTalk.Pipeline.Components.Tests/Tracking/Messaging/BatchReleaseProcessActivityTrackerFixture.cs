#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using System.Collections.Generic;
using System.Linq;
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	[TestFixture]
	public class BatchReleaseProcessActivityTrackerFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			MessageMock = new Unit.Message.Mock<IBaseMessage>();
			MessageMock.Setup(m => m.GetProperty(TrackingProperties.MessagingStepActivityId)).Returns(ActivityId.NewActivityId());

			PipelineContextMock = new Mock<IPipelineContext>();

			ProcessMock = new Mock<BatchReleaseProcess>("pseudo-process-activity-id", new Mock<EventStream>().Object);

			ActivityFactory = new Mock<IBatchProcessActivityFactory>();
			ActivityFactory.Setup(af => af.CreateProcess(It.IsAny<IBaseMessage>(), It.IsAny<string>())).Returns(ProcessMock.Object);
			ActivityFactory.Setup(af => af.FindProcess(It.IsAny<string>())).Returns(ProcessMock.Object);

			_activityFactoryFactory = PipelineContextExtensions.ActivityFactoryFactory;
			PipelineContextExtensions.ActivityFactoryFactory = pipelineContext => ActivityFactory.As<IActivityFactory>().Object;
		}

		[TearDown]
		public void TearDown()
		{
			PipelineContextExtensions.ActivityFactoryFactory = _activityFactoryFactory;
		}

		#endregion

		[Test]
		public void TrackActivityWhenBatchTrackingContextIsEmpty()
		{
			var sut = BatchReleaseProcessActivityTracker.Create(PipelineContextMock.Object, MessageMock.Object);
			sut.TrackActivity(new BatchTrackingContext());

			ActivityFactory.Verify(af => af.CreateProcess(It.IsAny<IBaseMessage>(), It.IsAny<string>()), Times.Never());
			ActivityFactory.Verify(af => af.FindProcess(It.IsAny<string>()), Times.Never());

			ProcessMock.Verify(p => p.TrackActivity(), Times.Never());
			ProcessMock.Verify(p => p.AddSteps(It.IsAny<IEnumerable<string>>()), Times.Never());
		}

		[Test]
		public void TrackActivityWhenBatchTrackingContextIsNull()
		{
			var sut = BatchReleaseProcessActivityTracker.Create(PipelineContextMock.Object, MessageMock.Object);
			sut.TrackActivity(null);

			ActivityFactory.Verify(af => af.CreateProcess(It.IsAny<IBaseMessage>(), It.IsAny<string>()), Times.Never());
			ActivityFactory.Verify(af => af.FindProcess(It.IsAny<string>()), Times.Never());

			ProcessMock.Verify(p => p.TrackActivity(), Times.Never());
			ProcessMock.Verify(p => p.AddSteps(It.IsAny<IEnumerable<string>>()), Times.Never());
		}

		[Test]
		public void TrackActivityWhenBatchTrackingContextOnlyHasProcessActivityId()
		{
			var batchTrackingContext = new BatchTrackingContext { ProcessActivityId = ActivityId.NewActivityId() };

			var sut = BatchReleaseProcessActivityTracker.Create(PipelineContextMock.Object, MessageMock.Object);
			sut.TrackActivity(batchTrackingContext);

			ActivityFactory.Verify(af => af.CreateProcess(It.IsAny<IBaseMessage>(), It.IsAny<string>()), Times.Never());
			ActivityFactory.Verify(af => af.FindProcess(It.IsAny<string>()), Times.Never());

			ProcessMock.Verify(p => p.TrackActivity(), Times.Never());
			ProcessMock.Verify(p => p.AddSteps(It.IsAny<IEnumerable<string>>()), Times.Never());
		}

		[Test]
		public void TrackActivityWithAmbientProcessActivity()
		{
			var batchTrackingContext = new BatchTrackingContext {
				MessagingStepActivityIdList = new[] { ActivityId.NewActivityId(), ActivityId.NewActivityId(), ActivityId.NewActivityId() },
				ProcessActivityId = ActivityId.NewActivityId()
			};

			var sut = BatchReleaseProcessActivityTracker.Create(PipelineContextMock.Object, MessageMock.Object);
			sut.TrackActivity(batchTrackingContext);

			ActivityFactory.Verify(af => af.CreateProcess(It.IsAny<IBaseMessage>(), It.IsAny<string>()), Times.Never());
			// ReSharper disable once ImplicitlyCapturedClosure
			ActivityFactory.Verify(af => af.FindProcess(batchTrackingContext.ProcessActivityId), Times.Once());

			ProcessMock.Verify(p => p.TrackActivity(), Times.Once());
			ProcessMock.Verify(
				p =>
					p.AddSteps(
						It.Is<IEnumerable<string>>(
							list =>
								list.SequenceEqual(
									batchTrackingContext.MessagingStepActivityIdList
										.Concat(new[] { MessageMock.Object.GetProperty(TrackingProperties.MessagingStepActivityId) })))),
				Times.Once());
		}

		[Test]
		public void TrackActivityWithoutAmbientProcessActivity()
		{
			var batchTrackingContext = new BatchTrackingContext {
				MessagingStepActivityIdList = new[] { ActivityId.NewActivityId(), ActivityId.NewActivityId(), ActivityId.NewActivityId() }
			};

			var sut = BatchReleaseProcessActivityTracker.Create(PipelineContextMock.Object, MessageMock.Object);
			sut.TrackActivity(batchTrackingContext);

			ActivityFactory.Verify(af => af.CreateProcess(MessageMock.Object, It.IsAny<string>()), Times.Once());
			ActivityFactory.Verify(af => af.FindProcess(It.IsAny<string>()), Times.Never());

			ProcessMock.Verify(p => p.TrackActivity(), Times.Once());
			ProcessMock.Verify(
				p =>
					p.AddSteps(
						It.Is<IEnumerable<string>>(
							list =>
								list.SequenceEqual(
									batchTrackingContext.MessagingStepActivityIdList
										.Concat(new[] { MessageMock.Object.GetProperty(TrackingProperties.MessagingStepActivityId) })))),
				Times.Once());
		}

		private Mock<IBatchProcessActivityFactory> ActivityFactory { get; set; }

		private Unit.Message.Mock<IBaseMessage> MessageMock { get; set; }

		private Mock<IPipelineContext> PipelineContextMock { get; set; }

		private Mock<BatchReleaseProcess> ProcessMock { get; set; }

		private Func<IPipelineContext, IActivityFactory> _activityFactoryFactory;
	}
}

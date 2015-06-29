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
using System.Collections.Generic;
using System.Linq;
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
	public class ProcessFixture
	{
		[Test]
		public void ActivityIsBegunAndActivityIdWrittenInMessageContext()
		{
			var eventStream = new Mock<EventStream>();
			var pipelineContext = new Mock<IPipelineContext>();
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var message = new Unit.Message.Mock<IBaseMessage>();

			var sut = new Process(pipelineContext.Object, message.Object, "process-name");
			var activityId = sut.ActivityId;

			message.Verify(m => m.SetProperty(TrackingProperties.ProcessActivityId, activityId), Times.Once());
			eventStream.Verify(s => s.BeginActivity(Process.ActivityName, activityId), Times.Once());
		}

		[Test]
		public void ActivityIsCommittedAndEnded()
		{
			var eventStream = new Mock<EventStream>();
			var pipelineContext = new Mock<IPipelineContext>();
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var message = new Unit.Message.Mock<IBaseMessage>();

			var sut = new Process(pipelineContext.Object, message.Object, "process-name");
			var activityId = sut.ActivityId;
			sut.TrackActivity();

			eventStream.Verify(s => s.UpdateActivity(Process.ActivityName, activityId, It.IsAny<object[]>()), Times.Once());
			eventStream.Verify(s => s.Flush(), Times.Once());
			eventStream.Verify(s => s.EndActivity(Process.ActivityName, activityId), Times.Once());
		}

		[Test]
		public void MessagingStepIsAffiliatedToProcess()
		{
			Dictionary<string, object> data = null;
			var processMessagingStepActivityId = string.Empty;
			var eventStream = new Mock<EventStream>();
			eventStream
				.Setup(e => e.BeginActivity(ProcessMessagingStep.ActivityName, It.IsAny<string>()))
				.Callback<string, string>((n, i) => processMessagingStepActivityId = i);
			eventStream
				.Setup(es => es.UpdateActivity(ProcessMessagingStep.ActivityName, It.Is<string>(id => id == processMessagingStepActivityId), It.IsAny<object[]>()))
				.Callback<string, string, object[]>((n, id, d) => data = Enumerable.Range(0, d.Length / 2).ToDictionary(i => (string) d[i * 2], i => d[i * 2 + 1]))
				.Verifiable();

			var pipelineContext = new Mock<IPipelineContext>();
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var message = new Unit.Message.Mock<IBaseMessage>();
			message.Setup(m => m.GetProperty(ErrorReportProperties.ErrorType)).Returns(TrackingStatus.FailedMessage);

			var processActivityId = ActivityId.NewActivityId();
			var sut = new Process(processActivityId, eventStream.Object);
			var messagingStep = new MessagingStep(pipelineContext.Object, message.Object);
			var messagingStepActivityId = messagingStep.ActivityId;
			sut.AddStep(messagingStep);

			eventStream.Verify();
			eventStream.Verify(s => s.BeginActivity(ProcessMessagingStep.ActivityName, processMessagingStepActivityId), Times.Once());
			eventStream.Verify(s => s.UpdateActivity(ProcessMessagingStep.ActivityName, processMessagingStepActivityId, It.IsAny<object[]>()), Times.Once());
			eventStream.Verify(s => s.Flush(), Times.Once());
			eventStream.Verify(s => s.EndActivity(ProcessMessagingStep.ActivityName, processMessagingStepActivityId), Times.Once());

			var expectedData = new Dictionary<string, object> {
				{ ProcessMessagingStep.MessagingStepActivityIDFieldName, messagingStepActivityId },
				// capture of Status is what distinguishes affiliation of a MessagingStep from affiliation of a MessagingStepReference
				{ ProcessMessagingStep.MessagingStepStatusFieldName, TrackingStatus.FailedMessage },
				{ ProcessMessagingStep.ProcessActivityIDFieldName, processActivityId }
			};
			Assert.That(data, Is.EquivalentTo(expectedData));
		}

		[Test]
		public void MessagingStepReferenceIsAffiliatedToProcess()
		{
			Dictionary<string, object> data = null;
			var processMessagingStepActivityId = string.Empty;
			var eventStream = new Mock<EventStream>();
			eventStream
				.Setup(e => e.BeginActivity(ProcessMessagingStep.ActivityName, It.IsAny<string>()))
				.Callback<string, string>((n, i) => processMessagingStepActivityId = i);
			eventStream
				.Setup(es => es.UpdateActivity(ProcessMessagingStep.ActivityName, It.Is<string>(id => id == processMessagingStepActivityId), It.IsAny<object[]>()))
				.Callback<string, string, object[]>((n, id, d) => data = Enumerable.Range(0, d.Length / 2).ToDictionary(i => (string) d[i * 2], i => d[i * 2 + 1]))
				.Verifiable();

			var processActivityId = ActivityId.NewActivityId();
			var messagingStepActivityId = ActivityId.NewActivityId();

			var sut = new Process(processActivityId, eventStream.Object);
			sut.AddStep(new MessagingStepReference(messagingStepActivityId, eventStream.Object));

			eventStream.Verify();
			eventStream.Verify(s => s.BeginActivity(ProcessMessagingStep.ActivityName, processMessagingStepActivityId), Times.Once());
			eventStream.Verify(s => s.UpdateActivity(ProcessMessagingStep.ActivityName, processMessagingStepActivityId, It.IsAny<object[]>()), Times.Once());
			eventStream.Verify(s => s.Flush(), Times.Once());
			eventStream.Verify(s => s.EndActivity(ProcessMessagingStep.ActivityName, processMessagingStepActivityId), Times.Once());

			var expectedData = new Dictionary<string, object> {
				{ ProcessMessagingStep.MessagingStepActivityIDFieldName, messagingStepActivityId },
				{ ProcessMessagingStep.ProcessActivityIDFieldName, processActivityId }
			};
			Assert.That(data, Is.EquivalentTo(expectedData));
		}

		[Test]
		public void ProcessPropertiesAreTracked()
		{
			var interchangeId = Guid.NewGuid();
			var message = new Unit.Message.Mock<IBaseMessage>();
			message.Setup(m => m.GetProperty(BtsProperties.InterchangeID)).Returns(interchangeId.ToString());
			message.Setup(m => m.GetProperty(TrackingProperties.Value1)).Returns("value-1");
			message.Setup(m => m.GetProperty(TrackingProperties.Value2)).Returns("value-2");
			message.Setup(m => m.GetProperty(TrackingProperties.Value3)).Returns("value-3");
			message.Setup(m => m.GetProperty(ErrorReportProperties.ErrorType)).Returns(TrackingStatus.FailedMessage);

			var eventStream = new Mock<EventStream>();

			var pipelineContext = new Mock<IPipelineContext>();
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var sut = new Process(pipelineContext.Object, message.Object, "process-name");

			Dictionary<string, object> data = null;
			eventStream
				.Setup(es => es.UpdateActivity(Process.ActivityName, It.Is<string>(id => id == sut.ActivityId), It.IsAny<object[]>()))
				.Callback<string, string, object[]>((n, id, d) => data = Enumerable.Range(0, d.Length / 2).ToDictionary(i => (string) d[i * 2], i => d[i * 2 + 1]))
				.Verifiable();

			sut.TrackActivity();

			eventStream.Verify();

			var expectedData = new Dictionary<string, object> {
				{ Process.InterchangeIDFieldName, interchangeId.AsNormalizedActivityId() },
				{ Process.ProcessNameFieldName, "process-name" },
				{ Process.StatusFieldName, TrackingStatus.Failed },
				{ Process.Value1FieldName, "value-1" },
				{ Process.Value2FieldName, "value-2" },
				{ Process.Value3FieldName, "value-3" },
				// ReSharper disable PossibleInvalidOperationException
				{ Process.BeginTimeFieldName, sut.BeginTime.Value },
				{ Process.EndTimeFieldName, sut.EndTime.Value }
				// ReSharper restore PossibleInvalidOperationException
			};
			Assert.That(data, Is.EquivalentTo(expectedData));
		}
	}
}

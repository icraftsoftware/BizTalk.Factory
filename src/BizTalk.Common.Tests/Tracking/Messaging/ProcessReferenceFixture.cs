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

using System.Collections.Generic;
using System.Linq;
using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Component.Interop;
using Moq;
using NUnit.Framework;
using MessageMock = Be.Stateless.BizTalk.Unit.Message.Mock<Microsoft.BizTalk.Message.Interop.IBaseMessage>;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	[TestFixture]
	public class ProcessReferenceFixture
	{
		[Test]
		public void MessagingStepIsAffiliatedToProcess()
		{
			var processMessagingStepActivityId = string.Empty;
			Dictionary<string, object> data = null;
			var eventStream = new Mock<EventStream>();
			eventStream
				.Setup(e => e.BeginActivity(ProcessMessagingStep.ActivityName, It.IsAny<string>()))
				.Callback<string, string>((n, i) => processMessagingStepActivityId = i);
			eventStream
				.Setup(es => es.UpdateActivity(ProcessMessagingStep.ActivityName, It.Is<string>(id => id == processMessagingStepActivityId), It.IsAny<object[]>()))
				.Callback<string, string, object[]>((n, id, d) => data = Enumerable.Range(0, d.Length / 2).ToDictionary(i => (string) d[i * 2], i => d[i * 2 + 1]));

			var pipelineContext = new Mock<IPipelineContext>();
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);
			var message = new MessageMock();
			var messagingStep = new MessagingStep(pipelineContext.Object, message.Object);

			var processActivityId = ActivityId.NewActivityId();
			var sut = new ProcessReference(processActivityId, eventStream.Object);
			sut.AddStep(messagingStep);

			eventStream.Verify(s => s.BeginActivity(ProcessMessagingStep.ActivityName, processMessagingStepActivityId), Times.Once());
			eventStream.Verify(s => s.UpdateActivity(ProcessMessagingStep.ActivityName, processMessagingStepActivityId, It.IsAny<object[]>()), Times.Once());
			eventStream.Verify(s => s.Flush(), Times.Once());
			eventStream.Verify(s => s.EndActivity(ProcessMessagingStep.ActivityName, processMessagingStepActivityId), Times.Once());

			var expectedData = new Dictionary<string, object> {
				{ ProcessMessagingStep.MessagingStepActivityIDFieldName, messagingStep.ActivityId },
				{ ProcessMessagingStep.ProcessActivityIDFieldName, processActivityId }
			};
			Assert.That(data, Is.EquivalentTo(expectedData));
		}
	}
}

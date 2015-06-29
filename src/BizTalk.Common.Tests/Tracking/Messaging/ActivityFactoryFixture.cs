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

using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	[TestFixture]
	public class ActivityFactoryFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			PipelineContextMock = new Mock<IPipelineContext>();
			PipelineContextMock.Setup(pc => pc.GetEventStream()).Returns(new Mock<EventStream>().Object);
		}

		#endregion

		[Test]
		public void CreateMessagingStepReturnsRegularMessagingStep()
		{
			var messageMock = new Unit.Message.Mock<IBaseMessage>();
			var factory = new ActivityFactory(PipelineContextMock.Object);
			Assert.That(factory.CreateMessagingStep(messageMock.Object), Is.TypeOf<MessagingStep>());
		}

		[Test]
		public void CreateProcessReturnsRegularBatchReleaseProcess()
		{
			var messageMock = new Unit.Message.Mock<IBaseMessage>();
			var factory = (IBatchProcessActivityFactory) new ActivityFactory(PipelineContextMock.Object);
			Assert.That(factory.CreateProcess(messageMock.Object, "name"), Is.TypeOf<BatchReleaseProcess>());
		}

		[Test]
		public void CreateProcessReturnsRegularProcess()
		{
			var messageMock = new Unit.Message.Mock<IBaseMessage>();
			var factory = new ActivityFactory(PipelineContextMock.Object);
			Assert.That(factory.CreateProcess(messageMock.Object, "name"), Is.TypeOf<Process>());
		}

		[Test]
		public void FindMessagingStepReturnsMessagingStepReference()
		{
			var factory = new ActivityFactory(PipelineContextMock.Object);
			Assert.That(factory.FindMessagingStep(new TrackingContext { MessagingStepActivityId = "pseudo-activity-id" }), Is.TypeOf<MessagingStepReference>());
		}

		[Test]
		public void FindProcessReturnsBatchReleaseProcessReference()
		{
			var factory = (IBatchProcessActivityFactory) new ActivityFactory(PipelineContextMock.Object);
			Assert.That(factory.FindProcess("pseudo-activity-id"), Is.TypeOf<BatchReleaseProcessReference>());
		}

		[Test]
		public void FindProcessReturnsProcessReference()
		{
			var factory = new ActivityFactory(PipelineContextMock.Object);
			Assert.That(factory.FindProcess(new TrackingContext { ProcessActivityId = "pseudo-activity-id" }), Is.TypeOf<ProcessReference>());
		}

		private Mock<IPipelineContext> PipelineContextMock { get; set; }
	}
}

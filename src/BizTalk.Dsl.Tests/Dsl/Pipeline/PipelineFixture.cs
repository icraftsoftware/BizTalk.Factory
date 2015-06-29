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
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	[TestFixture]
	public class PipelineFixture
	{
		[Test]
		public void CanOnlyCreateIReceiveOrISendPipelineStageListBasedPipelines()
		{
			Assert.That(
				() => new InvalidPipeline(),
				Throws.InnerException.InstanceOf<ArgumentException>().With.InnerException.Message.EqualTo(
					"A pipeline does not support IPipelineStageList as a stage container because it does not derive from either IReceivePipelineStageList or ISendPipelineStageList."));
		}

		[Test]
		public void VisitReceivePipeline()
		{
			var sut = new ReceivePipelineImpl();

			// workaround Moq's inability to verify that calls have proceeded in a prescribed order. following recursive
			// setups simply ensure the calls are made in a specific order; as first call is matched we setup the second
			// call in the first call's callback, then when second call is matched we setup the third call in the second
			// call's callback, and so on and on... until we have setup all the expected calls. notice that for this
			// workaround to work the Mock's behavior must be *MockBehavior.Strict* or else any call missed would be
			// unnoticed (and all its consecutive calls as well since they would not have been setup via its callback).
			var visitor = new Mock<IPipelineVisitor>(MockBehavior.Strict);
			// ReSharper disable ImplicitlyCapturedClosure
			visitor.Setup(v => v.VisitPipeline(sut))
				// ReSharper restore ImplicitlyCapturedClosure
				.Callback(
					() => visitor.Setup(v => v.VisitStage(sut.Stages.Decode))
						.Callback(
							() => visitor.Setup(v => v.VisitStage(sut.Stages.Disassemble))
								.Callback(
									() => visitor.Setup(v => v.VisitStage(sut.Stages.Validate))
										.Callback(
											() => visitor.Setup(v => v.VisitStage(sut.Stages.ResolveParty))
										)
								)
						)
				);

			((IVisitable<IPipelineVisitor>) sut).Accept(visitor.Object);

			visitor.Verify();
		}

		[Test]
		public void VisitSendPipeline()
		{
			var sut = new SendPipelineImpl();

			// workaround Moq's inability to verify that calls have proceeded in a prescribed order. following recursive
			// setups simply ensure the calls are made in a specific order; as first call is matched we setup the second
			// call in the first call's callback, then when second call is matched we setup the third call in the second
			// call's callback, and so on and on... until we have setup all the expected calls. notice that for this
			// workaround to work the Mock's behavior must be *MockBehavior.Strict* or else any call missed would be
			// unnoticed (and all its consecutive calls as well since they would not have been setup via its callback).
			var visitor = new Mock<IPipelineVisitor>(MockBehavior.Strict);
			// ReSharper disable ImplicitlyCapturedClosure
			visitor.Setup(v => v.VisitPipeline(sut))
				// ReSharper restore ImplicitlyCapturedClosure
				.Callback(
					() => visitor.Setup(v => v.VisitStage(sut.Stages.PreAssemble))
						.Callback(
							() => visitor.Setup(v => v.VisitStage(sut.Stages.Assemble))
								.Callback(
									() => visitor.Setup(v => v.VisitStage(sut.Stages.Encode))
								)
						)
				);

			((IVisitable<IPipelineVisitor>) sut).Accept(visitor.Object);

			visitor.Verify();
		}

		private class InvalidPipeline : Pipeline<IPipelineStageList>
		{
			public InvalidPipeline() : base(new ReceivePipelineStageList()) { }
		}

		private class ReceivePipelineImpl : ReceivePipeline { }

		private class SendPipelineImpl : SendPipeline { }
	}
}

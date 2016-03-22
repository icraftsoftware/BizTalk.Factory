#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Component;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	[TestFixture]
	public class StageFixture
	{
		[Test]
		public void CanOnlyCreateStageForKnownCategory()
		{
			Assert.That(
				() => new Stage(Guid.NewGuid()),
				Throws.InstanceOf<KeyNotFoundException>());
		}

		[Test]
		public void FetchComponentFromStage()
		{
			var component = new FailedMessageRoutingEnablerComponent();

			var stage = new Stage(StageCategory.Decoder.Id);
			stage.AddComponent(component);

			Assert.That(stage.Component<FailedMessageRoutingEnablerComponent>(), Is.SameAs(component));
		}

		[Test]
		public void VisitorWalkedPath()
		{
			var failedMessageRoutingEnablerComponent = new FailedMessageRoutingEnablerComponent();
			var messageConsumerComponent = new MessageConsumerComponent();

			var sut = new Stage(StageCategory.Any.Id);
			sut.AddComponent(failedMessageRoutingEnablerComponent)
				.AddComponent(messageConsumerComponent);

			// workaround Moq's inability to verify that calls have proceeded in a prescribed order. following recursive
			// setups simply ensure the calls are made in a specific order; as first call is matched we setup the second
			// call in the first call's callback, then when second call is matched we setup the third call in the second
			// call's callback, and so on and on... until we have setup all the expected calls. notice that for this
			// workaround to work the Mock's behavior must be *MockBehavior.Strict* or else any call missed would be
			// unnoticed (and all its consecutive calls as well since they would not have been setup via its callback).
			var visitor = new Mock<IPipelineVisitor>(MockBehavior.Strict);

			// ReSharper disable ImplicitlyCapturedClosure
			visitor.Setup(v => v.VisitStage(sut))
				// ReSharper restore ImplicitlyCapturedClosure
				.Callback(
					() => visitor.Setup(
						v => v.VisitComponent(
							It.Is<PipelineComponentDescriptor<FailedMessageRoutingEnablerComponent>>(
								// ReSharper disable SuspiciousTypeConversion.Global
								c => ReferenceEquals((FailedMessageRoutingEnablerComponent) c, failedMessageRoutingEnablerComponent)
								// ReSharper restore SuspiciousTypeConversion.Global
								)
							)
						)
						.Callback(
							() => visitor.Setup(
								v => v.VisitComponent(
									It.Is<PipelineComponentDescriptor<MessageConsumerComponent>>(
										// ReSharper disable SuspiciousTypeConversion.Global
										c => ReferenceEquals((MessageConsumerComponent) c, messageConsumerComponent)
										// ReSharper restore SuspiciousTypeConversion.Global
										)
									)
								)
						)
				);

			((IVisitable<IPipelineVisitor>) sut).Accept(visitor.Object);

			visitor.Verify();
		}
	}
}

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

using System.IO;
using System.Text;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Unit.Component;
using Be.Stateless.IO.Extensions;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	public class PolicyRunnerComponentFixture : PipelineComponentFixture<PolicyRunnerComponentFixture.PolicyRunnerComponentWithDefaultDummyPolicy>
	{
		#region Nested Type: PolicyRunnerComponentWithDefaultDummyPolicy

		public class PolicyRunnerComponentWithDefaultDummyPolicy : PolicyRunnerComponent
		{
			public PolicyRunnerComponentWithDefaultDummyPolicy()
			{
				// dummy default policy not to impact Enabled and disturb PipelineComponentFixture<> non-regression tests
				Policy = new PolicyName("name", 1, 0);
			}
		}

		#endregion

		[SetUp]
		public new void SetUp()
		{
			MessageMock.Object.BodyPart.Data = new MemoryStream(_content);
			_policyMock = new Mock<IPolicy>();
			Policy.Factory = ruleSetInfo => _policyMock.Object;
		}

		[Test]
		public void PolicyExecutionIsDeferred()
		{
			var sut = CreatePipelineComponent();
			sut.ExecutionMode = PluginExecutionMode.Deferred;
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			_policyMock.Verify(pc => pc.Execute(It.IsAny<object[]>()), Times.Never());
			MessageMock.Object.BodyPart.Data.Drain();
			_policyMock.Verify(pc => pc.Execute(It.IsAny<object[]>()), Times.Once());
		}

		[Test]
		public void PolicyExecutionIsImmediate()
		{
			var sut = CreatePipelineComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			_policyMock.Verify(pc => pc.Execute(It.IsAny<object[]>()), Times.Once());
			MessageMock.Object.BodyPart.Data.Drain();
			// policy has still been executed only once and not an extra second time
			_policyMock.Verify(pc => pc.Execute(It.IsAny<object[]>()), Times.Once());
		}

		private readonly byte[] _content = Encoding.Unicode.GetBytes(new string('A', 512));
		private Mock<IPolicy> _policyMock;
	}
}

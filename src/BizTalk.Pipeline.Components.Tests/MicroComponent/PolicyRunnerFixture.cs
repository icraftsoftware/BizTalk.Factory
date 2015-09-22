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
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.IO.Extensions;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class PolicyRunnerFixture : MicroPipelineComponentFixture
	{
		[Test]
		public void DoesNothingWhenNoPolicy()
		{
			using (var dataStream = new MemoryStream(Encoding.Unicode.GetBytes(new string('A', 512))))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				var policyMock = new Mock<IPolicy>();
				Policy.Factory = ruleSetInfo => policyMock.Object;

				var sut = new PolicyRunner {
					ExecutionMode = PluginExecutionMode.Immediate,
				};

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
				policyMock.Verify(pc => pc.Execute(It.IsAny<object[]>()), Times.Never());

				MessageMock.Object.BodyPart.Data.Drain();
				policyMock.Verify(pc => pc.Execute(It.IsAny<object[]>()), Times.Never());
			}
		}

		[Test]
		public void PolicyExecutionIsDeferred()
		{
			using (var dataStream = new MemoryStream(Encoding.Unicode.GetBytes(new string('A', 512))))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				var policyMock = new Mock<IPolicy>();
				Policy.Factory = ruleSetInfo => policyMock.Object;

				var sut = new PolicyRunner {
					ExecutionMode = PluginExecutionMode.Deferred,
					PolicyName = new PolicyName("name", 1, 0)
				};

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
				policyMock.Verify(pc => pc.Execute(It.IsAny<object[]>()), Times.Never());

				MessageMock.Object.BodyPart.Data.Drain();
				policyMock.Verify(pc => pc.Execute(It.IsAny<object[]>()), Times.Once());
			}
		}

		[Test]
		public void PolicyExecutionIsImmediate()
		{
			using (var dataStream = new MemoryStream(Encoding.Unicode.GetBytes(new string('A', 512))))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				var policyMock = new Mock<IPolicy>();
				Policy.Factory = ruleSetInfo => policyMock.Object;

				var sut = new PolicyRunner {
					ExecutionMode = PluginExecutionMode.Immediate,
					PolicyName = new PolicyName("name", 1, 0)
				};

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
				policyMock.Verify(pc => pc.Execute(It.IsAny<object[]>()), Times.Once());
				policyMock.ResetCalls();

				MessageMock.Object.BodyPart.Data.Drain();
				policyMock.Verify(pc => pc.Execute(It.IsAny<object[]>()), Times.Never());
			}
		}
	}
}

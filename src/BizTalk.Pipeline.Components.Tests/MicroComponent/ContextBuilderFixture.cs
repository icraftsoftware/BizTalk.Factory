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

using System.IO;
using System.Text;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.IO.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class ContextBuilderFixture : MicroPipelineComponentFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			BuilderMock = new Mock<IContextBuilder>();
		}

		#endregion

		[Test]
		public void ContextBuilderPluginExecutionIsDeferred()
		{
			using (var inputStram = new MemoryStream(Encoding.Unicode.GetBytes(new string('A', 512))))
			{
				MessageMock.Object.BodyPart.Data = inputStram;

				var sut = new ContextBuilder {
					BuilderType = typeof(DummyBuilder),
					ExecutionMode = PluginExecutionMode.Deferred
				};

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
				BuilderMock.Verify(pc => pc.Execute(It.IsAny<IBaseMessageContext>()), Times.Never());

				MessageMock.Object.BodyPart.Data.Drain();
				BuilderMock.Verify(pc => pc.Execute(It.IsAny<IBaseMessageContext>()), Times.Once());
			}
		}

		[Test]
		public void ContextBuilderPluginExecutionIsImmediate()
		{
			using (var inputStram = new MemoryStream(Encoding.Unicode.GetBytes(new string('A', 512))))
			{
				MessageMock.Object.BodyPart.Data = inputStram;

				var sut = new ContextBuilder {
					BuilderType = typeof(DummyBuilder),
					ExecutionMode = PluginExecutionMode.Immediate
				};

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
				BuilderMock.Verify(pc => pc.Execute(It.IsAny<IBaseMessageContext>()), Times.Once());

				BuilderMock.ResetCalls();
				MessageMock.Object.BodyPart.Data.Drain();
				BuilderMock.Verify(pc => pc.Execute(It.IsAny<IBaseMessageContext>()), Times.Never());
			}
		}

		private class DummyBuilder : IContextBuilder
		{
			#region IContextBuilder Members

			public void Execute(IBaseMessageContext context)
			{
				BuilderMock.Object.Execute(context);
			}

			#endregion
		}

		private static Mock<IContextBuilder> BuilderMock { get; set; }
	}
}

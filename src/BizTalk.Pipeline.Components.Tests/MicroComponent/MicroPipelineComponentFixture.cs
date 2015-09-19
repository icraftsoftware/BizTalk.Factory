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

using System.Runtime.InteropServices;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public abstract class MicroPipelineComponentFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void MicroPipelineComponentFixtureSetUp()
		{
			MessageMock = new Unit.Message.Mock<IBaseMessage> { DefaultValue = DefaultValue.Mock };

			PipelineContextMock = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			// default behaviour analogous to actual IPipelineContext implementation
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(It.IsAny<string>()))
				.Callback<string>(t => { throw new COMException("Could not locate document specification with type: " + t); });
		}

		#endregion

		protected Unit.Message.Mock<IBaseMessage> MessageMock { get; set; }

		protected Mock<IPipelineContext> PipelineContextMock { get; set; }
	}
}

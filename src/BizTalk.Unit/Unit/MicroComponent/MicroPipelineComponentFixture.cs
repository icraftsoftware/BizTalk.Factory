#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using Be.Stateless.BizTalk.Component.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.MicroComponent
{
	[TestFixture]
	public abstract class MicroPipelineComponentFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void MicroPipelineComponentFixtureSetUp()
		{
			MessageMock = new Message.Mock<IBaseMessage> { DefaultValue = DefaultValue.Mock };
			PipelineContextMock = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			// default behavior analogous to actual IPipelineContext implementation
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(It.IsAny<string>()))
				.Callback<string>(
					t => {
						throw new COMException(
							string.Format("Finding the document specification by message type \"{0}\" failed. Verify the schema deployed properly.", t),
							PipelineContextExtensions.E_SCHEMA_NOT_FOUND);
					});
		}

		#endregion

		protected Message.Mock<IBaseMessage> MessageMock { get; set; }

		protected Mock<IPipelineContext> PipelineContextMock { get; set; }
	}
}

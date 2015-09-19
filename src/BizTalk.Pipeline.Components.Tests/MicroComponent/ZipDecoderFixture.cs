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

using Be.Stateless.BizTalk.Unit.Resources;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class ZipDecoderFixture : MicroPipelineComponentFixture
	{
		[Test]
		public void WrapsMessageStreamInZipInputStream()
		{
			var bodyPart = new Mock<IBaseMessagePart>();
			bodyPart.Setup(p => p.GetOriginalDataStream()).Returns(ResourceManager.Load("Data.message.zip"));
			bodyPart.SetupProperty(p => p.Data);
			MessageMock.Setup(m => m.BodyPart).Returns(bodyPart.Object);

			var sut = new ZipDecoder();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			Assert.IsInstanceOf<ZipInputStream>(MessageMock.Object.BodyPart.Data);
		}
	}
}

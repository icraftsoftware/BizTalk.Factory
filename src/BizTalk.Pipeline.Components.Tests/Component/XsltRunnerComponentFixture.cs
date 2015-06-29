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

using System;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class XsltRunnerComponentFixture : XsltRunnerComponentFixtureBase<XsltRunnerComponent>
	{
		[Test]
		public void DoesNothingWhenNoXslt()
		{
			var probeStreamMock = new Mock<IProbeStream>();
			StreamExtensions.StreamProberFactory = stream => probeStreamMock.Object;
			var transformStreamMock = new Mock<ITransformStream>();
			StreamExtensions.StreamTransformerFactory = stream => transformStreamMock.Object;

			var sut = CreatePipelineComponent();
			Assert.That(sut.Map, Is.Null);
			Assert.That(sut.Enabled);
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			probeStreamMock.VerifyGet(ps => ps.MessageType, Times.Never());
			transformStreamMock.Verify(ps => ps.Apply(It.IsAny<Type>()), Times.Never());
		}
	}
}

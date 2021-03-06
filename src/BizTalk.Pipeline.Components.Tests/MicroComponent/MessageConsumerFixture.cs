﻿#region Copyright & License

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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class MessageConsumerFixture : MicroPipelineComponentFixture
	{
		[Test]
		public void MessageIsDrainedAndAbsorbed()
		{
			var content = Encoding.Unicode.GetBytes("Hello there.");
			using (var inputStream = new MemoryStream(content))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				Assert.That(MessageMock.Object.BodyPart.GetOriginalDataStream().Position, Is.EqualTo(0));

				var sut = new MessageConsumer();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				// message has been drained
				Assert.That(MessageMock.Object.BodyPart.GetOriginalDataStream().Position, Is.EqualTo(content.Length));

				// generation of ack message has been discarded
				MessageMock.Verify(m => m.SetProperty(BtsProperties.AckRequired, false), Times.Once());
			}
		}
	}
}

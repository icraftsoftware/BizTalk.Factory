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

using System.IO;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.IO;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class MultipartFormDataContentEncoderFixture : MicroPipelineComponentFixture
	{
		[Test]
		[Ignore("MessageMock need to be fixed to support BodyPartName setup.")]
		public void ContentBodyPartHasName()
		{
			using (var dataStream = ResourceManager.Load("Data.BatchContent.xml"))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				MessageMock.Setup(m => m.BodyPartName).Returns("implicit");

				var sut = new MultipartFormDataContentEncoder { UseBodyPartNameAsContentName = true };
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(MessageMock.Object.BodyPart.ContentType, Does.Match("multipart/form-data; boundary=\"[a-f\\d]{8}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{12}\""));
				using (var reader = new StreamReader(MessageMock.Object.BodyPart.Data))
				{
					var content = reader.ReadToEnd();
					Assert.That(content, Does.Match("^(--[a-f\\d]{8}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{12})\r\nContent-Disposition: form-data; name=implicit\r\n[\\w\\W]+\r\n\\1--\r\n$"));
				}
			}
		}

		[Test]
		public void ContentHasName()
		{
			using (var dataStream = ResourceManager.Load("Data.BatchContent.xml"))
			{
				MessageMock.Object.BodyPart.Data = dataStream;

				var sut = new MultipartFormDataContentEncoder { ContentName = "explicit" };
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(MessageMock.Object.BodyPart.ContentType, Does.Match("multipart/form-data; boundary=\"[a-f\\d]{8}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{12}\""));
				using (var reader = new StreamReader(MessageMock.Object.BodyPart.Data))
				{
					var content = reader.ReadToEnd();
					Assert.That(
						content,
						Does.Match("^(--[a-f\\d]{8}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{12})\r\nContent-Disposition: form-data; name=explicit\r\n[\\w\\W]+\r\n\\1--\r\n$"));
				}
			}
		}

		[Test]
		public void ContentIsMultipart()
		{
			using (var dataStream = ResourceManager.Load("Data.BatchContent.xml"))
			{
				MessageMock.Object.BodyPart.Data = dataStream;

				var sut = new MultipartFormDataContentEncoder();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(MessageMock.Object.BodyPart.ContentType, Does.Match("multipart/form-data; boundary=\"[a-f\\d]{8}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{12}\""));
				using (var reader = new StreamReader(MessageMock.Object.BodyPart.Data))
				{
					var content = reader.ReadToEnd();
					Assert.That(content, Does.Match("^(--[a-f\\d]{8}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{12})\r\nContent-Disposition: form-data\r\n[\\w\\W]+\r\n\\1--\r\n$"));
				}
			}
		}

		[Test]
		public void WrapsMessageStreamInMultipartFormDataContentStream()
		{
			var bodyPart = new Mock<IBaseMessagePart>();
			bodyPart.Setup(p => p.GetOriginalDataStream()).Returns(new StringStream("content"));
			bodyPart.SetupProperty(p => p.Data);
			MessageMock.Setup(m => m.BodyPart).Returns(bodyPart.Object);

			var sut = new MultipartFormDataContentEncoder();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			Assert.That(MessageMock.Object.BodyPart.Data, Is.InstanceOf<MultipartFormDataContentStream>());
		}
	}
}

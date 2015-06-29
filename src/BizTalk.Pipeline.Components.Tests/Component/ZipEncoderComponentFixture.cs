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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Unit.Component;
using Be.Stateless.IO;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;
using ZipInputStream = ICSharpCode.SharpZipLib.Zip.ZipInputStream;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class ZipEncoderComponentFixture : PipelineComponentFixture<ZipEncoderComponent>
	{
		#region Setup/Teardown

		[SetUp]
		public void Initialize()
		{
			var bodyPart = new Mock<IBaseMessagePart>();
			bodyPart.Setup(p => p.GetOriginalDataStream()).Returns(new StringStream("content"));
			bodyPart.SetupProperty(p => p.Data);
			MessageMock.Setup(m => m.BodyPart).Returns(bodyPart.Object);
		}

		#endregion

		[Test]
		public void OutboundTransportLocationAbsolutePathIsKeptUnchanged()
		{
			MessageMock
				.Setup(m => m.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"\file.txt");

			var sut = CreatePipelineComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.OutboundTransportLocation, @"\file.zip"));
		}

		[Test]
		public void OutboundTransportLocationFileExtensionIsChangedToZip()
		{
			const string location = @"\\unc\folder\file.txt";
			MessageMock
				.Setup(m => m.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(location);

			var sut = CreatePipelineComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.OutboundTransportLocation, location.Replace(".txt", ".zip")));
		}

		[Test]
		public void OutboundTransportLocationWithOnlyFilename()
		{
			MessageMock
				.Setup(m => m.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(@"file.txt");

			var sut = CreatePipelineComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.OutboundTransportLocation, "file.zip"));
		}

		[Test]
		public void WrapsMessageStreamInZipOutputStream()
		{
			const string location = "sftp://host/folder/file.txt";
			MessageMock
				.Setup(m => m.GetProperty(BizTalkFactoryProperties.OutboundTransportLocation))
				.Returns(location);

			var sut = CreatePipelineComponent();
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			Assert.IsInstanceOf<ZipOutputStream>(MessageMock.Object.BodyPart.Data);
			var clearStream = new ZipInputStream(MessageMock.Object.BodyPart.Data);
			var entry = clearStream.GetNextEntry();
			Assert.That(entry.Name, Is.EqualTo(System.IO.Path.GetFileName(location)));
		}

		[Test]
		public void ZipEntryNameIsDerivedFromOutboundTransportLocation()
		{
			var sut = CreatePipelineComponent();
			Assert.That(
				() => sut.Execute(PipelineContextMock.Object, MessageMock.Object),
				Throws.TypeOf<InvalidOperationException>()
					.With.Message.EqualTo("BizTalkFactoryProperties.OutboundTransportLocation has to be set in context in order to determine zip entry name."));
		}
	}
}

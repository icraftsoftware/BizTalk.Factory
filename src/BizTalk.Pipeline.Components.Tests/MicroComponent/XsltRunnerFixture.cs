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

using System;
using System.IO;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.Unit.MicroComponent;
using Be.Stateless.BizTalk.Unit.Transform;
using Be.Stateless.IO;
using Be.Stateless.Reflection;
using Microsoft.BizTalk.Streaming;
using Microsoft.XLANGs.BaseTypes;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class XsltRunnerFixture : MicroPipelineComponentFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void Backup()
		{
			_proberFactory = StreamExtensions.StreamProberFactory;
			_transformerFactory = StreamExtensions.StreamTransformerFactory;
		}

		[TearDown]
		public void Restore()
		{
			StreamExtensions.StreamProberFactory = _proberFactory;
			StreamExtensions.StreamTransformerFactory = _transformerFactory;
		}

		#endregion

		[Test]
		public void DoesNothingWhenNoXslt()
		{
			var probeStreamMock = new Mock<IProbeStream>();
			StreamExtensions.StreamProberFactory = stream => probeStreamMock.Object;
			var transformStreamMock = new Mock<ITransformStream>();
			StreamExtensions.StreamTransformerFactory = stream => transformStreamMock.Object;

			// CAUTION! does not call CreateXsltRunner() as this test concerns only XsltRunner and none of its derived types
			var sut = new XsltRunner();
			Assert.That(sut.MapType, Is.Null);
			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			probeStreamMock.VerifyGet(ps => ps.MessageType, Times.Never());
			transformStreamMock.Verify(ps => ps.Apply(It.IsAny<Type>()), Times.Never());
		}

		[Test]
		public void EncodingDefaultsToUtf8WithoutSignature()
		{
			Assert.That(CreateXsltRunner().Encoding, Is.EqualTo(new UTF8Encoding(false)));
		}

		[Test]
		public virtual void ReplacesMessageOriginalDataStreamWithTransformResult()
		{
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("urn:ns#root"))
				.Returns(Schema<Schemas.Xml.Any>.DocumentSpec);

			var sut = CreateXsltRunner();
			sut.Encoding = Encoding.UTF8;
			sut.MapType = typeof(IdentityTransform);

			using (var dataStream = new StringStream("<root xmlns='urn:ns'></root>"))
			using (var transformedStream = dataStream.Transform().Apply(sut.MapType))
			{
				MessageMock.Object.BodyPart.Data = dataStream;

				var transformStreamMock = new Mock<ITransformStream>(MockBehavior.Strict);
				StreamExtensions.StreamTransformerFactory = stream => transformStreamMock.Object;
				transformStreamMock
					.Setup(ts => ts.ExtendWith(MessageMock.Object.Context))
					.Returns(transformStreamMock.Object);
				transformStreamMock
					.Setup(ts => ts.Apply(sut.MapType, sut.Encoding))
					.Returns(transformedStream)
					.Verifiable();

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				transformStreamMock.VerifyAll();

				Assert.That(MessageMock.Object.BodyPart.Data, Is.TypeOf<MarkableForwardOnlyEventingReadStream>());
				// ReSharper disable once PossibleInvalidCastException
				Assert.That(
					Reflector.GetField((MarkableForwardOnlyEventingReadStream) MessageMock.Object.BodyPart.Data, "m_data"),
					Is.SameAs(transformedStream));
			}
		}

		[Test]
		public virtual void XsltEntailsMessageTypeIsPromoted()
		{
			PipelineContextMock
				.Setup(m => m.GetDocumentSpecByType("urn:ns#root"))
				.Returns(Schema<Batch.Content>.DocumentSpec);

			var sut = CreateXsltRunner();
			sut.Encoding = Encoding.UTF8;
			sut.MapType = typeof(IdentityTransform);

			using (var dataStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
			}

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, Schema<Batch.Content>.MessageType), Times.Once());
			MessageMock.Verify(m => m.Promote(BtsProperties.SchemaStrongName, new SchemaMetadata<Batch.Content>().DocumentSpec.DocSpecStrongName), Times.Once());
		}

		[Test]
		public virtual void XsltEntailsMessageTypeIsPromotedOnlyIfOutputMethodIsXml()
		{
			PipelineContextMock
				.Setup(m => m.GetDocumentSpecByType("urn:ns#root"))
				.Returns(Schema<Batch.Content>.DocumentSpec);

			var sut = CreateXsltRunner();
			sut.Encoding = Encoding.UTF8;
			sut.MapType = typeof(AnyToText);

			using (var dataStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
			}

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, Schema<Batch.Content>.MessageType), Times.Never());
			MessageMock.Verify(m => m.Promote(BtsProperties.SchemaStrongName, new SchemaMetadata<Batch.Content>().DocumentSpec.DocSpecStrongName), Times.Never());
		}

		[Test]
		public virtual void XsltFromContextHasPrecedenceOverConfiguredOne()
		{
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("urn:ns#root"))
				.Returns(Schema<Schemas.Xml.Any>.DocumentSpec);

			var sut = CreateXsltRunner();
			sut.Encoding = Encoding.UTF8;
			sut.MapType = typeof(TransformBase);

			var mapType = typeof(IdentityTransform);
			using (var dataStream = new StringStream("<root xmlns='urn:ns'></root>"))
			using (var transformedStream = dataStream.Transform().Apply(mapType))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				MessageMock
					.Setup(m => m.GetProperty(BizTalkFactoryProperties.MapTypeName))
					.Returns(Transform<IdentityTransform>.MapTypeName);

				var transformStreamMock = new Mock<ITransformStream>(MockBehavior.Strict);
				StreamExtensions.StreamTransformerFactory = stream => transformStreamMock.Object;
				transformStreamMock
					.Setup(ts => ts.ExtendWith(MessageMock.Object.Context))
					.Returns(transformStreamMock.Object);
				transformStreamMock
					.Setup(ts => ts.Apply(mapType, sut.Encoding))
					.Returns(transformedStream)
					.Verifiable();

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				transformStreamMock.Verify(ts => ts.Apply(sut.MapType, sut.Encoding), Times.Never());
				transformStreamMock.VerifyAll();
			}
		}

		protected virtual XsltRunner CreateXsltRunner()
		{
			return new XsltRunner();
		}

		private Func<MarkableForwardOnlyEventingReadStream, IProbeStream> _proberFactory;
		private Func<Stream[], ITransformStream> _transformerFactory;
	}
}

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
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.Unit.Component;
using Be.Stateless.BizTalk.Unit.Transform;
using Be.Stateless.IO;
using Be.Stateless.Reflection;
using Be.Stateless.Text;
using Microsoft.BizTalk.Streaming;
using Microsoft.XLANGs.BaseTypes;
using Moq;
using NUnit.Framework;
using Any = Be.Stateless.BizTalk.Schemas.Xml.Any;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public abstract class XsltRunnerComponentFixtureBase<T> : PipelineComponentFixture<T> where T : XsltRunnerComponent, new()
	{
		#region Setup/Teardown

		[SetUp]
		public void Backup()
		{
			_dataStream = new StringStream("<root xmlns='urn:ns'></root>");
			MessageMock.Object.BodyPart.Data = _dataStream;

			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("urn:ns#root"))
				.Returns(Schema<Any>.DocumentSpec);
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("urn-one#letter"))
				.Returns(Schema<Any>.DocumentSpec);

			_proberFactory = StreamExtensions.StreamProberFactory;
			_transformerFactory = StreamExtensions.StreamTransformerFactory;
		}

		[TearDown]
		public void Restore()
		{
			_dataStream.Dispose();

			StreamExtensions.StreamProberFactory = _proberFactory;
			StreamExtensions.StreamTransformerFactory = _transformerFactory;
		}

		#endregion

		[Test]
		public void EncodingDefaultsToUtf8WithoutSignature()
		{
			var sut = CreatePipelineComponent();
			Assert.That(sut.Encoding, Is.EqualTo(new UTF8Encoding()));
		}

		[Test]
		public void ReplacesMessageOriginalDataStreamWithTransformResult()
		{
			var sut = CreatePipelineComponent();
			sut.Enabled = true;
			sut.Encoding = Encoding.UTF8;
			sut.Map = typeof(IdentityTransform);

			using (var transformedStream = _dataStream.Transform().Apply(sut.Map))
			{
				var transformStreamMock = new Mock<ITransformStream>(MockBehavior.Strict);
				StreamExtensions.StreamTransformerFactory = stream => transformStreamMock.Object;
				transformStreamMock
					.Setup(ts => ts.ExtendWith(MessageMock.Object.Context))
					.Returns(transformStreamMock.Object);
				transformStreamMock
					.Setup(ts => ts.Apply(sut.Map, sut.Encoding))
					.Returns(transformedStream)
					.Verifiable();

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				transformStreamMock.VerifyAll();

				Assert.That(MessageMock.Object.BodyPart.Data, Is.TypeOf<MarkableForwardOnlyEventingReadStream>());
				// ReSharper disable once PossibleInvalidCastException
				Assert.That(Reflector.GetField((MarkableForwardOnlyEventingReadStream) MessageMock.Object.BodyPart.Data, "m_data"), Is.SameAs(transformedStream));
			}
		}

		[Test]
		public void XsltEntailsMessageTypeIsPromoted()
		{
			var sut = CreatePipelineComponent();
			sut.Enabled = true;
			sut.Encoding = Encoding.UTF8;
			sut.Map = typeof(IdentityTransform);

			PipelineContextMock
				.Setup(m => m.GetDocumentSpecByType("urn:ns#root"))
				.Returns(Schema<Batch.Content>.DocumentSpec);

			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, Schema<Batch.Content>.MessageType));
			MessageMock.Verify(m => m.Promote(BtsProperties.SchemaStrongName, new SchemaMetadata<Batch.Content>().DocumentSpec.DocSpecStrongName));
		}

		[Test]
		public void XsltEntailsMessageTypeIsPromotedOnlyIfOutputMethodIsXml()
		{
			var sut = CreatePipelineComponent();
			sut.Enabled = true;
			sut.Encoding = Encoding.UTF8;
			sut.Map = typeof(AnyToText);

			PipelineContextMock
				.Setup(m => m.GetDocumentSpecByType("urn:ns#root"))
				.Returns(Schema<Batch.Content>.DocumentSpec);

			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, Schema<Batch.Content>.MessageType), Times.Never());
			MessageMock.Verify(m => m.Promote(BtsProperties.SchemaStrongName, new SchemaMetadata<Batch.Content>().DocumentSpec.DocSpecStrongName), Times.Never());
		}

		[Test]
		public void XsltFromContextHasPrecedenceOverConfiguredOne()
		{
			var sut = CreatePipelineComponent();
			sut.Enabled = true;
			sut.Encoding = Encoding.UTF8;
			sut.Map = typeof(TransformBase);

			var transform = typeof(IdentityTransform);

			using (var transformedStream = _dataStream.Transform().Apply(transform))
			{
				MessageMock
					.Setup(m => m.GetProperty(BizTalkFactoryProperties.MapTypeName))
					.Returns(Transform<IdentityTransform>.MapTypeName);

				var transformStreamMock = new Mock<ITransformStream>(MockBehavior.Strict);
				StreamExtensions.StreamTransformerFactory = stream => transformStreamMock.Object;
				transformStreamMock
					.Setup(ts => ts.ExtendWith(MessageMock.Object.Context))
					.Returns(transformStreamMock.Object);
				transformStreamMock
					.Setup(ts => ts.Apply(transform, sut.Encoding))
					.Returns(transformedStream)
					.Verifiable();

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				// ReSharper disable once ImplicitlyCapturedClosure
				transformStreamMock.Verify(ts => ts.Apply(sut.Map, sut.Encoding), Times.Never());
				transformStreamMock.VerifyAll();
			}
		}

		static XsltRunnerComponentFixtureBase()
		{
			// PipelineComponentFixture<XsltRunnerComponent> assumes and needs the following converters
			TypeDescriptor.AddAttributes(
				typeof(Encoding),
				new Attribute[] {
					new TypeConverterAttribute(typeof(EncodingConverter))
				});
			TypeDescriptor.AddAttributes(
				typeof(Type),
				new Attribute[] {
					new TypeConverterAttribute(typeof(TypeNameConverter))
				});
		}

		protected override object GetValueForProperty(string name)
		{
			switch (name)
			{
				case "Encoding":
					return Encoding.GetEncoding("iso-8859-1");
				case "Map":
					return typeof(TransformBase);
				default:
					return base.GetValueForProperty(name);
			}
		}

		private Func<MarkableForwardOnlyEventingReadStream, IProbeStream> _proberFactory;
		private Func<Stream[], ITransformStream> _transformerFactory;
		private Stream _dataStream;
	}
}

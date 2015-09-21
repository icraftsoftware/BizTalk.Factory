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

using System.IO;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.Transforms.ToXml;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.BizTalk.Unit.Transform;
using Be.Stateless.IO;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	public class EnvelopeBuilderFixture : XsltRunnerFixture
	{
		[Test]
		public void BatchContentIsTransformedToSpecificEnvelope()
		{
			string actualEnvelopeContent;
			string expectedEnvelopeContent;
			var sut = new EnvelopeBuilder();

			var envelopeStream = new StringStream(MessageFactory.CreateEnvelope<Envelope>().OuterXml);
			var batchStream = new StringStream(MessageFactory.CreateMessage<Batch.Content>(ResourceManager.LoadString("Data.BatchContent.xml")).OuterXml);
			var transformedStream = new Stream[] { envelopeStream, batchStream }.Transform().Apply(sut.MapType, sut.Encoding);
			using (var expectedReader = XmlReader.Create(transformedStream, new XmlReaderSettings { CloseInput = true }))
			{
				expectedReader.Read();
				expectedEnvelopeContent = expectedReader.ReadOuterXml();
			}

			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(Schema<Envelope>.MessageType))
				.Returns(Schema<Envelope>.DocumentSpec);

			using (var dataStream = ResourceManager.Load("Data.BatchContent.xml"))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
				using (var actualReader = XmlReader.Create(MessageMock.Object.BodyPart.Data, new XmlReaderSettings { CloseInput = true, IgnoreWhitespace = true }))
				{
					actualReader.Read();
					actualEnvelopeContent = actualReader.ReadOuterXml();
				}
			}

			Assert.That(actualEnvelopeContent, Is.EqualTo(expectedEnvelopeContent));
		}

		[Test]
		public void FailsWhenMessageIsNotBatchContent()
		{
			var probeStreamMock = new Mock<IProbeBatchContentStream>().As<IProbeStream>();
			StreamExtensions.StreamProberFactory = stream => probeStreamMock.Object;

			var transformStreamMock = new Mock<ITransformStream>();
			StreamExtensions.StreamTransformerFactory = stream => transformStreamMock.Object;

			var sut = new EnvelopeBuilder();

			Assert.That(
				() => sut.Execute(PipelineContextMock.Object, MessageMock.Object),
				Throws.InvalidOperationException.With.Message.EqualTo(
					string.Format(
						"No EnvelopeSpecName has been found in {0} message and no envelope can be applied.",
						typeof(Batch.Content).Name)));
		}

		[Test]
		public void MapDefaultsToBatchContentToAnyEnvelope()
		{
			Assert.That(new EnvelopeBuilder().MapType, Is.SameAs(typeof(BatchContentToAnyEnvelope)));
		}

		[Test]
		public void MessageIsProbedForBatchDescriptor()
		{
			var probeStreamMock = new Mock<IProbeStream>();
			var probeBatchContentStreamMock = probeStreamMock.As<IProbeBatchContentStream>();
			StreamExtensions.StreamProberFactory = stream => probeStreamMock.Object;
			probeStreamMock
				.Setup(ps => ps.MessageType)
				.Returns(Schema<Envelope>.MessageType)
				.Verifiable();
			probeBatchContentStreamMock
				.Setup(ps => ps.BatchDescriptor)
				.Returns(new BatchDescriptor { EnvelopeSpecName = new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName })
				.Verifiable();

			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(Schema<Envelope>.MessageType))
				.Returns(Schema<Envelope>.DocumentSpec);

			using (var dataStream = ResourceManager.Load("Data.BatchContent.xml"))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				var sut = new EnvelopeBuilder();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
			}

			probeStreamMock.VerifyAll();
			probeBatchContentStreamMock.VerifyAll();
		}

		[Test]
		public void MessageIsTransformedToEnvelope()
		{
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("urn:ns#root"))
				.Returns(Schema<Any>.DocumentSpec);

			var sut = new EnvelopeBuilder();

			using (var dataStream = ResourceManager.Load("Data.BatchContent.xml"))
			using (var transformedStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = dataStream;

				var transformStreamMock = new Mock<ITransformStream>(MockBehavior.Strict);
				StreamExtensions.StreamTransformerFactory = stream => transformStreamMock.Object;
				transformStreamMock
					.Setup(ts => ts.ExtendWith(MessageMock.Object.Context))
					.Returns(transformStreamMock.Object)
					.Verifiable();
				transformStreamMock
					.Setup(ts => ts.Apply(typeof(BatchContentToAnyEnvelope), sut.Encoding))
					.Returns(transformedStream)
					.Verifiable();

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				transformStreamMock.VerifyAll();
			}
		}

		[Test]
		public void MessageIsTransformedToEnvelopeViaEnvelopeSpecNameAnnotation()
		{
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("urn:ns#root"))
				.Returns(Schema<Any>.DocumentSpec);

			var sut = new EnvelopeBuilder();

			using (var inputStream = ResourceManager.Load("Data.BatchContentWithTransform.xml"))
			using (var transformedStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var transformStreamMock = new Mock<ITransformStream>(MockBehavior.Strict);
				StreamExtensions.StreamTransformerFactory = stream => transformStreamMock.Object;
				transformStreamMock
					.Setup(ts => ts.ExtendWith(MessageMock.Object.Context))
					.Returns(transformStreamMock.Object);
				transformStreamMock
					.Setup(ts => ts.Apply(typeof(IdentityTransform), sut.Encoding))
					.Returns(transformedStream)
					.Verifiable();

				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				transformStreamMock.VerifyAll();
			}
		}

		[Test]
		public void PartitionIsPromoted()
		{
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(Schema<Batch.Content>.MessageType))
				.Returns(Schema<Batch.Content>.DocumentSpec);
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(Schema<Envelope>.MessageType))
				.Returns(Schema<Envelope>.DocumentSpec);

			using (var dataStream = ResourceManager.Load("Data.BatchContent.xml"))
			{
				MessageMock.Object.BodyPart.Data = dataStream;
				var sut = new EnvelopeBuilder();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
			}

			MessageMock.Verify(m => m.Promote(BizTalkFactoryProperties.EnvelopePartition, "p-one"));
		}

		[Test]
		public override void ReplacesMessageOriginalDataStreamWithTransformResult()
		{
			var probeStreamMock = new Mock<IProbeStream>();
			var probeBatchContentStreamMock = probeStreamMock.As<IProbeBatchContentStream>();
			probeStreamMock
				.Setup(ps => ps.MessageType)
				.Returns(Schema<Envelope>.MessageType);
			probeBatchContentStreamMock
				.Setup(ps => ps.BatchDescriptor)
				.Returns(new BatchDescriptor { EnvelopeSpecName = new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName });
			StreamExtensions.StreamProberFactory = stream => probeStreamMock.Object;

			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(Schema<Envelope>.MessageType))
				.Returns(Schema<Envelope>.DocumentSpec);

			base.ReplacesMessageOriginalDataStreamWithTransformResult();
		}

		[Test]
		public override void XsltEntailsMessageTypeIsPromoted()
		{
			PipelineContextMock
				.Setup(m => m.GetDocumentSpecByType(Schema<Envelope>.MessageType))
				.Returns(Schema<Batch.Content>.DocumentSpec);

			var probeStreamMock = new Mock<IProbeStream>();
			var probeBatchContentStreamMock = probeStreamMock.As<IProbeBatchContentStream>();
			probeStreamMock
				.Setup(ps => ps.MessageType)
				.Returns(Schema<Envelope>.MessageType);
			probeBatchContentStreamMock
				.Setup(ps => ps.BatchDescriptor)
				.Returns(new BatchDescriptor { EnvelopeSpecName = new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName });
			StreamExtensions.StreamProberFactory = stream => probeStreamMock.Object;

			base.XsltEntailsMessageTypeIsPromoted();
		}

		[Test]
		public override void XsltEntailsMessageTypeIsPromotedOnlyIfOutputMethodIsXml()
		{
			var probeStreamMock = new Mock<IProbeStream>();
			var probeBatchContentStreamMock = probeStreamMock.As<IProbeBatchContentStream>();
			probeBatchContentStreamMock
				.Setup(ps => ps.BatchDescriptor)
				.Returns(new BatchDescriptor { EnvelopeSpecName = new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName });
			StreamExtensions.StreamProberFactory = stream => probeStreamMock.Object;

			base.XsltEntailsMessageTypeIsPromotedOnlyIfOutputMethodIsXml();
		}

		[Test]
		public override void XsltFromContextHasPrecedenceOverConfiguredOne()
		{
			var probeStreamMock = new Mock<IProbeStream>();
			var probeBatchContentStreamMock = probeStreamMock.As<IProbeBatchContentStream>();
			probeStreamMock
				.Setup(ps => ps.MessageType)
				.Returns(Schema<Envelope>.MessageType);
			probeBatchContentStreamMock
				.Setup(ps => ps.BatchDescriptor)
				.Returns(new BatchDescriptor { EnvelopeSpecName = new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName });
			StreamExtensions.StreamProberFactory = stream => probeStreamMock.Object;

			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(Schema<Envelope>.MessageType))
				.Returns(Schema<Envelope>.DocumentSpec);

			base.XsltFromContextHasPrecedenceOverConfiguredOne();
		}

		protected override XsltRunner CreateXsltRunner()
		{
			return new EnvelopeBuilder();
		}
	}
}

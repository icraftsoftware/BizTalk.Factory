#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class EnvelopeBuilderComponentFixture : XsltRunnerComponentFixtureBase<EnvelopeBuilderComponent>
	{
		#region Setup/Teardown

		[SetUp]
		public new void SetUp()
		{
			_dataStream = ResourceManager.Load("Data.BatchContent.xml");
			MessageMock.Object.BodyPart.Data = _dataStream;

			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("http://schemas.microsoft.com/BizTalk/2003/aggschema#Root"))
				.Returns(Schema<Batch.Content>.DocumentSpec);
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(Schema<Batch.Content>.MessageType))
				.Returns(Schema<Batch.Content>.DocumentSpec);
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(Schema<Envelope>.MessageType))
				.Returns(Schema<Envelope>.DocumentSpec);
		}

		[TearDown]
		public void TearDown()
		{
			_dataStream.Dispose();
		}

		#endregion

		[Test]
		public void BatchContentIsTransformedToSpecificEnvelope()
		{
			var sut = CreatePipelineComponent();

			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			var envelope = MessageFactory.CreateEnvelope<Envelope>().OuterXml;
			var envelopeStream = new StringStream(envelope);
			var batch = MessageFactory.CreateMessage<Batch.Content>(ResourceManager.LoadString("Data.BatchContent.xml")).OuterXml;
			var batchStream = new StringStream(batch);
			var transformedStream = new Stream[] { envelopeStream, batchStream }.Transform().Apply(sut.Map, sut.Encoding);
			using (var expectedReader = XmlReader.Create(transformedStream, new XmlReaderSettings { CloseInput = true }))
			using (var actualReader = XmlReader.Create(MessageMock.Object.BodyPart.Data, new XmlReaderSettings { CloseInput = true, IgnoreWhitespace = true }))
			{
				expectedReader.Read();
				var expected = expectedReader.ReadOuterXml();
				actualReader.Read();
				var actual = actualReader.ReadOuterXml();
				Assert.That(actual, Is.EqualTo(expected));
			}
		}

		[Test]
		public void FailsWhenMessageIsNotBatchContent()
		{
			var probeStreamMock = new Mock<IProbeBatchContentStream>().As<IProbeStream>();
			StreamExtensions.StreamProberFactory = stream => probeStreamMock.Object;

			var transformStreamMock = new Mock<ITransformStream>();
			StreamExtensions.StreamTransformerFactory = stream => transformStreamMock.Object;

			using (var inputStream = new MemoryStream(Encoding.Unicode.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = CreatePipelineComponent();
				Assert.That(
					() => sut.Execute(PipelineContextMock.Object, MessageMock.Object),
					Throws.InvalidOperationException.With.Message.EqualTo(
						string.Format(
							"No EnvelopeSpecName has been found in {0} message and no envelope can be applied.",
							typeof(Batch.Content).Name)));
			}
		}

		[Test]
		public void MapDefaultsToBatchContentToAnyEnvelope()
		{
			var sut = CreatePipelineComponent();
			Assert.That(sut.Map, Is.SameAs(typeof(BatchContentToAnyEnvelope)));
		}

		[Test]
		public void MessageIsProbedForBatchDescriptor()
		{
			var sut = CreatePipelineComponent();

			var probeStreamMock = new Mock<IProbeStream>();
			var probeBatchContentStreamMock = probeStreamMock.As<IProbeBatchContentStream>();
			StreamExtensions.StreamProberFactory = stream => probeStreamMock.Object;
			probeStreamMock
				.Setup(ps => ps.MessageType)
				.Returns(Schema<Envelope>.MessageType);
			probeBatchContentStreamMock
				.Setup(ps => ps.BatchDescriptor)
				.Returns(new BatchDescriptor { EnvelopeSpecName = new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName })
				.Verifiable();

			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			probeStreamMock.VerifyAll();
			probeBatchContentStreamMock.VerifyAll();
		}

		[Test]
		public void MessageIsTransformedToEnvelope()
		{
			var sut = CreatePipelineComponent();

			using (var transformedStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				var transformStreamMock = new Mock<ITransformStream>(MockBehavior.Strict);
				StreamExtensions.StreamTransformerFactory = stream => transformStreamMock.Object;
				transformStreamMock
					.Setup(ts => ts.ExtendWith(MessageMock.Object.Context))
					.Returns(transformStreamMock.Object);
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
			var sut = CreatePipelineComponent();

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
			var sut = CreatePipelineComponent();

			sut.Execute(PipelineContextMock.Object, MessageMock.Object);

			MessageMock.Verify(m => m.Promote(BizTalkFactoryProperties.EnvelopePartition, "p-one"));
		}

		private Stream _dataStream;
	}
}

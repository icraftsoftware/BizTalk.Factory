#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.XPath;
using Be.Stateless.IO;
using Be.Stateless.IO.Extensions;
using Microsoft.BizTalk.Streaming;
using Moq;
using NUnit.Framework;
using MicroPipelineComponentFixture = Be.Stateless.BizTalk.Unit.MicroComponent.MicroPipelineComponentFixture;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class ContextPropertyExtractorFixture : MicroPipelineComponentFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("urn:ns#root"))
				.Returns(Schema<Any>.DocumentSpec);
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("urn-one#letter"))
				.Returns(Schema<Any>.DocumentSpec);
			_schemaMetadataFactory = SchemaBaseExtensions.SchemaMetadataFactory;
		}

		[TearDown]
		public void TearDown()
		{
			SchemaBaseExtensions.SchemaMetadataFactory = _schemaMetadataFactory;
		}

		#endregion

		[Test]
		public void BuildPropertyExtractorCollectionGivesPrecedenceToSchemaExtractorsOverPipelineExtractors()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				var annotationsMock = new Mock<ISchemaAnnotations>();
				annotationsMock.Setup(am => am.Extractors).Returns(
					new PropertyExtractorCollection(
						new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/to", ExtractionMode.Demote),
						new XPathExtractor(TrackingProperties.Value2.QName, "/letter/*/salutations")));
				SchemaMetadataMock = new Mock<ISchemaMetadata>();
				SchemaMetadataMock.Setup(sm => sm.Annotations).Returns(annotationsMock.Object);
				SchemaBaseExtensions.SchemaMetadataFactory = type => SchemaMetadataMock.Object;

				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/from", ExtractionMode.Promote),
						new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph")
					}
				};
				var extractors = sut.BuildPropertyExtractorCollection(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(
					extractors,
					Is.EqualTo(
						new[] {
							new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/to", ExtractionMode.Demote),
							new XPathExtractor(TrackingProperties.Value2.QName, "/letter/*/salutations"),
							new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph")
						}));
			}
		}

		[Test]
		public void BuildPropertyExtractorCollectionYieldsPipelineExtractorsWhenNoSchemaExtractors()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/from", ExtractionMode.Promote),
						new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph")
					}
				};
				var extractors = sut.BuildPropertyExtractorCollection(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(extractors, Is.EqualTo(sut.Extractors));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void Deserialize()
		{
			var microPipelineComponentType = typeof(ContextPropertyExtractor);
			var xml = string.Format(
				"<mComponent name=\"{0}\"><Extractors /></mComponent>",
				microPipelineComponentType.AssemblyQualifiedName);
			using (var reader = XmlReader.Create(new StringStream(xml)))
			{
				Assert.That(() => reader.DeserializeMicroPipelineComponent(), Throws.Nothing);
			}
		}

		[Test]
		public void DoesNothingWhenNoSchemaNorPipelineExtractors()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<unknown></unknown>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#unknown");

				var sut = new ContextPropertyExtractor();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(MessageMock.Object.BodyPart.Data, Is.SameAs(inputStream));
				Assert.That(MessageMock.Object.BodyPart.Data, Is.Not.TypeOf<XPathMutatorStream>());
				PipelineContextMock.Verify(pc => pc.ResourceTracker, Times.Never());

				MessageMock.Verify(m => m.Context.Promote(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
				MessageMock.Verify(m => m.Context.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
			}
		}

		[Test]
		public void MessageContextManipulationsAreDelegatedToPropertyExtractors()
		{
			const string content = "<s1:letter xmlns:s1=\"urn-one\">"
				+ "<s1:headers><s1:subject>inquiry</s1:subject></s1:headers>"
				+ "<s1:body>"
				+ "<s1:paragraph>paragraph-one</s1:paragraph>"
				+ "<s1:footer>trail</s1:footer>"
				+ "</s1:body>" +
				"</s1:letter>";

			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
			{
				var propertyExtractorMock = new Mock<PropertyExtractor>(BizTalkFactoryProperties.SenderName.QName, ExtractionMode.Clear) { CallBase = true };
				var constantExtractorMock = new Mock<ConstantExtractor>(TrackingProperties.Value2.QName, "value2", ExtractionMode.Write) { CallBase = true };
				var xpathExtractorMock = new Mock<XPathExtractor>(TrackingProperties.Value1.QName, "/*[local-name()='letter']/*/*[local-name()='paragraph']", ExtractionMode.Write) {
					CallBase = true
				};

				var annotationsMock = new Mock<ISchemaAnnotations>();
				annotationsMock.Setup(am => am.Extractors).Returns(
					new PropertyExtractorCollection(
						propertyExtractorMock.Object,
						constantExtractorMock.Object
						));
				SchemaMetadataMock = new Mock<ISchemaMetadata>();
				SchemaMetadataMock.Setup(sm => sm.Annotations).Returns(annotationsMock.Object);
				SchemaBaseExtensions.SchemaMetadataFactory = type => SchemaMetadataMock.Object;

				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn-one#letter");

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new XPathExtractor(TrackingProperties.ProcessName.QName, "/*[local-name()='letter']/*/*[local-name()='subject']", ExtractionMode.Promote),
						xpathExtractorMock.Object,
						new XPathExtractor(TrackingProperties.Value3.QName, "/*[local-name()='letter']/*/*[local-name()='footer']")
					}
				};
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);
				MessageMock.Object.BodyPart.Data.Drain();

				propertyExtractorMock.Verify(pe => pe.Execute(MessageMock.Object.Context));
				constantExtractorMock.Verify(pe => pe.Execute(MessageMock.Object.Context));
				//var newValue = "paragraph-one";
				// TODO not supported by moq xpathExtractorMock.Verify(pe => pe.Execute(MessageMock.Object.Context, "paragraph-one", ref newValue));

				MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.SenderName, null));
				MessageMock.Verify(m => m.SetProperty(TrackingProperties.Value1, "paragraph-one"));
				MessageMock.Verify(m => m.SetProperty(TrackingProperties.Value2, "value2"));
				MessageMock.Verify(m => m.SetProperty(TrackingProperties.Value3, "trail"));
				MessageMock.Verify(m => m.Promote(TrackingProperties.ProcessName, "inquiry"));
			}
		}

		[Test]
		public void OriginalDataStreamIsNotWrappedWhenThereIsNoXPathExtractors()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new ConstantExtractor(TrackingProperties.ProcessName.QName, "process.name")
					}
				};
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(MessageMock.Object.BodyPart.Data, Is.SameAs(inputStream));
				Assert.That(MessageMock.Object.BodyPart.Data, Is.Not.TypeOf<XPathMutatorStream>());
				PipelineContextMock.Verify(pc => pc.ResourceTracker, Times.Never());
			}
		}

		[Test]
		public void OriginalDataStreamIsWrappedInXPathMutatorStreamWhenThereIsXPathExtractors()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/from", ExtractionMode.Promote)
					}
				};
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(MessageMock.Object.BodyPart.Data, Is.TypeOf<XPathMutatorStream>());
				PipelineContextMock.Verify(pc => pc.ResourceTracker, Times.AtLeastOnce);
			}
		}

		[Test]
		public void Serialize()
		{
			var component = new ContextPropertyExtractor();
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				component.Serialize(writer);
			}
			Assert.That(
				builder.ToString(),
				Is.EqualTo(
					string.Format(
						"<mComponent name=\"{0}\"><Extractors /></mComponent>",
						component.GetType().AssemblyQualifiedName)));
		}

		private Mock<ISchemaMetadata> SchemaMetadataMock { get; set; }

		private Func<Type, ISchemaMetadata> _schemaMetadataFactory;
	}
}

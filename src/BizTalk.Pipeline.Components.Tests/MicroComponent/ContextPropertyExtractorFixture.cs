﻿#region Copyright & License

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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.XPath;
using Be.Stateless.IO.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.MicroComponent
{
	[TestFixture]
	public class ContextPropertyExtractorFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			MessageMock = new Unit.Message.Mock<IBaseMessage> { DefaultValue = DefaultValue.Mock };

			PipelineContextMock = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			// default behaviour analogous to actual IPipelineContext implementation
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType(It.IsAny<string>()))
				.Callback<string>(t => { throw new COMException("Could not locate document specification with type: " + t); });
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
		public void BuildExtractorCollectionDoesNothing()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/from", ExtractionMode.Promote),
						new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph", ExtractionMode.Write)
					}
				};
				var extractors = sut.BuildExtractorCollection(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(extractors, Is.SameAs(sut.Extractors));
			}
		}

		[Test]
		public void BuildExtractorCollectionGivesPrecedenceToSchemaExtractorsOverPipelineExtractors()
		{
			SchemaMetadataMock = new Mock<ISchemaMetadata>();
			SchemaBaseExtensions.SchemaMetadataFactory = type => SchemaMetadataMock.Object;

			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/from", ExtractionMode.Promote),
						new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph", ExtractionMode.Write)
					}
				};

				var annotationsMock = new Mock<ISchemaAnnotations>();
				annotationsMock.Setup(am => am.Extractors).Returns(
					new[] {
						new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/to", ExtractionMode.Demote),
						new XPathExtractor(TrackingProperties.Value2.QName, "/letter/*/salutations", ExtractionMode.Write)
					});
				SchemaMetadataMock.Setup(sm => sm.Annotations).Returns(annotationsMock.Object);

				var extractors = sut.BuildExtractorCollection(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(
					extractors,
					Is.EqualTo(
						new[] {
							new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/to", ExtractionMode.Demote),
							new XPathExtractor(TrackingProperties.Value2.QName, "/letter/*/salutations", ExtractionMode.Write),
							new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph", ExtractionMode.Write)
						}));
			}
		}

		[Test]
		public void DoesNothingWhenCannotFindDocumentSpecification()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<unknown></unknown>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#unknown");

				var sut = new ContextPropertyExtractor();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(MessageMock.Object.BodyPart.Data, Is.SameAs(inputStream));
				PipelineContextMock.Verify(pc => pc.ResourceTracker, Times.Never());
				Assert.That(MessageMock.Object.BodyPart.Data, Is.Not.TypeOf<XPathMutatorStream>());

				MessageMock.Verify(m => m.Context.Promote(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
				MessageMock.Verify(m => m.Context.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
			}
		}

		[Test]
		public void DoesNothingWhenNoXPathExtractors()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;
				MessageMock.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("urn:ns#root");

				var sut = new ContextPropertyExtractor();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(MessageMock.Object.BodyPart.Data, Is.SameAs(inputStream));
				PipelineContextMock.Verify(pc => pc.ResourceTracker, Times.Never());
				Assert.That(MessageMock.Object.BodyPart.Data, Is.Not.TypeOf<XPathMutatorStream>());

				MessageMock.Verify(m => m.Context.Promote(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
				MessageMock.Verify(m => m.Context.Write(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never());
			}
		}

		[Test]
		public void UpdateMessageContext()
		{
			const string content = "<s1:letter xmlns:s1='urn-one'>" +
				"<s1:headers><s1:subject>inquiry</s1:subject></s1:headers>" +
				"<s1:body><s1:paragraph>paragraph-one</s1:paragraph></s1:body>" +
				"</s1:letter>";
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new XPathExtractor(TrackingProperties.ProcessName.QName, "/*[local-name()='letter']/*/*[local-name()='subject']", ExtractionMode.Promote),
					}
				};
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				MessageMock.Object.BodyPart.Data.Drain();

				MessageMock.Verify(m => m.Promote(TrackingProperties.ProcessName, "inquiry"));
			}
		}

		[Test]
		public void WrapsOriginalDataStreamInXPathMutatorStream()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<root xmlns='urn:ns'></root>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = new ContextPropertyExtractor {
					Extractors = new[] {
						new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/from", ExtractionMode.Promote),
					}
				};
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(MessageMock.Object.BodyPart.Data, Is.TypeOf<XPathMutatorStream>());
			}
		}

		private Mock<IPipelineContext> PipelineContextMock { get; set; }

		private Unit.Message.Mock<IBaseMessage> MessageMock { get; set; }

		private Mock<ISchemaMetadata> SchemaMetadataMock { get; set; }

		private Func<Type, ISchemaMetadata> _schemaMetadataFactory;
	}
}
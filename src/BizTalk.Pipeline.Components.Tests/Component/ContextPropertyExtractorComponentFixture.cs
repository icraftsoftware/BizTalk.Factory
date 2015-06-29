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

using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Unit.Component;
using Be.Stateless.BizTalk.XPath;
using Be.Stateless.IO.Extensions;
using Microsoft.BizTalk.Streaming;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class ContextPropertyExtractorComponentFixture : PipelineComponentFixture<ContextPropertyExtractorComponent>
	{
		#region Setup/Teardown

		[SetUp]
		public new void SetUp()
		{
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("urn:ns#root"))
				.Returns(Schema<Any>.DocumentSpec);
			PipelineContextMock
				.Setup(pc => pc.GetDocumentSpecByType("urn-one#letter"))
				.Returns(Schema<Any>.DocumentSpec);
		}

		#endregion

		[Test]
		public void DoesNothingCannotFindDocumentSpecification()
		{
			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("<unknown></unknown>")))
			{
				MessageMock.Object.BodyPart.Data = inputStream;

				var sut = CreatePipelineComponent();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

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

				var sut = CreatePipelineComponent();
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				//Assert.That(MessageMock.Object.BodyPart.Data, Is.SameAs(inputStream));
				//PipelineContextMock.Verify(pc => pc.ResourceTracker, Times.Never());
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

				var sut = CreatePipelineComponent();
				sut.Extractors = new XPathExtractorCollection {
					new XPathExtractor(TrackingProperties.ProcessName.QName, "/*[local-name()='letter']/*/*[local-name()='subject']", ExtractionMode.Promote),
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

				var sut = CreatePipelineComponent();
				sut.Extractors = new XPathExtractorCollection {
					new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/from", ExtractionMode.Promote),
				};
				sut.Execute(PipelineContextMock.Object, MessageMock.Object);

				Assert.That(MessageMock.Object.BodyPart.Data, Is.TypeOf<XPathMutatorStream>());
			}
		}

		static ContextPropertyExtractorComponentFixture()
		{
			// PipelineComponentFixture<ContextPropertyExtractorComponent> assumes and needs the following converter
			TypeDescriptor.AddAttributes(
				typeof(XPathExtractorCollection),
				new Attribute[] {
					new TypeConverterAttribute(typeof(XPathExtractorCollectionConverter))
				});
		}

		protected override object GetValueForProperty(string name)
		{
			switch (name)
			{
				case "Extractors":
					return new XPathExtractorCollection {
						new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/from", ExtractionMode.Promote),
						new XPathExtractor(BizTalkFactoryProperties.ReceiverName.QName, "/letter/*/to", ExtractionMode.Promote),
						new XPathExtractor(TrackingProperties.ProcessName.QName, "/letter/*/subject", ExtractionMode.Write),
						new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph", ExtractionMode.Write),
						new XPathExtractor(TrackingProperties.Value2.QName, "/letter/*/salutations", ExtractionMode.Write),
						new XPathExtractor(TrackingProperties.Value3.QName, "/letter/*/signature", ExtractionMode.Write),
					};
				default:
					return base.GetValueForProperty(name);
			}
		}
	}
}

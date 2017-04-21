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

using System.IO;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.XPath;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class PropertyExtractorCollectionSerializerSurrogateFixture
	{
		[Test]
		public void ReadXmlForExtractorPrecedence()
		{
			var xml = string.Format(
				"<Extractors>"
					+ "<san:Properties precedence='schemaOnly' xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 mode='clear'/>"
					+ "</san:Properties>"
					+ "</Extractors>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollectionSerializerSurrogate(new PropertyExtractorCollection());
				sut.ReadXml(reader);
				Assert.That(sut.Precedence, Is.EqualTo(ExtractorPrecedence.SchemaOnly));
			}
		}

		[Test]
		public void WriteXmlForExtractorPrecedence()
		{
			var xml = string.Format(
				"<s0:Properties precedence=\"pipelineOnly\" xmlns:s0=\"{0}\" xmlns:s1=\"urn\">"
					+ "<s1:Property1 xpath=\"*/some-node\" />"
					+ "</s0:Properties>",
				SchemaAnnotations.NAMESPACE);

			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				var sut = new PropertyExtractorCollectionSerializerSurrogate(
					new PropertyExtractorCollection(
						ExtractorPrecedence.PipelineOnly,
						new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node")));
				sut.WriteXml(writer);
			}

			Assert.That(builder.ToString(), Is.EqualTo(xml));
		}
	}
}

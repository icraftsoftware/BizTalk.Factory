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

using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.Schema;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.XPath
{
	[TestFixture]
	public class XPathExtractorEnumerableSerializerFixture
	{
		[Test]
		public void ReadXml()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>" + "<s0:Property1 xpath='*/some-node'/><s0:Property2 promoted='true' xpath='*/other-node'/></san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new XPathExtractorEnumerableSerializer();
				sut.ReadXml(reader);

				Assert.That(
					sut.Extractors,
					Is.EqualTo(
						new[] {
							new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
							new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote)
						}));
			}
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		public void ReadXmlThrowsWhenDuplicatePropertyToExtract()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>" + "<s0:PropertyName xpath='*'/><s0:PropertyName xpath='*'/></san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new XPathExtractorEnumerableSerializer();

				Assert.That(
					// ReSharper disable once AccessToDisposedClosure
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<XmlException>()
						.With.InnerException.Message.EqualTo("The following properties are declared multiple times: [urn:PropertyName]."));
			}
		}

		[Test]
		public void ReadXmlThrowsWhenPropertyToExtractHasEmptyXPath()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>" + "<s0:PropertyName xpath=''/></san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new XPathExtractorEnumerableSerializer();

				Assert.That(
					// ReSharper disable once AccessToDisposedClosure
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<Microsoft.BizTalk.XPath.XPathException>()
						.With.InnerException.Message.StartsWith("Bad Query string encoundered in XPath:"));
			}
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		public void ReadXmlThrowsWhenPropertyToExtractHasNoNamespace()
		{
			var xml = string.Format(
				"<san:Properties xmlns:san='{0}'>" + "<PropertyName xpath='*'/></san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new XPathExtractorEnumerableSerializer();

				Assert.That(
					// ReSharper disable once AccessToDisposedClosure
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<XmlException>()
						.With.InnerException.Message.EqualTo("The following properties are not associated with the target namespace URI of some property schema: [PropertyName]."));
			}
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		public void ReadXmlThrowsWhenPropertyToExtractHasNoXPath()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>" + "<s0:PropertyName /></san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new XPathExtractorEnumerableSerializer();

				Assert.That(
					// ReSharper disable once AccessToDisposedClosure
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<XmlException>()
						.With.InnerException.Message.EqualTo("Attribute 'xpath' was not found. Line 1, position 98."));
			}
		}

		[Test]
		public void ReadXmlThrowsWhenRootElementMissesNamespace()
		{
			const string xml = "<Properties xmlns:s0='urn'><s0:PropertyName xpath='*'/></Properties>";
			var expected = string.Format("Element 'Properties' with namespace name '{0}' was not found. Line 1, position 2.", SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new XPathExtractorEnumerableSerializer();

				Assert.That(
					// ReSharper disable once AccessToDisposedClosure
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<XmlException>()
						.With.InnerException.Message.EqualTo(expected));
			}
		}

		[Test]
		public void ReadXmlThrowsWhenRootElementNameIsInvalid()
		{
			const string xml = "<san:Extractors xmlns:s0='urn' xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'><s0:PropertyName xpath='*'/></san:Extractors>";
			var expected = string.Format("Element 'Properties' with namespace name '{0}' was not found. Line 1, position 2.", SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new XPathExtractorEnumerableSerializer();

				Assert.That(
					// ReSharper disable once AccessToDisposedClosure
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<XmlException>()
						.With.InnerException.Message.EqualTo(expected));
			}
		}

		[Test]
		public void ReadXmlThrowsWhenRootElementNamespaceIsInvalid()
		{
			const string xml = "<san:Properties xmlns:s0='urn' xmlns:san='urn:schemas.stateless.be:biztalk:2012:12:extractors'><s0:PropertyName xpath='*'/></san:Properties>";
			var expected = string.Format("Element 'Properties' with namespace name '{0}' was not found. Line 1, position 2.", SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new XPathExtractorEnumerableSerializer();

				Assert.That(
					// ReSharper disable once AccessToDisposedClosure
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<XmlException>()
						.With.InnerException.Message.EqualTo(expected));
			}
		}

		[Test]
		public void WriteXml()
		{
			var xml = string.Format(
				"<s0:Properties xmlns:s0=\"{0}\" xmlns:s1=\"urn\">"
					+ "<s1:Property1 xpath=\"*/some-node\" />"
					+ "<s1:Property2 promoted=\"true\" xpath=\"*/other-node\" />"
					+ "</s0:Properties>",
				SchemaAnnotations.NAMESPACE);

			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				var sut = new XPathExtractorEnumerableSerializer {
					Extractors = new[] {
						new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
						new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote)
					}
				};
				sut.WriteXml(writer);
			}

			Assert.That(builder.ToString(), Is.EqualTo(xml));
		}
	}
}

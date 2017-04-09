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

using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.XPath;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
	public class PropertyExtractorCollectionFixture
	{
		[Test]
		public void ReadXml()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 xpath='*/some-node'/>"
					+ "<s0:Property2 promoted='true' xpath='*/other-node'/>"
					+ "<s0:Property3 mode='write' value='constant'/>"
					+ "<s0:Property4 mode='clear'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				Assert.That(
					sut,
					Is.EqualTo(
						new[] {
							new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
							new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote),
							new ConstantExtractor(new XmlQualifiedName("Property3", "urn"), "constant", ExtractionMode.Write),
							new PropertyExtractor(new XmlQualifiedName("Property4", "urn"), ExtractionMode.Clear)
						}));
			}
		}

		[Test]
		public void ReadXmlForConstantExtractor()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 value='constant'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				Assert.That(
					sut,
					Is.EqualTo(
						new PropertyExtractor[] {
							new ConstantExtractor(new XmlQualifiedName("Property1", "urn"), "constant", ExtractionMode.Write)
						}));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForConstantExtractorThrowsWhenModeAttributeIsInvalid()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 mode='demote' value='constant'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Assert.That(
					() => sut.ReadXml(reader),
					Throws.ArgumentException.With.Message.StartsWith("ExtractionMode 'Demote' is not supported by ConstantExtractor."));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForConstantExtractorThrowsWhenValueIsEmpty()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 value=''/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Assert.That(
					() => sut.ReadXml(reader),
					Throws.ArgumentNullException.With.Message.StartsWith("Value cannot be null."));
			}
		}

		[Test]
		public void ReadXmlForConstantExtractorWithModeAttribute()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 mode='promote' value='constant'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				Assert.That(
					sut,
					Is.EqualTo(
						new PropertyExtractor[] {
							new ConstantExtractor(new XmlQualifiedName("Property1", "urn"), "constant", ExtractionMode.Promote)
						}));
			}
		}

		[Test]
		public void ReadXmlForConstantExtractorWithPromotedAttribute()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 promoted='true' value='constant'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				Assert.That(
					sut,
					Is.EqualTo(
						new PropertyExtractor[] {
							new ConstantExtractor(new XmlQualifiedName("Property1", "urn"), "constant", ExtractionMode.Promote)
						}));
			}
		}

		[Test]
		public void ReadXmlForExtractorPrecedence()
		{
			var xml = string.Format(
				"<san:Properties precedence='schemaOnly' xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 mode='clear'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				Assert.That(sut.Precedence, Is.EqualTo(ExtractorPrecedence.SchemaOnly));
			}
		}

		[Test]
		public void ReadXmlForPropertyExtractor()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 mode='clear'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				Assert.That(
					sut,
					Is.EqualTo(new[] { new PropertyExtractor(new XmlQualifiedName("Property1", "urn"), ExtractionMode.Clear) }));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForPropertyExtractorThrowsWhenModeAttributeIsInvalid()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 mode='promote'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Assert.That(
					() => sut.ReadXml(reader),
					Throws.ArgumentException.With.Message.StartsWith("Invalid ExtractionMode, only Clear and Ignore are supported for PropertyExtractor without a Value or an XPath."));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForPropertyExtractorThrowsWhenModeAttributeIsMissing()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Assert.That(
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>().With.Message.EqualTo("ExtractionMode is missing for PropertyExtractor without a Value or an XPath."));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForPropertyExtractorThrowsWhenPromotedAttributeIsPresent()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 promoted='true'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Assert.That(
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>().With.Message.EqualTo("ExtractionMode is missing for PropertyExtractor without a Value or an XPath."));
			}
		}

		[Test]
		public void ReadXmlForXPathExtractor()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property1 xpath='*/some-node'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				Assert.That(
					sut,
					Is.EqualTo(
						new PropertyExtractor[] {
							new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write)
						}));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlForXPathExtractorThrowsWhenXPathIsEmpty()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:PropertyName xpath=''/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Assert.That(
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<Microsoft.BizTalk.XPath.XPathException>()
						// ReSharper disable once StringLiteralTypo
						.With.InnerException.Message.StartsWith("Bad Query string encoundered in XPath:"));
			}
		}

		[Test]
		public void ReadXmlForXPathExtractorWithModeAttribute()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property3 mode='demote' xpath='*/extra-node'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				Assert.That(
					sut,
					Is.EqualTo(
						new PropertyExtractor[] {
							new XPathExtractor(new XmlQualifiedName("Property3", "urn"), "*/extra-node", ExtractionMode.Demote)
						}));
			}
		}

		[Test]
		public void ReadXmlForXPathExtractorWithPromotedAttribute()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:Property2 promoted='true' xpath='*/other-node'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				Assert.That(
					sut,
					Is.EqualTo(
						new PropertyExtractor[] {
							new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote)
						}));
			}
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlThrowsWhenDuplicatePropertyToExtract()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>"
					+ "<s0:PropertyName xpath='*'/><s0:PropertyName xpath='*'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Assert.That(
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<XmlException>()
						.With.InnerException.Message.EqualTo("The following properties are declared multiple times: [urn:PropertyName]."));
			}
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlThrowsWhenPropertyToExtractHasNoNamespace()
		{
			var xml = string.Format(
				"<san:Properties xmlns:san='{0}'>"
					+ "<PropertyName xpath='*'/>"
					+ "</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Assert.That(
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<XmlException>()
						.With.InnerException.Message.EqualTo("The following properties are not associated with the target namespace URI of some property schema: [PropertyName]."));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlThrowsWhenRootElementMissesNamespace()
		{
			const string xml = "<Properties xmlns:s0='urn'><s0:PropertyName xpath='*'/></Properties>";
			var expected = string.Format("Element 'Properties' with namespace name '{0}' was not found. Line 1, position 2.", SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Assert.That(
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<XmlException>()
						.With.InnerException.Message.EqualTo(expected));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlThrowsWhenRootElementNameIsInvalid()
		{
			const string xml = "<san:Extractors xmlns:s0='urn' xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'><s0:PropertyName xpath='*'/></san:Extractors>";
			var expected = string.Format("Element 'Properties' with namespace name '{0}' was not found. Line 1, position 2.", SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Assert.That(
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<XmlException>()
						.With.InnerException.Message.EqualTo(expected));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void ReadXmlThrowsWhenRootElementNamespaceIsInvalid()
		{
			const string xml = "<san:Properties xmlns:s0='urn' xmlns:san='urn:schemas.stateless.be:biztalk:2012:12:extractors'><s0:PropertyName xpath='*'/></san:Properties>";
			var expected = string.Format("Element 'Properties' with namespace name '{0}' was not found. Line 1, position 2.", SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				Assert.That(
					() => sut.ReadXml(reader),
					Throws.TypeOf<ConfigurationErrorsException>()
						.With.InnerException.TypeOf<XmlException>()
						.With.InnerException.Message.EqualTo(expected));
			}
		}

		[Test]
		public void ReadXmlWithoutProperties()
		{
			var xml = string.Format("<san:Properties xmlns:s0='urn' xmlns:san='{0}' />", SchemaAnnotations.NAMESPACE);

			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				var sut = new PropertyExtractorCollection();
				sut.ReadXml(reader);
				Assert.That(sut, Is.Empty);
			}
		}

		[Test]
		public void WriteXml()
		{
			var xml = string.Format(
				"<s0:Properties xmlns:s0=\"{0}\" xmlns:s1=\"urn\">"
					+ "<s1:Property1 xpath=\"*/some-node\" />"
					+ "<s1:Property2 mode=\"promote\" xpath=\"*/other-node\" />"
					+ "<s1:Property3 mode=\"promote\" value=\"constant\" />"
					+ "<s1:Property4 mode=\"clear\" />"
					+ "</s0:Properties>",
				SchemaAnnotations.NAMESPACE);

			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				var sut = new PropertyExtractorCollection(
					new[] {
						new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
						new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote),
						new ConstantExtractor(new XmlQualifiedName("Property3", "urn"), "constant", ExtractionMode.Promote),
						new PropertyExtractor(new XmlQualifiedName("Property4", "urn"), ExtractionMode.Clear),
					});
				sut.WriteXml(writer);
			}

			Assert.That(builder.ToString(), Is.EqualTo(xml));
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
				var sut = new PropertyExtractorCollection(
					new[] {
						new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write)
					}) {
						Precedence = ExtractorPrecedence.PipelineOnly
					};
				sut.WriteXml(writer);
			}

			Assert.That(builder.ToString(), Is.EqualTo(xml));
		}
	}
}

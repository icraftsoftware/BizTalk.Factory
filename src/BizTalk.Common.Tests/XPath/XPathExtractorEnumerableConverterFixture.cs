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

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Be.Stateless.BizTalk.Schema;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.XPath
{
	[TestFixture]
	public class XPathExtractorEnumerableConverterFixture
	{
		[Test]
		public void CanConvertFrom()
		{
			var sut = new XPathExtractorEnumerableConverter();
			Assert.That(sut.CanConvertFrom(typeof(string)));
			Assert.That(sut.CanConvertFrom(typeof(XElement)));
			Assert.That(sut.CanConvertFrom(typeof(IEnumerable<XElement>)));
		}

		[Test]
		public void CanConvertTo()
		{
			var sut = new XPathExtractorEnumerableConverter();
			Assert.That(sut.CanConvertTo(typeof(string)));
		}

		[Test]
		public void ConvertFrom()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>" + "<s0:Property1 xpath='*/some-node'/><s0:Property2 promoted='true' xpath='*/other-node'/></san:Properties>",
				SchemaAnnotations.NAMESPACE);

			var sut = new XPathExtractorEnumerableConverter();

			Assert.That(
				sut.ConvertFrom(xml),
				Is.EqualTo(
					new[] {
						new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
						new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote)
					}));
		}

		[Test]
		public void ConvertFromEmpty()
		{
			var sut = new XPathExtractorEnumerableConverter();

			Assert.That(
				sut.ConvertFrom(string.Empty),
				Is.SameAs(Enumerable.Empty<XPathExtractor>()));
		}

		[Test]
		public void ConvertFromNull()
		{
			var sut = new XPathExtractorEnumerableConverter();

			Assert.That(
				sut.ConvertFrom(null),
				Is.SameAs(Enumerable.Empty<XPathExtractor>()));
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		public void ConvertFromThrowsWhenDuplicatePropertyToExtract()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>" + "<s0:PropertyName xpath='*'/><s0:PropertyName xpath='*'/></san:Properties>",
				SchemaAnnotations.NAMESPACE);

			var sut = new XPathExtractorEnumerableConverter();

			Assert.That(
				() => sut.ConvertFrom(xml),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.InnerException.TypeOf<XmlException>()
					.With.InnerException.Message.EqualTo("The following properties are declared multiple times: [urn:PropertyName]."));
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		public void ConvertFromThrowsWhenPropertyToExtractHasEmptyXPath()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>" + "<s0:PropertyName xpath=''/></san:Properties>",
				SchemaAnnotations.NAMESPACE);

			var sut = new XPathExtractorEnumerableConverter();

			Assert.That(
				() => sut.ConvertFrom(xml),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.InnerException.TypeOf<XmlException>()
					.With.InnerException.Message.EqualTo(
						"The following properties are either not associated with the target namespace URI of some property schema " +
							"or have no XPath expression configured: [{urn}PropertyName]."));
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		public void ConvertFromThrowsWhenPropertyToExtractHasNoNamespace()
		{
			var xml = string.Format(
				"<san:Properties xmlns:san='{0}'>" + "<PropertyName xpath='*'/></san:Properties>",
				SchemaAnnotations.NAMESPACE);

			var sut = new XPathExtractorEnumerableConverter();

			Assert.That(
				() => sut.ConvertFrom(xml),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.InnerException.TypeOf<XmlException>()
					.With.InnerException.Message.EqualTo(
						"The following properties are either not associated with the target namespace URI of some property schema " +
							"or have no XPath expression configured: [PropertyName]."));
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		public void ConvertFromThrowsWhenPropertyToExtractHasNoXPath()
		{
			var xml = string.Format(
				"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>" + "<s0:PropertyName /></san:Properties>",
				SchemaAnnotations.NAMESPACE);

			var sut = new XPathExtractorEnumerableConverter();

			Assert.That(
				() => sut.ConvertFrom(xml),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.InnerException.TypeOf<XmlException>()
					.With.InnerException.Message.EqualTo(
						"The following properties are either not associated with the target namespace URI of some property schema " +
							"or have no XPath expression configured: [{urn}PropertyName]."));
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		public void ConvertFromThrowsWhenRootElementMissesNamespace()
		{
			const string xml = "<Properties xmlns:s0='urn'><s0:PropertyName xpath='*'/></Properties>";
			var expected = string.Format("Element \"Properties\" in namespace \"{0}\" was not found.", SchemaAnnotations.NAMESPACE);

			var sut = new XPathExtractorEnumerableConverter();

			Assert.That(
				() => sut.ConvertFrom(xml),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.InnerException.TypeOf<XmlException>()
					.With.InnerException.Message.EqualTo(expected));
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		public void ConvertFromThrowsWhenRootElementNameIsInvalid()
		{
			const string xml = "<san:Extractors xmlns:s0='urn' xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'><s0:PropertyName xpath='*'/></san:Extractors>";
			var expected = string.Format("Element \"Properties\" in namespace \"{0}\" was not found.", SchemaAnnotations.NAMESPACE);

			var sut = new XPathExtractorEnumerableConverter();

			Assert.That(
				() => sut.ConvertFrom(xml),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.InnerException.TypeOf<XmlException>()
					.With.InnerException.Message.EqualTo(expected));
		}

		[Test]
#if !DEBUG
		[Ignore("Only to be run in DEBUG configuration.")]
#endif
		public void ConvertFromThrowsWhenRootElementNamespaceIsInvalid()
		{
			const string xml = "<san:Properties xmlns:s0='urn' xmlns:san='urn:schemas.stateless.be:biztalk:2012:12:extractors'><s0:PropertyName xpath='*'/></san:Properties>";
			var expected = string.Format("Element \"Properties\" in namespace \"{0}\" was not found.", SchemaAnnotations.NAMESPACE);

			var sut = new XPathExtractorEnumerableConverter();

			Assert.That(
				() => sut.ConvertFrom(xml),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.InnerException.TypeOf<XmlException>()
					.With.InnerException.Message.EqualTo(expected));
		}

		[Test]
		public void ConvertTo()
		{
			var xml = string.Format(
				"<s0:Properties xmlns:s0=\"{0}\" xmlns:s1=\"urn\">"
					+ "<s1:Property1 xpath=\"*/some-node\" />"
					+ "<s1:Property2 promoted=\"true\" xpath=\"*/other-node\" />"
					+ "</s0:Properties>",
				SchemaAnnotations.NAMESPACE);

			var sut = new XPathExtractorEnumerableConverter();
			var extractorCollection = sut.ConvertFrom(xml);

			// ReSharper disable AssignNullToNotNullAttribute
			Assert.That(
				sut.ConvertTo(extractorCollection, typeof(string)),
				Is.EqualTo(xml));
			// ReSharper restore AssignNullToNotNullAttribute
		}

		[Test]
		public void ConvertToNull()
		{
			var sut = new XPathExtractorEnumerableConverter();

			Assert.That(
				sut.ConvertTo(Enumerable.Empty<XPathExtractor>(), typeof(string)),
				Is.Null);
		}
	}
}

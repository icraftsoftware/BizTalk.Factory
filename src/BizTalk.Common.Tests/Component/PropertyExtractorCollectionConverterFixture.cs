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

using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.XPath;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	public class PropertyExtractorCollectionConverterFixture
	{
		[Test]
		public void CanConvertFrom()
		{
			var sut = new PropertyExtractorCollectionConverter();
			Assert.That(sut.CanConvertFrom(typeof(string)));
		}

		[Test]
		public void CanConvertTo()
		{
			var sut = new PropertyExtractorCollectionConverter();
			Assert.That(sut.CanConvertTo(typeof(string)));
		}

		[Test]
		[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
		public void ConvertFrom()
		{
			var xml = string.Format(
				@"<san:Properties xmlns:s0='urn' xmlns:san='{0}'>
  <s0:Property1 xpath='*/some-node'/>
  <s0:Property2 promoted='true' xpath='*/other-node'/>
</san:Properties>",
				SchemaAnnotations.NAMESPACE);

			var sut = new PropertyExtractorCollectionConverter();
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
			var sut = new PropertyExtractorCollectionConverter();
			Assert.That(sut.ConvertFrom(string.Empty), Is.Empty);
		}

		[Test]
		public void ConvertFromNull()
		{
			var sut = new PropertyExtractorCollectionConverter();
			Assert.That(sut.ConvertFrom(null), Is.Empty);
		}

		[Test]
		[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
		public void ConvertTo()
		{
			var xml = string.Format(
				"<s0:Properties xmlns:s0=\"{0}\" xmlns:s1=\"urn\">"
					+ "<s1:Property1 xpath=\"*/some-node\" />"
					+ "<s1:Property2 mode=\"promote\" xpath=\"*/other-node\" />"
					+ "</s0:Properties>",
				SchemaAnnotations.NAMESPACE);

			var sut = new PropertyExtractorCollectionConverter();
			var extractorCollection = new PropertyExtractorCollection(
				new XPathExtractor(new XmlQualifiedName("Property1", "urn"), "*/some-node", ExtractionMode.Write),
				new XPathExtractor(new XmlQualifiedName("Property2", "urn"), "*/other-node", ExtractionMode.Promote));
			Assert.That(
				sut.ConvertTo(extractorCollection, typeof(string)),
				Is.EqualTo(xml));
		}

		[Test]
		public void ConvertToNull()
		{
			var sut = new PropertyExtractorCollectionConverter();
			Assert.That(sut.ConvertTo(new PropertyExtractorCollection(), typeof(string)), Is.Null);
		}
	}
}

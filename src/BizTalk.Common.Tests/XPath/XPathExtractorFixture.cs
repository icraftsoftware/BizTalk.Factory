#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Unit;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.XPath
{
	[TestFixture]
	[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
	public class XPathExtractorFixture
	{
		[Test]
		public void DemoteDoesNotThrowIfNewValueIsNullOrEmpty()
		{
			var messageMock = new MessageMock();
			var sut = new XPathExtractor(TrackingProperties.Value1.QName, "//value1", ExtractionMode.Demote);
			string newValue = null;
			Assert.That(() => sut.Execute(messageMock.Object.Context, "old", ref newValue), Throws.Nothing);
			Assert.That(newValue, Is.Null);
		}

		[Test]
		public void Equality()
		{
			Assert.That(
				new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write),
				Is.EqualTo(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write)));
		}

		[Test]
		public void InequalityOfExtractionMode()
		{
			Assert.That(
				new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write),
				Is.Not.EqualTo(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote)));
		}

		[Test]
		public void InequalityOfProperty()
		{
			Assert.That(
				new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write),
				Is.Not.EqualTo(new XPathExtractor(new XmlQualifiedName("prop2", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write)));
		}

		[Test]
		public void InequalityOfType()
		{
			Assert.That(
				new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write),
				Is.Not.EqualTo(new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote)));

			Assert.That(
				new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Promote),
				Is.Not.EqualTo(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write)));
		}

		[Test]
		public void InequalityOfXPathExpression()
		{
			Assert.That(
				new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'element']", ExtractionMode.Write),
				Is.Not.EqualTo(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "/*[local-name() = 'another']", ExtractionMode.Write)));
		}
	}
}

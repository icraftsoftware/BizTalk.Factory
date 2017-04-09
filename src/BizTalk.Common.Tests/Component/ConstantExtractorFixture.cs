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
using Be.Stateless.BizTalk.XPath;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
	public class ConstantExtractorFixture
	{
		[Test]
		public void Equality()
		{
			Assert.That(
				new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write),
				Is.EqualTo(new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write)));
		}

		[Test]
		public void InequalityOfExtractionMode()
		{
			Assert.That(
				new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write),
				Is.Not.EqualTo(new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Promote)));
		}

		[Test]
		public void InequalityOfProperty()
		{
			Assert.That(
				new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write),
				Is.Not.EqualTo(new ConstantExtractor(new XmlQualifiedName("prop2", "urn"), "value", ExtractionMode.Write)));
		}

		[Test]
		public void InequalityOfType()
		{
			Assert.That(
				new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write),
				Is.Not.EqualTo(new XPathExtractor(new XmlQualifiedName("prop", "urn"), "*/node", ExtractionMode.Promote)));

			Assert.That(
				new XPathExtractor(new XmlQualifiedName("prop", "urn"), "*/node", ExtractionMode.Promote),
				Is.Not.EqualTo(new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write)));
		}

		[Test]
		public void InequalityOfValue()
		{
			Assert.That(
				new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value", ExtractionMode.Write),
				Is.Not.EqualTo(new ConstantExtractor(new XmlQualifiedName("prop", "urn"), "value2", ExtractionMode.Write)));
		}
	}
}

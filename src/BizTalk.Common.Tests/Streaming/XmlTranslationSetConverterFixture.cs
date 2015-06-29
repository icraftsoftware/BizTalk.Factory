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
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Streaming
{
	[TestFixture]
	public class XmlTranslationSetConverterFixture
	{
		[Test]
		public void CanConvertFrom()
		{
			var sut = new XmlTranslationSetConverter();
			Assert.That(sut.CanConvertFrom(typeof(string)));
		}

		[Test]
		public void CanConvertTo()
		{
			var sut = new XmlTranslationSetConverter();
			Assert.That(sut.CanConvertTo(typeof(string)));
		}

		[Test]
		public void Deserialize()
		{
			Assert.That(
				XmlTranslationSetConverter.Deserialize(XML),
				Is.EqualTo(_translationSet));
		}

		[Test]
		public void DeserializeEmptyString()
		{
			Assert.That(
				XmlTranslationSetConverter.Deserialize(string.Empty),
				Is.EqualTo(XmlTranslationSet.Empty));
		}

		[Test]
		public void Serialize()
		{
			Assert.That(
				XmlTranslationSetConverter.Serialize(_translationSet),
				Is.EqualTo(XML.Replace(Environment.NewLine, string.Empty)));
		}

		private const string XML = @"<xt:XmlTranslations override=""true"" xmlns:xt=""" + XmlTranslationSet.NAMESPACE + @""">
<xt:NamespaceTranslation matchingPattern=""sourceUrnA"" replacementPattern=""targetUrnA"" />
<xt:NamespaceTranslation matchingPattern=""sourceUrnB"" replacementPattern=""targetUrnB"" />
</xt:XmlTranslations>";

		private static readonly XmlTranslationSet _translationSet = new XmlTranslationSet {
			Override = true,
			Items = new[] { new Xml.XmlNamespaceTranslation("sourceUrnA", "targetUrnA"), new Xml.XmlNamespaceTranslation("sourceUrnB", "targetUrnB") }
		};
	}
}

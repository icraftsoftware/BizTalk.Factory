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

using System.IO;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;

namespace Be.Stateless.Extensions
{
	[TestFixture]
	public class StringExtensionsFixture
	{
		[Test]
		[TestCase("name", true)]
		[TestCase("name.txt", true)]
		[TestCase("name-0.txt", true)]
		[TestCase("name_0.txt", true)]
		[TestCase(null, false)]
		[TestCase("", false)]
		[TestCase(@"\name.txt", false)]
		[TestCase(@"/name.txt", false)]
		public void IsFileName(string fileName, bool expectedPredicateResult)
		{
			Assert.That(fileName.IsFileName(), Is.EqualTo(expectedPredicateResult));
		}

		[Test]
		[TestCase("name", true)]
		[TestCase("name0", true)]
		[TestCase("name-0", true)]
		[TestCase("na-me", true)]
		[TestCase("na.me", true)]
		[TestCase("na.me.0", true)]
		[TestCase("ns0:name", true)]
		[TestCase("ns-0:name-0", true)]
		[TestCase(null, false)]
		[TestCase("", false)]
		[TestCase("-name", false)]
		[TestCase("ns:-name", false)]
		[TestCase(".name", false)]
		[TestCase("ns:.name", false)]
		[TestCase("0name", false)]
		[TestCase("ns:0name", false)]
		[TestCase("0ns:0name", false)]
		[TestCase(":name", false)]
		[TestCase(":name:name", false)]
		[TestCase("ns0::name", false)]
		[TestCase("ns:name:suffix", false)]
		public void IsQName(string qName, bool expectedPredicateResult)
		{
			Assert.That(qName.IsQName(), Is.EqualTo(expectedPredicateResult));
		}

		[Test]
		[TestCase(null, 3, "")]
		[TestCase("", 3, "")]
		[TestCase("123456", 3, "456")]
		[TestCase("123456", 9, "123456")]
		public void Right(string @string, int length, string expectedResult)
		{
			Assert.That(@string.Right(length), Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(null, 3, "")]
		[TestCase(null, -3, "")]
		[TestCase("", 3, "")]
		[TestCase("", -3, "")]
		[TestCase("123456", 3, "123")]
		[TestCase("123456", -3, "456")]
		[TestCase("123456", 9, "123456")]
		[TestCase("123456", -9, "123456")]
		public void SubstringEx(string @string, int length, string expectedResult)
		{
			Assert.That(@string.SubstringEx(length), Is.EqualTo(expectedResult));
		}

		[Test]
		public void ToQName()
		{
			const string content = @"<xsl:stylesheet version='1.0'
	xmlns:xsl='http://www.w3.org/1999/XSL/Transform'
	xmlns:bi='http://schemas.microsoft.com/Sql/2008/05/TableOp/dbo/BatchItems'
	xmlns:tt='http://schemas.microsoft.com/Sql/2008/05/Types/Tables/dbo'
	xmlns:bts='http:=//schemas.microsoft.com/BizTalk/2003/system-properties'
	xmlns:tp='urn:schemas.stateless.be:biztalk:properties:tracking:2012:04'>
</xsl:stylesheet>";

			var nsm = new XPathDocument(new StringReader(content)).CreateNavigator();
			nsm.MoveToFollowing(XPathNodeType.Element);

			Assert.That(
				"no-namespace".ToQName(nsm),
				Is.EqualTo(new XmlQualifiedName("no-namespace", null)));
			Assert.That(
				"bts:OutboundTransportLocation".ToQName(nsm),
				Is.EqualTo(new XmlQualifiedName("OutboundTransportLocation", "http:=//schemas.microsoft.com/BizTalk/2003/system-properties")));
			Assert.That(
				"tp:MessagingStepActivityID".ToQName(nsm),
				Is.EqualTo(new XmlQualifiedName("MessagingStepActivityID", "urn:schemas.stateless.be:biztalk:properties:tracking:2012:04")));
			Assert.That(
				() => ":MessagingStepActivityID".ToQName(nsm),
				Throws.ArgumentException.With.Message.EqualTo("':MessagingStepActivityID' is not a valid XML qualified name.\r\nParameter name: qName"));
		}

		[Test]
		[TestCase("0value", false, null, null)]
		[TestCase("value", true, "", "value")]
		[TestCase("ns:value", true, "ns", "value")]
		public void TryParseQName(string qName, bool success, string prefix, string localPart)
		{
			string actualPrefix, actualLocalPart;
			Assert.That(qName.TryParseQName(out actualPrefix, out actualLocalPart), Is.EqualTo(success));
			Assert.That(actualPrefix, Is.EqualTo(prefix));
			Assert.That(actualLocalPart, Is.EqualTo(localPart));
		}
	}
}

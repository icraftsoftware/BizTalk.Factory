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
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;

namespace Be.Stateless.Extensions
{
	[TestFixture]
	public class StringExtensionsFixture
	{
		[Test]
		public void IsFileName()
		{
			Assert.That("name".IsFileName());
			Assert.That("name.txt".IsFileName());
			Assert.That("name-0.txt".IsFileName());
			Assert.That("name_0.txt".IsFileName());

			Assert.That(((string) null).IsFileName(), Is.False);
			Assert.That("".IsFileName(), Is.False);
			Assert.That(@"\name.txt".IsFileName(), Is.False);
			Assert.That(@"/name.txt".IsFileName(), Is.False);
		}

		[Test]
		public void IsQName()
		{
			Assert.That("name".IsQName());
			Assert.That("name0".IsQName());
			Assert.That("name-0".IsQName());
			Assert.That("na-me".IsQName());
			Assert.That("na.me".IsQName());
			Assert.That("na.me.0".IsQName());
			Assert.That("ns0:name".IsQName());
			Assert.That("ns-0:name-0".IsQName());

			Assert.That(((string) null).IsQName(), Is.False);
			Assert.That("".IsQName(), Is.False);
			Assert.That("0name".IsQName(), Is.False);
			Assert.That("-name".IsQName(), Is.False);
			Assert.That("ns:-name".IsQName(), Is.False);
			Assert.That(".name".IsQName(), Is.False);
			Assert.That("ns:.name".IsQName(), Is.False);
			Assert.That("ns:0name".IsQName(), Is.False);
			Assert.That("0ns:0name".IsQName(), Is.False);
			Assert.That(":name".IsQName(), Is.False);
			Assert.That(":name:name".IsQName(), Is.False);
			Assert.That("ns0::name".IsQName(), Is.False);
			Assert.That("ns:name:suffix".IsQName(), Is.False);
		}

		[Test]
		public void Right()
		{
			Assert.That(((string) null).Right(3), Is.EqualTo(string.Empty));
			Assert.That("".Right(3), Is.EqualTo(string.Empty));
			Assert.That("123456".Right(3), Is.EqualTo("456"));
			Assert.That("123456".Right(9), Is.EqualTo("123456"));
		}

		[Test]
		public void SubstringEx()
		{
			Assert.That(((string) null).SubstringEx(3), Is.EqualTo(string.Empty));
			Assert.That(((string) null).SubstringEx(-3), Is.EqualTo(string.Empty));

			Assert.That("".SubstringEx(3), Is.EqualTo(string.Empty));
			Assert.That("".SubstringEx(-3), Is.EqualTo(string.Empty));

			Assert.That("123456".SubstringEx(3), Is.EqualTo("123"));
			Assert.That("123456".SubstringEx(-3), Is.EqualTo("456"));

			Assert.That("123456".SubstringEx(9), Is.EqualTo("123456"));
			Assert.That("123456".SubstringEx(-9), Is.EqualTo("123456"));
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
				Throws.ArgumentException.With.Message.EqualTo("':MessagingStepActivityID' is not a valid XML qualified name.\r\nParameter name: qname"));
		}
	}
}

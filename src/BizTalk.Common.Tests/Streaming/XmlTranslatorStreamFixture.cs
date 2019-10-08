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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Linq.Extensions;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Streaming
{
	[TestFixture]
	public class XmlTranslatorStreamFixture
	{
		[Test]
		public void AbsorbXmlDeclaration()
		{
			using (var reader = new StreamReader(
				new XmlTranslatorStream(
					XmlReader.Create(new StringReader(@"<?xml version='1.0'?><test att='22'>value</test>")),
					Encoding.Default,
					new XmlNamespaceTranslation[] { },
					XmlTranslationModes.AbsorbXmlDeclaration)))
			{
				Assert.That(reader.ReadToEnd(), Is.EqualTo("<test att=\"22\">value</test>"));
			}
		}

		[Test]
		public void ChangeXmlEncoding()
		{
			using (var reader = new StreamReader(
				new XmlTranslatorStream(
					XmlReader.Create(new StringReader(@"<?xml version='1.0'?><test att='22'>value</test>")),
					Encoding.GetEncoding("iso-8859-1"),
					new XmlNamespaceTranslation[] { },
					XmlTranslationModes.Default)))
			{
				Assert.That(reader.ReadToEnd(), Is.EqualTo("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><test att=\"22\">value</test>"));
			}
		}

		[Test]
		public void DoesNotAbsorbXmlDeclaration()
		{
			using (var reader = new StreamReader(
				new XmlTranslatorStream(
					XmlReader.Create(new StringReader(@"<?xml version='1.0'?><test att='22'>value</test>")),
					Encoding.UTF8,
					new XmlNamespaceTranslation[] { },
					XmlTranslationModes.Default)))
			{
				Assert.That(reader.ReadToEnd(), Is.EqualTo("<?xml version=\"1.0\" encoding=\"utf-8\"?><test att=\"22\">value</test>"));
			}
		}

		[Test]
		public void DoesNotOutputXmlDeclarationIfOriginallyMissing()
		{
			using (var reader = new StreamReader(
				new XmlTranslatorStream(
					XmlReader.Create(new StringReader(@"<test att='22'>value</test>")),
					Encoding.UTF8,
					new XmlNamespaceTranslation[] { },
					XmlTranslationModes.Default)))
			{
				Assert.That(reader.ReadToEnd(), Is.EqualTo("<test att=\"22\">value</test>"));
			}
		}

		[Test]
		public void ProcessAttributes()
		{
			using (var reader = XmlReader.Create(new StringReader(@"<test xmlns:ns='stuff' ns:att='22'>value</test>")))
			{
				var navigator = new XPathDocument(
					new XmlTranslatorStream(
						reader,
						Encoding.Default,
						new[] { new XmlNamespaceTranslation("stuff", "urn:test") },
						XmlTranslationModes.TranslateAttributeNamespace)).CreateNavigator();

				Assert.That(navigator.Select("/test/@ns:att", "ns=urn:test").Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void RemoveDefaultNamespace()
		{
			using (var reader = XmlReader.Create(new StringReader(@"<test xmlns='stuff' att='22'>value</test>")))
			{
				var navigator = new XPathDocument(
					new XmlTranslatorStream(
						reader,
						Encoding.Default,
						new[] { new XmlNamespaceTranslation("stuff", "") },
						XmlTranslationModes.Default)).CreateNavigator();

				Assert.That(navigator.Select("/test").Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void RemoveNamespace()
		{
			using (var reader = XmlReader.Create(new StringReader(@"<ns:test xmlns:ns='stuff' att='22'>value</ns:test>")))
			{
				var navigator = new XPathDocument(
					new XmlTranslatorStream(
						reader,
						Encoding.Default,
						new[] { new XmlNamespaceTranslation("stuff", "") },
						XmlTranslationModes.Default)).CreateNavigator();

				Assert.That(navigator.Select("/test").Count, Is.EqualTo(1));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "StringLiteralTypo")]
		public void RemoveVersionFromWcfLobNamespaces()
		{
			const string input = @"<Receive xmlns='http://Microsoft.LobServices.Sap/2007/03/Idoc/3/ANY_IDOC//701/Receive'>
	<idocData>
		<EDI_DC40 xmlns='http://Microsoft.LobServices.Sap/2007/03/Types/Idoc/3/ANY_IDOC//701'>
			<IDOCTYP xmlns='http://Microsoft.LobServices.Sap/2007/03/Types/Idoc/Common/'>ANY_IDOC</IDOCTYP>
		</EDI_DC40>
	</idocData>
</Receive>";

			using (var reader = XmlReader.Create(new StringReader(input)))
			{
				var navigator = new XPathDocument(
					new XmlTranslatorStream(
						reader,
						Encoding.Default,
						new[] {
							new XmlNamespaceTranslation(
								@"http://Microsoft\.LobServices\.Sap/2007/03(/Types)?/Idoc(?:/\d)/(\w+)/(?:/\d{3})(/\w+)?",
								"http://Microsoft.LobServices.Sap/2007/03$1/Idoc/$2$3")
						},
						XmlTranslationModes.Default)).CreateNavigator();

				Assert.That(navigator.Select("//s0:*", "s0=http://Microsoft.LobServices.Sap/2007/03/Idoc/ANY_IDOC/Receive").Count, Is.EqualTo(2));
				Assert.That(navigator.Select("//s1:*", "s1=http://Microsoft.LobServices.Sap/2007/03/Types/Idoc/ANY_IDOC").Count, Is.EqualTo(1));
				Assert.That(navigator.Select("//s2:*", "s2=http://Microsoft.LobServices.Sap/2007/03/Types/Idoc/Common/").Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ReplaceDefaultNamespace()
		{
			using (var reader = XmlReader.Create(new StringReader(@"<test xmlns='stuff' att='22'>value</test>")))
			{
				var navigator = new XPathDocument(
					new XmlTranslatorStream(
						reader,
						Encoding.Default,
						new[] { new XmlNamespaceTranslation("stuff", "urn:test") },
						XmlTranslationModes.Default)).CreateNavigator();

				Assert.That(navigator.Select("/s0:test", "s0=urn:test").Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ReplaceGlobalNamespace()
		{
			using (var reader = XmlReader.Create(new StringReader(@"<testField att='22'>value</testField>")))
			{
				var navigator = new XPathDocument(
					new XmlTranslatorStream(
						reader,
						Encoding.Default,
						new[] { new XmlNamespaceTranslation(string.Empty, "urn:test") },
						XmlTranslationModes.Default))
					.CreateNavigator();

				Assert.That(
					navigator.Select("/s0:testField", "s0=urn:test").Count,
					Is.EqualTo(1));
			}
		}

		[Test]
		public void ReplaceGlobalNamespaceWhenOtherNamespaceDeclarationsArePresent()
		{
			using (var reader = XmlReader.Create(new StringReader(@"<test><other xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' /></test>")))
			{
				var navigator = new XPathDocument(
					new XmlTranslatorStream(
						reader,
						Encoding.Default,
						new[] { new XmlNamespaceTranslation(string.Empty, "urn:test") },
						XmlTranslationModes.Default)).CreateNavigator();

				Assert.That(
					navigator.Select("/s0:test/s0:other/@xsi:nil", "s0=urn:test", "xsi=http://www.w3.org/2001/XMLSchema-instance").Count,
					Is.EqualTo(1));
			}
		}

		[Test]
		public void ReplaceNamespace()
		{
			using (var reader = XmlReader.Create(new StringReader(@"<ns:test xmlns:ns='stuff' att='22'>value</ns:test>")))
			{
				var navigator = new XPathDocument(
					new XmlTranslatorStream(
						reader,
						Encoding.Default,
						new[] { new XmlNamespaceTranslation("stuff", "urn:test") },
						XmlTranslationModes.Default)).CreateNavigator();

				Assert.That(navigator.Select("/s0:test", "s0=urn:test").Count, Is.EqualTo(1));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "StringLiteralTypo")]
		public void RestoreVersionInWcfLobNamespaces()
		{
			const string input = @"<Send xmlns='http://Microsoft.LobServices.Sap/2007/03/Idoc/ANY_IDOC/Send'>
	<idocData>
		<EDI_DC40 xmlns='http://Microsoft.LobServices.Sap/2007/03/Types/Idoc/ANY_IDOC'>
			<IDOCTYP xmlns='http://Microsoft.LobServices.Sap/2007/03/Types/Idoc/Common/'>ANY_IDOC</IDOCTYP>
		</EDI_DC40>
	</idocData>
</Send>";

			using (var reader = XmlReader.Create(new StringReader(input)))
			{
				var navigator = new XPathDocument(
					new XmlTranslatorStream(
						reader,
						Encoding.Default,
						new[] {
							new XmlNamespaceTranslation(
								@"http://Microsoft\.LobServices\.Sap/2007/03/((?:Types/)?Idoc(?!/Common))/(\w+)(/Send)?",
								"http://Microsoft.LobServices.Sap/2007/03/$1/3/$2//701$3")
						},
						XmlTranslationModes.Default)).CreateNavigator();

				Assert.That(navigator.Select("//s0:*", "s0=http://Microsoft.LobServices.Sap/2007/03/Idoc/3/ANY_IDOC//701/Send").Count, Is.EqualTo(2));
				Assert.That(navigator.Select("//s1:*", "s1=http://Microsoft.LobServices.Sap/2007/03/Types/Idoc/3/ANY_IDOC//701").Count, Is.EqualTo(1));
				Assert.That(navigator.Select("//s2:*", "s2=http://Microsoft.LobServices.Sap/2007/03/Types/Idoc/Common/").Count, Is.EqualTo(1));
			}
		}
	}

	internal static class XPathNavigatorExtensions
	{
		public static XPathNodeIterator Select(this XPathNavigator navigator, string xpath, params string[] namespaces)
		{
			var nsr = new XmlNamespaceManager(navigator.NameTable);
			namespaces
				.Select(namespaceWithPrefix => namespaceWithPrefix.Split('='))
				.Each(splitString => nsr.AddNamespace(splitString[0], splitString[1]));
			return navigator.Select(xpath, nsr);
		}
	}
}

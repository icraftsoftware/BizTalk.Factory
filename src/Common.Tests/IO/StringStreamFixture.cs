﻿#region Copyright & License

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
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Xml.Extensions;
using NUnit.Framework;

namespace Be.Stateless.IO
{
	[TestFixture]
	public class StringStreamFixture
	{
		[Test]
		public void PlainTextLengthIsByteCountPlusBom()
		{
			const string content = "Hello world! And some @#$%^&*éèöäñ";
			using (var stream = new StringStream(content))
			{
				Assert.That(stream.Length, Is.EqualTo(Encoding.Unicode.GetByteCount(content) + Encoding.Unicode.GetPreamble().Length));
			}
		}

		[Test]
		public void PlainTextRoundTripping()
		{
			const string content = "Hello world! And some @#$%^&*éèöäñ";
			using (var stream = new StringStream(content))
			{
				var reader = new StreamReader(stream);
				Assert.That(reader.ReadToEnd(), Is.EqualTo(content));
			}
		}

		[Test]
		public void Utf16EmbeddedResourceRoundTripping()
		{
			using (var stream = CreateXmlDocument(ResourceManager.LoadString("Data.utf-16.xml")).AsStream())
			{
				var actual = XDocument.Parse(stream.ReadToEnd());
				Assert.That(XNode.DeepEquals(actual, XDocument.Parse(ResourceManager.LoadString("Data.utf-16.xml"))));
			}
		}

		[Test]
		public void Utf16XmlReaderRoundTripping()
		{
			using (var stream = CreateXmlDocument(ResourceManager.LoadString("Data.utf-16.xml")).AsStream())
			using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings { CloseInput = true }))
			{
				xmlReader.MoveToContent();
				var actual = XDocument.Parse(xmlReader.ReadOuterXml());
				Assert.That(XNode.DeepEquals(actual, XDocument.Parse(ResourceManager.LoadString("Data.utf-16.xml"))));
			}
		}

		[Test]
		public void Utf8EmbeddedResourceRoundTripping()
		{
			using (var stream = CreateXmlDocument(ResourceManager.LoadString("Data.utf-8.xml")).AsStream())
			{
				var actual = XDocument.Parse(stream.ReadToEnd());
				Assert.That(XNode.DeepEquals(actual, XDocument.Parse(ResourceManager.LoadString("Data.utf-8.xml"))));
			}
		}

		[Test]
		public void Utf8XmlReaderRoundTripping()
		{
			using (var stream = CreateXmlDocument(ResourceManager.LoadString("Data.utf-8.xml")).AsStream())
			using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings { CloseInput = true }))
			{
				xmlReader.MoveToContent();
				var actual = XDocument.Parse(xmlReader.ReadOuterXml());
				Assert.That(XNode.DeepEquals(actual, XDocument.Parse(ResourceManager.LoadString("Data.utf-8.xml"))));
			}
		}

		[Test]
		public void XmlTextLengthIsByteCountPlusBom()
		{
			const string content = "<root><node>content</node></root>";
			using (var stream = new StringStream(content))
			{
				Assert.That(stream.Length, Is.EqualTo(Encoding.Unicode.GetByteCount(content) + Encoding.Unicode.GetPreamble().Length));
			}
		}

		[Test]
		public void XmlTextRoundTripping()
		{
			const string content = "<root><node>content</node></root>";
			using (var reader = XmlReader.Create(new StringStream(content), new XmlReaderSettings { CloseInput = true }))
			{
				reader.MoveToContent();
				Assert.That(reader.ReadOuterXml(), Is.EqualTo(content));
			}
		}

		private XmlDocument CreateXmlDocument(string xmlContent)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xmlContent);
			return xmlDocument;
		}
	}
}

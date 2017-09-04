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
using System.Linq;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Transforms.ToXml;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.BizTalk.Xml.Xsl;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using Be.Stateless.Reflection;
using Be.Stateless.Text.Extensions;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Xml
{
	[TestFixture]
	public class CompositeXmlReaderFixture
	{
		#region Setup/Teardown

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			// reason to do this at fixture setup time is to avoid the 1st XSLT test to pay the initialization cost of
			// these data structures, should one want to keep an eye on the timing reported by R# test runner

			_xslPart1 = MessageFactory.CreateEnvelope<Envelope>().OuterXml;
			_xslPart2 = MessageFactory.CreateMessage<Batch.Content>(ResourceManager.LoadString("Data.BatchContent.xml")).OuterXml;

			_xslTransformDescriptor = new XslCompiledTransformDescriptor(new XslCompiledTransformDescriptorBuilder(typeof(BatchContentToAnyEnvelope)));

			_xslNamespaceManager = new XmlNamespaceManager(new NameTable());
			_xslNamespaceManager.AddNamespace("env", new SchemaMetadata(typeof(Envelope)).TargetNamespace);
			_xslNamespaceManager.AddNamespace("tns", new SchemaMetadata(typeof(Batch.Release)).TargetNamespace);
		}

		#endregion

		[Test]
		public void AggregatesIndividualStreamsOfDifferentEncodings()
		{
			using (var part1 = new MemoryStream(_part1))
			using (var part2 = new MemoryStream(_part2))
			using (var part3 = new MemoryStream(_part3))
			using (var composite = CompositeXmlReader.Create(new[] { part1, part2, part3 }))
			{
				composite.Read();
				Assert.That(composite.ReadOuterXml(), Is.EqualTo(EXPECTED));
			}
		}

		[Test]
		public void AggregatesInitiatedXmlReaders()
		{
			var part1 = XmlReader.Create(new StringReader(PART_1), new XmlReaderSettings { CloseInput = true });
			var part2 = XmlReader.Create(new StringReader(PART_2), new XmlReaderSettings { CloseInput = true });
			var part3 = XmlReader.Create(new StringReader(PART_3), new XmlReaderSettings { CloseInput = true });

			part2.MoveToContent();

			using (var composite = CompositeXmlReader.Create(new[] { part1, part2, part3 }))
			{
				composite.MoveToContent();
				Assert.That(composite.ReadOuterXml(), Is.EqualTo(EXPECTED));
			}
		}

		[Test]
		public void AggregatesUninitiatedXmlReaders()
		{
			var part1 = XmlReader.Create(new StringReader(PART_1), new XmlReaderSettings { CloseInput = true });
			var part2 = XmlReader.Create(new StringReader(PART_2), new XmlReaderSettings { CloseInput = true });
			var part3 = XmlReader.Create(new StringReader(PART_3), new XmlReaderSettings { CloseInput = true });

			using (var composite = CompositeXmlReader.Create(new[] { part1, part2, part3 }))
			{
				composite.MoveToContent();
				Assert.That(composite.ReadOuterXml(), Is.EqualTo(EXPECTED));
			}
		}

		[Test]
		public void CompoundXmlReadersShareXmlNameTable()
		{
			var part1 = XmlReader.Create(new StringReader(PART_1), new XmlReaderSettings { CloseInput = true });
			var part2 = XmlReader.Create(new StringReader(PART_2), new XmlReaderSettings { CloseInput = true });
			var part3 = XmlReader.Create(new StringReader(PART_3), new XmlReaderSettings { CloseInput = true });

			using (var composite = CompositeXmlReader.Create(new[] { part1, part2, part3 }))
			{
				var outlineReader = (XmlReader) Reflector.GetField(composite, "_outlineReader");
				Assert.That(outlineReader.NameTable, Is.SameAs(composite.NameTable));

				var readers = (XmlReader[]) Reflector.GetField(composite, "_readers");
				Assert.That(readers.Select(r => r.NameTable), Is.All.SameAs(composite.NameTable));
			}
		}

		[Test]
		public void InputStreamsAreClosed()
		{
			var mock1 = new Mock<MemoryStream>(MockBehavior.Default, _part1) { CallBase = true };
			var mock2 = new Mock<MemoryStream>(MockBehavior.Default, _part2) { CallBase = true };
			var mock3 = new Mock<MemoryStream>(MockBehavior.Default, _part3) { CallBase = true };

			using (CompositeXmlReader.Create(new[] { mock1.Object, mock2.Object, mock3.Object }, new XmlReaderSettings { CloseInput = true })) { }

			mock1.Verify(s => s.Close());
			mock2.Verify(s => s.Close());
			mock3.Verify(s => s.Close());
		}

		[Test]
		public void SupportsXslTransformOfAggregatedStreams()
		{
			var part1 = new StringStream(_xslPart1);
			var part2 = new StringStream(_xslPart2);

			var builder = new StringBuilder();
			using (var sut = CompositeXmlReader.Create(new[] { part1, part2 }))
			using (var writer = XmlWriter.Create(builder))
			{
				_xslTransformDescriptor.XslCompiledTransform.Transform(sut, _xslTransformDescriptor.Arguments, writer);
			}

			var xdoc = new XmlDocument();
			xdoc.Load(builder.GetReaderAtContent());
			Assert.That(xdoc.SelectSingleNode("/*").IfNotNull(n => n.LocalName), Is.EqualTo("Envelope"));
			Assert.That(xdoc.SelectNodes("/env:Envelope/tns:ReleaseBatch", _xslNamespaceManager).IfNotNull(n => n.Count), Is.EqualTo(3));
		}

		[Test]
		public void SupportsXslTransformOfAggregatedXmlReaders()
		{
			var part1 = new StringStream(_xslPart1);
			var part2 = new StringStream(_xslPart2);

			var builder = new StringBuilder();
			using (var sut = CompositeXmlReader.Create(new[] { XmlReader.Create(part1), XmlReader.Create(part2) }))
			using (var writer = XmlWriter.Create(builder))
			{
				_xslTransformDescriptor.XslCompiledTransform.Transform(sut, _xslTransformDescriptor.Arguments, writer);
			}

			var xdoc = new XmlDocument();
			xdoc.Load(builder.GetReaderAtContent());
			Assert.That(xdoc.SelectSingleNode("/*").IfNotNull(n => n.LocalName), Is.EqualTo("Envelope"));
			Assert.That(xdoc.SelectNodes("/env:Envelope/tns:ReleaseBatch", _xslNamespaceManager).IfNotNull(n => n.Count), Is.EqualTo(3));
		}

		private readonly byte[] _part1 = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + PART_1);
		private readonly byte[] _part2 = Encoding.Unicode.GetBytes("<?xml version=\"1.0\" encoding=\"utf-16\" ?>" + PART_2);
		private readonly byte[] _part3 = Encoding.GetEncoding("iso-8859-1").GetBytes("<?xml version=\"1.0\" encoding=\"iso-8859-1\" ?>" + PART_3);

		private string _xslPart1;
		private string _xslPart2;
		private XslCompiledTransformDescriptor _xslTransformDescriptor;
		private XmlNamespaceManager _xslNamespaceManager;

		private const string PART_1 = "<part-one xmlns=\"part-one\"><child-one>one</child-one></part-one>";

		private const string PART_2 = "<part-two xmlns=\"part-two\"><child-two>two</child-two></part-two>";

		private const string PART_3 = "<part-six xmlns=\"part-six\"><child-six>six</child-six></part-six>";

		private const string EXPECTED = "<agg:Root xmlns:agg=\"http://schemas.microsoft.com/BizTalk/2003/aggschema\">" +
			"<agg:InputMessagePart_0>" + PART_1 + "</agg:InputMessagePart_0>" +
			"<agg:InputMessagePart_1>" + PART_2 + "</agg:InputMessagePart_1>" +
			"<agg:InputMessagePart_2>" + PART_3 + "</agg:InputMessagePart_2></agg:Root>";
	}
}

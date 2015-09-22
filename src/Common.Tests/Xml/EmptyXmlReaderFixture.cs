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

using System;
using System.Xml;
using NUnit.Framework;

namespace Be.Stateless.Xml
{
	[TestFixture]
	public class EmptyXmlReaderFixture
	{
		[Test]
		public void MoveToContent()
		{
			var expected = new XmlDocument().CreateNavigator().ReadSubtree();
			var sut = EmptyXmlReader.Create();

			Assert.That(sut.ReadState, Is.EqualTo(expected.ReadState));
			Assert.That(sut.MoveToContent(), Is.EqualTo(expected.MoveToContent()));
			Assert.That(sut.ReadState, Is.EqualTo(expected.ReadState));
		}

		[Test]
		public void Read()
		{
			var expected = new XmlDocument().CreateNavigator().ReadSubtree();
			var sut = EmptyXmlReader.Create();

			Assert.That(sut.ReadState, Is.EqualTo(expected.ReadState));
			Assert.That(sut.Read(), Is.EqualTo(expected.Read()));
			Assert.That(sut.ReadState, Is.EqualTo(expected.ReadState));
		}

		[Test]
		public void ReadStateEndOfFileBehavior()
		{
			var expected = new XmlDocument().CreateNavigator().ReadSubtree();
			var sut = EmptyXmlReader.Create();

			Assert.That(sut.Read(), Is.EqualTo(expected.Read()));

			ValidateExpectedBehavior(sut, expected);
		}

		[Test]
		public void ReadStateInitialBehavior()
		{
			var expected = new XmlDocument().CreateNavigator().ReadSubtree();
			var sut = EmptyXmlReader.Create();

			ValidateExpectedBehavior(sut, expected);
		}

		[Test]
		public void ReadToEnd()
		{
			var expected = new XmlDocument().CreateNavigator().ReadSubtree();
			var sut = EmptyXmlReader.Create();
			using (expected)
			using (sut)
			{
				while (expected.Read()) { }
				while (sut.Read()) { }
				ValidateExpectedBehavior(sut, expected);
			}
			ValidateExpectedBehavior(sut, expected);
		}

		private void ValidateExpectedBehavior(XmlReader sut, XmlReader expected)
		{
			Assert.That(sut.ReadState, Is.EqualTo(expected.ReadState));
			Assert.That(sut.AttributeCount, Is.EqualTo(expected.AttributeCount));
			Assert.That(sut.BaseURI, Is.EqualTo(expected.BaseURI));
			Assert.That(sut.Depth, Is.EqualTo(expected.Depth));
			Assert.That(sut.EOF, Is.EqualTo(expected.EOF));
			Assert.That(sut.HasValue, Is.EqualTo(expected.HasValue));
			Assert.That(sut.IsEmptyElement, Is.EqualTo(expected.IsEmptyElement));
			Assert.That(sut.LocalName, Is.EqualTo(expected.LocalName));
			Assert.That(sut.NameTable, Is.Not.Null);
			Assert.That(sut.NamespaceURI, Is.EqualTo(expected.NamespaceURI));
			Assert.That(sut.NodeType, Is.EqualTo(expected.NodeType));
			Assert.That(sut.Prefix, Is.EqualTo(expected.Prefix));
			Assert.That(sut.Value, Is.EqualTo(expected.Value));

			Assert.That(expected.Close, Throws.Nothing);
			Assert.That(sut.Close, Throws.Nothing);
			Assert.That(() => expected.GetAttribute(0), Throws.TypeOf<ArgumentOutOfRangeException>());
			Assert.That(() => sut.GetAttribute(0), Throws.TypeOf<ArgumentOutOfRangeException>());
			Assert.That(sut.GetAttribute("name"), Is.EqualTo(expected.GetAttribute("name")));
			Assert.That(sut.GetAttribute("name", "ns"), Is.EqualTo(expected.GetAttribute("name", "ns")));
			Assert.That(sut.LookupNamespace("ns"), Is.EqualTo(expected.LookupNamespace("ns")));
			Assert.That(() => expected.MoveToAttribute(1), Throws.TypeOf<ArgumentOutOfRangeException>());
			Assert.That(() => sut.MoveToAttribute(1), Throws.TypeOf<ArgumentOutOfRangeException>());
			Assert.That(sut.MoveToAttribute("name"), Is.EqualTo(expected.MoveToAttribute("name")));
			Assert.That(sut.MoveToAttribute("name", "ns"), Is.EqualTo(expected.MoveToAttribute("name", "ns")));
			Assert.That(sut.MoveToElement(), Is.EqualTo(expected.MoveToElement()));
			Assert.That(sut.MoveToFirstAttribute(), Is.EqualTo(expected.MoveToFirstAttribute()));
			Assert.That(sut.MoveToNextAttribute(), Is.EqualTo(expected.MoveToNextAttribute()));
			Assert.That(sut.ReadAttributeValue(), Is.EqualTo(expected.ReadAttributeValue()));
			Assert.That(expected.ResolveEntity, Throws.TypeOf<InvalidOperationException>());
			Assert.That(sut.ResolveEntity, Throws.TypeOf<InvalidOperationException>());
		}
	}
}

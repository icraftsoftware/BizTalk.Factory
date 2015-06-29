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

using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Be.Stateless.Text.Extensions
{
	[TestFixture]
	public class StringBuilderExtensionsFixture
	{
		[Test]
		public void GetReaderAtContent()
		{
			using (var xmlReader = new StringBuilder("<part-one><child-one>one</child-one></part-one>").GetReaderAtContent())
			{
				Assert.That(xmlReader.ReadState, Is.EqualTo(ReadState.Interactive));
				Assert.That(xmlReader.NodeType, Is.EqualTo(XmlNodeType.Element));
				Assert.That(xmlReader.LocalName, Is.EqualTo("part-one"));
			}
		}

		[Test]
		public void GetReaderAtEmptyContent()
		{
			using (var xmlReader = new StringBuilder().GetReaderAtContent())
			{
				Assert.That(xmlReader.ReadState, Is.EqualTo(ReadState.EndOfFile));
				Assert.That(xmlReader.NodeType, Is.EqualTo(XmlNodeType.None));
				Assert.That(xmlReader.LocalName, Is.Empty);
			}
		}

		[Test]
		public void GetReaderAtPseudoEmptyContent()
		{
			using (var xmlReader = new StringBuilder("   \r\n    \r\n  ").GetReaderAtContent())
			{
				Assert.That(xmlReader.ReadState, Is.EqualTo(ReadState.EndOfFile));
				Assert.That(xmlReader.NodeType, Is.EqualTo(XmlNodeType.None));
				Assert.That(xmlReader.LocalName, Is.Empty);
			}
		}
	}
}

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
using Be.Stateless.IO;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking
{
	[TestFixture]
	public class ArchiveDescriptorFixture
	{
		[Test]
		public void ReadXml()
		{
			using (var reader = XmlReader.Create(new StringStream(string.Format("<{0} source=\"from\" target=\"to\" />", typeof(ArchiveDescriptor).Name))))
			{
				var sut = ArchiveDescriptor.Create(reader);
				Assert.That(sut.Source, Is.EqualTo("from"));
				Assert.That(sut.Target, Is.EqualTo("to"));
			}
		}

		[Test]
		public void ReadXmlFails()
		{
			using (var reader = XmlReader.Create(new StringStream(string.Format("<{0} source=\"from\" target=\"to\" />", typeof(ArchiveDescriptorFixture).Name))))
			{
				// ReSharper disable once AccessToDisposedClosure
				Assert.That(() => ArchiveDescriptor.Create(reader), Throws.InstanceOf<XmlException>().With.Message.StartsWith("Element 'ArchiveDescriptor' was not found."));
			}

			using (var reader = XmlReader.Create(new StringStream(string.Format("<{0} from=\"from\" to=\"to\" />", typeof(ArchiveDescriptor).Name))))
			{
				// ReSharper disable once AccessToDisposedClosure
				Assert.That(() => ArchiveDescriptor.Create(reader), Throws.InstanceOf<XmlException>().With.Message.StartsWith("Attribute 'source' was not found."));
			}
		}

		[Test]
		public void WriteXml()
		{
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				var sut = new ArchiveDescriptor("from", "to");
				sut.WriteXml(writer);
			}
			Assert.That(builder.ToString(), Is.EqualTo(string.Format("<{0} source=\"from\" target=\"to\" />", typeof(ArchiveDescriptor).Name)));
		}
	}
}

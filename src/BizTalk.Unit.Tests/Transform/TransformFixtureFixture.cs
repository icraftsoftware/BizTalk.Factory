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

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BTF2Schemas;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Unit.Resources;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	[TestFixture]
	public class TransformFixtureFixture : TransformFixture<IdentityTransform>
	{
		#region Setup/Teardown

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			AddNamespace("tns", new SchemaMetadata(typeof(btf2_services_header)).TargetNamespace);
		}

		#endregion

		[Test]
		public void InvalidTransformResultThrows()
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(MessageFactory.CreateMessage<btf2_services_header>().OuterXml)))
			{
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => Transform<btf2_services_header>(stream), Throws.InstanceOf<XmlSchemaValidationException>());
				// ReSharper restore AccessToDisposedClosure
			}
		}

		[Test]
		public void ScalarAssertion()
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(_document.OuterXml)))
			{
				var result = Transform<btf2_services_header>(stream);
				Assert.That(result.Single("//*[1]/tns:sendBy/text()").Value, Is.EqualTo("2012-04-12T12:13:14"));
			}
		}

		[Test]
		public void StringJoinAssertion()
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(_document.OuterXml)))
			{
				var result = Transform<btf2_services_header>(stream);
				Assert.That(result.StringJoin("//tns:sendBy"), Is.EqualTo("2012-04-12T12:13:14#2012-04-12T23:22:21"));
			}
		}

		[Test]
		public void ValidTransformResultDoesNotThrow()
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(_document.OuterXml)))
			{
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => Transform<btf2_services_header>(stream), Throws.Nothing);
				// ReSharper restore AccessToDisposedClosure
			}
		}

		[Test]
		public void XPathAssertion()
		{
			using (var stream = new MemoryStream(Encoding.Default.GetBytes(_document.OuterXml)))
			{
				var result = Transform<btf2_services_header>(stream);
				Assert.That(result.Select("//tns:sendBy").Count, Is.EqualTo(2));
			}
		}

		private readonly XmlDocument _document = MessageFactory.CreateMessage<btf2_services_header>(ResourceManager.LoadString("Data.Message.xml"));
	}
}

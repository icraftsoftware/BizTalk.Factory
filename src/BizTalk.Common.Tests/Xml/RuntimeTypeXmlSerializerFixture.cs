#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Unit.Transform;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Xml
{
	[TestFixture]
	public class RuntimeTypeXmlSerializerFixture
	{
		[Test]
		public void SerializeMapType()
		{
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				var sut = (RuntimeTypeXmlSerializer) typeof(IdentityTransform);
				var serializer = new XmlSerializer(typeof(RuntimeTypeXmlSerializer));
				serializer.Serialize(writer, sut);
			}

			Assert.That(builder.ToString(), Is.EqualTo(string.Format("<RuntimeTypeXmlSerializer>{0}</RuntimeTypeXmlSerializer>", typeof(IdentityTransform).AssemblyQualifiedName)));
		}

		[Test]
		public void SerializeNullType()
		{
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				var sut = (RuntimeTypeXmlSerializer) (Type) null;
				var serializer = new XmlSerializer(typeof(RuntimeTypeXmlSerializer));
				serializer.Serialize(writer, sut);
			}

			Assert.That(
				builder.ToString(),
				Is.EqualTo("<RuntimeTypeXmlSerializer />"));
		}
	}
}

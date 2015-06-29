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
using System.Xml;
using System.Xml.Schema;
using BTF2Schemas;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Unit.Resources;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Schema
{
	[TestFixture]
	public class SchemaFixtureFixture
	{
		[Test]
		public void LaxValidation()
		{
			Assert.That(
				() => new EnvelopeSchemaFixture().ValidateInstanceDocument(ResourceManager.Load("Data.Envelope.xml"), XmlSchemaContentProcessing.Lax),
				Throws.Nothing);
		}

		[Test]
		public void StrictValidation()
		{
			Assert.That(
				() => new EnvelopeSchemaFixture().ValidateInstanceDocument(ResourceManager.Load("Data.Envelope.xml"), XmlSchemaContentProcessing.Strict),
				Throws.InstanceOf<XmlSchemaValidationException>()
					.With.Message.EqualTo("Warning: Could not find schema information for the element 'nested'."));
		}

		[Test]
		public void ValidatingInvalidXmlDocumentThrows()
		{
			var instance = MessageFactory.CreateMessage<btf2_services_header>();
			Assert.That(
				() => new DocumentSchemaFixture().ValidateInstanceDocument(instance),
				Throws.InstanceOf<XmlSchemaValidationException>()
					.With.Message.StartsWith("Error: The 'http://schemas.biztalk.org/btf-2-0/services:sendBy' element is invalid - The value '' is invalid according to its datatype 'http://www.w3.org/2001/XMLSchema:dateTime'"));
		}

		[Test]
		public void ValidatingInvalidXmlFileThrows()
		{
			var instance = MessageFactory.CreateMessage<btf2_services_header>();
			var tempFileName = Path.GetTempFileName();
			instance.Save(tempFileName);
			Assert.That(
				() => new DocumentSchemaFixture().ValidateInstanceDocument(tempFileName),
				Throws.InstanceOf<XmlSchemaValidationException>()
					.With.Message.StartsWith("Error: The 'http://schemas.biztalk.org/btf-2-0/services:sendBy' element is invalid - The value '' is invalid according to its datatype 'http://www.w3.org/2001/XMLSchema:dateTime'"));
		}

		[Test]
		public void ValidatingValidXmlDoesNotThrow()
		{
			var instance = MessageFactory.CreateMessage<btf2_services_header>(ResourceManager.LoadString("Data.Message.xml"));
			Assert.That(
				() => new DocumentSchemaFixture().ValidateInstanceDocument(instance),
				Throws.Nothing);
		}

		private class EnvelopeSchemaFixture : SchemaFixture<Envelope>
		{
			public new XmlDocument ValidateInstanceDocument(Stream stream, XmlSchemaContentProcessing contentProcessing)
			{
				return base.ValidateInstanceDocument(stream, contentProcessing);
			}
		}

		private class DocumentSchemaFixture : SchemaFixture<btf2_services_header>
		{
			public new XmlDocument ValidateInstanceDocument(string filepath)
			{
				return base.ValidateInstanceDocument(filepath);
			}

			public new XmlDocument ValidateInstanceDocument(XmlDocument document)
			{
				return base.ValidateInstanceDocument(document);
			}
		}
	}
}

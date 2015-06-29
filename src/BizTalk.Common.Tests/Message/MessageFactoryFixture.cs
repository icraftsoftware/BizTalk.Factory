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

using System.Xml.Schema;
using BTF2Schemas;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Unit.Resources;
using Microsoft.BizTalk.Edi.BaseArtifacts;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Message
{
	[TestFixture]
	public class MessageFactoryFixture
	{
		[Test]
		public void CreateEnvelope()
		{
			Assert.That(
				MessageFactory.CreateEnvelope<ResendControlEnvelope>().OuterXml,
				Is.EqualTo(
					"<ns0:ControlMessage xmlns:ns0=\"http://schemas.microsoft.com/BizTalk/2006/reliability-properties\">" +
						"<ns:parts-here xmlns:ns=\"urn:schemas.stateless.be:biztalk:batch:2012:12\" />" +
						"</ns0:ControlMessage>"));

			Assert.That(
				MessageFactory.CreateEnvelope<Envelope>().OuterXml,
				Is.EqualTo(
					"<ns0:Envelope xmlns:ns0=\"urn:schemas.stateless.be:biztalk:envelope:2013:07\">" +
						"<ns:parts-here xmlns:ns=\"urn:schemas.stateless.be:biztalk:batch:2012:12\" />" +
						"</ns0:Envelope>"
					));
		}

		[Test]
		public void CreateEnvelopeWithContent()
		{
			Assert.That(
				() => MessageFactory.CreateEnvelope<ResendControlEnvelope, Batch.Release>(
					"<ns0:ControlMessage xmlns:ns0=\"http://schemas.microsoft.com/BizTalk/2006/reliability-properties\">" +
						"<ns:ReleaseBatch xmlns:ns=\"urn:schemas.stateless.be:biztalk:batch:2012:12\">" +
						"<ns:EnvelopeSpecName>Be.Stateless.BizTalk.Schemas.Xml.BatchControl+ReleaseBatches</ns:EnvelopeSpecName>" +
						"</ns:ReleaseBatch>" +
						"</ns0:ControlMessage>"),
				Throws.Nothing);
		}

		[Test]
		public void CreatingMessageBySchemaTypeNeverThrows()
		{
			Assert.That(() => MessageFactory.CreateMessage<btf2_services_header>(), Throws.Nothing);
			Assert.That(() => MessageFactory.CreateMessage(typeof(btf2_services_header)), Throws.Nothing);
		}

		[Test]
		public void CreatingMessageForNonSchemaTypeThrows()
		{
			Assert.That(
				() => MessageFactory.CreateMessage(typeof(int)),
				Throws.ArgumentException.With.Message.StartsWith("System.Int32 does not derive from Microsoft.XLANGs.BaseTypes.SchemaBase."));
		}

		[Test]
		public void CreatingMessageWithInvalidContentThrows()
		{
			var message = MessageFactory.CreateMessage<btf2_services_header>();
			Assert.That(
				() => MessageFactory.CreateMessage<btf2_services_header>(message.InnerXml),
				Throws.InstanceOf<XmlSchemaValidationException>());
		}

		[Test]
		public void CreatingMessageWithValidContentDoesNotThrow()
		{
			Assert.That(() => MessageFactory.CreateMessage<btf2_services_header>(ResourceManager.LoadString("Data.Message.xml")), Throws.Nothing);
		}
	}
}

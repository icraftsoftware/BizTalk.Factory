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

using Be.Stateless.BizTalk.Schemas.Xml;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Schema
{
	[TestFixture]
	public class SchemaMetadataFixture
	{
		[Test]
		public void GetBodyXPathForEnvelopeSchema()
		{
			Assert.That(
				new SchemaMetadata(typeof(Envelope)).BodyXPath,
				Is.EqualTo("/*[local-name()='Envelope' and namespace-uri()='urn:schemas.stateless.be:biztalk:envelope:2013:07']"));
		}

		[Test]
		public void GetBodyXPathForNonEnvelopeSchema()
		{
			Assert.That(
				new SchemaMetadata(typeof(Batch.Content)).BodyXPath,
				Is.Null);
		}

		[Test]
		public void GetMessageType()
		{
			Assert.That(
				new SchemaMetadata(typeof(Batch.Release)).MessageType,
				Is.EqualTo("urn:schemas.stateless.be:biztalk:batch:2012:12#ReleaseBatch"));
		}

		[Test]
		public void GetMessageTypeForTypeOnlySchema()
		{
			Assert.That(new SchemaMetadata(typeof(TypeSchema)).MessageType, Is.Null);
		}

		[Test]
		public void GetTargetNamespace()
		{
			Assert.That(
				new SchemaMetadata(typeof(Batch.Release)).TargetNamespace,
				Is.EqualTo("urn:schemas.stateless.be:biztalk:batch:2012:12"));
		}

		[Test]
		public void GetTargetNamespaceForTypeOnlySchema()
		{
			Assert.That(
				new SchemaMetadata(typeof(TypeSchema)).TargetNamespace,
				Is.EqualTo("urn:schemas.stateless.be:unit:type"));
		}

		[Test]
		public void IsEnvelopeSchema()
		{
			Assert.That(new SchemaMetadata(typeof(Batch.Content)).IsEnvelopeSchema, Is.False);
			Assert.That(new SchemaMetadata(typeof(Envelope)).IsEnvelopeSchema);
		}
	}
}

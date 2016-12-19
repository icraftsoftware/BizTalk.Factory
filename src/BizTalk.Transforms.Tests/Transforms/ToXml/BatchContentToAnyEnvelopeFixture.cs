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

using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.BizTalk.Unit.Transform;
using Be.Stateless.IO;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Transforms.ToXml
{
	[TestFixture]
	public class BatchContentToAnyEnvelopeFixture : TransformFixture<BatchContentToAnyEnvelope>
	{
		#region Setup/Teardown

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			AddNamespace("env", new SchemaMetadata(typeof(Envelope)).TargetNamespace);
			AddNamespace("tns", new SchemaMetadata(typeof(Batch.Release)).TargetNamespace);
		}

		#endregion

		[Test]
		public void ValidateTransform()
		{
			var templateTargetEnvelope = MessageFactory.CreateEnvelope<Envelope>();
			var batchContentWithParts = MessageFactory.CreateMessage<Batch.Content>(ResourceManager.LoadString("Data.BatchContent.xml"));

			var templateTargetEnvelopeStream = new StringStream(templateTargetEnvelope.OuterXml);
			var batchContentWithPartsStream = new StringStream(batchContentWithParts.OuterXml);

			var result = Transform<Envelope, Batch.Release>(templateTargetEnvelopeStream, batchContentWithPartsStream);

			Assert.That(result.Single("/*").LocalName, Is.EqualTo("Envelope"));
			Assert.That(result.Select("/env:Envelope/tns:ReleaseBatch").Count, Is.EqualTo(3));
		}
	}
}

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

using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Pipeline.Interpreters;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.BizTalk.Xml;
using NUnit.Framework;
using Winterdom.BizTalk.PipelineTesting;

namespace Be.Stateless.BizTalk.Pipelines
{
	[TestFixture]
	public class BatchReceiveFixture
	{
		[Test]
		public void TransformBatchContentToEnvelope()
		{
			using (var stream = ResourceManager.Load("Data.BatchContent.nopartition.xml"))
			{
				var inputMessage = MessageHelper.CreateFromStream(stream);
				inputMessage.Promote(BtsProperties.InboundTransportLocation, "mssql://localhost//BizTalkFactoryTransientStateDb");

				var pipeline = PipelineFactory.CreateReceivePipeline(typeof(ReceivePipelineInterpreter<BatchReceive>));
				pipeline.AddDocSpec(typeof(Batch.Content));
				pipeline.AddDocSpec(typeof(Envelope));

				var outputMessages = pipeline.Execute(inputMessage);

				Assert.That(outputMessages, Is.Not.Null);
				Assert.That(outputMessages.Count, Is.EqualTo(1));
				Assert.That(outputMessages[0].GetProperty(BtsProperties.MessageType), Is.EqualTo(Schema<Envelope>.MessageType));
				Assert.That(outputMessages[0].GetProperty(BizTalkFactoryProperties.EnvelopePartition), Is.Null);
				Assert.That(outputMessages[0].IsPromoted(BizTalkFactoryProperties.EnvelopePartition), Is.False);
				using (var reader = ValidatingXmlReader.Create<Envelope, Batch.Release>(outputMessages[0].BodyPart.Data))
				{
					var xmlDocument = new XmlDocument();
					xmlDocument.Load(reader);
					Assert.That(xmlDocument.OuterXml, Is.EqualTo(ResourceManager.LoadXmlString("Data.ReleaseBatches.xml")));
				}
			}
		}

		[Test]
		public void TransformBatchContentToEnvelopeAndPromotePartition()
		{
			using (var stream = ResourceManager.Load("Data.BatchContent.xml"))
			{
				var inputMessage = MessageHelper.CreateFromStream(stream);
				inputMessage.Promote(BtsProperties.InboundTransportLocation, "mssql://localhost//BizTalkFactoryTransientStateDb");

				var pipeline = PipelineFactory.CreateReceivePipeline(typeof(ReceivePipelineInterpreter<BatchReceive>));
				pipeline.AddDocSpec(typeof(Batch.Content));
				pipeline.AddDocSpec(typeof(Envelope));

				var outputMessages = pipeline.Execute(inputMessage);

				Assert.That(outputMessages, Is.Not.Null);
				Assert.That(outputMessages.Count, Is.EqualTo(1));
				Assert.That(outputMessages[0].GetProperty(BtsProperties.MessageType), Is.EqualTo(Schema<Envelope>.MessageType));
				Assert.That(outputMessages[0].GetProperty(BizTalkFactoryProperties.EnvelopePartition), Is.EqualTo("partition-one"));
				Assert.That(outputMessages[0].IsPromoted(BizTalkFactoryProperties.EnvelopePartition));
				using (var reader = ValidatingXmlReader.Create<Envelope, Batch.Release>(outputMessages[0].BodyPart.Data))
				{
					var xmlDocument = new XmlDocument();
					xmlDocument.Load(reader);
					Assert.That(xmlDocument.OuterXml, Is.EqualTo(ResourceManager.LoadXmlString("Data.ReleaseBatches.xml")));
				}
			}
		}
	}
}

#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Pipeline.Interpreters;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.IO;
using NUnit.Framework;
using Winterdom.BizTalk.PipelineTesting;

namespace Be.Stateless.BizTalk.MicroPipelines
{
	[TestFixture]
	public class XmlReceiveFixture
	{
		[Test]
		public void ContextPropertyExtractorClearsPromotedProperty()
		{
			const string content = "<ns0:Any xmlns:ns0=\"urn:schemas.stateless.be:biztalk:any:2012:12\"><message>content</message></ns0:Any>";
			using (var stream = new StringStream(content))
			{
				var pipeline = PipelineFactory.CreateReceivePipeline(typeof(ReceivePipelineInterpreter<XmlReceive>));
				pipeline.AddDocSpec(typeof(Any));
				var microPipeline = (MicroPipelineComponent) pipeline.GetComponent(PipelineStage.Decode, 1);
				microPipeline.Components = new[] {
					new ContextPropertyExtractor { Extractors = new[] { new PropertyExtractor(BizTalkFactoryProperties.CorrelationToken.QName, ExtractionMode.Clear) } }
				};

				var inputMessage = MessageHelper.CreateFromStream(stream);
				inputMessage.Promote(BizTalkFactoryProperties.CorrelationToken, "promoted-token");
				Assert.That(inputMessage.GetProperty(BizTalkFactoryProperties.CorrelationToken), Is.EqualTo("promoted-token"));
				Assert.That(inputMessage.IsPromoted(BizTalkFactoryProperties.CorrelationToken), Is.True);

				var outputMessages = pipeline.Execute(inputMessage);

				Assert.That(outputMessages[0].GetProperty(BizTalkFactoryProperties.CorrelationToken), Is.Null);
				Assert.That(outputMessages[0].IsPromoted(BizTalkFactoryProperties.CorrelationToken), Is.False);
				using (var reader = new StreamReader(outputMessages[0].BodyPart.Data))
				{
					var readOuterXml = reader.ReadToEnd();
					Assert.That(readOuterXml, Is.EqualTo(content));
				}
			}
		}

		[Test]
		public void ContextPropertyExtractorClearsWrittenProperty()
		{
			const string content = "<ns0:Any xmlns:ns0=\"urn:schemas.stateless.be:biztalk:any:2012:12\"><message>content</message></ns0:Any>";
			using (var stream = new StringStream(content))
			{
				var pipeline = PipelineFactory.CreateReceivePipeline(typeof(ReceivePipelineInterpreter<XmlReceive>));
				pipeline.AddDocSpec(typeof(Any));
				var microPipeline = (MicroPipelineComponent) pipeline.GetComponent(PipelineStage.Decode, 1);
				microPipeline.Components = new[] {
					new ContextPropertyExtractor { Extractors = new[] { new PropertyExtractor(BizTalkFactoryProperties.CorrelationToken.QName, ExtractionMode.Clear) } }
				};

				var inputMessage = MessageHelper.CreateFromStream(stream);
				inputMessage.SetProperty(BizTalkFactoryProperties.CorrelationToken, "written-token");
				Assert.That(inputMessage.GetProperty(BizTalkFactoryProperties.CorrelationToken), Is.EqualTo("written-token"));
				Assert.That(inputMessage.IsPromoted(BizTalkFactoryProperties.CorrelationToken), Is.False);

				var outputMessages = pipeline.Execute(inputMessage);

				Assert.That(outputMessages[0].GetProperty(BizTalkFactoryProperties.CorrelationToken), Is.Null);
				Assert.That(outputMessages[0].IsPromoted(BizTalkFactoryProperties.CorrelationToken), Is.False);
				using (var reader = new StreamReader(outputMessages[0].BodyPart.Data))
				{
					var readOuterXml = reader.ReadToEnd();
					Assert.That(readOuterXml, Is.EqualTo(content));
				}
			}
		}
	}
}

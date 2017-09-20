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

using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple;
using Be.Stateless.BizTalk.EnvironmentSettings;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.MicroPipelines;
using Be.Stateless.BizTalk.XPath;

namespace Be.Stateless.BizTalk
{
	public class UnitTestBatchAddPartReceiveLocation : ReceiveLocation<NamingConvention>
	{
		public UnitTestBatchAddPartReceiveLocation()
		{
			Name = ReceiveLocationName.About("Batch.AddPart").FormattedAs.Xml;
			Enabled = true;
			ReceivePipeline = new ReceivePipeline<XmlReceive>(
				pipeline => {
					pipeline.Validator<MicroPipelineComponent>(
						pc => {
							pc.Components = new IMicroPipelineComponent[] {
								new ContextPropertyExtractor {
									Extractors = new[] {
										new XPathExtractor(BizTalkFactoryProperties.EnvelopeSpecName, "/*[local-name()='Any']/*[local-name()='EnvelopeSpecName']", ExtractionMode.Promote),
										new XPathExtractor(BizTalkFactoryProperties.EnvelopePartition, "/*[local-name()='Any']/*[local-name()='EnvelopePartition']"),
										new XPathExtractor(TrackingProperties.Value1, "/*[local-name()='Any']/*[local-name()='EnvelopeSpecName']"),
										new XPathExtractor(TrackingProperties.Value2, "/*[local-name()='Any']/*[local-name()='EnvironmentTag']"),
										new XPathExtractor(TrackingProperties.Value3, "/*[local-name()='Any']/*[local-name()='EnvelopePartition']")
									}
								},
								new ActivityTracker()
							};
						});
				});
			Transport.Adapter = new FileAdapter.Inbound(
				a => {
					a.FileMask = "*.xml.part";
					a.ReceiveFolder = @"C:\Files\Drops\BizTalk.Factory\In";
				});
			Transport.Host = CommonSettings.ReceiveHost;
		}
	}
}

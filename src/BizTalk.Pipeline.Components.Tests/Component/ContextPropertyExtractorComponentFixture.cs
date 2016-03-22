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

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Unit.Component;
using Be.Stateless.BizTalk.XPath;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Component
{
	[TestFixture]
	[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
	public class ContextPropertyExtractorComponentFixture : PipelineComponentFixture<ContextPropertyExtractorComponent>
	{
		static ContextPropertyExtractorComponentFixture()
		{
			// PipelineComponentFixture<ContextPropertyExtractorComponent> assumes and needs the following converter
			TypeDescriptor.AddAttributes(typeof(IEnumerable<XPathExtractor>), new TypeConverterAttribute(typeof(XPathExtractorEnumerableConverter)));
		}

		protected override object GetValueForProperty(string name)
		{
			switch (name)
			{
				case "Extractors":
					return new[] {
						new XPathExtractor(BizTalkFactoryProperties.SenderName.QName, "/letter/*/from", ExtractionMode.Promote),
						new XPathExtractor(BizTalkFactoryProperties.ReceiverName.QName, "/letter/*/to", ExtractionMode.Promote),
						new XPathExtractor(TrackingProperties.ProcessName.QName, "/letter/*/subject", ExtractionMode.Write),
						new XPathExtractor(TrackingProperties.Value1.QName, "/letter/*/paragraph", ExtractionMode.Write),
						new XPathExtractor(TrackingProperties.Value2.QName, "/letter/*/salutations", ExtractionMode.Write),
						new XPathExtractor(TrackingProperties.Value3.QName, "/letter/*/signature", ExtractionMode.Write),
					};
				default:
					return base.GetValueForProperty(name);
			}
		}
	}
}

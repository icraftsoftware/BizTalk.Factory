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

using System;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Microsoft.BizTalk.Component;

namespace Be.Stateless.BizTalk.Pipelines
{
	public class XmlTransmit : SendPipeline
	{
		public XmlTransmit()
		{
			Description = "XML send pipeline with BAM activity tracking.";
			Version = new Version(1, 0);
			VersionDependentGuid = new Guid("435e4afb-bba3-48f7-a820-777950b0e00f");
			Stages.PreAssemble
				.AddComponent(new FailedMessageRoutingEnablerComponent())
				.AddComponent(new ContextPropertyExtractorComponent { Enabled = false })
				.AddComponent(new PolicyRunnerComponent { Enabled = false })
				.AddComponent(new ActivityTrackerComponent { Enabled = false })
				.AddComponent(new XsltRunnerComponent { Enabled = false });
			Stages.Assemble
				.AddComponent(new XmlAsmComp());
			Stages.Encode
				.AddComponent(new ContextPropertyExtractorComponent())
				.AddComponent(new PolicyRunnerComponent())
				.AddComponent(new ActivityTrackerComponent())
				.AddComponent(new XsltRunnerComponent())
				.AddComponent(new XmlTranslatorComponent());
		}
	}
}

﻿#region Copyright & License

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

using System;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Microsoft.BizTalk.Component;

namespace Be.Stateless.BizTalk.Pipelines
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "DSL-based pipeline definition.")]
	public class XmlReceive : ReceivePipeline
	{
		public XmlReceive()
		{
			Description = "XML receive pipeline with BAM activity tracking.";
			Version = new Version(1, 0);
			VersionDependentGuid = new Guid("cb520638-c708-4de1-b8d7-667183cc38ae");
			Stages.Decode
				.AddComponent(new FailedMessageRoutingEnablerComponent())
				.AddComponent(new XmlTranslatorComponent { Enabled = false })
				.AddComponent(new ContextPropertyExtractorComponent { Enabled = false })
				.AddComponent(new PolicyRunnerComponent { Enabled = false })
				.AddComponent(new ActivityTrackerComponent { Enabled = false })
				.AddComponent(new XsltRunnerComponent { Enabled = false });
			Stages.Disassemble
				.AddComponent(new XmlDasmComp());
			Stages.ResolveParty
				.AddComponent(new ContextPropertyExtractorComponent())
				.AddComponent(new PolicyRunnerComponent())
				.AddComponent(new ActivityTrackerComponent())
				.AddComponent(new XsltRunnerComponent());
		}
	}
}

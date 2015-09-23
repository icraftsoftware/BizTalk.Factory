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

using System;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Microsoft.BizTalk.Component;

namespace Be.Stateless.BizTalk.MicroPipelines
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "DSL-based pipeline definition.")]
	public class XmlReceive : ReceivePipeline
	{
		public XmlReceive()
		{
			Description = "XML receive micropipeline.";
			Version = new Version(1, 0);
			VersionDependentGuid = new Guid("b4235590-184d-4ccf-90cd-b33de4a7ea9e");
			Stages.Decode
				.AddComponent(new MicroPipelineComponent { Enabled = true });
			Stages.Disassemble
				.AddComponent(new XmlDasmComp());
			Stages.Validate
				.AddComponent(new MicroPipelineComponent { Enabled = true });
		}
	}
}

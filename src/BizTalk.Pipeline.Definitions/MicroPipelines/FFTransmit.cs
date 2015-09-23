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
	public class FFTransmit : SendPipeline
	{
		public FFTransmit()
		{
			Description = "Flat-File send micropipeline.";
			Version = new Version(1, 0);
			VersionDependentGuid = new Guid("01843b52-d05d-4e57-845f-576274f10b83");
			Stages.PreAssemble
				.AddComponent(new MicroPipelineComponent { Enabled = true });
			Stages.Assemble
				.AddComponent(new FFAsmComp());
			Stages.Encode
				.AddComponent(new MicroPipelineComponent { Enabled = true });
		}
	}
}

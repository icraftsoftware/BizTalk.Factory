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

namespace Be.Stateless.BizTalk.MicroPipelines
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "DSL-based pipeline definition.")]
	public class PassThruReceive : ReceivePipeline
	{
		public PassThruReceive()
		{
			Description = "Pass-through receive micropipeline.";
			Version = new Version(1, 0);
			VersionDependentGuid = new Guid("0047c469-92fe-42bf-af61-ed3ce3307a16");
			Stages.Decode
				.AddComponent(new MicroPipelineComponent { Enabled = true });
		}
	}
}

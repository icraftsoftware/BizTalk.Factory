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
	public class PassThruTransmit : SendPipeline
	{
		public PassThruTransmit()
		{
			Description = "Pass-through send micropipeline.";
			Version = new Version(1, 0);
			VersionDependentGuid = new Guid("d80ec4c3-f816-4508-b8ed-28861b62bb47");
			Stages.PreAssemble
				.AddComponent(new FailedMessageRoutingEnablerComponent())
				.AddComponent(new MicroPipelineComponent { Enabled = true });
		}
	}
}

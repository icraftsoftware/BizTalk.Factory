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

namespace Be.Stateless.BizTalk.Pipelines
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "DSL-based pipeline definition.")]
	public class PassThruTransmit : SendPipeline
	{
		public PassThruTransmit()
		{
			Description = "Pass-through send pipeline with BAM activity tracking.";
			Version = new Version(1, 0);
			VersionDependentGuid = new Guid("678773b7-3ccb-4cf3-94ce-95328c1149a4");
			Stages.PreAssemble
				.AddComponent(new FailedMessageRoutingEnablerComponent())
				.AddComponent(new PolicyRunnerComponent { Enabled = false })
				.AddComponent(new ActivityTrackerComponent());
		}
	}
}

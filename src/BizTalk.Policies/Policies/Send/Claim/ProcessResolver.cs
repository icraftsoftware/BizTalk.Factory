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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.RuleEngine;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Policies.Send.Claim
{
	public class ProcessResolver : RuleSet
	{
		public ProcessResolver()
		{
			Rules.Add(
				Rule("ClaimCheckProcessResolver")
					.If(() => Context.Read(TrackingProperties.ProcessName).IsNullOrEmpty())
					.Then(() => Context.Write(TrackingProperties.ProcessName, Factory.Areas.Claim.Processes.Check))
				);
		}
	}
}

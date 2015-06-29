#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Unit.RuleEngine;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.RuleEngine.Dsl
{
	[TestFixture]
	public class MultiRulePolicyFixture : PolicyFixture
	{
		[Test]
		public void NoResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.MessageType).WithValue("UNKNOWN"))
				.Assert(Context.Property(BtsProperties.InboundTransportType).WithValue("WCF-SAP"));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).HasNotBeenSet());
		}

		[Test]
		public void ProcessOneResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.MessageType).WithValue("Z_IDOC#ONE"))
				.Assert(Context.Property(BtsProperties.InboundTransportType).WithValue("WCF-SAP"));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue(TestProcesses.One).HasBeenWritten());
		}

		[Test]
		public void ProcessTwoResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.MessageType).WithValue("Z_IDOC#TWO"))
				.Assert(Context.Property(BtsProperties.InboundTransportType).WithValue("WCF-SAP"));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue(TestProcesses.Two).HasBeenWritten());
		}

		public RuleSet RuleSet
		{
			get { return _ruleset ?? (_ruleset = new MultiRuleRuleSet()); }
		}

		public class MultiRuleRuleSet : RuleSet
		{
			public MultiRuleRuleSet()
			{
				Name = GetType().Name;

				Rules.Add(
					Rule("ProcessOne")
						.If(() => Context.Read(BtsProperties.MessageType) == "Z_IDOC#ONE" && Context.Read(BtsProperties.InboundTransportType) == "WCF-SAP")
						.Then(() => Context.Write(ResolvedProperties.ProcessName, TestProcesses.One))
					);

				Rules.Add(
					Rule("ProcessTwo")
						.If(() => Context.Read(BtsProperties.MessageType) == "Z_IDOC#TWO" && Context.Read(BtsProperties.InboundTransportType) == "WCF-SAP")
						.Then(() => Context.Write(ResolvedProperties.ProcessName, TestProcesses.Two))
					);
			}
		}

		private RuleSet _ruleset;
	}
}
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
	public class ResolvedProcessNameRuleFixture : PolicyFixture
	{
		[Test]
		public void QualifiedPropertyResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.IsRequestResponse).WithValue(true));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue(TestProcesses.One).HasBeenWritten());
		}

		[Test]
		public void QualifiedFieldResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.IsSolicitResponse).WithValue(true));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue(TestProcesses.Two).HasBeenWritten());
		}

		[Test]
		public void StaticConstResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.AckRequired).WithValue(true));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue(TestProcesses.Three).HasBeenWritten());
		}

		[Test]
		public void UnqualifiedFieldResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.SuppressRoutingFailureDiagnosticInfo).WithValue(true));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue(TestProcesses.Four).HasBeenWritten());
		}

		public RuleSet RuleSet
		{
			get { return _ruleset ?? (_ruleset = new ResolvedProcessNameRuleet()); }
		}

		public class ResolvedProcessNameRuleet : RuleSet
		{
			public ResolvedProcessNameRuleet()
			{
				Name = GetType().Name;

				Rules.Add(
					Rule("QualifiedProperty")
						.If(() => Context.Read(BtsProperties.IsRequestResponse))
						.Then(() => Context.Write(ResolvedProperties.ProcessName, TestProcesses.One))
					);

				Rules.Add(
					Rule("QualifiedField")
						.If(() => Context.Read(BtsProperties.IsSolicitResponse))
						.Then(() => Context.Write(ResolvedProperties.ProcessName, TestProcesses.Two))
					);

				Rules.Add(
					Rule("StaticConst")
						.If(() => Context.Read(BtsProperties.AckRequired))
						.Then(() => Context.Write(ResolvedProperties.ProcessName, TestProcesses.Three))
					);

				Rules.Add(
					Rule("UnqualifiedField")
						.If(() => Context.Read(BtsProperties.SuppressRoutingFailureDiagnosticInfo))
						.Then(() => Context.Write(ResolvedProperties.ProcessName, TestProcesses.Four))
					);
			}
		}

		private RuleSet _ruleset;
	}
}
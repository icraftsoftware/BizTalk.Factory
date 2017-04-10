#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	[TestFixture]
	public class ResolvedProcessNameRuleFixture : PolicyFixture<ResolvedProcessNameRuleFixture.ResolvedProcessNameRuleset>
	{
		[Test]
		public void QualifiedFieldResolution()
		{
			Facts.Assert(Context.Property(BtsProperties.IsSolicitResponse).WithValue(true));

			ExecutePolicy();

			Facts.Verify(Context.Property(TrackingProperties.ProcessName).WithValue(Dummy.Processes.Two).HasBeenWritten());
		}

		[Test]
		public void QualifiedPropertyResolution()
		{
			Facts.Assert(Context.Property(BtsProperties.IsRequestResponse).WithValue(true));

			ExecutePolicy();

			Facts.Verify(Context.Property(TrackingProperties.ProcessName).WithValue(Dummy.Processes.One).HasBeenWritten());
		}

		[Test]
		public void StaticConstResolution()
		{
			Facts.Assert(Context.Property(BtsProperties.AckRequired).WithValue(true));

			ExecutePolicy();

			Facts.Verify(Context.Property(TrackingProperties.ProcessName).WithValue(Dummy.Processes.Three).HasBeenWritten());
		}

		[Test]
		public void UnqualifiedFieldResolution()
		{
			Facts.Assert(Context.Property(BtsProperties.SuppressRoutingFailureDiagnosticInfo).WithValue(true));

			ExecutePolicy();

			Facts.Verify(Context.Property(TrackingProperties.ProcessName).WithValue(Dummy.Processes.Four).HasBeenWritten());
		}

		public class ResolvedProcessNameRuleset : RuleSet
		{
			public ResolvedProcessNameRuleset()
			{
				Name = GetType().Name;

				Rules.Add(
					Rule("QualifiedProperty")
						.If(() => Context.Read(BtsProperties.IsRequestResponse))
						.Then(() => Context.Write(TrackingProperties.ProcessName, Dummy.Processes.One))
					);

				Rules.Add(
					Rule("QualifiedField")
						.If(() => Context.Read(BtsProperties.IsSolicitResponse))
						.Then(() => Context.Write(TrackingProperties.ProcessName, Dummy.Processes.Two))
					);

				Rules.Add(
					Rule("StaticConst")
						.If(() => Context.Read(BtsProperties.AckRequired))
						.Then(() => Context.Write(TrackingProperties.ProcessName, Dummy.Processes.Three))
					);

				Rules.Add(
					Rule("UnqualifiedField")
						.If(() => Context.Read(BtsProperties.SuppressRoutingFailureDiagnosticInfo))
						.Then(() => Context.Write(TrackingProperties.ProcessName, Dummy.Processes.Four))
					);
			}
		}
	}
}

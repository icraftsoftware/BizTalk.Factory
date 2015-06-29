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
	public class LogicalPredicateRuleFixture : PolicyFixture
	{
		[Test]
		public void ProcessTenAResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.MessageType).WithValue("Z_IDOC#TEN"))
				.Assert(Context.Property(RoutingProperties.CorrelationToken).WithValue("Z2UMD_392_DEM_START"));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue("ProcessTen").HasBeenWritten());
		}

		[Test]
		public void ProcessTenBResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.MessageType).WithValue("Z_IDOC#TEN"))
				.Assert(Context.Property(RoutingProperties.CorrelationToken).WithValue("Z2UMD_392_DEM_CAN"));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue("ProcessTen").HasBeenWritten());
		}

		[Test]
		public void BooleanPredicateResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.AckRequired).WithValue(true));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(BtsProperties.ReceivePortName).WithValue("AckRequired").HasBeenWritten());
		}

		[Test]
		public void NegationResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.AckRequired).WithValue(false));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(BtsProperties.ReceivePortName).WithValue("NotAckRequired").HasBeenWritten());
		}

		[Test]
		public void NegationResolutionWhenBoolPropertyIsMissingFromFactBase()
		{
			// Asserting a false boolean is useless as it will be assumed to be false
			// RuleEngine.Facts.Assert(Context.Property(BtsProperties.AckRequired).WithValue(false));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(BtsProperties.ReceivePortName).WithValue("NotAckRequired").HasBeenWritten());
		}

		[Test]
		public void NotEqualToIntegerResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.ActualRetryCount).WithValue(2));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(BtsProperties.ReceivePortName).WithValue("NotEqualToInteger").HasBeenWritten());
		}

		[Test]
		public void NotEqualToStringResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.ReceiveLocationName).WithValue("ActualReceiveLocationName"));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(BtsProperties.ReceivePortName).WithValue("NotEqualToString").HasBeenWritten());
		}

		public RuleSet RuleSet
		{
			get { return _ruleset ?? (_ruleset = new LogicalPredicateRuleSet()); }
		}

		public class LogicalPredicateRuleSet : RuleSet
		{
			public LogicalPredicateRuleSet()
			{
				Name = GetType().Name;

				Rules.Add(
					Rule("ProcessTen")
						.If(() => Context.Read(BtsProperties.MessageType) == "Z_IDOC#TEN"
							&& (Context.Read(RoutingProperties.CorrelationToken) == "Z2UMD_392_DEM_START" || Context.Read(RoutingProperties.CorrelationToken) == "Z2UMD_392_DEM_CAN"))
						.Then(() => Context.Write(ResolvedProperties.ProcessName, "ProcessTen"))
					);

				Rules.Add(
					Rule("NotEqualToString")
						.If(() => Context.Read(BtsProperties.ReceiveLocationName) != "UnwantedReceiveLocationName")
						.Then(() => Context.Write(BtsProperties.ReceivePortName, "NotEqualToString"))
					);

				Rules.Add(
					Rule("BooleanPredicate")
						.If(() => Context.Read(BtsProperties.AckRequired))
						.Then(() => Context.Write(BtsProperties.ReceivePortName, "AckRequired"))
					);

				Rules.Add(
					Rule("Negation")
						.If(() => !Context.Read(BtsProperties.AckRequired))
						.Then(() => Context.Write(BtsProperties.ReceivePortName, "NotAckRequired"))
					);

				Rules.Add(
					Rule("NotEqualToInteger")
						.If(() => Context.Read(BtsProperties.ActualRetryCount) != 3)
						.Then(() => Context.Write(BtsProperties.ReceivePortName, "NotEqualToInteger"))
					);
			}
		}

		private RuleSet _ruleset;
	}
}
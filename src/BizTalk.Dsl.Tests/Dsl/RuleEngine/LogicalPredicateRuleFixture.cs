#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
	public class LogicalPredicateRuleFixture : PolicyFixture<LogicalPredicateRuleFixture.LogicalPredicateRuleSet>
	{
		[Test]
		public void BooleanPredicateResolution()
		{
			Facts.Assert(Context.Property(BtsProperties.AckRequired).WithValue(true));

			ExecutePolicy();

			Facts.Verify(Context.Property(BtsProperties.ReceivePortName).WithValue("AckRequired").HasBeenWritten());
		}

		[Test]
		public void NegationResolution()
		{
			Facts.Assert(Context.Property(BtsProperties.AckRequired).WithValue(false));

			ExecutePolicy();

			Facts.Verify(Context.Property(BtsProperties.ReceivePortName).WithValue("NotAckRequired").HasBeenWritten());
		}

		[Test]
		public void NegationResolutionWhenBoolPropertyIsMissingFromFactBase()
		{
			// Asserting a false boolean is useless as it will be assumed to be false
			// Facts.Assert(Context.Property(BtsProperties.AckRequired).WithValue(false));

			ExecutePolicy();

			Facts.Verify(Context.Property(BtsProperties.ReceivePortName).WithValue("NotAckRequired").HasBeenWritten());
		}

		[Test]
		public void NotEqualToIntegerResolution()
		{
			Facts.Assert(Context.Property(BtsProperties.ActualRetryCount).WithValue(2));

			ExecutePolicy();

			Facts.Verify(Context.Property(BtsProperties.ReceivePortName).WithValue("NotEqualToInteger").HasBeenWritten());
		}

		[Test]
		public void NotEqualToStringResolution()
		{
			Facts.Assert(Context.Property(BtsProperties.ReceiveLocationName).WithValue("ActualReceiveLocationName"));

			ExecutePolicy();

			Facts.Verify(Context.Property(BtsProperties.ReceivePortName).WithValue("NotEqualToString").HasBeenWritten());
		}

		[Test]
		public void ProcessTenAResolution()
		{
			Facts
				.Assert(Context.Property(BtsProperties.MessageType).WithValue("Z_IDOC#TEN"))
				.Assert(Context.Property(BizTalkFactoryProperties.CorrelationToken).WithValue("Z2UMD_392_DEM_START"));

			ExecutePolicy();

			Facts.Verify(Context.Property(TrackingProperties.ProcessName).WithValue("ProcessTen").HasBeenWritten());
		}

		[Test]
		public void ProcessTenBResolution()
		{
			Facts
				.Assert(Context.Property(BtsProperties.MessageType).WithValue("Z_IDOC#TEN"))
				.Assert(Context.Property(BizTalkFactoryProperties.CorrelationToken).WithValue("Z2UMD_392_DEM_CAN"));

			ExecutePolicy();

			Facts.Verify(Context.Property(TrackingProperties.ProcessName).WithValue("ProcessTen").HasBeenWritten());
		}

		public class LogicalPredicateRuleSet : RuleSet
		{
			public LogicalPredicateRuleSet()
			{
				Name = GetType().Name;

				Rules.Add(
					Rule("ProcessTen")
						.If(
							() => Context.Read(BtsProperties.MessageType) == "Z_IDOC#TEN"
								&& (Context.Read(BizTalkFactoryProperties.CorrelationToken) == "Z2UMD_392_DEM_START"
									|| Context.Read(BizTalkFactoryProperties.CorrelationToken) == "Z2UMD_392_DEM_CAN")
						)
						.Then(() => Context.Write(TrackingProperties.ProcessName, "ProcessTen"))
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
	}
}

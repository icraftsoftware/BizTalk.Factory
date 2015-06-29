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

using System;
using Be.Stateless.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Unit.RuleEngine;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.RuleEngine.Dsl
{
	[TestFixture]
	public class SaticMemberAccessRuleFixture : PolicyFixture
	{
		[Test]
		public void ExtensionMethodResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(RoutingProperties.CorrelationToken).WithValue("ExtensionMethodResolution"))
				.Assert(Context.Property(RoutingProperties.SenderName).WithValue("0000001234567890"));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue("00001234567890").HasBeenWritten());
		}

		[Test]
		public void StaticCallResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(RoutingProperties.CorrelationToken).WithValue("StaticCallResolution"));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue("HelloFromStaticCallResolution").HasBeenWritten());
		}

		[Test]
		public void SaticPropertyResolution()
		{
			RuleEngine.Facts
				.Assert(Context.Property(RoutingProperties.CorrelationToken).WithValue("StaticPropertyResolution"));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(FileProperties.FileCreationTime).WithValue(DateTime.Today).HasBeenWritten());
		}

		public RuleSet RuleSet
		{
			get { return _ruleset ?? (_ruleset = new SaticMemberAccessRuleSet()); }
		}

		public class SaticMemberAccessRuleSet : RuleSet
		{
			public SaticMemberAccessRuleSet()
			{
				Name = GetType().Name;

				Rules.Add(
					Rule("ExtensionMethod")
						.If(() => Context.Read(RoutingProperties.CorrelationToken) == "ExtensionMethodResolution")
						.Then(() => Context.Write(ResolvedProperties.ProcessName, Context.Read(RoutingProperties.SenderName).SubstringEx(-14)))
					);

				Rules.Add(
					Rule("StaticCall")
						.If(() => Context.Read(RoutingProperties.CorrelationToken) == "StaticCallResolution")
						.Then(() => Context.Write(ResolvedProperties.ProcessName, string.Concat("HelloFrom", Context.Read(RoutingProperties.CorrelationToken))))
					);

				Rules.Add(
					Rule("SaticProperty")
						.If(() => Context.Read(RoutingProperties.CorrelationToken) == "StaticPropertyResolution")
						.Then(() => Context.Write(FileProperties.FileCreationTime, DateTime.Today))
					);
			}
		}

		private RuleSet _ruleset;
	}
}
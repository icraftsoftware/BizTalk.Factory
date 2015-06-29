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
	public class WriteFromContextRuleFixture : PolicyFixture
	{
		[Test]
		public void ExecuteRule()
		{
			RuleEngine.Facts
				.Assert(Context.Property(BtsProperties.MessageType).WithValue("WriteFromContext"));

			RuleEngine.ExecutePolicy(RuleSet);

			RuleEngine.Facts
				.Verify(Context.Property(ResolvedProperties.ProcessName).WithValue("WriteFromContext").HasBeenWritten());
		}

		public RuleSet RuleSet
		{
			get { return _ruleset ?? (_ruleset = new WriteFromContextRuleSet()); }
		}

		public class WriteFromContextRuleSet : RuleSet
		{
			public WriteFromContextRuleSet()
			{
				Name = GetType().Name;

				Rules.Add(
					Rule("WriteFromContext")
						.If(() => Context.Read(BtsProperties.MessageType) == "WriteFromContext")
						.Then(() => Context.Write(ResolvedProperties.ProcessName, Context.Read(BtsProperties.MessageType)))
					);
			}
		}

		private RuleSet _ruleset;
	}
}
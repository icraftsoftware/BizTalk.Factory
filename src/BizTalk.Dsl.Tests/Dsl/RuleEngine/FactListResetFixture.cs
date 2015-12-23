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
using Be.Stateless.BizTalk.Unit.RuleEngine;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	[TestFixture]
	public class FactListResetFixture : PolicyFixture<FactListResetFixture.SimpleRuleSet>
	{
		[Test]
		public void ClearingFactList()
		{
			Facts.Assert(Context.Property(BtsProperties.MessageType).WithValue("message-type"));

			RuleEngine.Facts.Clear();

			ExecutePolicy();

			Facts.Verify(Context.Property(TrackingProperties.ProcessName).HasNotBeenSet());
		}

		[Test]
		public void WithoutClearingFactList()
		{
			Facts.Assert(Context.Property(BtsProperties.MessageType).WithValue("message-type"));

			ExecutePolicy();

			Assert.That(
				() => Facts.Verify(Context.Property(TrackingProperties.ProcessName).HasNotBeenSet()),
				Throws.TypeOf<MockException>());
		}

		public class SimpleRuleSet : RuleSet
		{
			public SimpleRuleSet()
			{
				Rules.Add(
					Rule("ProcessOne")
						.If(() => Context.Read(BtsProperties.MessageType) == "message-type")
						.Then(() => Context.Write(TrackingProperties.ProcessName, Dummy.Processes.One))
					);
			}
		}
	}
}

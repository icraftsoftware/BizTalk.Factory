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

using NUnit.Framework;

namespace Be.Stateless.BizTalk.RuleEngine
{
	[TestFixture]
	public class PolicyNameFixture
	{
		[Test]
		public void PolicyNameEqualsRuleSetInfo()
		{
			var rsi = new Microsoft.RuleEngine.RuleSetInfo("name", 1, 0);
			var pn1 = new PolicyName("name", 1, 0);
			var pn2 = new PolicyName("name", 1, 0);

			// PolicyName == PolicyName
			Assert.That(pn1 == pn2);
			Assert.That(pn2 == pn1);
			Assert.That(pn1.Equals(pn2));

			// PolicyName == RuleSetInfo
			Assert.That(pn1 == rsi);
			Assert.That(pn1.Equals(rsi));

			// RuleSetInfo == PolicyName
			Assert.That(rsi == pn1);
			Assert.That(rsi.Equals(pn1), Is.False);
		}
	}
}

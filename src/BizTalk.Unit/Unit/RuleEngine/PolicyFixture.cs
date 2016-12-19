#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Dsl.RuleEngine;
using Be.Stateless.Extensions;
using Microsoft.Win32;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.RuleEngine
{
	// make a non-generic base class to avoid multiple executions of the static constructor
	public abstract class PolicyFixture
	{
		static PolicyFixture()
		{
			var is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
			// As described at http://msdn.microsoft.com/en-us/library/aa950269.aspx, ensures that either
			// HKLM\Software\Microsoft\BusinessRules\3.0\StaticSupport or 
			// HKLM\Software\Wow6432Node\Microsoft\BusinessRules\3.0\StaticSupport is 1.
			var keyPath = string.Format(@"Software\{0}Microsoft\BusinessRules\3.0", is64BitOperatingSystem ? @"Wow6432Node\" : string.Empty);
			var keyValue = Registry.LocalMachine.OpenSubKey(keyPath);
			var support = keyValue.IfNotNull(k => k.GetValue("StaticSupport"));
			if (support == null || Convert.ToInt32(support) != 1)
				throw new NotSupportedException(
					"Business Rule Engine registry configuration is not correct: StaticSupport must be set to the DWORD value '1'; see http://msdn.microsoft.com/en-us/library/aa950269.aspx.");
		}
	}

	public abstract class PolicyFixture<T> : PolicyFixture
		where T : RuleSet, new()
	{
		protected ContextFactFactory Context
		{
			get { return ContextFactFactory.Instance; }
		}

		protected ContextFactList Facts
		{
			get { return RuleEngine.Facts; }
		}

		protected TestRuleEngine RuleEngine { get; private set; }

		protected T RuleSet { get; private set; }

		protected void ExecutePolicy()
		{
			RuleEngine.ExecutePolicy(RuleSet);
		}

		[SetUp]
		public void SetUp()
		{
			RuleEngine = new TestRuleEngine();
			RuleSet = new T();
		}

		[Test]
		public void PolicyCanBeTranslatedToBrl()
		{
			Assert.That(RuleSet.ToBrl(), Is.Not.Null);
		}
	}
}

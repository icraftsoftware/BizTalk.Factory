﻿#region Copyright & License

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

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Be.Stateless.BizTalk.Dsl.RuleEngine;
using Be.Stateless.BizTalk.Tracking.Messaging;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Install
{
	[TestFixture]
	public class RuleSetInstallerFixture : RuleSetInstaller
	{
		[Test]
		public void DiscoverProcessNames()
		{
			var expected = new[] {
				Dummy.Processes.Four,
				Dummy.Processes.One,
				Dummy.Processes.Three,
				Dummy.Processes.Two,
				UnitTesting.Processes.ProcessOne,
				UnitTesting.Processes.ProcessTwo,
				UnitTesting.Processes.ProcessSix
			};

			var processNames = GetProcessNames();

			Assert.That(processNames, Is.EquivalentTo(expected));
		}

		[Test]
		public void DiscoverRuleSets()
		{
			var expected = new[] {
				typeof(AlwaysTrueRuleFixture.AlwaysTrueRuleSet).FullName,
				typeof(LogicalPredicateRuleFixture.LogicalPredicateRuleSet).FullName,
				typeof(MultiRulePolicyFixture.MultiRuleRuleSet).FullName,
				typeof(ResolvedProcessNameRuleFixture.ResolvedProcessNameRuleset).FullName,
				typeof(StaticMemberAccessRuleFixture.StaticMemberAccessRuleSet).FullName,
				typeof(SchemaAndTransformRuleFixture.WriteFromContextRuleSet).FullName,
				typeof(TypeMemberAccessRuleFixture.TypeMemberAccessRuleSet).FullName,
				typeof(WriteFromContextRuleFixture.WriteFromContextRuleSet).FullName,
				typeof(MemberAccessRuleFixture.MemberAccessRuleSet).FullName,
				typeof(BoxingRuleFixture.BoxingRuleSet).FullName,
				typeof(FactListResetFixture.SimpleRuleSet).FullName,
				typeof(MessageContextPropertyAccessFixture.MessageContextPropertyAccessRuleSet).FullName
			};

			var ruleSets = RuleSets.Select(r => r.GetType().FullName);

			Assert.That(ruleSets, Is.EquivalentTo(expected));
		}

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		private class UnitTesting : ProcessName<UnitTesting>
		{
			public string ProcessOne { get; private set; }

			public string ProcessSix { get; private set; }

			public string ProcessTwo { get; private set; }
		}
	}
}

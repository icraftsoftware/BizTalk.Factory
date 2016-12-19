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
using System.Xml;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.Logging;
using Microsoft.RuleEngine;

namespace Be.Stateless.BizTalk.Unit.RuleEngine
{
	public class TestRuleEngine
	{
		public TestRuleEngine()
		{
			Facts = new ContextFactList(this);
		}

		public ContextFactList Facts { get; internal set; }

		public void ExecutePolicy(string filename)
		{
			var ruleStore = new FileRuleStore(filename);
			var ruleSetInfo = GetRuleSetInfo(filename);
			var ruleSet = ruleStore.GetRuleSet(ruleSetInfo);
			ExecutePolicy(ruleSet);
		}

		public void ExecutePolicy(RuleSet ruleSet)
		{
			using (var policy = new PolicyTester(ruleSet))
			{
				var fact = new Context(Facts.Context);
				policy.Execute(new object[] { fact }, new RuleSetTrackingLogger(LogManager.GetLogger(ruleSet.Name)));
			}
		}

		private RuleSetInfo GetRuleSetInfo(string filename)
		{
			var policySource = new XmlDocument();
			policySource.Load(filename);
			var namespaceManager = new XmlNamespaceManager(policySource.NameTable);
			namespaceManager.AddNamespace("tns", "http://schemas.microsoft.com/businessruleslanguage/2002");
			// ReSharper disable PossibleNullReferenceException
			var name = policySource.SelectSingleNode("/tns:brl/tns:ruleset/@name", namespaceManager).Value;
			var majorRevision = Convert.ToInt32(policySource.SelectSingleNode("/tns:brl/tns:ruleset/tns:version/@major", namespaceManager).Value);
			var minorRevision = Convert.ToInt32(policySource.SelectSingleNode("/tns:brl/tns:ruleset/tns:version/@minor", namespaceManager).Value);
			// ReSharper restore PossibleNullReferenceException
			return new RuleSetInfo(name, majorRevision, minorRevision);
		}
	}
}

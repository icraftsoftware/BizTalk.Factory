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

using System;
using System.Collections;
using System.Linq;
using Be.Stateless.BizTalk.Dsl.RuleEngine;
using Be.Stateless.BizTalk.Tracking;
using Microsoft.RuleEngine;
using RuleSet = Be.Stateless.BizTalk.Dsl.RuleEngine.RuleSet;

namespace Be.Stateless.BizTalk.Install
{
	public abstract class RuleSetInstaller : ProcessInstaller
	{
		#region Base Class Member Overrides

		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);
			InstallRuleSet();
		}

		protected override string[] ProcessNames
		{
			get { return _processNames ?? (_processNames = ProcessNameAttribute.GetProcessNames(GetType().Assembly.GetExportedTypes())); }
		}

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);
			UninstallRuleSet();
		}

		#endregion

		protected RuleSet[] RuleSets
		{
			get
			{
				return _ruleSets ?? (_ruleSets = GetType().Assembly.GetExportedTypes()
					.Where(t => typeof(RuleSet).IsAssignableFrom(t) && !t.IsAbstract)
					.Select(rst => (RuleSet) Activator.CreateInstance(rst))
					.ToArray());
			}
		}

		private void InstallRuleSet()
		{
			var driver = Configuration.GetDeploymentDriver();
			var store = driver.GetRuleStore();
			foreach (var ruleSet in RuleSets)
			{
				// uninstall any previously deployed rule set with the same version so that it can overridden
				if (driver.IsRuleSetDeployed(ruleSet.RuleSetInfo))
				{
					Console.WriteLine(
						"Deploying ruleset {0}, {1}.{2}: undeploying previously deployed ruleset.",
						ruleSet.Name,
						ruleSet.VersionInfo.MajorRevision,
						ruleSet.VersionInfo.MinorRevision);
					driver.Undeploy(ruleSet.RuleSetInfo);
					store.Remove(ruleSet);
				}
				Console.WriteLine(
					"Deploying ruleset {0}, {1}.{2}.",
					ruleSet.Name,
					ruleSet.VersionInfo.MajorRevision,
					ruleSet.VersionInfo.MinorRevision);
				store.Add(ruleSet, true);
				driver.Deploy(ruleSet.RuleSetInfo);
				/*
				 * TODO publish only and not deploy but affiliate policy to application (deploy will be done @ app startup by BTS), see BTSTask
				 * BTSTask.exe AddResource -Type:System.BizTalk:Rules -Overwrite -Name:'%(PolicyName)' -Version:%(PolicyVersion) -ApplicationName:$(BizTalkAppName)
				*/
			}
		}

		private void UninstallRuleSet()
		{
			var driver = Configuration.GetDeploymentDriver();
			var store = driver.GetRuleStore();
			foreach (var ruleSet in RuleSets)
			{
				// only uninstall if rule set is still there
				if (driver.IsRuleSetDeployed(ruleSet.RuleSetInfo))
				{
					Console.WriteLine(
						"Undeploying ruleset {0}, {1}.{2}.",
						ruleSet.Name,
						ruleSet.VersionInfo.MajorRevision,
						ruleSet.VersionInfo.MinorRevision);
					driver.Undeploy(ruleSet.RuleSetInfo);
					store.Remove(ruleSet);
				}
				else
				{
					Console.WriteLine(
						"Undeploying ruleset {0}, {1}.{2}: ruleset already undeployed.",
						ruleSet.Name,
						ruleSet.VersionInfo.MajorRevision,
						ruleSet.VersionInfo.MinorRevision);
				}
			}
		}

		private string[] _processNames;
		private RuleSet[] _ruleSets;
	}
}

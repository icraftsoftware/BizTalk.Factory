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
using System.Collections;
using System.Configuration.Install;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Be.Stateless.BizTalk.RuleEngine.Dsl;
using Be.Stateless.Extensions;
using Microsoft.RuleEngine;
using RuleSet = Be.Stateless.BizTalk.RuleEngine.Dsl.RuleSet;

namespace Be.Stateless.BizTalk.RuleEngine
{
	public abstract class RuleSetInstaller : Installer
	{
		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);
			InstallRuleSet();
			RegisterProcessNames();
		}

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);
			UnregisterProcessNames();
			UninstallRuleSet();
		}

		protected string[] ProcessNames
		{
			get { return _processNames ?? (_processNames = ProcessNameAttribute.GetProcessNames(GetType().Assembly.GetExportedTypes())); }
		}

		protected RuleSet[] RuleSets
		{
			get
			{
				return _ruleSets ?? (_ruleSets = GetType().Assembly.GetExportedTypes()
					.Where(t => t.BaseType == typeof(RuleSet))
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

		private void RegisterProcessNames()
		{
			if (DataSource.IsNullOrEmpty() || !ProcessNames.Any())
			{
				Console.WriteLine("Skipping process names registration.");
				return;
			}
			
			Console.WriteLine("Registering process names.");
			// TODO use sproc instead
			// insert or update in case process name has not been previously deleted
			const string upsertCmdText = @"MERGE INTO ProcessDescriptors PD
USING (SELECT @name AS Name) NPD ON PD.Name = NPD.Name
WHEN NOT MATCHED THEN INSERT (Name) VALUES (NPD.Name);";
			using (var cnx = MgmtDbConnection)
			using (var cmd = new SqlCommand(upsertCmdText, cnx))
			{
				cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar));
				cnx.Open();
				foreach (var processName in ProcessNames)
				{
					cmd.Parameters["@name"].Value = processName;
					cmd.ExecuteNonQuery();
				}
			}
		}

		private void UnregisterProcessNames()
		{
			if (DataSource.IsNullOrEmpty() || !MgmtDbExists() || !ProcessNames.Any())
			{
				Console.WriteLine("Skipping process names unregistration.");
				return;
			}

			Console.WriteLine("Unregistering process names.");
			// TODO use sproc instead
			const string deleteCmdText = "DELETE FROM ProcessDescriptors WHERE Name=@name";
			using (var cnx = MgmtDbConnection)
			using (var cmd = new SqlCommand(deleteCmdText, cnx))
			{
				cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar));
				cnx.Open();
				foreach (var processName in ProcessNames)
				{
					cmd.Parameters["@name"].Value = processName;
					cmd.ExecuteNonQuery();
				}
			}
		}

		private SqlConnection MgmtDbConnection
		{
			get
			{
				var builder = new SqlConnectionStringBuilder {
					DataSource = DataSource,
					InitialCatalog = MANAGEMENT_DATABASE_NAME,
					IntegratedSecurity = true
				};
				return new SqlConnection(builder.ConnectionString);
			}
		}

		private bool MgmtDbExists()
		{
			var builder = new SqlConnectionStringBuilder {
				DataSource = DataSource,
				InitialCatalog = "master",
				IntegratedSecurity = true
			};
			using (var cnx = new SqlConnection(builder.ConnectionString))
			{
				// TODO use sproc instead if one exists in master or elsewhere...
				var cmd = new SqlCommand(string.Format("SELECT DB_ID('{0}')", MANAGEMENT_DATABASE_NAME), cnx);
				cnx.Open();
				return cmd.ExecuteScalar() != DBNull.Value;
			}
		}

		private string DataSource
		{
			get { return Context.Parameters["Server"] ?? Context.Parameters["DataSource"]; }
		}

		private const string MANAGEMENT_DATABASE_NAME = "BizTalkFactoryMgmtDb";

		private string[] _processNames;
		private RuleSet[] _ruleSets;
	}
}
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
using System.Configuration.Install;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Tracking
{
	public abstract class ProcessInstaller : Installer
	{
		#region Base Class Member Overrides

		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);
			RegisterProcessNames();
		}

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);
			UnregisterProcessNames();
		}

		#endregion

		protected abstract string[] ProcessNames { get; }

		private string DataSource
		{
			get { return Context.Parameters["Server"] ?? Context.Parameters["DataSource"]; }
		}

		[SuppressMessage("ReSharper", "CollectionNeverQueried.Local", Justification = "SqlConnectionStringBuilder")]
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

		private void RegisterProcessNames()
		{
			if (DataSource.IsNullOrEmpty() || !ProcessNames.Any())
			{
				Console.WriteLine("Skipping process names registration.");
				return;
			}

			Console.WriteLine("Registering process names.");
			// insert or update in case process name has not been previously deleted
			const string upsertCmdText = @"MERGE INTO monitoring_ProcessDescriptors PD
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
			const string deleteCmdText = "DELETE FROM monitoring_ProcessDescriptors WHERE Name=@name";
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

		[SuppressMessage("ReSharper", "CollectionNeverQueried.Local", Justification = "SqlConnectionStringBuilder")]
		private bool MgmtDbExists()
		{
			var builder = new SqlConnectionStringBuilder {
				DataSource = DataSource,
				InitialCatalog = "master",
				IntegratedSecurity = true
			};
			using (var cnx = new SqlConnection(builder.ConnectionString))
			{
				var cmd = new SqlCommand(string.Format("SELECT DB_ID('{0}')", MANAGEMENT_DATABASE_NAME), cnx);
				cnx.Open();
				return cmd.ExecuteScalar() != DBNull.Value;
			}
		}

		private const string MANAGEMENT_DATABASE_NAME = "BizTalkFactoryMgmtDb";
	}
}

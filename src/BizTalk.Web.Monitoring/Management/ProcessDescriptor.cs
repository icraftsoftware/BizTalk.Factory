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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Be.Stateless.BizTalk.Web.Extensions;
using Be.Stateless.Logging;

namespace Be.Stateless.BizTalk.Web.Management
{
	public class ProcessDescriptor
	{
		public static IEnumerable<ProcessDescriptor> List
		{
			get
			{
				return RegisteredProcessNames
					.OrderBy(n => n.ToFriendlyProcessName())
					.Select(name => new ProcessDescriptor { Name = name });
			}
		}

		public string FriendlyName
		{
			get { return Name.ToFriendlyProcessName(); }
		}

		public string Name { get; private set; }

		private static IEnumerable<string> RegisteredProcessNames
		{
			get
			{
				// TODO ?? fetch names via EF TrackingEntities instead and rename to MonitoringEntities ??
				var processNames = new DataTable();
				try
				{
					using (var cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["BizTalkFactoryMgmtDb"].ConnectionString))
					using (var cmd = new SqlCommand("SELECT Name FROM ProcessDescriptors", cnx))
					using (var adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(processNames);
					}
				}
				catch (Exception ex)
				{
					_logger.Error("Failed to enumerate messaging-only process names.", ex);
				}
				return processNames.AsEnumerable().Select(row => (string) row["Name"]);
			}
		}

	  private static readonly ILog _logger = LogManager.GetLogger(typeof(ProcessDescriptor));
	}
}
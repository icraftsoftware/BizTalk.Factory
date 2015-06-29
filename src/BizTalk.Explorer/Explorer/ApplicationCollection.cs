#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using Microsoft.BizTalk.ExplorerOM;
using BizTalkApplicationCollection = Microsoft.BizTalk.ExplorerOM.ApplicationCollection;

namespace Be.Stateless.BizTalk.Explorer
{
	public class ApplicationCollection
	{
		public Application this[string name]
		{
			get
			{
				var application = BizTalkApplicationCollection[name];
				if (application == null)
					throw new Exception(
						string.Format(
							"BizTalk Server Application '{0}' cannot be found in BizTalk Server Group [{1}].",
							name,
							BizTalkServerGroup.ManagementDatabase));
				return new Application(application);
			}
		}

		private BizTalkApplicationCollection BizTalkApplicationCollection
		{
			get
			{
				var explorer = new BtsCatalogExplorer();
				try
				{
					explorer.ConnectionString = BizTalkServerGroup.ManagementDatabase.ConnectionString;
					explorer.Refresh();
					return explorer.Applications;
				}
				catch
				{
					// TODO ? necessary ?
					explorer.DiscardChanges();
					throw;
				}
			}
		}
	}
}

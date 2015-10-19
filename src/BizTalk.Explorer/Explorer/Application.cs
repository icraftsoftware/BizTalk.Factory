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
using BizTalkApplication = Microsoft.BizTalk.ExplorerOM.Application;

namespace Be.Stateless.BizTalk.Explorer
{
	public class Application
	{
		public Application(BizTalkApplication application)
		{
			if (application == null) throw new ArgumentNullException("application");
			BizTalkApplication = application;
			Orchestrations = new OrchestrationCollection(BizTalkApplication.Orchestrations);
			ReceivePorts = new ReceivePortCollection(BizTalkApplication.ReceivePorts);
			SendPorts = new SendPortCollection(BizTalkApplication.SendPorts);
		}

		public OrchestrationCollection Orchestrations { get; private set; }

		public ReceivePortCollection ReceivePorts { get; private set; }

		public SendPortCollection SendPorts { get; private set; }

		private BizTalkApplication BizTalkApplication { get; set; }

		public void ApplyChanges()
		{
			BizTalkApplication.BtsCatalogExplorer.SaveChanges();
			// TODO ? BtsCatalogExplorer.Refresh() would be enough ?
			// reload application management data to ensure consistency
			BizTalkApplication = BizTalkServerGroup.Applications[BizTalkApplication.Name].BizTalkApplication;
		}
	}
}

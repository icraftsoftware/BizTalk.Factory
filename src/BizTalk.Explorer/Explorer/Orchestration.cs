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
using Microsoft.BizTalk.ExplorerOM;

namespace Be.Stateless.BizTalk.Explorer
{
	public class Orchestration
	{
		public Orchestration(BtsOrchestration orchestration)
		{
			if (orchestration == null) throw new ArgumentNullException("orchestration");
			BizTalkOrchestration = orchestration;
		}

		public OrchestrationStatus Status
		{
			get { return BizTalkOrchestration.Status; }
			set { BizTalkOrchestration.Status = value; }
		}

		private BtsOrchestration BizTalkOrchestration { get; set; }

		public void Start()
		{
			if (Status != OrchestrationStatus.Started) Status = OrchestrationStatus.Started;
		}

		public void Stop()
		{
			if (Status != OrchestrationStatus.Enlisted) Status = OrchestrationStatus.Enlisted;
		}

		public void Enlist()
		{
			if (Status == OrchestrationStatus.Enlisted) Status = OrchestrationStatus.Enlisted;
		}

		public void Unenlist()
		{
			if (Status != OrchestrationStatus.Unenlisted) Status = OrchestrationStatus.Unenlisted;
		}
	}
}

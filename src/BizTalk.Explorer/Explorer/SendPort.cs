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
using BizTalkSendPort = Microsoft.BizTalk.ExplorerOM.SendPort;

namespace Be.Stateless.BizTalk.Explorer
{
	public class SendPort
	{
		public SendPort(BizTalkSendPort port)
		{
			if (port == null) throw new ArgumentNullException("port");
			BizTalkSendPort = port;
		}

		public PortStatus Status
		{
			get { return BizTalkSendPort.Status; }
			set { BizTalkSendPort.Status = value; }
		}

		private BizTalkSendPort BizTalkSendPort { get; set; }

		public void Start()
		{
			if (Status != PortStatus.Started) Status = PortStatus.Started;
		}

		public void Stop()
		{
			if (Status != PortStatus.Stopped) Status = PortStatus.Stopped;
		}

		public void Enlist()
		{
			if (Status == PortStatus.Bound) Status = PortStatus.Stopped;
		}

		public void Unenlist()
		{
			if (Status != PortStatus.Bound) Status = PortStatus.Bound;
		}
	}
}

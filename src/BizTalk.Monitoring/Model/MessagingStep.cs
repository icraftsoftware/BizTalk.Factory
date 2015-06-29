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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Monitoring.Model
{
	// ReSharper disable ClassNeverInstantiated.Global
	// ReSharper disable UnusedAutoPropertyAccessor.Global
	// ReSharper disable UnusedMember.Global
	public class MessagingStep : IActivity
	{
		#region IActivity Members

		public long? RecordID { get; set; }
		public string ActivityID { get; set; }
		public DateTime BeginTime { get; set; }
		public string Name { get; set; }
		public string Status { get; set; }
		public DateTime LastModified { get; set; }

		#endregion

		public virtual MessageContext Context { get; set; }
		public virtual MessageBody Message { get; set; }
		public virtual List<Process> Processes { get; set; }

		public string ErrorCode { get; set; }
		public string ErrorDescription { get; set; }
		public string InterchangeID { get; set; }
		public string MachineName { get; set; }
		public string MessageID { get; set; }
		public int? MessageSize { get; set; }
		public string MessageType { get; set; }
		public int? RetryCount { get; set; }
		public string TransportLocation { get; set; }
		public string TransportType { get; set; }
		public string Value1 { get; set; }
		public string Value2 { get; set; }
		public string Value3 { get; set; }

		public string FriendlyMessageType
		{
			get
			{
				if (MessageType.IsNullOrEmpty()) return null;
				var match = Regex.Match(MessageType, @"^http://Microsoft\.LobServices\.Sap.*/Idoc/(.*)/.*$");
				if (match.Success) return match.Groups[1].Value;
				var i = MessageType.LastIndexOf("#", StringComparison.Ordinal);
				if (i > 0) return MessageType.Substring(i + 1);
				i = MessageType.IndexOf(",", StringComparison.Ordinal);
				if (i >= 0) return MessageType.Substring(0, i);
				return MessageType;
			}
		}
	}
}

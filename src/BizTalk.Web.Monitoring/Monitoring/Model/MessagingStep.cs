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
using System.Text.RegularExpressions;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Web.Monitoring.Model
{
	public partial class MessagingStep
	{
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

		public Context Context
		{
			get { return RawContext ?? new Context(); }
		}

		public Message Message
		{
			get { return RawMessage ?? new Message(); }
		}
	}
}
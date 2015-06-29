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
using System.Linq;
using System.Text;
using Be.Stateless.IO;

namespace Be.Stateless.BizTalk.Processes.Claim
{
	internal class FakeClaimStream : EnumerableStream
	{
		private static IEnumerable<string> Content
		{
			get
			{
				yield return "<ns0:Any xmlns:ns0='urn:schemas.stateless.be:biztalk:any:2012:12'>";
				yield return "<CorrelationToken>embedded-correlation-token</CorrelationToken>";
				yield return "<OutboundTransportLocation>outbound-transport-location</OutboundTransportLocation>";
				yield return "<ReceiverName>embedded-receiver-name</ReceiverName>";
				yield return "<SenderName>embedded-sender-name</SenderName>";
				for (var i = 0; i < 20000; i++)
				{
					yield return
						string.Format(
							"<extra-content><record><field>{0}</field><field>{1}</field><field>{2}</field></record></extra-content>",
							Guid.NewGuid(),
							Guid.NewGuid(),
							Guid.NewGuid());
				}
				yield return "</ns0:Any>";
			}
		}

		public FakeClaimStream() : base(Content.Select(s => Encoding.UTF8.GetBytes(s))) { }
	}
}

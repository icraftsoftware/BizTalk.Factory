#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class MessageDirectionExtensions
	{
		public static MessageDirection Direction(this IBaseMessage message)
		{
			// It's imperative to check outbound context properties first. If send port subscribes to a receive port, all
			// of the context properties of the receive location will also be present in the outbound context, though
			// demoted, which would confuse this code if it'd check the inbound context properties first.
			if (!message.GetProperty(BtsProperties.OutboundTransportLocation).IsNullOrEmpty()) return MessageDirection.Outbound;
			if (!message.GetProperty(BtsProperties.InboundTransportLocation).IsNullOrEmpty()) return MessageDirection.Inbound;
			throw new Exception("Unable to determine message direction.");
		}

		public static MessageDirection FailedDirection(this IBaseMessage message)
		{
			if (!message.GetProperty(ErrorReportProperties.OutboundTransportLocation).IsNullOrEmpty()) return MessageDirection.Outbound;
			if (!message.GetProperty(ErrorReportProperties.InboundTransportLocation).IsNullOrEmpty()) return MessageDirection.Inbound;
			throw new Exception("Unable to determine message direction.");
		}

		public static bool IsInbound(this MessageDirection messageDirection)
		{
			return messageDirection == MessageDirection.Inbound;
		}

		public static bool IsOutbound(this MessageDirection messageDirection)
		{
			return messageDirection == MessageDirection.Outbound;
		}
	}
}

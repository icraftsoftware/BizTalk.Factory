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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.ContextProperties;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public static class PortTypeExtensions
	{
		public static PortType PortType(this IBaseMessage message)
		{
			if (message.IsSolicitResponse()) return Message.PortType.SolicitResponseSendPort;
			if (message.IsRequestResponse()) return Message.PortType.RequestResponseReceivePort;
			if (message.Direction().IsInbound()) return Message.PortType.OneWayReceivePort;
			if (message.Direction().IsOutbound()) return Message.PortType.OneWaySendPort;
			throw new Exception("Unable to determine port type.");
		}

		public static bool IsOneWay(this PortType portType)
		{
			return portType == Message.PortType.OneWayReceivePort || portType == Message.PortType.OneWaySendPort;
		}

		public static bool IsTwoWay(this PortType portType)
		{
			return portType == Message.PortType.RequestResponseReceivePort || portType == Message.PortType.SolicitResponseSendPort;
		}

		public static bool IsInitiatingMessageExchangePattern(this IBaseMessage message)
		{
			return (message.PortType().IsRequestResponse() && message.Direction().IsInbound())
				|| (message.PortType().IsSolicitResponse() && message.Direction().IsOutbound())
				|| message.PortType().IsOneWay();
		}

		private static bool IsRequestResponse(this IBaseMessage message)
		{
			return message.GetProperty(BtsProperties.IsRequestResponse) ?? false;
		}

		public static bool IsRequestResponse(this PortType portType)
		{
			return portType == Message.PortType.RequestResponseReceivePort;
		}

		private static bool IsSolicitResponse(this IBaseMessage message)
		{
			if (message.GetProperty(BtsProperties.IsSolicitResponse) ?? false) return true;
			return message.Direction().IsInbound() && (message.GetProperty(BtsProperties.WasSolicitResponse) ?? false);
		}

		public static bool IsSolicitResponse(this PortType portType)
		{
			return portType == Message.PortType.SolicitResponseSendPort;
		}
	}
}

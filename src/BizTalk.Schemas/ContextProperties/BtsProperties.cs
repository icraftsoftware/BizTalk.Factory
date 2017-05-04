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

using System.Diagnostics.CodeAnalysis;
using BTS;

namespace Be.Stateless.BizTalk.ContextProperties
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class BtsProperties
	{
		public static readonly MessageContextProperty<AckRequired, bool> AckRequired
			= new MessageContextProperty<AckRequired, bool>();

		public static readonly MessageContextProperty<ActualRetryCount, int> ActualRetryCount
			= new MessageContextProperty<ActualRetryCount, int>();

		public static readonly MessageContextProperty<InboundTransportLocation, string> InboundTransportLocation
			= new MessageContextProperty<InboundTransportLocation, string>();

		public static readonly MessageContextProperty<InboundTransportType, string> InboundTransportType
			= new MessageContextProperty<InboundTransportType, string>();

		public static readonly MessageContextProperty<InterchangeID, string> InterchangeID
			= new MessageContextProperty<InterchangeID, string>();

		public static readonly MessageContextProperty<IsDynamicSend, bool> IsDynamicSend
			= new MessageContextProperty<IsDynamicSend, bool>();

		public static readonly MessageContextProperty<IsRequestResponse, bool> IsRequestResponse
			= new MessageContextProperty<IsRequestResponse, bool>();

		public static readonly MessageContextProperty<IsSolicitResponse, bool> IsSolicitResponse
			= new MessageContextProperty<IsSolicitResponse, bool>();

		public static readonly MessageContextProperty<MessageDestination, string> MessageDestination
			= new MessageContextProperty<MessageDestination, string>();

		public static readonly MessageContextProperty<MessageID, string> MessageID
			= new MessageContextProperty<MessageID, string>();

		public static readonly MessageContextProperty<MessageType, string> MessageType
			= new MessageContextProperty<MessageType, string>();

		public static readonly MessageContextProperty<Operation, string> Operation
			= new MessageContextProperty<Operation, string>();

		[SuppressMessage("ReSharper", "IdentifierTypo")]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public static readonly MessageContextProperty<OutboundTransportCLSID, string> OutboundTransportCLSID
			= new MessageContextProperty<OutboundTransportCLSID, string>();

		public static readonly MessageContextProperty<OutboundTransportLocation, string> OutboundTransportLocation
			= new MessageContextProperty<OutboundTransportLocation, string>();

		public static readonly MessageContextProperty<OutboundTransportType, string> OutboundTransportType
			= new MessageContextProperty<OutboundTransportType, string>();

		public static readonly MessageContextProperty<ReceiveLocationName, string> ReceiveLocationName
			= new MessageContextProperty<ReceiveLocationName, string>();

		public static readonly MessageContextProperty<ReceivePipelineConfig, string> ReceivePipelineConfig
			= new MessageContextProperty<ReceivePipelineConfig, string>();

		public static readonly MessageContextProperty<ReceivePortName, string> ReceivePortName
			= new MessageContextProperty<ReceivePortName, string>();

		public static readonly MessageContextProperty<RetryCount, int> RetryCount
			= new MessageContextProperty<RetryCount, int>();

		public static readonly MessageContextProperty<RetryInterval, int> RetryInterval
			= new MessageContextProperty<RetryInterval, int>();

		public static readonly MessageContextProperty<RouteMessageOnFailure, bool> RouteMessageOnFailure
			= new MessageContextProperty<RouteMessageOnFailure, bool>();

		public static readonly MessageContextProperty<SchemaStrongName, string> SchemaStrongName
			= new MessageContextProperty<SchemaStrongName, string>();

		public static readonly MessageContextProperty<SPName, string> SendPortName
			= new MessageContextProperty<SPName, string>();

		public static readonly MessageContextProperty<SuppressRoutingFailureDiagnosticInfo, bool> SuppressRoutingFailureDiagnosticInfo
			= new MessageContextProperty<SuppressRoutingFailureDiagnosticInfo, bool>();

		public static readonly MessageContextProperty<SuspendMessageOnRoutingFailure, bool> SuspendMessageOnRoutingFailure
			= new MessageContextProperty<SuspendMessageOnRoutingFailure, bool>();

		public static readonly MessageContextProperty<TransmitWorkID, string> TransmitWorkId
			= new MessageContextProperty<TransmitWorkID, string>();

		public static readonly MessageContextProperty<WasSolicitResponse, bool> WasSolicitResponse
			= new MessageContextProperty<WasSolicitResponse, bool>();
	}
}

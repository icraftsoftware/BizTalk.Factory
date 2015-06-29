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
using ErrorReport;

namespace Be.Stateless.BizTalk.ContextProperties
{
	// ReSharper disable InconsistentNaming
	/// <summary>
	/// The properties that are promoted to the context of an error message.
	/// </summary>
	/// <seealso href="http://msdn.microsoft.com/en-us/library/aa578516(BTS.10).aspx"/>
	/// <seealso href="http://support.microsoft.com/kb/944532"/>
	public static class ErrorReportProperties
	{
		/// <summary>
		/// Error description. Same diagnostic text as is written to the Application Event Log regarding this
		/// messaging failure.
		/// </summary>
		public static readonly MessageContextProperty<Description, string> Description
			= new MessageContextProperty<Description, string>();

		/// <summary>
		/// Indicates the type of message that the error contains. This property always contains the value
		/// FailedMessage, meaning that the error contains the original failed message.
		/// </summary>
		public static readonly MessageContextProperty<ErrorType, string> ErrorType
			= new MessageContextProperty<ErrorType, string>();

		/// <summary>
		/// Adapter that is associated with the error that generates the error report.
		/// </summary>
		public static readonly MessageContextProperty<FailureAdapter, string> FailureAdapter
			= new MessageContextProperty<FailureAdapter, string>();

		/// <summary>
		/// Error code. A hexadecimal value that is reported in the BizTalk Server Administration console.
		/// </summary>
		public static readonly MessageContextProperty<FailureCode, string> FailureCode
			= new MessageContextProperty<FailureCode, string>();

		/// <summary>
		/// Instance ID of the message or instance that has an error.
		/// </summary>
		public static readonly MessageContextProperty<FailureInstanceID, string> FailureInstanceID
			= new MessageContextProperty<FailureInstanceID, string>();

		/// <summary>
		/// Message ID of the message that has an error.
		/// </summary>
		public static readonly MessageContextProperty<FailureMessageID, string> FailureMessageID
			= new MessageContextProperty<FailureMessageID, string>();

		/// <summary>
		/// Current UTC time when the error report is generated.
		/// </summary>
		public static readonly MessageContextProperty<FailureTime, DateTime> FailureTime
			= new MessageContextProperty<FailureTime, DateTime>();

		/// <summary>
		/// URI of the receive location where the failure happened.
		/// </summary>
		/// <remarks>
		/// Promoted if the failure happened during inbound processing (in a receive port).
		/// Not promoted if the failure happened in a send port.
		/// </remarks>
		public static readonly MessageContextProperty<InboundTransportLocation, string> InboundTransportLocation
			= new MessageContextProperty<InboundTransportLocation, string>();

		/// <summary>
		/// Message type of failed message, or empty if message type is indeterminate.
		/// </summary>
		public static readonly MessageContextProperty<MessageType, string> MessageType
			= new MessageContextProperty<MessageType, string>();

		/// <summary>
		/// URI of the send location where the failure happened.
		/// </summary>
		/// <remarks>
		/// Promoted if the failure happened during outbound processing (in a send port).
		/// Not promoted if the failure happened in a receive port.
		/// </remarks>
		public static readonly MessageContextProperty<OutboundTransportLocation, string> OutboundTransportLocation
			= new MessageContextProperty<OutboundTransportLocation, string>();

		/// <summary>
		/// Name of the server where the failure happened.
		/// </summary>
		public static readonly MessageContextProperty<ProcessingServer, string> ProcessingServer
			= new MessageContextProperty<ProcessingServer, string>();

		/// <summary>
		/// Name of the receive port where the failure happened.
		/// </summary>
		/// <remarks>
		/// Promoted if the failure happened during inbound processing (in a receive port).
		/// Not promoted if the failure happened in a send port.
		/// </remarks>
		public static readonly MessageContextProperty<ReceivePortName, string> ReceivePortName
			= new MessageContextProperty<ReceivePortName, string>();

		/// <summary>
		/// This property provides the ID of the routing failure report that BizTalk Server generates when there
		/// is a routing failure. A routing failure report is a special message that BizTalk Server generates
		/// and suspends. This message does not have a body, but it has the context of the failed message. Using
		/// this ID, an error-handling orchestration or a send port can query the MessageBox database and
		/// process the routing failure report. For example, an orchestration may want to terminate the routing
		/// failure report after it gets the failed message.
		/// </summary>
		public static readonly MessageContextProperty<RoutingFailureReportID, string> RoutingFailureReportID
			= new MessageContextProperty<RoutingFailureReportID, string>();

		/// <summary>
		/// Name of the send port where the failure happened.
		/// </summary>
		/// <remarks>
		/// Promoted if the failure happened during outbound processing (in a send port).
		/// Not promoted if the failure happened in a receive port.
		/// </remarks>
		public static readonly MessageContextProperty<SendPortName, string> SendPortName
			= new MessageContextProperty<SendPortName, string>();
	}
}
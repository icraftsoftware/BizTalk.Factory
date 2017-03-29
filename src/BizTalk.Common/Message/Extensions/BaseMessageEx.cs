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
using System.IO;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	/// <summary>
	/// Various <see cref="IBaseMessage"/> extension methods that allow for shorter and <b>type-safe</b> statements.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Noticeably, offers <b>type-safe</b> and easier to use <see cref="IBaseMessageContext"/> property getters,
	/// setters, and promoters.
	/// </para>
	/// </remarks>
	public static class BaseMessageEx
	{
		public static bool HasFailed(this IBaseMessage message)
		{
			return message.GetProperty(ErrorReportProperties.ErrorType) != null;
		}

		public static Stream AsStream(this XLANGMessage message)
		{
			return message[0].AsStream();
		}

		#region message direction and exchange pattern

		public static MessageDirection FailedDirection(this IBaseMessage message)
		{
			if (!message.GetProperty(ErrorReportProperties.OutboundTransportLocation).IsNullOrEmpty()) return MessageDirection.Outbound;
			if (!message.GetProperty(ErrorReportProperties.InboundTransportLocation).IsNullOrEmpty()) return MessageDirection.Inbound;
			throw new Exception("Unable to determine message direction.");
		}

		public static MessageDirection Direction(this IBaseMessage message)
		{
			// It's imperative to check outbound context properties first. If send port subscribes to a receive port, all
			// of the context properties of the receive location will also be present in the outbound context, though
			// demoted, which would confuse this code if it'd check the inbound context properties first.
			if (!message.GetProperty(BtsProperties.OutboundTransportLocation).IsNullOrEmpty()) return MessageDirection.Outbound;
			if (!message.GetProperty(BtsProperties.InboundTransportLocation).IsNullOrEmpty()) return MessageDirection.Inbound;
			throw new Exception("Unable to determine message direction.");
		}

		public static bool IsInitiatingMessageExchangePattern(this IBaseMessage message)
		{
			return (message.PortType().IsRequestResponse() && message.Direction().IsInbound())
				|| (message.PortType().IsSolicitResponse() && message.Direction().IsOutbound())
				|| message.PortType().IsOneWay();
		}

		public static PortType PortType(this IBaseMessage message)
		{
			if (IsSolicitResponse(message)) return Message.PortType.SolicitResponseSendPort;
			if (IsRequestResponse(message)) return Message.PortType.RequestResponseReceivePort;
			if (message.Direction().IsInbound()) return Message.PortType.OneWayReceivePort;
			if (message.Direction().IsOutbound()) return Message.PortType.OneWaySendPort;
			throw new Exception("Unable to determine port type.");
		}

		private static bool IsRequestResponse(IBaseMessage message)
		{
			return message.GetProperty(BtsProperties.IsRequestResponse) ?? false;
		}

		private static bool IsSolicitResponse(IBaseMessage message)
		{
			if (message.GetProperty(BtsProperties.IsSolicitResponse) ?? false) return true;
			return message.Direction().IsInbound() && (message.GetProperty(BtsProperties.WasSolicitResponse) ?? false);
		}

		#endregion

		#region message's tracking context accessor

		/// <summary>
		/// Returns the <see cref="TrackingContext"/> associated to the <paramref name="message"/>.
		/// </summary>
		/// <param name="message">
		/// Message whose associated <see cref="TrackingContext"/> will be returned.
		/// </param>
		/// <returns>
		/// The <see cref="TrackingContext"/> associated to the <paramref name="message"/>.
		/// </returns>
		public static TrackingContext GetTrackingContext(this IBaseMessage message)
		{
			if (message == null) throw new ArgumentNullException("message");
			return new TrackingContext {
				ProcessActivityId = message.GetProperty(TrackingProperties.ProcessActivityId),
				ProcessingStepActivityId = message.GetProperty(TrackingProperties.ProcessingStepActivityId),
				MessagingStepActivityId = message.GetProperty(TrackingProperties.MessagingStepActivityId)
			};
		}

		/// <summary>
		/// Returns the <see cref="TrackingContext"/> associated to the <paramref name="message"/>.
		/// </summary>
		/// <param name="message">
		/// Message whose associated <see cref="TrackingContext"/> will be returned.
		/// </param>
		/// <param name="throwOnEmpty">
		/// <c>true</c> to throw an <see cref="InvalidOperationException"/> if none of the discrete activity Ids of the
		/// <see cref="TrackingContext"/> are set in the <paramref name="message"/>'s context.
		/// </param>
		/// <returns>
		/// The <see cref="TrackingContext"/> associated to the <paramref name="message"/>.
		/// </returns>
		public static TrackingContext GetTrackingContext(this IBaseMessage message, bool throwOnEmpty)
		{
			var trackingContext = message.GetTrackingContext();
			if (throwOnEmpty && trackingContext.IsEmpty()) throw new InvalidOperationException("Invalid TrackingContext: None of its discrete activity Ids are set.");
			return trackingContext;
		}

		/// <summary>
		/// Returns the <see cref="TrackingContext"/> associated to the <paramref name="message"/>.
		/// </summary>
		/// <param name="message">
		/// Message whose associated <see cref="TrackingContext"/> will be returned.
		/// </param>
		/// <returns>
		/// The <see cref="TrackingContext"/> associated to the <paramref name="message"/>.
		/// </returns>
		public static TrackingContext GetTrackingContext(this XLANGMessage message)
		{
			if (message == null) throw new ArgumentNullException("message");
			return new TrackingContext {
				ProcessActivityId = message.GetProperty(TrackingProperties.ProcessActivityId),
				ProcessingStepActivityId = message.GetProperty(TrackingProperties.ProcessingStepActivityId),
				MessagingStepActivityId = message.GetProperty(TrackingProperties.MessagingStepActivityId)
			};
		}

		/// <summary>
		/// Associate the specified <see cref="TrackingContext"/> <paramref name="context"/> to the <paramref
		/// name="message"/>.
		/// </summary>
		/// <param name="message">
		/// Message to associate the <paramref name="context"/> with.
		/// </param>
		/// <param name="context">
		/// The <see cref="TrackingContext"/> to be associated with the <paramref name="message"/>.
		/// </param>
		public static TrackingContext SetTrackingContext(this IBaseMessage message, TrackingContext context)
		{
			if (message == null) throw new ArgumentNullException("message");
			message.SetProperty(TrackingProperties.ProcessActivityId, context.ProcessActivityId);
			message.SetProperty(TrackingProperties.ProcessingStepActivityId, context.ProcessingStepActivityId);
			message.SetProperty(TrackingProperties.MessagingStepActivityId, context.MessagingStepActivityId);
			return context;
		}

		/// <summary>
		/// Associate the specified <see cref="TrackingContext"/> <paramref name="context"/> to the <paramref
		/// name="message"/>.
		/// </summary>
		/// <param name="message">
		/// Message to associate the <paramref name="context"/> with.
		/// </param>
		/// <param name="context">
		/// The <see cref="TrackingContext"/> to be associated with the <paramref name="message"/>.
		/// </param>
		public static TrackingContext SetTrackingContext(this XLANGMessage message, TrackingContext context)
		{
			if (message == null) throw new ArgumentNullException("message");
			message.SetProperty(TrackingProperties.ProcessActivityId, context.ProcessActivityId);
			message.SetProperty(TrackingProperties.ProcessingStepActivityId, context.ProcessingStepActivityId);
			message.SetProperty(TrackingProperties.MessagingStepActivityId, context.MessagingStepActivityId);
			return context;
		}

		#endregion
	}
}

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
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Tracking.Extensions
{
	public static class TrackingContextExtensions
	{
		/// <summary>
		/// Clear the <see cref="TrackingContext"/> associated to the <paramref name="message"/>.
		/// </summary>
		/// <param name="message">
		/// Message whose associated <see cref="TrackingContext"/> will be cleared.
		/// </param>
		public static void ClearTrackingContext(this IBaseMessage message)
		{
			if (message == null) throw new ArgumentNullException("message");
			message.DeleteProperty(TrackingProperties.ProcessActivityId);
			message.DeleteProperty(TrackingProperties.ProcessingStepActivityId);
			message.DeleteProperty(TrackingProperties.MessagingStepActivityId);
		}

		/// <summary>
		/// Clear the <see cref="TrackingContext"/> stored into the <paramref name="messageContext"/>.
		/// </summary>
		/// <param name="messageContext">
		/// Message context to clear the <see cref="TrackingContext"/> from.
		/// </param>
		public static void ClearTrackingContext(this IBaseMessageContext messageContext)
		{
			if (messageContext == null) throw new ArgumentNullException("messageContext");
			messageContext.DeleteProperty(TrackingProperties.ProcessActivityId);
			messageContext.DeleteProperty(TrackingProperties.ProcessingStepActivityId);
			messageContext.DeleteProperty(TrackingProperties.MessagingStepActivityId);
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
		/// Returns the <see cref="TrackingContext"/> stored into the <paramref name="messageContext"/>.
		/// </summary>
		/// <param name="messageContext">
		/// Message context that stores the <see cref="TrackingContext"/> to be returned.
		/// </param>
		/// <returns>
		/// The <see cref="TrackingContext"/> stored into the <paramref name="messageContext"/>.
		/// </returns>
		public static TrackingContext GetTrackingContext(this IBaseMessageContext messageContext)
		{
			if (messageContext == null) throw new ArgumentNullException("messageContext");
			return new TrackingContext {
				ProcessActivityId = messageContext.GetProperty(TrackingProperties.ProcessActivityId),
				ProcessingStepActivityId = messageContext.GetProperty(TrackingProperties.ProcessingStepActivityId),
				MessagingStepActivityId = messageContext.GetProperty(TrackingProperties.MessagingStepActivityId)
			};
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
		/// Store the given <see cref="TrackingContext"/> into the <paramref name="messageContext"/>.
		/// </summary>
		/// <param name="messageContext">
		/// Message context to store the <paramref name="trackingContext"/> into.
		/// </param>
		/// <param name="trackingContext">
		/// The <see cref="TrackingContext"/> to be stored into the <paramref name="messageContext"/>.
		/// </param>
		public static TrackingContext SetTrackingContext(this IBaseMessageContext messageContext, TrackingContext trackingContext)
		{
			if (messageContext == null) throw new ArgumentNullException("messageContext");
			messageContext.SetProperty(TrackingProperties.ProcessActivityId, trackingContext.ProcessActivityId);
			messageContext.SetProperty(TrackingProperties.ProcessingStepActivityId, trackingContext.ProcessingStepActivityId);
			messageContext.SetProperty(TrackingProperties.MessagingStepActivityId, trackingContext.MessagingStepActivityId);
			return trackingContext;
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
	}
}

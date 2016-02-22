#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Runtime.Caching;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Tracking.Messaging;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Tracks process and messaging step activities altogether and feeds the BAM tracking activity model and ensures the
	/// automatic propagation of the activity tracking context for solicit-response ports as well.
	/// </summary>
	public class ActivityTracker : IMicroPipelineComponent
	{
		#region Nested Type: Context

		internal class Context
		{
			internal Context(IPipelineContext pipelineContext, IBaseMessage message, ActivityTrackingModes trackingModes, PolicyName trackingResolutionPolicy)
			{
				Message = message;
				PipelineContext = pipelineContext;
				TrackingModes = trackingModes;
				TrackingResolver = TrackingResolver.Create(trackingResolutionPolicy, message);
			}

			internal IBaseMessage Message { get; private set; }

			internal IPipelineContext PipelineContext { get; private set; }

			internal ActivityTrackingModes TrackingModes { get; set; }

			internal TrackingResolver TrackingResolver { get; private set; }
		}

		#endregion

		public ActivityTracker()
		{
			TrackingContextCacheDuration = TimeSpan.FromSeconds(60);
			TrackingModes = ActivityTrackingModes.Body;
		}

		#region IMicroPipelineComponent Members

		public virtual IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			if (TrackingModes != ActivityTrackingModes.None)
			{
				var messageDirection = message.Direction();
				var isSolicitResponse = message.PortType().IsSolicitResponse();

				// TODO ?? what if message.BodyPart == null ??

				// tracking context can only be restored for the inbound message of a Solicit-Response MEP, i.e. when BizTalk was the initiator of the 2-way MEP
				if (messageDirection.IsInbound() && isSolicitResponse) RestoreCachedTrackingContext(message);

				var context = new Context(pipelineContext, message, TrackingModes, TrackingResolutionPolicyName);
				var activityTracker = Tracking.Messaging.ActivityTracker.Create(context);
				var messageBodyTracker = MessageBodyTracker.Create(context);

				// try to replace a claim token message with its original body's payload stream
				messageBodyTracker.TryCheckOutMessageBody();
				// ensure a TrackingStream has been setup and ascertain TrackingModes
				messageBodyTracker.SetupTracking();
				// perform the necessary setup to ensure TrackingContext is initiated early but information is collected at stream's end
				activityTracker.TrackActivity();
				// try to replace message body's original payload with a claim token, this will drain the stream and, hence, ensure activity tracking is completed
				messageBodyTracker.TryCheckInMessageBody();

				// tracking context can only be cached for the outbound message of a Solicit-Response MEP, i.e. when BizTalk is the initiator of the 2-way MEP
				if (messageDirection.IsOutbound() && isSolicitResponse) CacheTrackingContext(message);
			}
			return message;
		}

		#endregion

		/// <summary>
		/// How long activity tracking contexts will be kept in cache when propagated through solicit-response
		/// ports. Any negative value disables caching.
		/// </summary>
		[XmlElement(typeof(TimeSpanXmlSerializer))]
		public TimeSpan TrackingContextCacheDuration { get; set; }

		/// <summary>
		/// Level of tracking to use, or the extent of message data to capture.
		/// </summary>
		public ActivityTrackingModes TrackingModes { get; set; }

		/// <summary>
		/// Policy used to resolve either the process name of a messaging-only flow, <see
		/// cref="TrackingProperties.ProcessName"/>, or the archive's target location, <see
		/// cref="BizTalkFactoryProperties.ArchiveTargetLocation"/>, should neither one of them be found in message
		/// context.
		/// </summary>
		[XmlElement("TrackingResolutionPolicy", typeof(PolicyNameXmlSerializer))]
		public PolicyName TrackingResolutionPolicyName { get; set; }

		private void CacheTrackingContext(IBaseMessage message)
		{
			var duration = (int) TrackingContextCacheDuration.TotalSeconds;
			// if propagation of TrackingContext is not disabled, cache the current TrackingContext
			if (duration > -1)
			{
				_logger.DebugFormat("Caching current tracking context for {0} seconds", duration);
				TrackingContextCache.Instance.Set(
					message.GetProperty(BtsProperties.TransmitWorkId),
					message.GetTrackingContext(),
					duration);
			}
		}

		private void RestoreCachedTrackingContext(IBaseMessage message)
		{
			var duration = (int) TrackingContextCacheDuration.TotalSeconds;
			// if propagation of TrackingContext is not disabled, restore a previously cached TrackingContext
			if (duration > -1)
			{
				_logger.Debug("Restoring cached tracking context");
				var trackingContext = TrackingContextCache.Instance.Get(message.GetProperty(BtsProperties.TransmitWorkId));
				message.SetTrackingContext(trackingContext);
			}
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ActivityTracker));
	}
}

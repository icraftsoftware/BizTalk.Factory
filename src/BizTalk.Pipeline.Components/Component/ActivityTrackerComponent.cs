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
using System.ComponentModel;
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.Component.Interop;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Runtime.Caching;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Tracking.Messaging;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Tracks process and messaging step activities altogether and feeds the BAM tracking activity model and ensures the
	/// automatic propagation of the activity tracking context for solicit-response ports as well.
	/// </summary>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Any)]
	[Guid(CLASS_ID)]
	public class ActivityTrackerComponent : PipelineComponent
	{
		#region Nested type: Context

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

		/// <summary>
		/// Creates a new instance of <see cref="ActivityTrackerComponent"/>.
		/// </summary>
		public ActivityTrackerComponent()
		{
			TrackingContextRetentionDuration = 60;
			TrackingModes = ActivityTrackingModes.Body;
		}

		#region IBaseComponent members

		/// <summary>
		/// Description of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Description of the pipeline component.")]
		public override string Description
		{
			get
			{
				return "Tracks process and messaging step activities altogether and feeds the BAM tracking activity model;" +
					" ensures the automatic propagation of the activity tracking context for solicit-response ports as well.";
			}
		}

		#endregion

		#region IPersistPropertyBag members

		/// <summary>
		/// Gets class ID of component for usage from unmanaged code.
		/// </summary>
		/// <param name="classId">Class ID of the component</param>
		public override void GetClassID(out Guid classId)
		{
			classId = new Guid(CLASS_ID);
		}

		/// <summary>
		/// Loads configuration properties for the component
		/// </summary>
		/// <param name="propertyBag">Configuration property bag</param>
		protected override void Load(IPropertyBag propertyBag)
		{
			propertyBag.ReadProperty("TrackingContextRetentionDuration", value => TrackingContextRetentionDuration = value);
			propertyBag.ReadProperty<ActivityTrackingModes>("TrackingModes", value => TrackingModes = value);
			propertyBag.ReadProperty("TrackingResolutionPolicy", value => TrackingResolutionPolicy = PolicyName.Parse(value));
		}

		/// <summary>
		/// Saves the current component configuration into the property bag
		/// </summary>
		/// <param name="propertyBag">Configuration property bag</param>
		protected override void Save(IPropertyBag propertyBag)
		{
			propertyBag.WriteProperty("TrackingContextRetentionDuration", TrackingContextRetentionDuration);
			propertyBag.WriteProperty("TrackingModes", TrackingModes);
			propertyBag.WriteProperty("TrackingResolutionPolicy", TrackingResolutionPolicy.IfNotNull(prp => prp.ToString()));
		}

		#endregion

		#region Base Class Member Overrides

		/// <summary>
		/// Enables or disables the pipeline component.
		/// </summary>
		/// <remarks>
		/// Whether to let this pipeline component execute or not.
		/// </remarks>
		[Browsable(true)]
		[Description("Enables or disables the pipeline component.")]
		public override bool Enabled
		{
			get { return base.Enabled && TrackingModes != ActivityTrackingModes.None; }
			set { base.Enabled = value; }
		}

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var messageDirection = message.Direction();
			var isSolicitResponse = message.PortType().IsSolicitResponse();

			// TODO ?? what if message.BodyPart == null ??

			// tracking context can only be restored for the inbound message of a Solicit-Response MEP, i.e. when BizTalk was the initiator of the 2-way MEP
			if (messageDirection.IsInbound() && isSolicitResponse) RestoreCachedTrackingContext(message);

			var context = new Context(pipelineContext, message, TrackingModes, TrackingResolutionPolicy);
			var activityTracker = ActivityTracker.Create(context);
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

			return message;
		}

		#endregion

		/// <summary>
		/// How many seconds activity tracking contexts will be kept in cache when propagated through solicit-response
		/// ports. A value of -1 disables caching.
		/// </summary>
		[Browsable(true)]
		[Description("How many seconds activity tracking contexts will be kept in cache when propagated through solicit-response ports. Any negative value disables caching.")]
		// ReSharper disable once MemberCanBePrivate.Global
		public int TrackingContextRetentionDuration { get; set; }

		/// <summary>
		/// Level of tracking to use, or the extent of message data to capture.
		/// </summary>
		[Browsable(true)]
		[Description("Level of tracking to use, or the extent of message data to capture.")]
		public ActivityTrackingModes TrackingModes { get; set; }

		/// <summary>
		/// Policy used to resolve either the process name of a messaging-only flow, <see
		/// cref="TrackingProperties.ProcessName"/>, or the archive's target location, <see
		/// cref="BizTalkFactoryProperties.ArchiveTargetLocation"/>, should neither one of them be found in message
		/// context.
		/// </summary>
		[Browsable(true)]
		[Description("Policy used to resolve either the process name of a messaging-only flow or the archive's target location should neither be found in message context.")]
		[TypeConverter(typeof(PolicyNameConverter))]
		// ReSharper disable once MemberCanBePrivate.Global
		public PolicyName TrackingResolutionPolicy { get; set; }

		#region TrackingContext Propagation

		private void CacheTrackingContext(IBaseMessage message)
		{
			// if propagation of TrackingContext is not disabled, cache the current TrackingContext
			if (TrackingContextRetentionDuration > -1)
			{
				_logger.DebugFormat("Caching current tracking context for {0} seconds", TrackingContextRetentionDuration);
				TrackingContextCache.Instance.Add(
					message.GetProperty(BtsProperties.TransmitWorkId),
					message.GetTrackingContext(),
					TrackingContextRetentionDuration);
			}
		}

		private void RestoreCachedTrackingContext(IBaseMessage message)
		{
			// if propagation of TrackingContext is not disabled, restore a previously cached TrackingContext
			if (TrackingContextRetentionDuration > -1)
			{
				_logger.Debug("Restoring cached tracking context");
				var trackingContext = TrackingContextCache.Instance.Remove(message.GetProperty(BtsProperties.TransmitWorkId));
				message.SetTrackingContext(trackingContext);
			}
		}

		#endregion

		private const string CLASS_ID = "1b4c83dd-7e71-453c-80de-457f70e8a703";
		private static readonly ILog _logger = LogManager.GetLogger(typeof(ActivityTrackerComponent));
	}
}

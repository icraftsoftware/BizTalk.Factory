#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.Extensions;
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
		/// <summary>
		/// Creates a new instance of <see cref="ActivityTrackerComponent"/>.
		/// </summary>
		public ActivityTrackerComponent()
		{
			_microComponent = new ActivityTracker();
		}

		protected ActivityTrackerComponent(ActivityTracker batchTracker)
		{
			_microComponent = batchTracker;
		}

		#region Base Class Member Overrides

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
			return _microComponent.Execute(pipelineContext, message);
		}

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

		/// <summary>
		/// How many seconds activity tracking contexts will be kept in cache when propagated through solicit-response
		/// ports. Any negative value disables caching.
		/// </summary>
		[Browsable(true)]
		[Description("How many seconds activity tracking contexts will be kept in cache when propagated through solicit-response ports. Any negative value disables caching.")]
		public int TrackingContextRetentionDuration
		{
			get { return _microComponent.TrackingContextRetentionDuration; }
			set { _microComponent.TrackingContextRetentionDuration = value; }
		}

		/// <summary>
		/// Level of tracking to use, or the extent of message data to capture.
		/// </summary>
		[Browsable(true)]
		[Description("Level of tracking to use, or the extent of message data to capture.")]
		public ActivityTrackingModes TrackingModes
		{
			get { return _microComponent.TrackingModes; }
			set { _microComponent.TrackingModes = value; }
		}

		/// <summary>
		/// Policy used to resolve either the process name of a messaging-only flow, <see
		/// cref="TrackingProperties.ProcessName"/>, or the archive's target location, <see
		/// cref="BizTalkFactoryProperties.ArchiveTargetLocation"/>, should neither one of them be found in message
		/// context.
		/// </summary>
		[Browsable(true)]
		[Description("Policy used to resolve either the process name of a messaging-only flow or the archive's target location should neither be found in message context.")]
		[TypeConverter(typeof(PolicyNameConverter))]
		public PolicyName TrackingResolutionPolicy
		{
			get { return _microComponent.TrackingResolutionPolicy; }
			set { _microComponent.TrackingResolutionPolicy = value; }
		}

		private const string CLASS_ID = "1b4c83dd-7e71-453c-80de-457f70e8a703";
		private readonly ActivityTracker _microComponent;
	}
}

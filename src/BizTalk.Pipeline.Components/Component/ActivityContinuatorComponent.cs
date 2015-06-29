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
using Be.Stateless.BizTalk.Runtime.Caching;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Any)]
	[Guid(CLASS_ID)]
	public class ActivityContinuatorComponent : PipelineComponent
	{
		public ActivityContinuatorComponent()
		{
			CacheExpirationDuration = 60;
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Description of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Description of the pipeline component.")]
		public override string Description
		{
			get { return "Ensures the automatic propagation of the activity tracking context for a solicit-response port."; }
		}

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			// tracking context can only be continuated when BizTalk initiates a 2-way MEP, i.e. a Solicit-Response
			if (message.PortType().IsSolicitResponse()) ContinueTracking(message);
			return message;
		}

		#endregion

		#region IPersistPropertyBag members

		/// <summary>
		/// Gets class ID of component for usage from unmanaged code.
		/// </summary>
		/// <param name="classId">
		/// Class ID of the component
		/// </param>
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
			propertyBag.ReadProperty("CacheExpirationDuration", value => CacheExpirationDuration = value);
		}

		/// <summary>
		/// Saves the current component configuration into the property bag
		/// </summary>
		/// <param name="propertyBag">Configuration property bag</param>
		protected override void Save(IPropertyBag propertyBag)
		{
			propertyBag.WriteProperty("CacheExpirationDuration", CacheExpirationDuration);
		}

		#endregion

		[Browsable(true)]
		[Description("Duration in seconds for how long activity tracking contexts will be kept in cache.")]
		public int CacheExpirationDuration { get; set; }

		private void ContinueTracking(IBaseMessage message)
		{
			if (message.Direction().IsOutbound()) CacheTrackingContext(message);
			else RestoreCachedTrackingContext(message);
		}

		private void CacheTrackingContext(IBaseMessage message)
		{
			TrackingContextCache.Instance.Add(
				message.GetProperty(BtsProperties.TransmitWorkId),
				message.GetTrackingContext(),
				CacheExpirationDuration);
		}

		private void RestoreCachedTrackingContext(IBaseMessage message)
		{
			var trackingContext = TrackingContextCache.Instance.Remove(message.GetProperty(BtsProperties.TransmitWorkId));
			message.SetTrackingContext(trackingContext);
		}

		private const string CLASS_ID = "cd984c0c-e1a3-41dd-9a73-fa68e6cd5ea7";
	}
}

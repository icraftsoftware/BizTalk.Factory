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
using System.ComponentModel;
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.Component.Interop;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Any)]
	[Guid(CLASS_ID)]
	public class FailedMessageRoutingEnablerComponent : PipelineComponent
	{
		public FailedMessageRoutingEnablerComponent()
		{
			EnableFailedMessageRouting = true;
			SuppressRoutingFailureReport = true;
		}

		#region IBaseComponent members

		/// <summary>
		/// Description of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Description of the pipeline component.")]
		public override string Description
		{
			get { return "Enables routing of failed messages and prevents routing failure reports from being generated."; }
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
			propertyBag.ReadProperty("EnableFailedMessageRouting", value => EnableFailedMessageRouting = value);
			propertyBag.ReadProperty("SuppressRoutingFailureReport", value => SuppressRoutingFailureReport = value);
		}

		/// <summary>
		/// Saves the current component configuration into the property bag
		/// </summary>
		/// <param name="propertyBag">Configuration property bag</param>
		protected override void Save(IPropertyBag propertyBag)
		{
			propertyBag.WriteProperty("EnableFailedMessageRouting", EnableFailedMessageRouting);
			propertyBag.WriteProperty("SuppressRoutingFailureReport", SuppressRoutingFailureReport);
		}

		#endregion

		#region Base Class Member Overrides

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			if (EnableFailedMessageRouting) message.SetProperty(BtsProperties.RouteMessageOnFailure, true);
			if (SuppressRoutingFailureReport) message.SetProperty(BtsProperties.SuppressRoutingFailureDiagnosticInfo, true);
			return message;
		}

		#endregion

		[Browsable(true)]
		[Description("Enables or disables routing of failed messages and whether to avoid suspended message instances.")]
		public bool EnableFailedMessageRouting { get; set; }

		[Browsable(true)]
		[Description("Whether to prevent the generation of a routing failure report upon message routing failure.")]
		public bool SuppressRoutingFailureReport { get; set; }

		private const string CLASS_ID = "f3e3e009-379c-43c7-a41a-4dd6a5bcf95b";
	}
}

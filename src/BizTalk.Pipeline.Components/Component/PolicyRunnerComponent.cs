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
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Any)]
	[Guid(CLASS_ID)]
	public class PolicyRunnerComponent : PipelineComponent
	{
		#region Base Class Member Overrides

		/// <summary>
		/// Description of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Description of the pipeline component.")]
		public override string Description
		{
			get { return "Executes a BizTalk Business Rule Policy against facts asserted from the message context."; }
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
			get { return base.Enabled && Policy != null; }
			set { base.Enabled = value; }
		}

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Scheduling policy '{0}' for {1} execution.", Policy.ToString(), ExecutionMode.ToString());

			if (ExecutionMode == PluginExecutionMode.Deferred)
			{
				message.BodyPart.WrapOriginalDataStream(
					originalStream => {
						var substitutionStream = new EventingReadStream(originalStream);
						substitutionStream.AfterLastReadEvent += (src, args) => {
							if (_logger.IsDebugEnabled) _logger.DebugFormat("Executing policy '{0}' that was scheduled for deferred execution.", Policy.ToString());
							RuleEngine.Policy.Execute(Policy, new Context(message.Context));
						};
						return substitutionStream;
					},
					pipelineContext.ResourceTracker);
			}
			else
			{
				RuleEngine.Policy.Execute(Policy, new Context(message.Context));
			}

			return message;
		}

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
			propertyBag.ReadProperty<PluginExecutionMode>("ExecutionMode", value => ExecutionMode = value);
			propertyBag.ReadProperty("Policy", value => Policy = PolicyName.Parse(value));
		}

		/// <summary>
		/// Saves the current component configuration into the property bag
		/// </summary>
		/// <param name="propertyBag">Configuration property bag</param>
		protected override void Save(IPropertyBag propertyBag)
		{
			propertyBag.WriteProperty("ExecutionMode", ExecutionMode);
			propertyBag.WriteProperty("Policy", Policy.IfNotNull(p => p.ToString()));
		}

		#endregion

		/// <summary>
		/// The Business Rule Policy execution mode, either <see cref="PluginExecutionMode.Immediate"/> or <see
		/// cref="PluginExecutionMode.Deferred"/>.
		/// </summary>
		[Browsable(true)]
		[Description("The Business Rule Policy execution mode, either Immediate or Deferred.")]
		public PluginExecutionMode ExecutionMode { get; set; }

		/// <summary>
		/// The Business Rule Policy to be executed.
		/// </summary>
		[Browsable(true)]
		[Description("The Business Rule Policy to be executed.")]
		[TypeConverter(typeof(PolicyNameConverter))]
		public PolicyName Policy { get; set; }

		private const string CLASS_ID = "d320f64f-59a5-4c2b-bf6c-22409b7893a9";
		private static readonly ILog _logger = LogManager.GetLogger(typeof(PolicyRunnerComponent));
	}
}

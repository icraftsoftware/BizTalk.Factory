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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.Component.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Runs a sequence of micro pipeline components, similarly to what a pipeline would do if the micro pipeline
	/// components were regular pipeline components.
	/// </summary>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Any)]
	[Guid(CLASS_ID)]
	public class MicroPipelineComponent : PipelineComponent
	{
		public MicroPipelineComponent()
		{
			Components = Enumerable.Empty<IMicroPipelineComponent>();
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Description of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Description of the pipeline component.")]
		public override string Description
		{
			get { return "Runs a sequence of micro pipeline components as if they were regular pipeline components."; }
		}

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			return Components.Aggregate(
				message,
				(inputMessage, microPipelineComponent) => microPipelineComponent.Execute(pipelineContext, inputMessage));
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
			propertyBag.ReadProperty("Components", value => Components = MicroPipelineComponentEnumerableConverter.Deserialize(value));
		}

		/// <summary>
		/// Saves the current component configuration into the property bag
		/// </summary>
		/// <param name="propertyBag">Configuration property bag</param>
		protected override void Save(IPropertyBag propertyBag)
		{
			propertyBag.WriteProperty("Components", MicroPipelineComponentEnumerableConverter.Serialize(Components));
		}

		#endregion

		/// <summary>
		/// List of micro pipeline components that will be run in sequence by the micro pipeline.
		/// </summary>
		[Browsable(true)]
		[Description("List of micro pipeline components that will be run in sequence by the micro pipeline.")]
		[TypeConverter(typeof(MicroPipelineComponentEnumerableConverter))]
		public IEnumerable<IMicroPipelineComponent> Components { get; set; }

		private const string CLASS_ID = "02dd03e8-9509-4799-a196-a8c68e02d933";
		// TODO private static readonly ILog _logger = LogManager.GetLogger(typeof(MicroPipelineComponent));
	}
}

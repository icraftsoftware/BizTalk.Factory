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
using System.Configuration;
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.Component.Interop;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Delegates building of message context to a pluggable <see cref="IContextBuilder"/> component.
	/// </summary>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Any)]
	[Guid(CLASS_ID)]
	public class ContextBuilderComponent : PipelineComponent
	{
		public ContextBuilderComponent()
		{
			_microComponent = new ContextBuilder();
		}

		#region Base Class Member Overrides

		[Browsable(false)]
		public override string Description
		{
			get { return "Delegates building of message context to a pluggable builder component."; }
		}

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			return _microComponent.Execute(pipelineContext, message);
		}

		public override void GetClassID(out Guid classId)
		{
			classId = new Guid(CLASS_ID);
		}

		protected override void Load(IPropertyBag propertyBag)
		{
			propertyBag.ReadProperty<PluginExecutionMode>("ExecutionMode", value => ExecutionMode = value);
			propertyBag.ReadProperty("Builder", value => Builder = Type.GetType(value, true));
		}

		protected override void Save(IPropertyBag propertyBag)
		{
			propertyBag.WriteProperty("ExecutionMode", ExecutionMode);
			propertyBag.WriteProperty("Builder", Builder.IfNotNull(m => m.AssemblyQualifiedName));
		}

		#endregion

		/// <summary>
		/// The type name of the context builder plugin that will be called upon.
		/// </summary>
		[Browsable(true)]
		[Description("The type name of the context builder plugin that will be called upon.")]
		[TypeConverter(typeof(TypeNameConverter))]
		public Type Builder
		{
			get { return _microComponent.BuilderType; }
			set { _microComponent.BuilderType = value; }
		}

		/// <summary>
		/// The plugin execution mode, either <see cref="PluginExecutionMode.Immediate"/> or <see cref="PluginExecutionMode.Deferred"/>.
		/// </summary>
		[Browsable(true)]
		[Description("The plugin execution mode, either Immediate or Deferred.")]
		public PluginExecutionMode ExecutionMode
		{
			get { return _microComponent.ExecutionMode; }
			set { _microComponent.ExecutionMode = value; }
		}

		private const string CLASS_ID = "6ecc3f2a-27a1-432b-a715-6ea7110b0f18";
		private readonly ContextBuilder _microComponent;
	}
}

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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
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
	public class ContextBuilderComponent : ExtensiblePipelineComponent<IContextBuilder>
	{
		#region Base Class Member Overrides

		[Browsable(false)]
		public override string Description
		{
			get { return "Delegates building of message context to a pluggable builder component."; }
		}

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			ResolvePlugin(message, BizTalkFactoryProperties.ContextBuilderTypeName, Builder)
				.IfNotNull(
					contextBuilder => {
						if (ExecutionMode == PluginExecutionMode.Deferred)
						{
							if (_logger.IsDebugEnabled) _logger.DebugFormat("Scheduling builder plugin '{0}' for deferred execution.", Builder.ToString());
							message.BodyPart.WrapOriginalDataStream(
								originalStream => {
									var substitutionStream = new EventingReadStream(originalStream);
									substitutionStream.AfterLastReadEvent += (src, args) => {
										if (_logger.IsDebugEnabled) _logger.DebugFormat("Executing builder plugin '{0}' that was scheduled for deferred execution.", Builder.ToString());
										contextBuilder.Execute(message.Context);
									};
									return substitutionStream;
								},
								pipelineContext.ResourceTracker);
						}
						else
						{
							if (_logger.IsDebugEnabled) _logger.DebugFormat("Executing builder plugin '{0}' that is scheduled for immediate execution.", Builder.ToString());
							contextBuilder.Execute(message.Context);
						}
					});
			return message;
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
		public Type Builder { get; set; }

		/// <summary>
		/// The plugin execution mode, either <see cref="PluginExecutionMode.Immediate"/> or <see cref="PluginExecutionMode.Deferred"/>.
		/// </summary>
		[Browsable(true)]
		[Description("The plugin execution mode, either Immediate or Deferred.")]
		public PluginExecutionMode ExecutionMode { get; set; }

		private const string CLASS_ID = "6ecc3f2a-27a1-432b-a715-6ea7110b0f18";
		private static readonly ILog _logger = LogManager.GetLogger(typeof(ContextBuilderComponent));
	}
}

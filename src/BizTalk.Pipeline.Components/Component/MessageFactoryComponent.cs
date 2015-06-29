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
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Pipeline component that replaces the current message's <see cref="IBaseMessage.BodyPart"/> by a new one whose
	/// creation is delegated to either a contextual or configurable message factory.
	/// </summary>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Any)]
	[Guid(CLASS_ID)]
	public class MessageFactoryComponent : ExtensiblePipelineComponent<IMessageFactory>
	{
		#region Base Class Member Overrides

		[Browsable(false)]
		public override string Description
		{
			get { return "Replace current message by a new one whose creation is delegated to a message factory."; }
		}

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			ResolvePlugin(message, BizTalkFactoryProperties.MessageFactoryTypeName, Factory)
				.IfNotNull(messageFactory => message.BodyPart.SetDataStream(messageFactory.CreateMessage(message), pipelineContext.ResourceTracker));
			return message;
		}

		public override void GetClassID(out Guid classId)
		{
			classId = new Guid(CLASS_ID);
		}

		protected override void Load(IPropertyBag propertyBag)
		{
			propertyBag.ReadProperty("Factory", value => Factory = Type.GetType(value, true));
		}

		protected override void Save(IPropertyBag propertyBag)
		{
			propertyBag.WriteProperty("Factory", Factory.IfNotNull(m => m.AssemblyQualifiedName));
		}

		#endregion

		/// <summary>
		/// The type name of the message factory that will be called to create the message.
		/// </summary>
		[Browsable(true)]
		[Description("The type name of the BizTalk message factory.")]
		[TypeConverter(typeof(TypeNameConverter))]
		public Type Factory { get; set; }

		private const string CLASS_ID = "86117eff-8636-4eb9-a491-2d83c449c8d2";
	}
}

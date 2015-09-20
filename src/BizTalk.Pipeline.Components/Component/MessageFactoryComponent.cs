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
using System.IO;
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.Component.Interop;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Micro pipeline component that replaces the <see cref="Stream"/> of the current message's <see
	/// cref="IBaseMessage.BodyPart"/> by a new one whose creation is delegated to either a contextual or statically
	/// configurable <see cref="IMessageFactory"/> plugin.
	/// </summary>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Any)]
	[Guid(CLASS_ID)]
	public class MessageFactoryComponent : PipelineComponent
	{
		public MessageFactoryComponent()
		{
			_microComponent = new MessageBodyStreamFactory();
		}

		#region Base Class Member Overrides

		[Browsable(false)]
		public override string Description
		{
			get { return "Replace current message by a new one whose creation is delegated to a message factory."; }
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
			propertyBag.ReadProperty("Factory", value => Factory = Type.GetType(value, true));
		}

		protected override void Save(IPropertyBag propertyBag)
		{
			propertyBag.WriteProperty("Factory", Factory.IfNotNull(m => m.AssemblyQualifiedName));
		}

		#endregion

		/// <summary>
		/// The type of the <see cref="IMessageFactory"/> plugin that will be called to create the <see cref="Stream"/> of
		/// the message's <see cref="IBaseMessage.BodyPart"/>.
		/// </summary>
		[Browsable(true)]
		[Description("The type name of the BizTalk message factory.")]
		[TypeConverter(typeof(TypeNameConverter))]
		public Type Factory
		{
			get { return _microComponent.FactoryType; }
			set { _microComponent.FactoryType = value; }
		}

		private const string CLASS_ID = "86117eff-8636-4eb9-a491-2d83c449c8d2";
		private readonly MessageBodyStreamFactory _microComponent;
	}
}

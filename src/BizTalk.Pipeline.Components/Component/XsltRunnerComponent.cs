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
using System.Text;
using Be.Stateless.BizTalk.Component.Interop;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.Extensions;
using Be.Stateless.Text;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Pipeline component that applies an XSL Transformation on messages along their way in the pipeline.
	/// </summary>
	/// <remarks>
	/// Contrary to maps statically-configured at the receive or send port level, <see cref="XsltRunnerComponent"/>
	/// allows for:
	/// <list type="bullet">
	/// <item>the transform to be dynamically chosen;</item>
	/// <item>the transform to be executed anywhere in the pipeline. This is particularly interesting, as we can choose
	/// to place it before or after an XML assembler/disassembler;</item>
	/// <item>the execution to be conditional;</item>
	/// <item>arguments to be dynamically supplied to the transform.</item>
	/// </list>
	/// </remarks>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Any)]
	[Guid(CLASS_ID)]
	public class XsltRunnerComponent : PipelineComponent
	{
		public XsltRunnerComponent()
		{
			_microComponent = new XsltRunner();
		}

		protected XsltRunnerComponent(XsltRunner xsltRunner)
		{
			_microComponent = xsltRunner;
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Description of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Description of the pipeline component.")]
		public override string Description
		{
			get { return "Applies an XSL Transformation to the message."; }
		}

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			return _microComponent.Execute(pipelineContext, message);
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
			propertyBag.ReadProperty("Encoding", value => Encoding = EncodingConverter.Deserialize(value));
			propertyBag.ReadProperty("Map", value => Map = Type.GetType(value, true));
		}

		/// <summary>
		/// Saves the current component configuration into the property bag
		/// </summary>
		/// <param name="propertyBag">Configuration property bag</param>
		protected override void Save(IPropertyBag propertyBag)
		{
			propertyBag.WriteProperty("Encoding", EncodingConverter.Serialize(Encoding));
			propertyBag.WriteProperty("Map", Map.IfNotNull(m => m.AssemblyQualifiedName));
		}

		#endregion

		/// <summary>
		/// Encoding to use for output and, if Unicode, whether to emit a byte order mark.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="UTF8Encoding"/> with a BOM preamble.
		/// </remarks>
		[Browsable(true)]
		[Description("Encoding to use for output and, if Unicode, whether to emit a byte order mark.")]
		[TypeConverter(typeof(EncodingConverter))]
		public Encoding Encoding
		{
			get { return _microComponent.Encoding; }
			set { _microComponent.Encoding = value; }
		}

		/// <summary>
		/// The type name of the BizTalk Map to apply on the pipeline message.
		/// </summary>
		[Browsable(true)]
		[Description("The type name of the BizTalk Map to apply.")]
		[TypeConverter(typeof(TypeNameConverter))]
		public Type Map
		{
			get { return _microComponent.MapType; }
			set { _microComponent.MapType = value; }
		}

		private const string CLASS_ID = "2b3d21e9-af69-451a-b35b-f582f42affe8";
		private readonly XsltRunner _microComponent;
	}
}

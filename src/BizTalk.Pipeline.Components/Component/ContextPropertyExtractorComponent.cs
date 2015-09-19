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
using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.Component.Interop;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.XPath;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// This component allows to promote or write properties in the message context whose values are extracted out of an
	/// XML message by defining less restrictive XPath expressions than the traditional canonical XPath expressions
	/// supported by BizTalk Server; limitations are however still present and relatively strong.
	/// </summary>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Any)]
	[Guid(CLASS_ID)]
	public class ContextPropertyExtractorComponent : PipelineComponent
	{
		/// <summary>
		/// Creates a new intansce of a <see cref="ContextPropertyExtractorComponent"/>.
		/// </summary>
		public ContextPropertyExtractorComponent()
		{
			_microComponent = new ContextPropertyExtractor();
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Description of the pipeline component.
		/// </summary>
		[Browsable(false)]
		[Description("Description of the pipeline component.")]
		public override string Description
		{
			get { return "Promotes or writes properties in context by extracting values out of messages using XPath expressions."; }
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
			propertyBag.ReadProperty("Extractors", value => Extractors = XPathExtractorEnumerableConverter.Deserialize(value));
		}

		/// <summary>
		/// Saves the current component configuration into the property bag
		/// </summary>
		/// <param name="propertyBag">Configuration property bag</param>
		protected override void Save(IPropertyBag propertyBag)
		{
			propertyBag.WriteProperty("Extractors", XPathExtractorEnumerableConverter.Serialize(Extractors));
		}

		#endregion

		/// <summary>
		/// XPath expressions used to extract values out of XML message and either promote or write them in the context.
		/// </summary>
		[Browsable(true)]
		[Description("Pipeline's configuration of the properties to extract out of the current message.")]
		[TypeConverter(typeof(XPathExtractorEnumerableConverter))]
		public IEnumerable<XPathExtractor> Extractors
		{
			get { return _microComponent.Extractors; }
			set { _microComponent.Extractors = value; }
		}

		private const string CLASS_ID = "7428622e-9b6a-4b2b-b895-56271d6d557c";
		private readonly ContextPropertyExtractor _microComponent;
	}
}

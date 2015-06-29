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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.Component.Interop;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.Extensions;
using Be.Stateless.Text;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Component
{
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Decoder)]
	[ComponentCategory(CategoryTypes.CATID_Encoder)]
	[Guid(CLASS_ID)]
	public class XmlTranslatorComponent : PipelineComponent
	{
		public XmlTranslatorComponent()
		{
			Modes = XmlTranslationModes.Default;
			Encoding = new UTF8Encoding();
			Translations = XmlTranslationSet.Empty;
		}

		#region Base Class Member Overrides

		protected internal override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var replacementList = BuildReplacementSet(message);
			if (replacementList.Items.Any())
			{
				message.BodyPart.WrapOriginalDataStream(
					originalStream => {
						var substitutionStream = new XmlTranslatorStream(
							new XmlTextReader(originalStream),
							Encoding,
							replacementList.Items,
							Modes);
						return substitutionStream;
					},
					pipelineContext.ResourceTracker);
			}
			return message;
		}

		#endregion

		#region IBaseComponent members

		/// <summary>
		/// Description of the component
		/// </summary>
		[Browsable(false)]
		public override string Description
		{
			get { return "This component moves elements (and optionally attributes) from one namespace to another in the XML stream constituting the body of the message."; }
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
			propertyBag.ReadProperty("Encoding", value => Encoding = EncodingConverter.Deserialize(value));
			propertyBag.ReadProperty<XmlTranslationModes>("Modes", value => Modes = value);
			propertyBag.ReadProperty("Translations", value => Translations = XmlTranslationSetConverter.Deserialize(value));
		}

		/// <summary>
		/// Saves the current component configuration into the property bag
		/// </summary>
		/// <param name="propertyBag">Configuration property bag</param>
		protected override void Save(IPropertyBag propertyBag)
		{
			propertyBag.WriteProperty("Encoding", EncodingConverter.Serialize(Encoding));
			propertyBag.WriteProperty("Modes", Modes);
			propertyBag.WriteProperty("Translations", XmlTranslationSetConverter.Serialize(Translations));
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
		public Encoding Encoding { get; set; }

		[Browsable(true)]
		[Description("Collection of translations.")]
		[TypeConverter(typeof(XmlTranslationSetConverter))]
		public XmlTranslationSet Translations { get; set; }

		[Browsable(true)]
		[Description("The replacement Modes.")]
		public XmlTranslationModes Modes { get; set; }

		#region XmlNamespaceTranslation

		internal XmlTranslationSet BuildReplacementSet(IBaseMessage message)
		{
			var translations = message.GetProperty(BizTalkFactoryProperties.XmlTranslations);
			if (translations.IsNullOrEmpty()) return Translations;

			var contextReplacements = XmlTranslationSetConverter.Deserialize(translations);
			return contextReplacements.Union(Translations);
		}

		#endregion

		private const string CLASS_ID = "9c260659-9675-4f99-9f96-71d10a46c41d";
	}
}

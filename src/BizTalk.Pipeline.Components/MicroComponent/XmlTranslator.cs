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

using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// This component moves elements (and optionally attributes) from one namespace to another in the XML stream
	/// constituting the body of the message.
	/// </summary>
	public class XmlTranslator : IMicroPipelineComponent
	{
		public XmlTranslator()
		{
			Modes = XmlTranslationModes.Default;
			Encoding = new UTF8Encoding();
			Translations = XmlTranslationSet.Empty;
		}

		#region IMicroPipelineComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var translationSet = BuildXmlTranslationSet(message);
			if (translationSet.Items.Any())
			{
				message.BodyPart.WrapOriginalDataStream(
					originalStream => {
						var substitutionStream = new XmlTranslatorStream(
							new XmlTextReader(originalStream),
							Encoding,
							translationSet.Items,
							Modes);
						return substitutionStream;
					},
					pipelineContext.ResourceTracker);
			}
			return message;
		}

		#endregion

		/// <summary>
		/// Encoding to use for output and, if Unicode, whether to emit a byte order mark.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="UTF8Encoding"/> with a BOM preamble.
		/// </remarks>
		[XmlElement(typeof(EncodingXmlSerializer))]
		public Encoding Encoding { get; set; }

		// TODO ?? rename to XmlTranslationScopes ??
		public XmlTranslationModes Modes { get; set; }

		/// <summary>
		/// Set of XML translations to perform.
		/// </summary>
		public XmlTranslationSet Translations { get; set; }

		internal XmlTranslationSet BuildXmlTranslationSet(IBaseMessage message)
		{
			var translations = message.GetProperty(BizTalkFactoryProperties.XmlTranslations);
			if (translations.IsNullOrEmpty()) return Translations;

			var contextReplacements = XmlTranslationSetConverter.Deserialize(translations);
			return contextReplacements.Union(Translations);
		}
	}
}

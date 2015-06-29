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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Streaming
{
	public class XmlTranslationSetConverter : ExpandableObjectConverter
	{
		public static XmlTranslationSet Deserialize(string xml)
		{
			if (xml.IsNullOrEmpty()) return XmlTranslationSet.Empty;
			using (var reader = new StringReader(xml))
			{
				var xmlSerializer = new XmlSerializer(typeof(XmlTranslationSet));
				return (XmlTranslationSet) xmlSerializer.Deserialize(reader);
			}
		}

		public static string Serialize(XmlTranslationSet translations)
		{
			if (!translations.Items.Any()) return null;
			using (var stream = new MemoryStream())
			using (var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = new UTF8Encoding(false), Indent = false, OmitXmlDeclaration = true }))
			{
				var xmlSerializer = new XmlSerializer(typeof(XmlTranslationSet));
				xmlSerializer.Serialize(
					xmlWriter,
					translations,
					new XmlSerializerNamespaces(new[] { new XmlQualifiedName("xt", XmlTranslationSet.NAMESPACE) }));
				return Encoding.UTF8.GetString(stream.ToArray());
			}
		}

		#region Base Class Member Overrides

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var xml = value as string;
			return xml != null ? Deserialize(xml) : base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			var set = value as XmlTranslationSet;
			return set != null && destinationType == typeof(string) ? Serialize(set) : base.ConvertTo(context, culture, value, destinationType);
		}

		#endregion
	}
}

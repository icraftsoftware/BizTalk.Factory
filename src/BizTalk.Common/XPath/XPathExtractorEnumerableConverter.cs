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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.XPath
{
	/// <summary>
	/// Converts an <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s back and forth to a <see
	/// cref="string"/>.
	/// </summary>
	/// <remarks>
	/// Notice that <see cref="XPathExtractorEnumerableConverter"/> delegates the XML serialization and deseriliazation
	/// to <see cref="XPathExtractorEnumerableSerializer"/>.
	/// </remarks>
	/// <seealso cref="XPathExtractorEnumerableSerializer"/>
	public class XPathExtractorEnumerableConverter : ExpandableObjectConverter
	{
		/// <summary>
		/// Deserializes an <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s from its XML serialization <see
		/// cref="string"/>.
		/// </summary>
		/// <param name="xml">
		/// A <see cref="string"/> denoting the XML serialization of an <see cref="IEnumerable{T}"/> of <see
		/// cref="XPathExtractor"/>s.
		/// </param>
		/// <returns>
		/// The deserialized <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s.
		/// </returns>
		/// <seealso cref="Serialize"/>
		public static IEnumerable<XPathExtractor> Deserialize(string xml)
		{
			if (xml.IsNullOrEmpty()) return Enumerable.Empty<XPathExtractor>();

			using (var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true }))
			{
				var serializer = new XPathExtractorEnumerableSerializer();
				serializer.ReadXml(reader);
				return serializer.Extractors;
			}
		}

		/// <summary>
		/// Serializes an <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s to its XML <see cref="string"/>
		/// representation.
		/// </summary>
		/// <param name="extractors">
		/// The <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/> to serialize.
		/// </param>
		/// <returns>
		/// A <see cref="string"/> that represents the <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s.
		/// </returns>
		/// <seealso cref="Deserialize"/>
		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Any does not really enumerate.")]
		public static string Serialize(IEnumerable<XPathExtractor> extractors)
		{
			return !extractors.Any()
				? null
				: new XPathExtractorEnumerableSerializer(extractors).ToString();
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter, using
		/// the specified context.
		/// </summary>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		/// <param name="context">
		/// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.
		/// </param>
		/// <param name="sourceType">
		/// A <see cref="T:System.Type"/> that represents the type you want to convert from.
		/// </param>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		/// <param name="context">
		/// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.
		/// </param>
		/// <param name="destinationType">
		/// A <see cref="T:System.Type"/> that represents the type you want to convert to.
		/// </param>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Object"/> that represents the converted value.
		/// </returns>
		/// <param name="context">
		/// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.
		/// </param>
		/// <param name="culture">
		/// The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.
		/// </param>
		/// <param name="value">
		/// The <see cref="T:System.Object"/> to convert.
		/// </param>
		/// <exception cref="T:System.NotSupportedException">
		/// The conversion cannot be performed.
		/// </exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var xml = value as string;
			return value == null || xml != null ? Deserialize(xml) : base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Object"/> that represents the converted value.
		/// </returns>
		/// <param name="context">
		/// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.
		/// </param>
		/// <param name="culture">
		/// A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.
		/// </param>
		/// <param name="value">
		/// The <see cref="T:System.Object"/> to convert.
		/// </param>
		/// <param name="destinationType">
		/// The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// The <paramref name="destinationType"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// The conversion cannot be performed.
		/// </exception>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			var collection = value as IEnumerable<XPathExtractor>;
			return collection != null && destinationType == typeof(string) ? Serialize(collection) : base.ConvertTo(context, culture, value, destinationType);
		}

		#endregion
	}
}

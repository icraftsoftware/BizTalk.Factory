#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using System.Text;
using System.Text.RegularExpressions;
using Be.Stateless.Extensions;

namespace Be.Stateless.Text
{
	/// <summary>
	/// Converts an <see cref="Encoding"/> back-and-forth to a <see cref="string"/>.
	/// </summary>
	public class EncodingConverter : ExpandableObjectConverter
	{
		/// <summary>
		/// Converts the given <see cref="string"/> to a <see cref="Encoding"/> instance.
		/// </summary>
		/// <param name="encoding">
		/// The <see cref="string"/> to convert to a <see cref="Encoding"/> instance.
		/// </param>
		/// <returns>
		/// A <see cref="Encoding"/> instance that represents the converted <paramref name="encoding"/>.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// The conversion cannot be performed.
		/// </exception>
		/// <remarks>
		/// An <see cref="Encoding"/> can be converted back to a <see cref="string"/> via the <see cref="Serialize"/>
		/// methods.
		/// </remarks>
		/// <remarks>
		/// <para>
		/// The serialization format of an encoding is as follows <c>&lt;Name&gt;[ with signature]</c> where
		/// <c>&lt;Name&gt;</c> is the <see cref="Encoding.WebName"/> of the encoding and <c>with signature</c> is an
		/// optional modifier that is present only for a Unicode encoding if a byte order mark preamble has be emitted.
		/// </para>
		/// <para>
		/// The <see cref="Encoding"/> instance can be converted back to its <see cref="string"/> serialized
		/// representation via the <see cref="Serialize"/> methods.
		/// </para>
		/// </remarks>
		/// <seealso cref="Serialize"/>
		public static Encoding Deserialize(string encoding)
		{
			if (encoding.IsNullOrEmpty()) throw new NotSupportedException(string.Format("Cannot parse a null or empty string into a {0}.", typeof(Encoding).Name));

			var match = _regex.Match(encoding);
			if (!match.Success) throw new NotSupportedException(string.Format("'{0}' format is invalid and cannot be parsed into a {1}.", encoding, typeof(Encoding).Name));

			var name = match.Groups["Name"].Value;
			var emitPreamble = match.Groups["Preamble"].Success;
			return name.Equals(Encoding.UTF8.WebName, StringComparison.OrdinalIgnoreCase)
				? new UTF8Encoding(emitPreamble)
				: name.Equals(Encoding.Unicode.WebName, StringComparison.OrdinalIgnoreCase)
					? new UnicodeEncoding(!BitConverter.IsLittleEndian, emitPreamble)
					: name.Equals(Encoding.UTF32.WebName, StringComparison.OrdinalIgnoreCase)
						? new UTF32Encoding(!BitConverter.IsLittleEndian, emitPreamble)
						: Encoding.GetEncoding(name);
		}

		/// <summary>
		/// Converts a <see cref="Encoding"/> instance to its <see cref="string"/> representation.
		/// </summary>
		/// <param name="encoding">
		/// The <see cref="Encoding"/> instance to serialize.
		/// </param>
		/// <returns>
		/// A <see cref="string"/> that represents the <see cref="Encoding"/>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The serialization format of an encoding is as follows <c>&lt;Name&gt;[ with signature]</c> where
		/// <c>&lt;Name&gt;</c> is the <see cref="Encoding.WebName"/> of the encoding and <c>with signature</c> is an
		/// optional modifier that is present only for a Unicode encoding if a byte order mark preamble has be emitted.
		/// </para>
		/// <para>
		/// The <see cref="string"/> serialized representation of an <see cref="Encoding"/> can be converted back to an
		/// <see cref="Encoding"/> via the <see cref="Deserialize"/> methods.
		/// </para>
		/// </remarks>
		/// <seealso cref="Deserialize"/>
		public static string Serialize(Encoding encoding)
		{
			return encoding.GetPreamble().Length > 0 ? string.Format("{0} {1}", encoding.WebName, ENCODING_SIGNATURE_MODIFIER) : encoding.WebName;
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
			var displayName = value as string;
			return displayName != null ? Deserialize(displayName) : base.ConvertFrom(context, culture, value);
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
			var encoding = value as Encoding;
			return encoding != null && destinationType == typeof(string) ? Serialize(encoding) : base.ConvertTo(context, culture, value, destinationType);
		}

		#endregion

		internal const string ENCODING_SIGNATURE_MODIFIER = "with signature";

		private static readonly Regex _regex = new Regex(
			string.Format(@"^(?<Name>\w[\w\-_]+?)(?<Preamble>(?i:\s{0}))?$", ENCODING_SIGNATURE_MODIFIER),
			RegexOptions.Singleline | RegexOptions.Compiled);
	}
}

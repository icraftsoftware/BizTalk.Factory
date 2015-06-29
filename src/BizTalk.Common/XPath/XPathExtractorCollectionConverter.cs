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
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.XPath
{
	/// <summary>
	/// Converts either an <see cref="IEnumerable{T}"/> of <see cref="XElement"/>s to an <see
	/// cref="XPathExtractorCollection"/>, or a <see cref="string"/> back and forth to an <see
	/// cref="XPathExtractorCollection"/>.
	/// </summary>
	public class XPathExtractorCollectionConverter : ExpandableObjectConverter
	{
		/// <summary>
		/// Deserializes an <see cref="XPathExtractorCollection"/> from its XML serialization string.
		/// </summary>
		/// <param name="xml">
		/// An <see cref="XPathExtractorCollection"/> XML serialization string.
		/// </param>
		/// <returns>
		/// The deserialized <see cref="XPathExtractorCollection"/>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// Parses and deserializes an <see cref="XPathExtractorCollection"/> from an XML string structured as follows:
		/// <code><![CDATA[
		/// <san:Properties extractorPrecedence='schema'
		///                 xmlns:tp='urn:schemas.stateless.be:biztalk:properties:tracking:2012:04'
		///                 xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'>
		///   <tp:Value1 xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Id']" />
		///   <tp:Value2 xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Subject']" />
		///   <tp:Value3 promoted="true"
		///              xpath="/*[local-name()='Send']/*[local-name()='Message']*[local-name()='Priority']" />
		/// </san:Properties>
		/// ]]></code>
		/// </para>
		/// <para>
		/// The <see cref="XPathExtractorCollection"/> instance can be converted back to its <see cref="string"/>
		/// serialized representation via the <see cref="Serialize"/> methods.
		/// </para>
		/// </remarks>
		/// <seealso cref="Serialize"/>
		public static XPathExtractorCollection Deserialize(string xml)
		{
			if (xml.IsNullOrEmpty()) return new XPathExtractorCollection(Enumerable.Empty<XPathExtractor>());

			try
			{
				var propertiesElement = XDocument.Load(new StringReader(xml))
					.Elements()
					.SingleOrDefault(e => e.Name.LocalName == "Properties" && e.Name.NamespaceName == SchemaAnnotations.NAMESPACE);

				if (propertiesElement == null)
					throw new XmlException(
						string.Format(
							"Element \"Properties\" in namespace \"{0}\" was not found.",
							SchemaAnnotations.NAMESPACE));

				return ConvertFrom(propertiesElement);
			}
			catch (XmlException exception)
			{
				throw CreateConfigurationException(exception);
			}
		}

		/// <summary>
		/// Converts an <see cref="IEnumerable{T}"/> of <see cref="XElement"/>s to an <see
		/// cref="XPathExtractorCollection"/>.
		/// </summary>
		/// <param name="propertiesElement">
		/// The <see cref="IEnumerable{T}"/> of <see cref="XElement"/>s to convert to <see
		/// cref="XPathExtractorCollection"/>.
		/// </param>
		/// <returns>
		/// The converted <see cref="XPathExtractorCollection"/> equivalent.
		/// </returns>
		/// <remarks>
		/// The <see cref="IEnumerable{T}"/> of <see cref="XElement"/>s to convert from must adhere to the following XML
		/// pseudo-schema, specifically, its structure and annotations namespace
		/// (<c>urn:schemas.stateless.be:biztalk:annotations:2013:01</c>):<code><![CDATA[
		/// <san:Properties extractorPrecedence='schema'
		///                 xmlns:tp='urn:schemas.stateless.be:biztalk:properties:tracking:2012:04'
		///                 xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'>
		///   <tp:Value1 xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Id']" />
		///   <tp:Value2 xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Subject']" />
		///   <tp:Value3 promoted="true"
		///              xpath="/*[local-name()='Send']/*[local-name()='Message']*[local-name()='Priority']" />
		/// </san:Properties>
		/// ]]></code>
		/// </remarks>
		public static XPathExtractorCollection ConvertFrom(XElement propertiesElement)
		{
			try
			{
				// ReSharper disable PossibleMultipleEnumeration
				CheckPropertiesValidity(propertiesElement);

				// TODO deserialize all values of ExtractorPrecedence (@extractorPrecedence)
				// TODO deserialize all values of ExtractionMode

				var extractors = propertiesElement.Elements().Select(
					pe => new XPathExtractor(
						new XmlQualifiedName(pe.Name.LocalName, pe.Name.NamespaceName),
						// ReSharper disable PossibleNullReferenceException
						pe.Attribute("xpath").Value,
						// ReSharper restore PossibleNullReferenceException
						pe.Attribute("promoted").IfNotNull(p => bool.Parse(p.Value)) ? ExtractionMode.Promote : ExtractionMode.Write));

				CheckExtractorsUnicity(extractors);

				return new XPathExtractorCollection(extractors);
				// ReSharper restore PossibleMultipleEnumeration
			}
			catch (XmlException exception)
			{
				throw CreateConfigurationException(exception);
			}
		}

		/// <summary>
		/// Converts an <see cref="XPathExtractorCollection"/> instance to its <see cref="string"/> representation.
		/// </summary>
		/// <param name="collection">
		/// The <see cref="XPathExtractorCollection"/> instance to serialize.
		/// </param>
		/// <returns>
		/// A <see cref="string"/> that represents the <see cref="XPathExtractorCollection"/>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// Parses and deserializes an <see cref="XPathExtractorCollection"/> from an XML string structured as follows:
		/// <code><![CDATA[
		/// <san:Properties extractorPrecedence='schema'
		///                 xmlns:tp='urn:schemas.stateless.be:biztalk:properties:tracking:2012:04'
		///                 xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'>
		///   <tp:Value1 xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Id']" />
		///   <tp:Value2 xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Subject']" />
		///   <tp:Value3 promoted="true"
		///              xpath="/*[local-name()='Send']/*[local-name()='Message']*[local-name()='Priority']" />
		/// </san:Properties>
		/// ]]></code>
		/// </para>
		/// <para>
		/// The <see cref="string"/> serialized representation of an <see cref="XPathExtractorCollection"/> can be
		/// converted back to an <see cref="XPathExtractorCollection"/> via the <see cref="Deserialize"/> methods.
		/// </para>
		/// </remarks>
		/// <seealso cref="Deserialize"/>
		public static string Serialize(IEnumerable<XPathExtractor> collection)
		{
			// ReSharper disable PossibleMultipleEnumeration
			if (!collection.Any()) return null;

			// cache xmlns while constructing xml infoset...
			var nsCache = new XmlDictionary();
			var xdoc = new XElement(
				XName.Get("Properties", nsCache.Add(SchemaAnnotations.NAMESPACE).Value),
				collection.Select(
					p => new XElement(
						XName.Get(p.PropertyName.Name, nsCache.Add(p.PropertyName.Namespace).Value),
						// TODO serialize all values of ExtractorPrecedence (@extractorPrecedence)
						// TODO serialize all values of ExtractionMode
						p.ExtractionMode == ExtractionMode.Promote ? new XAttribute("promoted", true) : null,
						new XAttribute("xpath", p.XPathExpression.XPath))
					)
				);
			// ReSharper restore PossibleMultipleEnumeration

			// ... and declare/alias all of them at the root element level to minimize xml string size
			XmlDictionaryString xds;
			for (var i = 0; nsCache.TryLookup(i, out xds); i++)
			{
				xdoc.Add(new XAttribute(XNamespace.Xmlns + "s" + xds.Key.ToString(CultureInfo.InvariantCulture), xds.Value));
			}
			return xdoc.ToString(SaveOptions.DisableFormatting);
		}

		#region Helpers

		private static ConfigurationErrorsException CreateConfigurationException(XmlException exception)
		{
			const string message = @"Invalid schema annotations or pipeline configuration, it must be an XML string structured as follows:
<san:Properties xmlns:s0='urn0' xmlns:s1='urn1' xmlns:san='" + SchemaAnnotations.NAMESPACE + @"'>
  <s0:PropertyName1 xpath='*'/>
</san:Properties>";
			var configurationException = new ConfigurationErrorsException(message, exception);
			return configurationException;
		}

		[Conditional("DEBUG")]
		private static void CheckPropertiesValidity(XElement propertiesElement)
		{
			// ensure each property to extract is associated to a property schema's namespace and has a non empty XPath expression
			var offendingProperties = propertiesElement
				.Elements()
				.Where(e => e.Name.NamespaceName.IsNullOrEmpty() || e.Attribute("xpath").IfNotNull(a => a.Value).IsNullOrEmpty());

			// ReSharper disable PossibleMultipleEnumeration
			if (offendingProperties.Any())
				throw new XmlException(
					string.Format(
						"The following properties are either not associated with the target namespace URI of some property schema or have no XPath expression configured: [{0}].",
						string.Join("], [", offendingProperties.Select(op => op.Name.ToString()).ToArray())));
			// ReSharper restore PossibleMultipleEnumeration
		}

		[Conditional("DEBUG")]
		private static void CheckExtractorsUnicity(IEnumerable<XPathExtractor> extractors)
		{
			// ensure no property is extracted multiple times (irrespectively of XPaths)
			var duplicates = extractors
				.GroupBy(ex => ex.PropertyName)
				.Where(g => g.Count() > 1)
				.Select(g => g.Key);

			// ReSharper disable PossibleMultipleEnumeration
			if (duplicates.Any())
				throw new XmlException(
					string.Format(
						"The following properties are declared multiple times: [{0}].",
						string.Join("], [", duplicates.Select(p => p.ToString()).ToArray())));
			// ReSharper restore PossibleMultipleEnumeration
		}

		#endregion

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
			return sourceType == typeof(string) || sourceType == typeof(IEnumerable<XElement>) || base.CanConvertFrom(context, sourceType);
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
			if (xml != null) return Deserialize(xml);

			var enumerable = value as IEnumerable<XElement>;
			if (enumerable != null) return ConvertFrom(enumerable);

			return base.ConvertFrom(context, culture, value);
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
			var collection = value as XPathExtractorCollection;
			return collection != null && destinationType == typeof(string) ? Serialize(collection) : base.ConvertTo(context, culture, value, destinationType);
		}

		#endregion
	}
}

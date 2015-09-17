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
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Extensions;

namespace Be.Stateless.BizTalk.XPath
{
	/// <summary>
	/// Serializes an <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s back and forth to XML.
	/// </summary>
	/// <remarks>
	/// The XML serialization format complies to the following XML pseudo-schema, specifically, its structure and
	/// annotation namespace (<c>urn:schemas.stateless.be:biztalk:annotations:2013:01</c>):<code><![CDATA[
	/// <san:Properties extractorPrecedence='pipeline | pipelineOnly | schema | schemaOnly'
	///                 xmlns:tp='urn:schemas.stateless.be:biztalk:properties:tracking:2012:04'
	///                 xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'>
	///   <tp:Value1 xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Id']" />
	///   <tp:Value2 xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Subject']" />
	///   <tp:Value3 promoted="true"
	///              xpath="/*[local-name()='Send']/*[local-name()='Message']*[local-name()='Priority']" />
	/// </san:Properties>
	/// ]]></code>
	/// </remarks>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Required by XML serialization")]
	public class XPathExtractorEnumerableSerializer : IXmlSerializable
	{
		/// <summary>
		/// Deserializes an <see cref="XElement"/> into an <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s.
		/// </summary>
		/// <param name="propertiesElement">
		/// The <see cref="XElement"/> to deserialize into an <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s.
		/// </param>
		/// <returns>
		/// The deserialized <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s.
		/// </returns>
		/// <seealso cref="Serialize"/>
		internal static IEnumerable<XPathExtractor> Deserialize(XElement propertiesElement)
		{
			// TODO use XmlReader API, see ReadXmlProperties

			try
			{
				// TODO deserialize all values of ExtractorPrecedence (@extractorPrecedence)
				// TODO deserialize all values of ExtractionMode

				var extractors = propertiesElement.Elements().Select(
					pe => new XPathExtractor(
						new XmlQualifiedName(pe.Name.LocalName, pe.Name.NamespaceName),
						pe.Attribute("xpath").Value,
						pe.Attribute("promoted").IfNotNull(p => bool.Parse(p.Value)) ? ExtractionMode.Promote : ExtractionMode.Write))
					.ToArray();

				return extractors;
			}
			catch (XmlException exception)
			{
				throw new ConfigurationErrorsException(MESSAGE, exception);
			}
		}

		public XPathExtractorEnumerableSerializer() { }

		public XPathExtractorEnumerableSerializer(IEnumerable<XPathExtractor> extractors)
		{
			if (extractors == null) throw new ArgumentNullException("extractors");
			Extractors = extractors;
		}

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Deserializes the <see cref="Extractors"/> state property from its XML serialization format.
		/// </summary>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> to deserialize the <see cref="Extractors"/> state property from.
		/// </param>
		public void ReadXml(XmlReader reader)
		{
			try
			{
				if (reader.IsStartElement("Extractors"))
				{
					reader.ReadStartElement();
					ReadXmlProperties(reader);
					reader.ReadEndElement("Extractors");
				}
				else
				{
					ReadXmlProperties(reader);
				}
				CheckExtractorsValidity(Extractors);
				CheckExtractorsUnicity(Extractors);
			}
			catch (Microsoft.BizTalk.XPath.XPathException exception)
			{
				throw new ConfigurationErrorsException(MESSAGE, exception);
			}
			catch (XmlException exception)
			{
				throw new ConfigurationErrorsException(MESSAGE, exception);
			}
		}

		/// <summary>
		/// Serializes the <see cref="Extractors"/> state property to its XML serialization format.
		/// </summary>
		/// <param name="writer">
		/// The <see cref="XmlWriter"/> to serialize the <see cref="Extractors"/> state property to.
		/// </param>
		public void WriteXml(XmlWriter writer)
		{
			if (Extractors.Any()) Serialize(Extractors).WriteTo(writer);
		}

		#endregion

		#region Base Class Member Overrides

		/// <summary>
		/// Serializes the <see cref="Extractors"/> state property to an XML <see cref="string"/>.
		/// </summary>
		/// <returns>
		/// The <see cref="string"/> corresponding to the XML serialization format of the <see cref="Extractors"/> state
		/// property.
		/// </returns>
		public override string ToString()
		{
			return Serialize(Extractors).ToString(SaveOptions.DisableFormatting);
		}

		#endregion

		/// <summary>
		/// The <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s to either serialize or deserialize.
		/// </summary>
		public IEnumerable<XPathExtractor> Extractors { get; set; }

		private void ReadXmlProperties(XmlReader reader)
		{
			reader.ReadStartElement("Properties", SchemaAnnotations.NAMESPACE);
			var list = new List<XPathExtractor>();
			while (!reader.IsEndElement("Properties", SchemaAnnotations.NAMESPACE))
			{
				var extractor = new XPathExtractor(
					new XmlQualifiedName(reader.LocalName, reader.NamespaceURI),
					reader.GetMandatoryAttribute("xpath"),
					// TODO deserialize all values of ExtractorPrecedence (@extractorPrecedence)
					// TODO deserialize all values of ExtractionMode
					Convert.ToBoolean(reader.GetAttribute("promoted")) ? ExtractionMode.Promote : ExtractionMode.Write);
				list.Add(extractor);
				reader.Read();
			}
			reader.ReadEndElement("Properties", SchemaAnnotations.NAMESPACE);
			Extractors = list.ToArray();
		}

		/// <summary>
		/// Serializes an <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s to an <see cref="XElement"/>.
		/// </summary>
		/// <param name="extractors">
		/// The <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s to serialize to an <see cref="XElement"/>.
		/// </param>
		/// <returns>
		/// The <see cref="XElement"/> corresponding to the serialized <see cref="IEnumerable{T}"/> of <see
		/// cref="XPathExtractor"/>s.
		/// </returns>
		/// <seealso cref="Deserialize"/>
		private XElement Serialize(IEnumerable<XPathExtractor> extractors)
		{
			// TODO use XmlWriter API

			// cache xmlns while constructing xml infoset...
			var nsCache = new XmlDictionary();
			var xElement = new XElement(
				XName.Get("Properties", nsCache.Add(SchemaAnnotations.NAMESPACE).Value),
				extractors.Select(
					p => new XElement(
						XName.Get(p.PropertyName.Name, nsCache.Add(p.PropertyName.Namespace).Value),
						// TODO serialize all values of ExtractorPrecedence (@extractorPrecedence)
						// TODO serialize all values of ExtractionMode
						p.ExtractionMode == ExtractionMode.Promote ? new XAttribute("promoted", true) : null,
						new XAttribute("xpath", p.XPathExpression.XPath))
					)
				);

			// ... and declare/alias all of them at the root element level to minimize xml string size
			XmlDictionaryString xds;
			for (var i = 0; nsCache.TryLookup(i, out xds); i++)
			{
				xElement.Add(new XAttribute(XNamespace.Xmlns + "s" + xds.Key.ToString(CultureInfo.InvariantCulture), xds.Value));
			}
			return xElement;
		}

		#region Validators

		[Conditional("DEBUG")]
		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Any does not really enumerate.")]
		private static void CheckExtractorsValidity(IEnumerable<XPathExtractor> extractors)
		{
			// ensure each property to extract is associated to a property schema's namespace and has a non empty XPath expression
			var invalidExtractors = extractors
				.Where(e => e.PropertyName.Namespace.IsNullOrEmpty());

			if (invalidExtractors.Any())
				throw new XmlException(
					string.Format(
						"The following properties are not associated with the target namespace URI of some property schema: [{0}].",
						string.Join("], [", invalidExtractors.Select(ie => ie.PropertyName.ToString()).ToArray())));
		}

		[Conditional("DEBUG")]
		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Any does not really enumerate.")]
		private static void CheckExtractorsUnicity(IEnumerable<XPathExtractor> extractors)
		{
			// ensure no property is extracted multiple times (irrespectively of XPaths)
			var duplicates = extractors
				.GroupBy(ex => ex.PropertyName)
				.Where(g => g.Count() > 1)
				.Select(g => g.Key);

			if (duplicates.Any())
				throw new XmlException(
					string.Format(
						"The following properties are declared multiple times: [{0}].",
						string.Join("], [", duplicates.Select(p => p.ToString()).ToArray())));
		}

		#endregion

		private const string MESSAGE = @"Invalid schema annotations or pipeline configuration, it must be an XML string structured as follows:
<san:Properties xmlns:s0='urn0' xmlns:s1='urn1' xmlns:san='" + SchemaAnnotations.NAMESPACE + @"'>
  <s0:PropertyName1 xpath='*'/>
</san:Properties>";
	}
}

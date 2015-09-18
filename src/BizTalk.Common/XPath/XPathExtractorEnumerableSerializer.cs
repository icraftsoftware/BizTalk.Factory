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
using Be.Stateless.Linq.Extensions;
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
		internal static IEnumerable<XPathExtractor> Deserialize(XElement propertiesElement)
		{
			// TODO use XmlReader API, see ReadXmlProperties

			try
			{
				// TODO deserialize all values of ExtractorPrecedence (@precedence)
				var extractors = propertiesElement.Elements().Select(
					pe => new XPathExtractor(
						new XmlQualifiedName(pe.Name.LocalName, pe.Name.NamespaceName),
						pe.Attribute("xpath").Value,
						// TODO deserialize all values of ExtractionMode (@mode)
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
			if (Extractors.Any()) WriteXmlProperties(writer);
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
				// TODO deserialize all values of ExtractorPrecedence (@precedence)
				var extractor = new XPathExtractor(
					new XmlQualifiedName(reader.LocalName, reader.NamespaceURI),
					reader.GetMandatoryAttribute("xpath"),
					// TODO deserialize all values of ExtractionMode (@mode)
					Convert.ToBoolean(reader.GetAttribute("promoted")) ? ExtractionMode.Promote : ExtractionMode.Write);
				list.Add(extractor);
				reader.Read();
			}
			reader.ReadEndElement("Properties", SchemaAnnotations.NAMESPACE);
			Extractors = list.ToArray();
		}

		private void WriteXmlProperties(XmlWriter writer)
		{
			// so as to declare ns prefixes at the root element level to minimize xml string size
			var nsCache = new XmlDictionary();
			nsCache.Add(SchemaAnnotations.NAMESPACE);
			Extractors.Each(e => nsCache.Add(e.PropertyName.Namespace));

			XmlDictionaryString xds;
			writer.WriteStartElement("s0", "Properties", SchemaAnnotations.NAMESPACE);
			// TODO serialize all values of ExtractorPrecedence (@precedence)
			for (var i = 0; nsCache.TryLookup(i, out xds); i++)
			{
				// see https://msdn.microsoft.com/en-us/library/system.xml.xmltextwriter(v=vs.110).aspx
				// - XmlTextWriter maintains a namespace stack corresponding to all the namespaces defined in the current element stack
				// - XmlTextWriter also allows you to override the current namespace declaration
				// ReSharper disable once AssignNullToNotNullAttribute
				writer.WriteAttributeString("xmlns", "s" + (xds.Key).ToString(CultureInfo.InvariantCulture), null, xds.Value);
			}
			Extractors.Each(
				e => {
					writer.WriteStartElement(e.PropertyName.Name, e.PropertyName.Namespace);
					// TODO serialize all values of ExtractionMode (@mode)
					if (e.ExtractionMode == ExtractionMode.Promote) writer.WriteAttributeString("promoted", "true");
					writer.WriteAttributeString("xpath", e.XPathExpression.XPath);
					writer.WriteEndElement();
				});
			writer.WriteEndElement();
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

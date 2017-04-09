#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.XPath;
using Be.Stateless.Extensions;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Xml.Extensions;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// Collection of <see cref="PropertyExtractor"/>-derived extractors that supports back and forth serialization to
	/// XML.
	/// </summary>
	/// <remarks>
	/// The XML serialization format complies to the following XML pseudo-schema, specifically, its structure and
	/// annotation namespace (<c>urn:schemas.stateless.be:biztalk:annotations:2013:01</c>):<code><![CDATA[
	/// <san:Properties [extractorPrecedence='pipeline | pipelineOnly | schema | schemaOnly']
	///                 xmlns:tp='urn:schemas.stateless.be:biztalk:properties:tracking:2012:04'
	///                 xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'>
	///   <tp:Value1 [mode="clear | ignore | promote | write"]
	///              [promoted="true"] value="constant-string-literal" />
	///   <tp:Value2 [mode="clear | demote | ignore | promote | write"]
	///              [promoted="true"]
	///              xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Subject']" />
	///   <tp:Value3 mode="clear | ignore"] />
	/// </san:Properties>
	/// ]]></code>
	/// </remarks>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Required by XML serialization")]
	public class PropertyExtractorCollection : IEnumerable<PropertyExtractor>, IXmlSerializable
	{
		#region Operators

		public static implicit operator PropertyExtractorCollection(PropertyExtractor[] extractors)
		{
			return new PropertyExtractorCollection(extractors);
		}

		#endregion

		public PropertyExtractorCollection()
		{
			Extractors = Enumerable.Empty<PropertyExtractor>();
		}

		public PropertyExtractorCollection(IEnumerable<PropertyExtractor> extractors)
		{
			if (extractors == null) throw new ArgumentNullException("extractors");
			Extractors = extractors;
		}

		#region IEnumerable<PropertyExtractor> Members

		public IEnumerator<PropertyExtractor> GetEnumerator()
		{
			return Extractors.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

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
		public virtual void ReadXml(XmlReader reader)
		{
			try
			{
				ReadXmlProperties(reader);
				ValidateExtractors();
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
		/// Precedence of the <see cref="PropertyExtractorCollection"/> to be used should extractors declared by XML
		/// schema annotations be merged to extractors configured by the pipeline.
		/// </summary>
		public ExtractorPrecedence Precedence { get; set; }

		/// <summary>
		/// The <see cref="IEnumerable{T}"/> of <see cref="XPathExtractor"/>s to either serialize or deserialize.
		/// </summary>
		private IEnumerable<PropertyExtractor> Extractors { get; set; }

		private void ReadXmlProperties(XmlReader reader)
		{
			var list = new List<PropertyExtractor>();
			reader.MoveToContent();
			reader.AssertStartElement("Properties", SchemaAnnotations.NAMESPACE);
			Precedence = reader.HasAttribute("precedence")
				? reader.GetAttribute("precedence").IfNotNull(v => v.Parse<ExtractorPrecedence>())
				: default(ExtractorPrecedence);
			reader.ReadStartElement("Properties", SchemaAnnotations.NAMESPACE);
			while (reader.NodeType == XmlNodeType.Element)
			{
				var name = new XmlQualifiedName(reader.LocalName, reader.NamespaceURI);
				var mode = reader.HasAttribute("mode")
					? reader.GetAttribute("mode").IfNotNull(v => v.Parse<ExtractionMode>())
					: reader.HasAttribute("promoted")
						? Convert.ToBoolean(reader.GetAttribute("promoted"))
							? ExtractionMode.Promote
							: default(ExtractionMode)
						: default(ExtractionMode);
				if (reader.HasAttribute("xpath"))
				{
					var extractor = new XPathExtractor(name, reader.GetAttribute("xpath"), mode);
					list.Add(extractor);
				}
				else if (reader.HasAttribute("value"))
				{
					var extractor = new ConstantExtractor(name, reader.GetAttribute("value"), mode);
					list.Add(extractor);
				}
				else if (!reader.HasAttribute("mode"))
				{
					throw new ConfigurationErrorsException("ExtractionMode is missing for PropertyExtractor without a Value or an XPath.");
				}
				else
				{
					var extractor = new PropertyExtractor(name, mode);
					list.Add(extractor);
				}
				reader.Read();
			}
			if (!reader.EOF)
			{
				reader.AssertEndElement("Properties", SchemaAnnotations.NAMESPACE);
				reader.ReadEndElement();
			}
			Extractors = list.ToArray();
		}

		private void WriteXmlProperties(XmlWriter writer)
		{
			writer.WriteStartElement("s0", "Properties", SchemaAnnotations.NAMESPACE);
			{
				if (Precedence != default(ExtractorPrecedence)) writer.WriteAttributeString("precedence", Precedence.ToString().ToCamelCase());

				// declare all the namespaces and their prefixes at the parent element
				var nsCache = new XmlDictionary();
				nsCache.Add(SchemaAnnotations.NAMESPACE);
				Extractors.Each(e => nsCache.Add(e.PropertyName.Namespace));
				XmlDictionaryString xds;
				for (var i = 0; nsCache.TryLookup(i, out xds); i++)
				{
					// see https://msdn.microsoft.com/en-us/library/system.xml.xmltextwriter(v=vs.110).aspx
					// - XmlTextWriter also allows you to override the current namespace declaration
					// ReSharper disable once AssignNullToNotNullAttribute
					writer.WriteAttributeString("xmlns", "s" + xds.Key.ToString(CultureInfo.InvariantCulture), null, xds.Value);
				}

				Extractors.Each(
					e => {
						// see https://msdn.microsoft.com/en-us/library/system.xml.xmltextwriter(v=vs.110).aspx
						// - XmlTextWriter maintains a namespace stack corresponding to all the namespaces defined in the current element stack
						writer.WriteStartElement(e.PropertyName.Name, e.PropertyName.Namespace);
						e.WriteXmlCore(writer);
						writer.WriteEndElement();
					});
			}
			writer.WriteEndElement();
		}

		[Conditional("DEBUG")]
		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Any does not really enumerate.")]
		private void ValidateExtractors()
		{
			// ensure each property to extract is associated to a property schema namespace
			var invalidExtractors = Extractors
				.Where(e => e.PropertyName.Namespace.IsNullOrEmpty());

			if (invalidExtractors.Any())
				throw new XmlException(
					string.Format(
						"The following properties are not associated with the target namespace URI of some property schema: [{0}].",
						string.Join("], [", invalidExtractors.Select(ie => ie.PropertyName.ToString()).ToArray())));

			// ensure no property is extracted multiple times (irrespectively of XPaths)
			var duplicateExtractors = Extractors
				.GroupBy(ex => ex.PropertyName)
				.Where(g => g.Count() > 1)
				.Select(g => g.Key);

			if (duplicateExtractors.Any())
				throw new XmlException(
					string.Format(
						"The following properties are declared multiple times: [{0}].",
						string.Join("], [", duplicateExtractors.Select(p => p.ToString()).ToArray())));
		}

		private const string MESSAGE = @"Invalid schema annotations or pipeline configuration, it must be an XML string satisfying the following grammar:
<san:Properties [extractorPrecedence='schema|schemaOnly|pipeline|pipelineOnly'] xmlns:s0='urn0' xmlns:s1='urn1' xmlns:san='" + SchemaAnnotations.NAMESPACE + @"'>
  <s0:Property1 [mode='clear|ignore|promote|write'] value='constant' />
  <s0:Property2 [promoted='true'] value='constant' />
  <s1:Property3 [mode='clear|demote|ignore|promote|write'] xpath='*' />
  <s1:Property4 [promoted='true'] xpath='*' />
  <s1:Property5 mode='clear|ignore' />
</san:Properties>";
	}
}

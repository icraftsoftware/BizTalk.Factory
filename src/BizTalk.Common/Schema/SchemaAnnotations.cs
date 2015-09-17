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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using Be.Stateless.BizTalk.XPath;
using Be.Stateless.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Schema
{
	/// <summary>
	/// Provides access to annotations embedded in <see cref="SchemaBase"/>-derived schema definitions.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Only annotations declared in the schema annotation namespace defined by BizTalk Factory, i.e.
	/// <c>urn:schemas.stateless.be:biztalk:annotations:2013:01</c> are considered; all other annotations are discarded.
	/// </para>
	/// <para>
	/// Notice that there is no transitive discovery of the annotations across the XSD type definitions and that only
	/// annotations embedded directly underneath the root node of the relevant <see cref="SchemaBase"/>-derived schema
	/// are loaded.
	/// </para>
	/// </remarks>
	/// <example>
	/// The following example illustrates how to embed BizTalk Factory annotations. Note that there other annotations
	/// specific to Microsoft BizTalk Server might coexist but are not illustrated.
	/// <code>
	/// <![CDATA[
	/// <xs:schema targetNamespace='urn:schemas.stateless.be:biztalk:tests:annotated:2013:01'
	///            xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'
	///            xmlns:xs='http://www.w3.org/2001/XMLSchema'>
	///   <xs:element name='Root'>
	///     <xs:annotation>
	///       <xs:appinfo>
	///         ...
	///         <san:EnvelopeMapSpecName>
	///           Be.Stateless.BizTalk.Unit.Transform.IdentityTransform, Be.Stateless.BizTalk.Unit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3707daa0b119fc14
	///         </san:EnvelopeMapSpecName>
	///         <san:Properties xmlns:tp='urn:schemas.stateless.be:biztalk:properties:tracking:2012:04'>
	///           <tp:Value1 xpath=""/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Id']"" />
	///         </san:Properties>
	///         ...
	///       </xs:appinfo>
	///     </xs:annotation>
	///     <xs:complexType />
	///   </xs:element>
	/// </xs:schema>
	/// ]]>
	/// </code>
	/// </example>
	public class SchemaAnnotations : ISchemaAnnotations
	{
		#region Nested Type: EmptySchemaAnnotations

		internal class EmptySchemaAnnotations : ISchemaAnnotations
		{
			#region ISchemaAnnotations Members

			public Type EnvelopingMap
			{
				get { return null; }
			}

			public IEnumerable<XPathExtractor> Extractors
			{
				get { return Enumerable.Empty<XPathExtractor>(); }
			}

			#endregion
		}

		#endregion

		public static ISchemaAnnotations Create(ISchemaMetadata metadata)
		{
			if (metadata == null) throw new ArgumentNullException("metadata");

			if (metadata.Type.Assembly.FullName.StartsWith("Microsoft.", StringComparison.Ordinal)) return Empty;

			// ReSharper disable PossibleMultipleEnumeration
			var annotations = SelectAnnotations(metadata);
			if (!annotations.Any()) return Empty;

			var envelopeMap = metadata.IsEnvelopeSchema
				? annotations
					.SingleOrDefault(e => e.Name.LocalName == "EnvelopeMapSpecName")
					.IfNotNull(e => e.Value)
					.IfNotNull(n => Type.GetType(n, true))
				: null;

			var extractors = annotations
				.SingleOrDefault(e => e.Name.LocalName == "Properties")
				.IfNotNull(p => XPathExtractorEnumerableSerializer.Deserialize(p))
				?? Enumerable.Empty<XPathExtractor>();
			// ReSharper restore PossibleMultipleEnumeration

			return new SchemaAnnotations {
				EnvelopingMap = envelopeMap,
				Extractors = extractors
			};
		}

		private static IEnumerable<XElement> SelectAnnotations(ISchemaMetadata metadata)
		{
			var schema = (SchemaBase) Activator.CreateInstance(metadata.Type);

			var xdoc = XDocument.Load(new StringReader(schema.XmlContent));
			var namespaceManager = new XmlNamespaceManager(new NameTable());
			namespaceManager.AddNamespace("xs", XmlSchema.Namespace);
			namespaceManager.AddNamespace("san", NAMESPACE);
			var annotations = xdoc.XPathSelectElements(
				string.Format("/*/xs:element[@name='{0}']/xs:annotation//san:*", metadata.RootElementName),
				namespaceManager);
			return annotations;
		}

		private SchemaAnnotations() { }

		#region ISchemaAnnotations Members

		public Type EnvelopingMap { get; private set; }

		public IEnumerable<XPathExtractor> Extractors { get; private set; }

		#endregion

		public const string NAMESPACE = "urn:schemas.stateless.be:biztalk:annotations:2013:01";
		public static readonly ISchemaAnnotations Empty = new EmptySchemaAnnotations();
	}
}

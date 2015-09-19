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

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.XPath;
using Be.Stateless.Extensions;
using Be.Stateless.Linq;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.BizTalk.XPath;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// This component allows to promote or write properties in the message context whose values are extracted out of an
	/// XML message by defining less restrictive XPath expressions than the traditional canonical XPath expressions
	/// supported by BizTalk Server; limitations are however still present and relatively strong.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Most properties to promote or write should be configured directly in the message schemas, and not at the pipeline
	/// level using this component's configuration. The same XML fragment used to configure these properties can be both
	/// embedded directly as annotations in a schema or configured at the pipeline level.
	/// </para>
	/// <para>
	/// The known limitations on the supported XPath expressions are:
	/// <list type="bullet">
	/// <item>
	/// No namespace prefixes may be used in the XPath queries. If you need to match a specific namespace, a predicate in
	/// the form of <c>/*[local-name()='element' and namespace-uri()='urn']</c> must be used.
	/// </item>
	/// <item>
	/// Position predicates only work when expressed directly on an element name. Thus <c>/element[1]</c> works, but
	/// <c>/*[local-name()='element' and position()=1]</c> doesn't.
	/// </item>
	/// <item>
	/// Most XPath functions may not be used.
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// Internally, the component uses the <see cref="XPathMutatorStream"/> from BizTalk, which itself uses the <see
	/// cref="XPathReader"/>. The limitations thus come from these classes. For useful background information about the
	/// implementation, the following resources may be consulted:
	/// <list type="bullet">
	/// <item>
	/// <see href="http://www.codit.eu/Blog/post/2010/01/22/The-BizTalk-XPathMutatorStream-pros-and-cons.aspx">The
	/// BizTalk XPathMutatorStream: pros and cons</see>
	/// </item>
	/// <item>
	/// <see href="http://bloggingabout.net/blogs/wellink/archive/2006/03/03/11207.aspx">XpathMutatorStream:  Great
	/// functionality for every BizTalk Developer.</see>
	/// </item>
	/// <item>
	/// <see href="http://martijnh.blogspot.com/2006/03/xpathmutatorstream.html">XPathMutatorStream</see>
	/// </item>
	/// <item>
	/// <see href="http://connectedthoughts.wordpress.com/2008/05/31/updating-repeating-nodes-in-an-xml-document-with-the-same-value-using-xpathmutatorstream/">
	/// Updating repeating nodes in an XML Document with the same value using XPathMutatorStream</see>
	/// </item>
	/// <item>
	/// <see href="http://msdn.microsoft.com/en-us/library/ms950778.aspx">The Best of Both Worlds: Combining XPath with
	/// the XmlReader</see>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// If the configured XPath expressions lead to multiple matches in the message, only the first match value is taken
	/// into account for extraction (i.e. write or promote) and other matches are discarded. The component can possibly
	/// be slightly enhanced to take a specific match index into account, by modifying the <see cref="OnMatch"/> method.
	/// Given the limitations of the component, it could be necessary to change its implementation in the future if it
	/// cannot answer to new requirements that may arise. The current implementation uses a full streaming approach, and
	/// the alternate way to do it is using an <see cref="System.Xml.XPath.XPathNavigator"/> in combination with a <see
	/// cref="VirtualStream"/>. That would allow almost all XPath queries of arbitrary complexity, at the cost of some
	/// performance loss.
	/// </para>
	/// </remarks>
	/// <example>
	/// Example of an XML fragment configuration that can either be declared at the pipeline-level configuration or
	/// directly embedded in a schema: <code><![CDATA[
	/// <san:Properties extractorPrecedence='pipeline | pipelineOnly | schema | schemaOnly'
	///                 xmlns:tp='urn:schemas.stateless.be:biztalk:properties:tracking:2012:04'
	///                 xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'>
	///   <tp:Value1 xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Id']" />
	///   <tp:Value2 xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Subject']" />
	///   <tp:Value3 promoted="true"
	///              xpath="/*[local-name()='Send']/*[local-name()='Message']*[local-name()='Priority']" />
	/// </san:Properties>
	/// ]]></code> Notice that the <c>san:extractorPrecedence</c> attribute is relevant only when configured at the
	/// pipeline level and can only have one of the following values: <c>pipeline</c>, <c>pipelineOnly</c>,
	/// <c>schema</c>, or <c>schemaOnly</c>.
	/// </example>
	public class ContextPropertyExtractor : IMicroPipelineComponent
	{
		public ContextPropertyExtractor()
		{
			Extractors = Enumerable.Empty<XPathExtractor>();
		}

		#region IMicroPipelineComponent Members

		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Any does not really enumerate.")]
		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var extractors = BuildExtractorCollection(pipelineContext, message);
			if (extractors.Any())
			{
				// setup a stream that will invoke our callback whenever an XPathExtractor's XPath expression is matched
				message.BodyPart.WrapOriginalDataStream(
					originalStream => XPathMutatorStreamFactory.Create(
						originalStream,
						extractors,
						(propertyName, value, extractionMode) => OnMatch(message.Context, propertyName, value, extractionMode)),
					pipelineContext.ResourceTracker);
			}
			return message;
		}

		#endregion

		[XmlIgnore]
		public IEnumerable<XPathExtractor> Extractors { get; set; }

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("Extractors")]
		public XPathExtractorEnumerableSerializerSurrogate ExtractorSerializerSurrogate
		{
			get { return new XPathExtractorEnumerableSerializerSurrogate(Extractors); }
			set { Extractors = value.Extractors; }
		}

		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Any does not really enumerate.")]
		internal IEnumerable<XPathExtractor> BuildExtractorCollection(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var messageType = message.GetProperty(BtsProperties.MessageType);
			if (messageType.IsNullOrEmpty())
			{
				message.BodyPart.WrapOriginalDataStream(
					originalStream => originalStream.AsMarkable(),
					pipelineContext.ResourceTracker);
				messageType = message.BodyPart.Data.EnsureMarkable().Probe().MessageType;
			}
			if (messageType.IsNullOrEmpty()) return Extractors;

			var schemaMetadata = pipelineContext.GetSchemaMetadataByType(messageType, false);
			var schemaAnnotations = schemaMetadata.Annotations.Extractors;

			// TODO take extractorPrecedence into account, right now precedence is always given to schema annotations if they exist

			return schemaAnnotations.Any()
				// merge configured and schema-annotated extractors so as to give precedence to schema annotations.
				// Union enumerates first and second in that order and yields each element that has not already been
				// yielded, see http://msdn.microsoft.com/en-us/library/bb358407(v=VS.90)
				? schemaAnnotations.Union(Extractors).Union(Extractors, new LambdaComparer<XPathExtractor>((le, re) => le.PropertyName == re.PropertyName))
				: Extractors;
		}

		private void OnMatch(IBaseMessageContext messageContext, XmlQualifiedName propertyName, string value, ExtractionMode extractionMode)
		{
			if (extractionMode == ExtractionMode.Promote)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Promoting property {0} with value {1} into context.", propertyName, value);
				messageContext.Promote(propertyName.Name, propertyName.Namespace, value);
			}
			else
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Writing property {0} with value {1} into context.", propertyName, value);
				messageContext.Write(propertyName.Name, propertyName.Namespace, value);
			}
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ContextPropertyExtractor));
	}
}

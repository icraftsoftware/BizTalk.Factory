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

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Streaming.Extensions;
using Be.Stateless.BizTalk.XPath;
using Be.Stateless.Extensions;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.BizTalk.XPath;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// This component allows to manipulate the message context by either clearing, demoting, writing or promoting
	/// property values. These values can either be constant or extracted out of an XML message by defining XPath
	/// expressions. Notice that these XPath expressions are less restrictive than the traditional canonical XPath
	/// expressions supported by BizTalk Server; limitations are however still present and relatively strong.
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
	/// be slightly enhanced to take a specific match index into account, by modifying the <see
	/// cref="ReactiveXPathExtractorCollection.OnMatch"/> method. Given the limitations of the component, it could be
	/// necessary to change its implementation in the future if it cannot meet the new requirements that may arise. The
	/// current implementation uses a full streaming approach, and the alternate way to do it is using an <see
	/// cref="System.Xml.XPath.XPathNavigator"/> in combination with a <see cref="VirtualStream"/>. That would allow
	/// almost all XPath queries of arbitrary complexity, at the cost of some performance loss.
	/// </para>
	/// </remarks>
	/// <example>
	/// <para>
	/// Example of an XML fragment configuration that can either be declared at the pipeline-level configuration or
	/// directly embedded in a schema, <see cref="PropertyExtractorCollection"/>: <code><![CDATA[
	/// <san:Properties [extractorPrecedence='pipeline | pipelineOnly | schema | schemaOnly']
	///                 xmlns:tp='urn:schemas.stateless.be:biztalk:properties:tracking:2012:04'
	///                 xmlns:san='urn:schemas.stateless.be:biztalk:annotations:2013:01'>
	///   <tp:Value1 [mode="clear | ignore | promote | write"]
	///              value="constant-string-literal" />
	///   <tp:Value2 [mode="clear | demote | ignore | promote | write"]
	///              xpath="/*[local-name()='Send']/*[local-name()='Message']/*[local-name()='Subject']" />
	///   <tp:Value3 mode="clear | ignore"] />
	/// </san:Properties>
	/// ]]></code>
	/// </para>
	/// <para>
	/// Notice that the <c>san:extractorPrecedence</c> attribute is relevant only when configured at the pipeline level
	/// and can only have one of the following values: <c>pipeline</c>, <c>pipelineOnly</c>, <c>schema</c>, or
	/// <c>schemaOnly</c>.
	/// </para>
	/// </example>
	/// <seealso cref="PropertyExtractorCollection"/>
	/// <seealso cref="ExtractionMode"/>
	/// <seealso cref="PropertyExtractor"/>
	/// <seealso cref="ConstantExtractor"/>
	/// <seealso cref="XPathExtractor"/>
	public class ContextPropertyExtractor : IMicroPipelineComponent
	{
		public ContextPropertyExtractor()
		{
			Extractors = PropertyExtractorCollection.Empty;
		}

		#region IMicroPipelineComponent Members

		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Any does not really enumerate.")]
		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var extractors = BuildPropertyExtractorCollection(pipelineContext, message);
			var xpathExtractors = extractors.OfType<XPathExtractor>().ToArray();
			var otherExtractors = extractors.Except(xpathExtractors).ToArray();
			if (otherExtractors.Any())
			{
				otherExtractors.Each(pe => pe.Execute(message.Context));
			}
			if (xpathExtractors.Any())
			{
				// setup a stream that will invoke our callback whenever an XPathExtractor's XPath expression is matched
				message.BodyPart.WrapOriginalDataStream(
					originalStream => XPathMutatorStreamFactory.Create(
						originalStream,
						xpathExtractors,
						() => message.Context),
					pipelineContext.ResourceTracker);
			}
			return message;
		}

		#endregion

		[XmlIgnore]
		public PropertyExtractorCollection Extractors { get; set; }

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XmlElement("Extractors")]
		public PropertyExtractorCollectionSerializerSurrogate ExtractorSerializerSurrogate
		{
			get { return new PropertyExtractorCollectionSerializerSurrogate(Extractors); }
			set { Extractors = value; }
		}

		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Any does not really enumerate.")]
		internal IEnumerable<PropertyExtractor> BuildPropertyExtractorCollection(IPipelineContext pipelineContext, IBaseMessage message)
		{
			var messageType = message.GetProperty(BtsProperties.MessageType);
			if (messageType.IsNullOrEmpty())
			{
				message.BodyPart.WrapOriginalDataStream(
					originalStream => originalStream.AsMarkable(),
					pipelineContext.ResourceTracker);
				messageType = message.BodyPart.Data.EnsureMarkable().Probe().MessageType;
			}
			var schemaMetadata = pipelineContext.GetSchemaMetadataByType(messageType, false);
			var schemaExtractors = schemaMetadata.Annotations.Extractors;
			return schemaExtractors.Union(Extractors);
		}
	}
}

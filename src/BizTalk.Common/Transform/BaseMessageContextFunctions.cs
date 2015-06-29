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
using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Transform
{
	/// <summary>
	/// XSLT extension object offering support for the <see cref="IBaseMessageContext"/> of the current <see
	/// cref="IBaseMessage"/>.
	/// </summary>
	/// <seealso cref="XsltArgumentList.AddExtensionObject"/>
	public class BaseMessageContextFunctions
	{
		public BaseMessageContextFunctions(IBaseMessageContext context, IXmlNamespaceResolver xmlNamespaceResolver)
		{
			if (context == null) throw new ArgumentNullException("context");
			if (xmlNamespaceResolver == null) throw new ArgumentNullException("xmlNamespaceResolver");
			_context = context;
			_xmlNamespaceResolver = xmlNamespaceResolver;
		}

		/// <summary>
		/// Returns the value of the property, identified by its XML Qualified name, from the current message context.
		/// </summary>
		/// <param name="qname">
		/// The XML Qualified name of the property, e.g. <c>bts:MessageType</c>.
		/// </param>
		/// <returns>
		/// The property value as a <see cref="string"/>.
		/// </returns>
		/// <remarks>
		/// <paramref name="qname"/> has to be an XML Qualified of the form <c>ns:name</c> where
		/// <list type="bullet">
		/// <item>
		/// <c>ns</c> is the prefix that the XSLT, to which an instance of this class will be added as an extension
		/// object, defines when declaring the target namespace of some property schema;
		/// </item>
		/// <item>
		/// <c>name</c> is the name a property defined in the latter property schema.
		/// </item>
		/// </list>
		/// </remarks>
		public string Read(string qname)
		{
			var qn = qname.ToQName(_xmlNamespaceResolver);
			return Convert.ToString(_context.Read(qn.Name, qn.Namespace));
		}

		/// <summary>
		/// The namespace that any XSLT must declare to <b>automatically</b> benefit from this extension object.
		/// </summary>
		/// <remarks>
		/// If an XSLT choose not to declare this namespace then it is up to itself to instantiate this class and add it
		/// as an extension object.
		/// </remarks>
		public const string TARGET_NAMESPACE = "urn:extensions.stateless.be:biztalk:message:context:2012:12";

		private readonly IBaseMessageContext _context;
		private readonly IXmlNamespaceResolver _xmlNamespaceResolver;
	}
}

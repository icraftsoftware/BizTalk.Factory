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
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.Transform;
using Be.Stateless.BizTalk.Xml.Xsl.Extensions;
using Be.Stateless.Extensions;
using Be.Stateless.Linq.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Xml.Xsl
{
	/// <summary>
	/// Wrapper for the <see cref="System.Xml.Xsl.XslCompiledTransform"/> equivalent of a <see
	/// cref="TransformBase"/>-derived transform.
	/// </summary>
	public class XslCompiledTransformDescriptor
	{
		/// <summary>
		/// Instantiate the <see cref="XslCompiledTransformDescriptor"/> wrapping the <see
		/// cref="System.Xml.Xsl.XslCompiledTransform"/> equivalent of a <see cref="TransformBase"/>-derived transform.
		/// </summary>
		/// <param name="transform">The <see cref="TransformBase"/>-derived transform.</param>
		public XslCompiledTransformDescriptor(Type transform)
		{
			if (!transform.IsTransform())
				throw new ArgumentException(
					string.Format("The type {0} does not derive from TransformBase.", transform.AssemblyQualifiedName),
					"transform");

			var transformBase = Activator.CreateInstance(transform) as TransformBase;
			if (transformBase == null) throw new ArgumentNullException("transform", string.Format("Cannot instantiate type '{0}'.", transform.AssemblyQualifiedName));

			var navigator = CreateNavigator(transformBase.XmlContent);

			Arguments = new Stateless.Xml.Xsl.XsltArgumentList(transformBase.TransformArgs);
			ExtensionRequirements = BuildExtensionRequirements(navigator);
			if ((ExtensionRequirements & ExtensionRequirements.MessageContext) == ExtensionRequirements.MessageContext) NamespaceResolver = BuildNamespaceResolver(navigator);
			XslCompiledTransform = BuildXslCompiledTransform(navigator);
		}

		/// <summary>
		/// The cloneable <see cref="T:Be.Stateless.Xml.Xsl.XsltArgumentList"/> equivalent of the <see
		/// cref="TransformBase"/>-derived <see cref="TransformBase.TransformArgs"/>.
		/// </summary>
		/// <remarks>
		/// Relying on the cloneable <see cref="T:Be.Stateless.Xml.Xsl.XsltArgumentList"/> allows a <see
		/// cref="XslCompiledTransformDescriptor"/> not to keep a reference on a <see cref="TransformBase"/> instance.
		/// </remarks>
		public Stateless.Xml.Xsl.XsltArgumentList Arguments { get; private set; }

		/// <summary>
		/// Requirements of a <see cref="XslCompiledTransform"/> in terms of extension objects.
		/// </summary>
		public ExtensionRequirements ExtensionRequirements { get; private set; }

		/// <summary>
		/// The <see cref="IXmlNamespaceResolver"/> that will be passed to the <see cref="BaseMessageContextFunctions"/>
		/// extension object.
		/// </summary>
		/// <remarks>
		/// The <see cref="IXmlNamespaceResolver"/> resolver will automatically resolve all of the namespaces declared in
		/// the stylesheet markup of <see cref="XslCompiledTransform"/> and is therefore suited to help <see
		/// cref="BaseMessageContextFunctions"/> extensions objects to resolve the namespace prefix of the XML Qualified
		/// name passed to <see cref="BaseMessageContextFunctions.Read"/>.
		/// </remarks>
		public IXmlNamespaceResolver NamespaceResolver { get; private set; }

		/// <summary>
		/// The <see cref="System.Xml.Xsl.XslCompiledTransform"/> equivalent of <see cref="TransformBase"/>-derived
		/// transform.
		/// </summary>
		public XslCompiledTransform XslCompiledTransform { get; private set; }

		#region Helpers

		private static XPathNavigator CreateNavigator(string xmlContent)
		{
			using (var stringReader = new StringReader(xmlContent))
			{
				var navigator = new XPathDocument(stringReader).CreateNavigator();
				navigator.MoveToFollowing(XPathNodeType.Element);
				return navigator;
			}
		}

		private ExtensionRequirements BuildExtensionRequirements(XPathNavigator navigator)
		{
			return !navigator.LookupPrefix(BaseMessageContextFunctions.TARGET_NAMESPACE).IsNullOrEmpty()
				? ExtensionRequirements.MessageContext
				: ExtensionRequirements.None;
		}

		private IXmlNamespaceResolver BuildNamespaceResolver(XPathNavigator navigator)
		{
			Debug.Assert(navigator.NameTable != null, "navigator.NameTable != null");
			var nsm = new XmlNamespaceManager(navigator.NameTable);
			navigator.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml)
				.Each(ns => nsm.AddNamespace(ns.Key, ns.Value));
			return nsm;
		}

		private XslCompiledTransform BuildXslCompiledTransform(IXPathNavigable stylesheet)
		{
			var transform =
#if DEBUG
				new XslCompiledTransform(true);
#else
				new XslCompiledTransform();
#endif
			transform.Load(stylesheet, XsltSettings.TrustedXslt, new XmlUrlResolver());
			return transform;
		}

		#endregion
	}
}

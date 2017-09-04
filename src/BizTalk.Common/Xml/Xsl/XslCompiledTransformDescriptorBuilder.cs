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
	public class XslCompiledTransformDescriptorBuilder
	{
		/// <summary>
		/// Create a <see cref="XslCompiledTransformDescriptorBuilder"/> instance that knows how to build the various
		/// constituents of a <see cref="XslCompiledTransformDescriptor"/> for the given <see
		/// cref="TransformBase"/>-derived transform.
		/// </summary>
		/// <param name="transform">The <see cref="TransformBase"/>-derived transform.</param>
		public XslCompiledTransformDescriptorBuilder(Type transform)
		{
			if (!transform.IsTransform())
				throw new ArgumentException(
					string.Format("The type {0} does not derive from TransformBase.", transform.AssemblyQualifiedName),
					"transform");

			var transformBase = Activator.CreateInstance(transform) as TransformBase;
			if (transformBase == null)
				throw new ArgumentException(
					"transform",
					string.Format(
						"Cannot instantiate type '{0}'.",
						transform.AssemblyQualifiedName));
			_transform = transform;
			_transformBase = transformBase;
			_navigator = BuildNavigator();
		}

		public virtual ExtensionRequirements BuildExtensionRequirements()
		{
			return !_navigator.LookupPrefix(BaseMessageContextFunctions.TARGET_NAMESPACE).IsNullOrEmpty()
				? ExtensionRequirements.MessageContext
				: ExtensionRequirements.None;
		}

		public virtual IXmlNamespaceResolver BuildNamespaceResolver()
		{
			Debug.Assert(_navigator.NameTable != null, "navigator.NameTable != null");
			var nsm = new XmlNamespaceManager(_navigator.NameTable);
			_navigator.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml)
				.Each(ns => nsm.AddNamespace(ns.Key, ns.Value));
			return nsm;
		}

		public virtual XslCompiledTransform BuildXslCompiledTransform()
		{
			var xslCompiledTransform = new XslCompiledTransform();
			xslCompiledTransform.Load(_navigator, XsltSettings.TrustedXslt, new EmbeddedXmlResolver(_transform));
			return xslCompiledTransform;
		}

		public virtual Stateless.Xml.Xsl.XsltArgumentList BuildXsltArgumentList()
		{
			return new Stateless.Xml.Xsl.XsltArgumentList(_transformBase.TransformArgs);
		}

		private XPathNavigator BuildNavigator()
		{
			using (var stringReader = new StringReader(_transformBase.XmlContent))
			{
				var navigator = new XPathDocument(stringReader).CreateNavigator();
				navigator.MoveToFollowing(XPathNodeType.Element);
				return navigator;
			}
		}

		protected readonly Type _transform;

		private readonly XPathNavigator _navigator;
		private readonly TransformBase _transformBase;
	}
}

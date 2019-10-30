#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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

using System.IO;
using System.Xml;
using System.Xml.XPath;
using Be.Stateless.Linq.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Unit.Transform.Extensions
{
	public static class XmlNamespaceManagerExtensions
	{
		/// <summary>
		/// Add all the namespaces and their prefixes declared in a given <see cref="TransformBase"/>-derived the XSLT transform.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="TransformBase"/>-derived XSLT transform to get the namespaces to declare from.
		/// </typeparam>
		/// <param name="xmlNamespaceManager">
		/// The <see cref="XmlNamespaceManager"/> to which to add the namespace declared in the given <typeparamref name="T"/> <see cref="TransformBase"/> transform.
		/// </param>
		/// <returns>
		/// The <see cref="XmlNamespaceManager"/> to which the namespace declarations have been added.
		/// </returns>
		public static XmlNamespaceManager AddNamespaces<T>(this XmlNamespaceManager xmlNamespaceManager) where T : TransformBase, new()
		{
			using (var sr = new StringReader(new T().XmlContent))
			{
				var navigator = new XPathDocument(sr).CreateNavigator();
				navigator.MoveToFollowing(XPathNodeType.Element);
				navigator.GetNamespacesInScope(XmlNamespaceScope.All).Each(n => xmlNamespaceManager.AddNamespace(n.Key, n.Value));
			}
			return xmlNamespaceManager;
		}
	}
}

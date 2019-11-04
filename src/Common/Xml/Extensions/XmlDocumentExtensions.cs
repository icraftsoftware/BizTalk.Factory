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

using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Be.Stateless.Extensions;
using Be.Stateless.IO;

namespace Be.Stateless.Xml.Extensions
{
	public static class XmlDocumentExtensions
	{
		public static Stream AsStream(this XmlDocument document)
		{
			if (document == null) throw new ArgumentNullException("document");
			// remove XmlDeclaration to avoid misleading StringStream with an utf-8 declaration, for instance, as it assumes utf-16
			document.ChildNodes.Cast<XmlNode>()
				.SingleOrDefault(node => node.NodeType == XmlNodeType.XmlDeclaration)
				.IfNotNull(node => document.RemoveChild(node));
			return new StringStream(document.OuterXml);
		}

		public static XmlNamespaceManager GetNamespaceManager(this XmlDocument document)
		{
			var namespaceManager = new XmlNamespaceManager(document.NameTable);
			namespaceManager.AddNamespace("xs", XmlSchema.Namespace);
			namespaceManager.AddNamespace("xsi", XmlSchema.InstanceNamespace);
			return namespaceManager;
		}
	}
}

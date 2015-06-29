#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using System.Linq;
using Be.Stateless.Extensions;

namespace Be.Stateless.Xml.Builder.Extensions
{
	internal static class XmlInformationItemBuilder
	{
		internal static IEnumerable<IXmlAttributeBuilder> GetAttributes(this IXmlInformationItemBuilder informationItem)
		{
			return ((IXmlElementBuilder) informationItem).Attributes ?? Enumerable.Empty<IXmlAttributeBuilder>();
		}

		internal static IEnumerable<IXmlNodeBuilder> GetChildNodes(this IXmlInformationItemBuilder informationItem)
		{
			return ((IXmlElementBuilder) informationItem).Nodes ?? Enumerable.Empty<IXmlNodeBuilder>();
		}

		internal static bool HasAttributes(this IXmlInformationItemBuilder informationItem)
		{
			return ((IXmlElementBuilder) informationItem).Attributes.IfNotNull(a => a.Any());
		}

		public static bool HasChildNodes(this IXmlInformationItemBuilder informationItem)
		{
			return ((IXmlElementBuilder) informationItem).Nodes.IfNotNull(n => n.Any());
		}
	}
}

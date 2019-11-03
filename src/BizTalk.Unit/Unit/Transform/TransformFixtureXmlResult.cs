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
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Be.Stateless.BizTalk.Xml;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	public class TransformFixtureXmlResult : XPathNavigatorDecorator, ITransformFixtureXmlResult
	{
		internal TransformFixtureXmlResult(XPathNavigator decoratedNavigator, XmlNamespaceManager xmlNamespaceManager) : base(decoratedNavigator)
		{
			if (xmlNamespaceManager == null) throw new ArgumentNullException("xmlNamespaceManager");
			XmlNamespaceManager = xmlNamespaceManager;
		}

		#region ITransformFixtureXmlResult Members

		public string XmlContent
		{
			get
			{
				var builder = new StringBuilder();
				using (var writer = new StringWriter(builder))
				using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
				{
					MoveToRoot();
					WriteSubtree(xmlWriter);
				}
				return builder.ToString();
			}
		}

		public XmlNamespaceManager XmlNamespaceManager { get; private set; }

		public string StringJoin(string xpath, char separator = '#')
		{
			return Select(xpath, XmlNamespaceManager)
				.OfType<XPathNavigator>()
				.Aggregate(string.Empty, (str, node) => str + node.Value + separator, str => str.TrimEnd(separator));
		}

		public override object Evaluate(string xpath)
		{
			return base.Evaluate(xpath, XmlNamespaceManager);
		}

		public override bool Matches(string xpath)
		{
			return base.Matches(XPathExpression.Compile(xpath, XmlNamespaceManager));
		}

		public override XPathNodeIterator Select(string xpath)
		{
			return base.Select(xpath, XmlNamespaceManager);
		}

		public override XPathNavigator SelectSingleNode(string xpath)
		{
			return base.SelectSingleNode(xpath, XmlNamespaceManager);
		}

		#endregion

		#region Base Class Member Overrides

		protected internal override XPathNavigator CreateXPathNavigatorDecorator(XPathNavigator decoratedNavigator)
		{
			return new TransformFixtureXmlResult(decoratedNavigator, XmlNamespaceManager);
		}

		#endregion
	}
}

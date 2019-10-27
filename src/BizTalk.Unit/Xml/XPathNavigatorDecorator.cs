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
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.XPath;

namespace Be.Stateless.BizTalk.Xml
{
	public abstract class XPathNavigatorDecorator : XPathNavigator
	{
		protected XPathNavigatorDecorator(XPathNavigator decoratedNavigator)
		{
			if (decoratedNavigator == null) throw new ArgumentNullException("decoratedNavigator");
			_decoratedNavigator = decoratedNavigator;
		}

		#region Base Class Member Overrides

		public override string BaseURI
		{
			get { return _decoratedNavigator.BaseURI; }
		}

		public override bool CanEdit
		{
			get { return false; }
		}

		public override XPathNavigator Clone()
		{
			return CreateXPathNavigatorDecorator(_decoratedNavigator.Clone());
		}

		public override XPathNavigator CreateNavigator()
		{
			return CreateXPathNavigatorDecorator(_decoratedNavigator.CreateNavigator());
		}

		public override object Evaluate(string xpath)
		{
			return _decoratedNavigator.Evaluate(xpath);
		}

		public override object Evaluate(string xpath, IXmlNamespaceResolver resolver)
		{
			return _decoratedNavigator.Evaluate(xpath, resolver);
		}

		public override object Evaluate(XPathExpression expr)
		{
			return _decoratedNavigator.Evaluate(expr);
		}

		public override object Evaluate(XPathExpression expr, XPathNodeIterator context)
		{
			return _decoratedNavigator.Evaluate(expr, context);
		}

		public override bool IsEmptyElement
		{
			get { return _decoratedNavigator.IsEmptyElement; }
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			return _decoratedNavigator.IsSamePosition(other);
		}

		public override string LocalName
		{
			get { return _decoratedNavigator.LocalName; }
		}

		public override bool MoveTo(XPathNavigator other)
		{
			return _decoratedNavigator.MoveTo(other);
		}

		public override bool MoveToFirstAttribute()
		{
			return _decoratedNavigator.MoveToFirstAttribute();
		}

		public override bool MoveToFirstChild()
		{
			return _decoratedNavigator.MoveToFirstChild();
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
		{
			return _decoratedNavigator.MoveToFirstNamespace(namespaceScope);
		}

		public override bool MoveToId(string id)
		{
			return _decoratedNavigator.MoveToId(id);
		}

		public override bool MoveToNext()
		{
			return _decoratedNavigator.MoveToNext();
		}

		public override bool MoveToNextAttribute()
		{
			return _decoratedNavigator.MoveToNextAttribute();
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
		{
			return _decoratedNavigator.MoveToNextNamespace(namespaceScope);
		}

		public override bool MoveToParent()
		{
			return _decoratedNavigator.MoveToParent();
		}

		public override bool MoveToPrevious()
		{
			return _decoratedNavigator.MoveToPrevious();
		}

		public override string Name
		{
			get { return _decoratedNavigator.Name; }
		}

		public override string NamespaceURI
		{
			get { return _decoratedNavigator.NamespaceURI; }
		}

		public override XmlNameTable NameTable
		{
			get { return _decoratedNavigator.NameTable; }
		}

		public override XPathNodeType NodeType
		{
			get { return _decoratedNavigator.NodeType; }
		}

		public override string Prefix
		{
			get { return _decoratedNavigator.Prefix; }
		}

		public override XmlReader ReadSubtree()
		{
			return _decoratedNavigator.ReadSubtree();
		}

		public override XPathNodeIterator Select(string xpath)
		{
			return new XPathNodeIteratorDecorator(_decoratedNavigator.Select(xpath), this);
		}

		public override XPathNodeIterator Select(string xpath, IXmlNamespaceResolver resolver)
		{
			return new XPathNodeIteratorDecorator(_decoratedNavigator.Select(xpath, resolver), this);
		}

		public override XPathNodeIterator Select(XPathExpression expr)
		{
			return new XPathNodeIteratorDecorator(_decoratedNavigator.Select(expr), this);
		}

		public override XPathNodeIterator SelectAncestors(XPathNodeType type, bool matchSelf)
		{
			return new XPathNodeIteratorDecorator(_decoratedNavigator.SelectAncestors(type, matchSelf), this);
		}

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public override XPathNodeIterator SelectAncestors(string name, string namespaceURI, bool matchSelf)
		{
			return new XPathNodeIteratorDecorator(_decoratedNavigator.SelectAncestors(name, namespaceURI, matchSelf), this);
		}

		public override XPathNodeIterator SelectChildren(XPathNodeType type)
		{
			return new XPathNodeIteratorDecorator(_decoratedNavigator.SelectChildren(type), this);
		}

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public override XPathNodeIterator SelectChildren(string name, string namespaceURI)
		{
			return new XPathNodeIteratorDecorator(_decoratedNavigator.SelectChildren(name, namespaceURI), this);
		}

		public override XPathNodeIterator SelectDescendants(XPathNodeType type, bool matchSelf)
		{
			return new XPathNodeIteratorDecorator(_decoratedNavigator.SelectDescendants(type, matchSelf), this);
		}

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public override XPathNodeIterator SelectDescendants(string name, string namespaceURI, bool matchSelf)
		{
			return new XPathNodeIteratorDecorator(_decoratedNavigator.SelectDescendants(name, namespaceURI, matchSelf), this);
		}

		public override XPathNavigator SelectSingleNode(string xpath)
		{
			return CreateXPathNavigatorDecorator(_decoratedNavigator.SelectSingleNode(xpath));
		}

		public override XPathNavigator SelectSingleNode(string xpath, IXmlNamespaceResolver resolver)
		{
			return CreateXPathNavigatorDecorator(_decoratedNavigator.SelectSingleNode(xpath, resolver));
		}

		public override XPathNavigator SelectSingleNode(XPathExpression expression)
		{
			return CreateXPathNavigatorDecorator(_decoratedNavigator.SelectSingleNode(expression));
		}

		#endregion

		#region Base Class Member Overrides

		public override string Value
		{
			get { return _decoratedNavigator.Value; }
		}

		#endregion

		protected internal abstract XPathNavigator CreateXPathNavigatorDecorator(XPathNavigator decoratedNavigator);

		private readonly XPathNavigator _decoratedNavigator;
	}
}

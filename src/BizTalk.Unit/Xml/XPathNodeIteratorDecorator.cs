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
using System.Xml.XPath;

namespace Be.Stateless.BizTalk.Xml
{
	public class XPathNodeIteratorDecorator : XPathNodeIterator
	{
		public XPathNodeIteratorDecorator(XPathNodeIterator decoratedIterator, XPathNavigatorDecorator xPathNavigatorDecoratorFactory)
		{
			if (decoratedIterator == null) throw new ArgumentNullException("decoratedIterator");
			if (xPathNavigatorDecoratorFactory == null) throw new ArgumentNullException("xPathNavigatorDecoratorFactory");
			_decoratedIterator = decoratedIterator;
			_xPathNavigatorDecoratorFactory = xPathNavigatorDecoratorFactory;
		}

		#region Base Class Member Overrides

		public override XPathNodeIterator Clone()
		{
			return new XPathNodeIteratorDecorator(_decoratedIterator.Clone(), _xPathNavigatorDecoratorFactory);
		}

		public override XPathNavigator Current
		{
			get { return _xPathNavigatorDecoratorFactory.CreateXPathNavigatorDecorator(_decoratedIterator.Current); }
		}

		public override int CurrentPosition
		{
			get { return _decoratedIterator.CurrentPosition; }
		}

		public override bool MoveNext()
		{
			return _decoratedIterator.MoveNext();
		}

		#endregion

		private readonly XPathNodeIterator _decoratedIterator;
		private readonly XPathNavigatorDecorator _xPathNavigatorDecoratorFactory;
	}
}

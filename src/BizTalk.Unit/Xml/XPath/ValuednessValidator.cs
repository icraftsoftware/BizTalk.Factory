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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using Be.Stateless.BizTalk.Xml.XPath.Extensions;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.BizTalk.Xml.XPath
{
	[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Necessary for mocking purposes.")]
	internal class ValuednessValidator
	{
		internal ValuednessValidator(XPathNavigator navigator, ValuednessValidationCallback validationCallback)
		{
			if (navigator == null) throw new ArgumentNullException("navigator");
			_navigator = navigator;
			_validationCallback = validationCallback;
			IsValid = true;
			OffendingPaths = new List<string>();
		}

		private bool IsValid { get; set; }

		private List<string> OffendingPaths { get; set; }

		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
		internal virtual bool Validate()
		{
			var emptyElements = _navigator.SelectEmptyElements();
			if (emptyElements.Any()) IsValid = false;
			var emptyAttributes = _navigator.SelectEmptyAttributes();
			if (emptyAttributes.Any()) IsValid = false;
			if (_validationCallback != null)
			{
				emptyElements.Each(n => InvokeValidationCallback(n.Clone()));
				emptyAttributes.Each(n => InvokeValidationCallback(n.Clone()));
			}
			if (!IsValid && OffendingPaths.Any())
			{
				var message = "The following nodes have either no value nor any child element:" + Environment.NewLine + string.Join(Environment.NewLine, OffendingPaths);
				throw new XmlException(message);
			}
			return IsValid;
		}

		private void InvokeValidationCallback(XPathNavigator offendingNodeNavigator)
		{
			// invalid as soon as there is an empty node
			IsValid = false;
			var args = new ValuednessValidationCallbackArgs(offendingNodeNavigator, XmlSeverityType.Error);
			_validationCallback(_navigator, args);
			// if error severity level has not been downgraded to warning accumulate empty nodes to throw a global exception afterwards
			if (args.Severity == XmlSeverityType.Error) OffendingPaths.Add(GetCurrentNodeXPath(offendingNodeNavigator));
		}

		private string GetCurrentNodeXPath(XPathNavigator navigator)
		{
			string path;
			if (navigator.NodeType == XPathNodeType.Attribute)
			{
				path = "/@" + navigator.Name;
			}
			else
			{
				path = "/" + navigator.Name;
				var position = GetCurrentNodePosition(navigator);
				if (position > 0) path += "[" + position + "]";
			}
			if (navigator.MoveToParent() && navigator.NodeType != XPathNodeType.Root)
			{
				path = GetCurrentNodeXPath(navigator) + path;
			}
			return path;
		}

		private int GetCurrentNodePosition(XPathNavigator navigator)
		{
			// return 0 if current node is the only child with the same name, or its actual position otherwise
			var previousCount = 0;
			var previousSiblingNavigator = navigator.Clone();
			while (previousSiblingNavigator.MoveToPrevious())
			{
				if (previousSiblingNavigator.Name == navigator.Name) previousCount++;
			}
			if (previousCount > 0) return previousCount + 1;

			var nextCount = 0;
			var nextSiblingNavigator = navigator.Clone();
			while (nextSiblingNavigator.MoveToNext())
			{
				if (nextSiblingNavigator.Name == navigator.Name) nextCount++;
			}
			return nextCount > 0 ? 1 : 0;
		}

		private readonly XPathNavigator _navigator;
		private readonly ValuednessValidationCallback _validationCallback;
	}
}

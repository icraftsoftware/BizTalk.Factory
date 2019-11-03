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
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using Be.Stateless.Xml.XPath.Extensions;

namespace Be.Stateless.BizTalk.Xml.XPath.Extensions
{
	public static class XPathNavigatorExtensions
	{
		#region Mock's Factory Hook Point

		internal static Func<XPathNavigator, ValuednessValidationCallback, ValuednessValidator> ValuednessValidatorFactory
		{
			get { return _valuednessValidatorFactory; }
			set { _valuednessValidatorFactory = value; }
		}

		#endregion

		/// <summary>
		/// Test whether the XML denoted by the <see cref="XPathNavigator"/> contains any empty element or attribute.
		/// </summary>
		/// <param name="navigator">
		/// The <see cref="XPathNavigator"/> over the XML to test for valuedness.
		/// </param>
		/// <param name="valuednessValidationCallback">
		/// An optional validation callback that can be used to demote the <see cref="XmlSeverityType"/> so that no exception is thrown if all the empty elements or
		/// attributes are only to be considered as <see cref="XmlSeverityType.Warning"/>.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if there are any empty element or attribute; <see langword="false" /> otherwise.
		/// </returns>
		/// <remarks>
		/// <para>
		/// An element is considered empty if it has either no children nor any value; an element, which has the xsi:nil='true', is not considered empty.
		/// </para>
		/// <para>
		/// An attribute is considered empty if it has no value.
		/// </para>
		/// <para>
		/// Empty elements or attributes will be considered as an <see cref="XmlSeverityType.Error"/> condition which will entail that an <see cref="XmlException"/> will
		/// be thrown unless a <see cref="ValuednessValidationCallback"/> validation callback is passed to demote the severity of the error for a given element or
		/// attribute to <see cref="XmlSeverityType.Warning"/>.
		/// </para>
		/// </remarks>
		public static bool CheckValuedness(this XPathNavigator navigator, ValuednessValidationCallback valuednessValidationCallback)
		{
			return ValuednessValidatorFactory(navigator, valuednessValidationCallback).Validate();
		}

		public static IEnumerable<XPathNavigator> Select(this IEnumerable<XPathNavigator> navigators, string xpath)
		{
			return navigators.SelectMany(n => n.Select(xpath).Cast<XPathNavigator>());
		}

		public static IEnumerable<XPathNavigator> SelectEmptyAttributes(this XPathNavigator navigator)
		{
			return navigator.Select("//@*[string(.) = '']").Cast<XPathNavigator>();
		}

		public static IEnumerable<XPathNavigator> SelectEmptyElements(this XPathNavigator navigator)
		{
			return navigator.Select("//*[not(@xsi:nil = 'true')][not(text() | *)]", navigator.GetNamespaceManager()).Cast<XPathNavigator>();
		}

		private static Func<XPathNavigator, ValuednessValidationCallback, ValuednessValidator> _valuednessValidatorFactory
			= (navigator, callback) => new ValuednessValidator(navigator, callback);
	}
}

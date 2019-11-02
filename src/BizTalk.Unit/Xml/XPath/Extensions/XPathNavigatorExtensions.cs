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

		public static IEnumerable<XPathNavigator> Select(this IEnumerable<XPathNavigator> navigators, string xpath)
		{
			return navigators.SelectMany(n => n.Select(xpath).Cast<XPathNavigator>());
		}

		public static bool CheckValuedness(this XPathNavigator navigator, ValuednessValidationCallback valuednessValidationCallback)
		{
			return ValuednessValidatorFactory(navigator, valuednessValidationCallback).Validate();
		}

		public static IEnumerable<XPathNavigator> SelectEmptyAttributes(this XPathNavigator navigator)
		{
			return navigator.Select("//@*[string(.) = '']").Cast<XPathNavigator>();
		}

		public static IEnumerable<XPathNavigator> SelectEmptyElements(this XPathNavigator navigator)
		{
			return navigator.Select("//*[not(text() | *)][not(@xsi:nil = 'true')]", navigator.GetNamespaceManager()).Cast<XPathNavigator>();
		}

		private static Func<XPathNavigator, ValuednessValidationCallback, ValuednessValidator> _valuednessValidatorFactory
			= (navigator, callback) => new ValuednessValidator(navigator, callback);
	}
}

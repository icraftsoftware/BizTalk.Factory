#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Be.Stateless.BizTalk.Transform
{
	/// <summary>
	/// XSLT extension object offering support for regular expressions.
	/// </summary>
	/// <seealso cref="XsltArgumentList.AddExtensionObject"/>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "XSLT Extension Object.")]
	public class RegexFunctions
	{
		public bool IsMatch(string input, string pattern)
		{
			return Regex.IsMatch(input, pattern);
		}

		/// <summary>
		/// Indicates whether the specified regular expression finds a match in the <b>whole</b> specified input string.
		/// </summary>
		/// <param name="input">The string to search for a match.</param>
		/// <param name="pattern">The regular expression pattern to match.</param>
		/// <returns>true if the regular expression finds a match; otherwise, false.</returns>
		[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global", Justification = "XSLT Extension Object.")]
		public bool IsOneOf(string input, string pattern)
		{
			return Regex.IsMatch(input, "^(" + pattern + ")$");
		}

		/// <summary>
		/// Searches the specified input string for the occurrence of the regular expression pattern.
		/// </summary>
		/// <param name="input">The string to search for a match.</param>
		/// <param name="pattern">The regular expression pattern to match.</param>
		/// <returns>The collection of groups matched by the regular expression pattern as a node set.</returns>
		/// <remarks>
		/// The first group is skipped as it matches the whole string. There will be as many nodes in the node set as
		/// there are matched groups.
		/// </remarks>
		public XPathNodeIterator Match(string input, string pattern)
		{
			var xdoc = new XElement(
				"groups",
				Regex.Match(input, pattern).Groups.Cast<Group>()
					.Skip(1) // skip first match (which match the 'whole' string)
					.Select(g => new XElement("group", g.Value)));
			return xdoc.CreateNavigator().Select("group");
		}

		public string Replace(string input, string pattern, string replacement)
		{
			return Regex.Replace(input, pattern, replacement);
		}
	}
}

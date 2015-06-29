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

namespace Be.Stateless.BizTalk.Transform
{
	/// <summary>
	/// XSLT extension object offering support for more concise logical operators, like an equivalent to the C# <c>?:</c>
	/// ternary operator.
	/// </summary>
	/// <seealso cref="System.Xml.Xsl.XsltArgumentList.AddExtensionObject(string, object)"/>
	public class OperatorFunctions
	{
		/// <summary>
		/// Returns one of two values depending on the value of a Boolean <paramref name="condition"/>.
		/// </summary>
		/// <param name="condition">
		/// The Boolean expression to evaluate as the condition.
		/// </param>
		/// <param name="then">
		/// The value to be returned if <paramref name="condition"/> evaluates to <c>true</c>.
		/// </param>
		/// <param name="else">
		/// The value to be returned if <paramref name="condition"/> evaluates to <c>false</c>.
		/// </param>
		/// <returns>
		/// Either <paramref name="then"/> or <paramref name="else"/> value depending on the Boolean value of <paramref
		/// name="condition"/>.
		/// </returns>
		public string Iif(bool condition, string @then, string @else)
		{
			return condition ? @then : @else;
		}
	}
}

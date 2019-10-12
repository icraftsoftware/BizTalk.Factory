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
using System.Text.RegularExpressions;
using System.Xml;

namespace Be.Stateless.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Performs an <see cref="Action{T}"/> delegate on the <paramref name="string"/> if it is not null nor empty.
		/// </summary>
		/// <param name="string">
		/// The string to test and to pass as argument to the <paramref name="action"/> delegate.
		/// </param>
		/// <param name="action">
		/// The <see cref="Action{T}"/> delegate to perform.
		/// </param>
		public static void IfNotNullOrEmpty(this string @string, Action<string> action)
		{
			if (@string.IsNullOrEmpty()) return;
			action(@string);
		}

		/// <summary>
		/// Performs an <see cref="Func{TResult}"/> delegate on the <paramref name="string"/> if it is not null nor empty
		/// and returns the <paramref name="function"/> delegate value.
		/// </summary>
		/// <typeparam name="TR">
		/// The return type of the <see cref="Func{TResult}"/> delegate.
		/// </typeparam>
		/// <param name="string">
		/// The string to test and to pass as argument to the <paramref name="function"/> delegate.
		/// </param>
		/// <param name="function">
		/// The <see cref="Func{TResult}"/> delegate to perform.
		/// </param>
		/// <returns>
		/// The result of the <paramref name="function"/> delegate, or <c>default(TR)</c> if the <paramref name="string"/>
		/// is null or empty.
		/// </returns>
		public static TR IfNotNullOrEmpty<TR>(this string @string, Func<string, TR> function)
		{
			return @string.IsNullOrEmpty() ? default(TR) : function(@string);
		}

		/// <summary>
		/// Verifies that the <paramref name="name"/> string is a valid file name.
		/// </summary>
		/// <param name="name">
		/// The file name to verify.
		/// </param>
		/// <returns>
		/// <c>true</c> if it is a valid file name; <c>false</c> otherwise.
		/// </returns>
		public static bool IsFileName(this string name)
		{
			return name.IfNotNullOrEmpty(n => n.IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
		}

		/// <summary>
		/// Indicates whether the specified string is null or an <see cref="string.Empty"/> string.
		/// </summary>
		/// <param name="string">
		/// The string to test.
		/// </param>
		/// <returns>
		/// <c>true</c> if the <paramref name="string"/> argument is null or an <see cref="string.Empty"/>; otherwise,
		/// <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty(this string @string)
		{
			return string.IsNullOrEmpty(@string);
		}

		/// <summary>
		/// Indicates whether a specified string is null, empty, or consists only of white-space characters.
		/// </summary>
		/// <param name="string">
		/// The string to test.
		/// </param>
		/// <returns>
		/// <c>true</c> if the <paramref name="string"/> argument is null or an <see cref="string.Empty"/>, or if it
		/// consists exclusively of white-space characters; otherwise <c>false</c>.
		/// </returns>
		public static bool IsNullOrWhiteSpace(this string @string)
		{
			return string.IsNullOrWhiteSpace(@string);
		}

		/// <summary>
		/// Verifies that the <paramref name="qName"/> string is a valid qualified name according to the W3C Extended
		/// Markup Language recommendation.
		/// </summary>
		/// <param name="qName">
		/// The QName to verify.
		/// </param>
		/// <returns>
		/// <c>true</c> if it is a valid qualified name; <c>false</c> otherwise.
		/// </returns>
		/// <seealso href="http://www.w3.org/TR/2009/REC-xml-names-20091208/#ns-qualnames"/>
		public static bool IsQName(this string qName)
		{
			// could have used http://msdn.microsoft.com/en-us/library/system.xml.xmlconvert.verifyncname.aspx but it is
			// not a predicate and rather throws an exception instead
			// see also XmlReader.IsName and XmlReader.IsNameToken
			string prefix;
			string localPart;
			return TryParseQName(qName, out prefix, out localPart);
		}

		/// <summary>
		/// Converts the string representation of the name or numeric value of one or more enumerated constants to an
		/// equivalent enumerated object.
		/// </summary>
		/// <typeparam name="T">
		/// An enumeration type.
		/// </typeparam>
		/// <param name="value">
		/// A string containing the name or value to convert.
		/// </param>
		/// <param name="ignoreCase">
		/// Whether to ignore case or not; <c>true</c> by default.
		/// </param>
		/// <returns>
		/// One or more enumerated constants of type <typeparamref name="T"/> that represents <paramref name="value"/>.
		/// </returns>
		public static T Parse<T>(this string value, bool ignoreCase = true) where T : struct
		{
			return (T) Enum.Parse(typeof(T), value, ignoreCase);
		}

		/// <summary>
		/// Try to parse a qualified name and extracts its prefix and local parts.
		/// </summary>
		/// <param name="qName">
		/// The qualified to parse.
		/// </param>
		/// <param name="prefix">
		/// The prefix part of the qName if it is a valid qualified name; <see cref="string.Empty"/> if there is no prefix
		/// part.
		/// </param>
		/// <param name="localPart">
		/// The local part of the qName if it is a valid qualified name.
		/// </param>
		/// <returns>
		/// <c>true</c> if it is a valid qualified name; <c>false</c> otherwise.
		/// </returns>
		/// <seealso href="http://www.w3.org/TR/xml-names/#ns-qualnames">Qualified Names</seealso>
		public static bool TryParseQName(this string qName, out string prefix, out string localPart)
		{
			// http://www.w3.org/TR/xml-names/#NT-NCName
			// see also Character Class Subtraction (https://docs.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions)
			const string ncNamePattern = @"[\w-[\d]][\w\-\.]*";
			// Qualified Names, see http://www.w3.org/TR/xml-names/#ns-qualnames
			const string qNamePattern = @"^(?:(?<prefix>" + ncNamePattern + @")\:)?(?<localPart>" + ncNamePattern + ")$";
			var match = Regex.Match(qName ?? string.Empty, qNamePattern);
			if (match.Success)
			{
				prefix = match.Groups["prefix"].Value;
				localPart = match.Groups["localPart"].Value;
				return true;
			}
			prefix = localPart = null;
			return false;
		}

		/// <summary>
		/// Extract the last <c>length</c> characters of a string.
		/// </summary>
		/// <param name="string">The string to extract characters of.</param>
		/// <param name="length">The number of characters to extract.</param>
		/// <returns>The substring of the input string.</returns>
		/// <remarks>
		/// Returns an empty string if the input string is null or empty. If the length of the input string is less than
		/// <c>length</c>, the whole string is returned.
		/// </remarks>
		public static string Right(this string @string, int length)
		{
			if (@string.IsNullOrEmpty()) return string.Empty;
			var startIndex = @string.Length - length;
			return startIndex < 0 ? @string : @string.Substring(startIndex);
		}

		public static string RTrimToChar(this string text, char character)
		{
			var i = text.LastIndexOf(character);
			return i < 0 ? text : text.Substring(0, i);
		}

		/// <summary>
		/// Extract the first or last <c>length</c> characters of a string, whether <c>length</c> is respectively positive
		/// or negative.
		/// </summary>
		/// <param name="string">The string to extract characters of.</param>
		/// <param name="length">The number of characters to extract.</param>
		/// <returns>The substring of the input string.</returns>
		/// <remarks>
		/// Returns an empty string if the input string is null or empty. If the length of the input string is less than
		/// <c>length</c>, the whole string is returned.
		/// </remarks>
		public static string SubstringEx(this string @string, int length)
		{
			if (@string.IsNullOrEmpty()) return string.Empty;
			return length < 0
				? @string.Right(-length)
				: length > @string.Length ? @string : @string.Substring(0, length);
		}

		/// <summary>
		/// Converts a Pascal-case string to a camel-case one.
		/// </summary>
		/// <param name="string">
		/// A Pascal-case string.
		/// </param>
		/// <returns>
		/// The camel-case equivalent string.
		/// </returns>
		public static string ToCamelCase(this string @string)
		{
			return char.ToLower(@string[0]) + @string.Substring(1);
		}

		public static string Tokens(this string text, char separator, params int[] tokens)
		{
			if (tokens.Length == 0 || text.IsNullOrEmpty()) return text;
			var strings = text.Split(separator);
			return strings.Length == 0 ? text : string.Join("/", tokens.Select(t => strings[t]).ToArray());
		}

		/// <summary>
		/// Given a qualified name <paramref name="qName"/>, e.g. <c>ns:name</c> where <c>ns</c> is an xmlns prefix and
		/// <c>name</c> is an node name, and a <see cref="IXmlNamespaceResolver"/> <paramref name="namespaceResolver"/>,
		/// parses the <paramref name="qName"/> string to return its typed <see cref="XmlQualifiedName"/> equivalent.
		/// </summary>
		/// <param name="qName">
		/// The qName string to parse.
		/// </param>
		/// <param name="namespaceResolver">
		/// The <see cref="IXmlNamespaceResolver"/> to use to resolve the xmlns prefix of the <paramref name="qName"/>.
		/// </param>
		/// <returns>
		/// The typed <see cref="XmlQualifiedName"/> equivalent of the <paramref name="qName"/>.
		/// </returns>
		public static XmlQualifiedName ToQName(this string qName, IXmlNamespaceResolver namespaceResolver)
		{
			string prefix;
			string localPart;
			var success = TryParseQName(qName, out prefix, out localPart);
			if (!success) throw new ArgumentException(string.Format("'{0}' is not a valid XML qualified name.", qName), "qName");
			var ns = namespaceResolver.LookupNamespace(prefix);
			return new XmlQualifiedName(localPart, ns);
		}
	}
}

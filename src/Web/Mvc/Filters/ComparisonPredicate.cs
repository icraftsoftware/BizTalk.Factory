#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using Be.Stateless.Extensions;

namespace Be.Stateless.Web.Mvc.Filters
{
	internal class ComparisonPredicate
	{
		/// <summary>
		/// Parse a URL's query string argument into a <see cref="ComparisonPredicate"/>
		/// instance, where the <paramref name="queryArgument"/>'s first character
		/// may denote a <see cref="ComparisonOperator"/> while the trailing
		/// characters denote the compared against value.
		/// </summary>
		/// <param name="queryArgument">
		/// The query string argument to parse.
		/// </param>
		/// <returns>
		/// The <see cref="ComparisonPredicate"/> denoting the value and comparison operator
		/// conveyed in the passed <paramref name="queryArgument"/>
		/// </returns>
		/// <remarks>
		/// For comparison operators, single characters that do not require URI
		/// encoding are used, namely:
		/// <list type="bullet">
		/// <item>
		///	<term><c>*</c>, an asterisk,</term>
		///	<description>
		///	which denotes <see cref="ComparisonOperator"/>.<see
		///	cref="ComparisonOperator.Any"/> if not followed by any character,
		///	or <see
		///	cref="ComparisonOperator"/>.<see cref="ComparisonOperator.Like"/>
		///	otherwise.
		///	</description>
		/// </item>
		/// <item>
		///	<term><c>-</c>, a dash,</term>
		///	<description>
		///	which denotes <see cref="ComparisonOperator"/>.<see
		///	cref="ComparisonOperator.Unlike"/>.
		///	</description>
		/// </item>
		/// <item>
		///	<term><c>!</c>, an exclamation mark,</term>
		///	<description>
		///	which denotes <see cref="ComparisonOperator"/>.<see
		///	cref="ComparisonOperator.NotEqual"/>.
		///	</description>
		/// </item>
		/// <item>
		///	<term><c>(</c>, a left parenthesis,</term>
		///	<description>
		///	which denotes <see cref="ComparisonOperator"/>.<see
		///	cref="ComparisonOperator.Lower"/>.
		///	</description>
		/// </item>
		/// <item>
		///	<term><c>)</c>, a right parenthesis,</term>
		///	<description>
		///	which denotes <see cref="ComparisonOperator"/>.<see
		///	cref="ComparisonOperator.Greater"/>.
		///	</description>
		/// </item>
		/// <item>
		///	<term><c>.</c>, a dot,</term>
		///	<description>
		///	which is reserved for future use.
		///	</description>
		/// </item>
		/// <item>
		///	<term><c>'</c>, a single quote,</term>
		///	<description>
		///	which is reserved for future use.
		///	</description>
		/// </item>
		/// <item>
		///	<term><c>_</c>, an underscore,</term>
		///	<description>
		///	which is reserved for future use.
		///	</description>
		/// </item>
		/// </list>
		/// When the first character is none of the above, the comparison operator
		/// defaults to <see cref="ComparisonOperator"/>.<see
		/// cref="ComparisonOperator.Equal"/>.
		/// </remarks>
		public static ComparisonPredicate Parse(string queryArgument)
		{
			if (queryArgument.IsNullOrEmpty() || queryArgument.Equals("*", StringComparison.OrdinalIgnoreCase))
				return new ComparisonPredicate {
					Operator = ComparisonOperator.Any,
					RawValue = queryArgument
				};

			// defaults to equal when 1st char is none of the following chars
			var @operator = ComparisonOperator.Equal;
			var value = queryArgument;

			if (queryArgument.StartsWith("*", StringComparison.OrdinalIgnoreCase))
			{
				@operator = ComparisonOperator.Like;
				value = queryArgument.Substring(1);
			}
			else if (queryArgument.StartsWith("-", StringComparison.OrdinalIgnoreCase))
			{
				@operator = ComparisonOperator.Unlike;
				value = queryArgument.Substring(1);
			}
			else if (queryArgument.StartsWith("!", StringComparison.OrdinalIgnoreCase))
			{
				@operator = ComparisonOperator.NotEqual;
				value = queryArgument.Substring(1);
			}
			else if (queryArgument.StartsWith("(", StringComparison.OrdinalIgnoreCase))
			{
				@operator = ComparisonOperator.Lower;
				value = queryArgument.Substring(1);
			}
			else if (queryArgument.StartsWith(")", StringComparison.OrdinalIgnoreCase))
			{
				@operator = ComparisonOperator.Greater;
				value = queryArgument.Substring(1);
			}

			return new ComparisonPredicate {
				Operator = @operator,
				RawValue = queryArgument,
				Value = value
			};
		}

		public ComparisonOperator Operator { get; private set; }

		public string RawValue { get; private set; }

		public string Value { get; private set; }
	}
}

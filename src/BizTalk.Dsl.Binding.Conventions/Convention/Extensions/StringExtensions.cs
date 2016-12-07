#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using System.Linq;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Extensions
{
	internal static class StringExtensions
	{
		internal static string TrimSuffix(this string @string, string suffix)
		{
			var i = @string.IndexOf(suffix, StringComparison.Ordinal);
			if (i > 0) @string = @string.SubstringEx(i);
			return @string;
		}

		internal static string Capitalize(this string @string)
		{
			// try to detect acronyms (.i.e. uppercase string) that are longer than 2 characters
			if (@string.All(char.IsUpper) && @string.Length > 2) @string = @string.ToLower();
			@string = char.ToUpper(@string[0]) + @string.Substring(1);
			return @string;
		}
	}
}

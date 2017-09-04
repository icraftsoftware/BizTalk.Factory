#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

namespace Be.Stateless.Extensions
{
	public static class ArrayExtensions
	{
		/// <summary>
		/// Returns a string representing that part of the path tree that is common to all the paths.
		/// </summary>
		/// <param name="paths">
		/// The set of paths describing the path tree to extract the longest common path from.
		/// </param>
		/// <param name="separator">
		/// Path separator; it defaults to a slash ('<c>/</c>').
		/// </param>
		/// <returns>
		/// The longest common path of the set of <paramref name="paths"/>.
		/// </returns>
		/// <remarks>
		/// The resultant path is a valid path and not just the longest common string; that is to say that no path segment
		/// will be truncated.
		/// </remarks>
		/// <seealso href="http://blogs.microsoft.co.il/yuvmaz/2013/05/10/longest-common-prefix-with-c-and-linq/">Longest Common Prefix with C# and LINQ</seealso>
		/// <seealso href="https://miafish.wordpress.com/2015/02/17/leetcode-oj-c-longest-common-prefix/">Longest Common Prefix</seealso>
		/// <seealso href="https://www.rosettacode.org/wiki/Find_common_directory_path">Find common directory path</seealso>
		/// <seealso href="https://stackoverflow.com/questions/2070356/find-common-prefix-of-strings">Find common prefix of strings</seealso>
		/// <seealso href="https://stackoverflow.com/questions/33709165/get-common-prefix-of-two-string">Get common prefix of two string</seealso>
		public static string CommonPath(this string[] paths, string separator = "/")
		{
			if (paths == null || paths.Length == 0) return string.Empty;
			var commonSegments = paths
				.Select(p => p.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries))
				.Aggregate(
					(accumulatedCommonSegments, pathSegments) => accumulatedCommonSegments
						.TakeWhile((segment, i) => i < pathSegments.Length && pathSegments[i].Equals(segment))
						.ToArray()
				);
			// https://stackoverflow.com/questions/14897121/using-enumerable-aggregate-method-over-an-empty-sequence
			return string.Join(separator, commonSegments);
		}

		/// <summary>
		/// Retrieves a subarray, from this <paramref name="array"/>, that starts at a specified position.
		/// </summary>
		/// <typeparam name="T">
		/// The type of this <paramref name="array"/>'s elements.
		/// </typeparam>
		/// <param name="array">
		/// This array.
		/// </param>
		/// <param name="startIndex">
		/// The zero-based starting position of a subarray in this <paramref name="array"/>.
		/// </param>
		/// <returns>
		/// An <see cref="Array"/> of <typeparamref name="T"/> that is equivalent to the subarray that begins at <paramref
		/// name="startIndex"/> in this <paramref name="array"/>, or <c>null</c> if <paramref name="startIndex"/> is equal
		/// to the length of this <paramref name="array"/>.
		/// </returns>
		public static T[] Subarray<T>(this T[] array, int startIndex)
		{
			if (array == null || startIndex >= array.Length) return null;
			if (startIndex <= 0) return array;
			var newLength = array.Length - startIndex;
			var range = new T[newLength];
			Buffer.BlockCopy(array, startIndex, range, 0, newLength);
			return range;
		}
	}
}

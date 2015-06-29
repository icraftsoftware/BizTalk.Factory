#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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

namespace Be.Stateless.Linq.Extensions
{
	public static class EnumerableExtensions
	{
		public static void Each<TSource>(this IEnumerable<TSource> enumerable, Action<TSource> action)
		{
			foreach (var source in enumerable)
			{
				action(source);
			}
		}

		public static void Each<TSource>(this IEnumerable<TSource> enumerable, Action<int, TSource> action)
		{
			var index = 0;
			foreach (var source in enumerable)
			{
				action(index++, source);
			}
		}

		/// <seealso href="http://brendan.enrick.com/blog/linq-your-collections-with-iequalitycomparer-and-lambda-expressions/"/>
		public static IEnumerable<TSource> Except<TSource>(
			this IEnumerable<TSource> first,
			IEnumerable<TSource> second,
			Func<TSource, TSource, bool> comparer)
		{
			return first.Except(second, new LambdaComparer<TSource>(comparer));
		}

		public static IEnumerable<TSource> Distinct<TSource>(
			this IEnumerable<TSource> enumerable,
			Func<TSource, TSource, bool> comparer)
		{
			return enumerable.Distinct(new LambdaComparer<TSource>(comparer));
		}
	}
}
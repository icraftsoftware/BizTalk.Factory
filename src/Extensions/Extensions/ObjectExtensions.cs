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

using System;
using System.Linq;

namespace Be.Stateless.Extensions
{
	public static class ObjectExtensions
	{
		/// <summary>
		/// Performs a <see cref="Action{T}"/> delegate on the <paramref name="object"/> if it is not null.
		/// </summary>
		/// <typeparam name="T">
		/// The type of <paramref name="object"/>.
		/// </typeparam>
		/// <param name="object">
		/// The object to test and to pass as argument to the <paramref name="action"/> delegate.
		/// </param>
		/// <param name="action">
		/// The <see cref="Action{T}"/> delegate to perform.
		/// </param>
		public static void IfNotNull<T>(this T @object, Action<T> action) where T : class
		{
			if (@object != null) action(@object);
		}

		/// <summary>
		/// Performs a <see cref="Func{TResult}"/> delegate on the <paramref name="object"/> if it is not null and returns
		/// the <paramref name="function"/> delegate value.
		/// </summary>
		/// <typeparam name="T">
		/// The type of <paramref name="object"/>.
		/// </typeparam>
		/// <typeparam name="TR">
		/// The return type of the <see cref="Func{TResult}"/> delegate.
		/// </typeparam>
		/// <param name="object">
		/// The object to test and to pass as argument to the <paramref name="function"/> delegate.
		/// </param>
		/// <param name="function">
		/// The <see cref="Func{TResult}"/> delegate to perform.
		/// </param>
		/// <returns>
		/// The result of the <paramref name="function"/> delegate, or <c>default(TR)</c> if the <paramref name="object"/>
		/// is null.
		/// </returns>
		public static TR IfNotNull<T, TR>(this T @object, Func<T, TR> function) where T : class
		{
			return @object == null ? default(TR) : function(@object);
		}

		/// <summary>
		/// Returns whether a given <paramref name="value"/> is in the list of <paramref name="values"/>.
		/// </summary>
		/// <param name="value">
		/// The value being searched for.
		/// </param>
		/// <param name="values">
		/// The array of values being searched into.
		/// </param>
		/// <returns>
		/// <c>true</c> if the <paramref name="values"/> contains <paramref name="value"/>; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsOneOf<T>(this T value, params T[] values)
		{
			return values.Any(any => Equals(any, value));
		}
	}
}

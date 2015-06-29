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

namespace Be.Stateless.Extensions
{
	public static class ArrayExtensions
	{
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

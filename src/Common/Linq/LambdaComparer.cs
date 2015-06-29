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

namespace Be.Stateless.Linq
{
	/// <summary>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <see href="http://brendan.enrick.com/blog/linq-your-collections-with-iequalitycomparer-and-lambda-expressions/"/>
	public class LambdaComparer<T> : IEqualityComparer<T>
	{
		public LambdaComparer(Func<T, T, bool> comparer)
			: this(comparer, o => 0)
		{
		}

		public LambdaComparer(Func<T, T, bool> comparer, Func<T, int> hasher)
		{
			if (comparer == null) throw new ArgumentNullException("comparer");
			if (hasher == null) throw new ArgumentNullException("hasher");
			_comparer = comparer;
			_hasher = hasher;
		}

		#region IEqualityComparer<T> Members

		public bool Equals(T x, T y)
		{
			return _comparer(x, y);
		}

		public int GetHashCode(T obj)
		{
			return _hasher(obj);
		}

		#endregion

		private readonly Func<T, T, bool> _comparer;
		private readonly Func<T, int> _hasher;
	}
}
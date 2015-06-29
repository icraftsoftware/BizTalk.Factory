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

using System;
using System.Collections;
using System.Collections.Generic;

namespace Be.Stateless.Xml.Builder
{
	// Current always references the last enumerated item, even after MoveNext has exhausted the enumerator
	internal static class ConservativeEnumerable
	{
		#region Nested type: ConservativeEnumerator

		private class ConservativeEnumerator<T> : IEnumerator<T>
		{
			public ConservativeEnumerator(IEnumerator<T> enumerator)
			{
				if (enumerator == null) throw new ArgumentNullException("enumerator");
				_enumerator = enumerator;
			}

			#region IEnumerator<T> Members

			public void Dispose()
			{
				_enumerator.Dispose();
			}

			public bool MoveNext()
			{
				if (!_enumerator.MoveNext()) return false;
				Current = _enumerator.Current;
				return true;
			}

			public void Reset()
			{
				_enumerator.Reset();
			}

			public T Current { get; private set; }

			object IEnumerator.Current
			{
				get { return Current; }
			}

			#endregion

			private readonly IEnumerator<T> _enumerator;
		}

		#endregion

		public static IEnumerator<T> GetConservativeEnumerator<T>(this IEnumerable<T> enumerable)
		{
			return new ConservativeEnumerator<T>(enumerable.GetEnumerator());
		}
	}
}

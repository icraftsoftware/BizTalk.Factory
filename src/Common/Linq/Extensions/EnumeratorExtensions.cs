﻿#region Copyright & License

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

using System.Collections;
using System.Collections.Generic;

namespace Be.Stateless.Linq.Extensions
{
	public static class EnumeratorExtensions
	{
		public static IEnumerable<T> Cast<T>(this IEnumerator enumerator)
		{
			while (enumerator.MoveNext())
			{
				yield return (T) enumerator.Current;
			}
		}
	}
}
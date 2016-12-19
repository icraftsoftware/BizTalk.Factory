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
using System.Collections;
using NUnit.Framework;

namespace Be.Stateless.Extensions
{
	[TestFixture]
	public class TypeExtensionsFixture
	{
		[Test]
		[TestCaseSource(typeof(TypeExtensionsFixture), "TypePairs")]
		public void IsSubclassOfGeneric(Type actual, Type baseType)
		{
			Assert.That(actual.IsSubclassOfOpenGenericType(baseType));
		}

		private static IEnumerable TypePairs
		{
			get
			{
				yield return new TestCaseData(typeof(Dummy), typeof(CompleteDummy<,>)).SetName("Dummy_vs_CompleteDummy");
				yield return new TestCaseData(typeof(Dummy), typeof(HalfDummy<>)).SetName("Dummy_vs_HalfDummy");
				yield return new TestCaseData(typeof(Dummy), typeof(NoDummy)).SetName("Dummy_vs_NoDummy");
				yield return new TestCaseData(typeof(Dummy), typeof(IHalfDummy<>)).SetName("Dummy_vs_IHalfDummy");
				yield return new TestCaseData(typeof(IHalfDummy<int>), typeof(IHalfDummy<>)).SetName("IHalfDummy_vs_IHalfDummy");
			}
		}

		private interface IHalfDummy<T> { }

		private class CompleteDummy<T1, T2> { }

		private class HalfDummy<T> : CompleteDummy<T, int>, IHalfDummy<T> { }

		private class NoDummy : HalfDummy<string> { }

		private class Dummy : NoDummy { }
	}
}

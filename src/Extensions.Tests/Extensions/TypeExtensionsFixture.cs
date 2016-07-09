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

using NUnit.Framework;

namespace Be.Stateless.Extensions
{
	[TestFixture]
	public class TypeExtensionsFixture
	{
		[Test]
		public void IsSubclassOfGeneric()
		{
			var dummy = new Dummy();
			Assert.That(dummy.GetType().IsSubclassOfOpenGeneric(typeof(CompleteDummy<,>)));
			Assert.That(dummy.GetType().IsSubclassOfOpenGeneric(typeof(HalfDummy<>)));
			Assert.That(dummy.GetType().IsSubclassOfOpenGeneric(typeof(NoDummy)));
		}

		private class CompleteDummy<T1, T2> { }

		private class HalfDummy<T> : CompleteDummy<T, int> { }

		private class NoDummy : HalfDummy<string> { }

		private class Dummy : NoDummy { }
	}
}

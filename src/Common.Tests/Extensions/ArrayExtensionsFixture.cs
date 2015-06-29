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

using NUnit.Framework;

namespace Be.Stateless.Extensions
{
	[TestFixture]
	public class ArrayExtensionsFixture
	{
		[Test]
		public void RangeOfNullArray()
		{
			byte[] array = null;
			// ReSharper disable ExpressionIsAlwaysNull
			Assert.That(array.Subarray(2), Is.Null);
			// ReSharper restore ExpressionIsAlwaysNull
		}

		[Test]
		public void RangeReturnsTailFromStartIndex()
		{
			var array = new byte[] { 1, 2, 3, 4, 5 };
			Assert.That(array.Subarray(2), Is.EqualTo(new byte[] { 3, 4, 5 }));
		}

		[Test]
		public void RangeStartIndexIsBelowLowerBound()
		{
			var array = new byte[] { 1, 2, 3 };
			Assert.That(array.Subarray(-7), Is.EqualTo(array));
		}

		[Test]
		public void RangeStartIndexIsBeyondUpperBound()
		{
			var array = new byte[] { 1, 2, 3 };
			Assert.That(array.Subarray(9), Is.Null);
		}
	}
}

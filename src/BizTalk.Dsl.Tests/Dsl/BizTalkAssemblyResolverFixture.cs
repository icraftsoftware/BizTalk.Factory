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

using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl
{
	[TestFixture]
	public class BizTalkAssemblyResolverFixture
	{
		[Test]
		public void RefineEmptyProbingPath()
		{
			Assert.That(
				BizTalkAssemblyResolver.RefineProbingPaths(string.Empty),
				Is.EqualTo(new string[] { }));
		}

		[Test]
		public void RefineEmptyProbingPathArray()
		{
			Assert.That(
				BizTalkAssemblyResolver.RefineProbingPaths(new[] { string.Empty }),
				Is.EqualTo(new string[] { }));
		}

		[Test]
		public void RefineJoinedProbingPaths()
		{
			Assert.That(
				BizTalkAssemblyResolver.RefineProbingPaths(@"c:\folder\one;c:\folder\two", @"c:\folder\six; ;;c:\folder\ten;"),
				Is.EqualTo(new[] { @"c:\folder\one", @"c:\folder\two", @"c:\folder\six", @"c:\folder\ten" }));
		}

		[Test]
		public void RefineNullProbingPath()
		{
			Assert.That(
				BizTalkAssemblyResolver.RefineProbingPaths(null),
				Is.EqualTo(new string[] { }));
		}

		[Test]
		public void RefineNullProbingPathArray()
		{
			Assert.That(
				BizTalkAssemblyResolver.RefineProbingPaths(new string[] { null }),
				Is.EqualTo(new string[] { }));
		}

		[Test]
		public void RefineProbingPath()
		{
			Assert.That(
				BizTalkAssemblyResolver.RefineProbingPaths(@"c:\folder\one"),
				Is.EqualTo(new[] { @"c:\folder\one" }));
		}

		[Test]
		public void RefineProbingPathArray()
		{
			var probingPaths = new[] { @"c:\folder\one\file.dll", @"c:\folder\two\file.dll" }.Select(Path.GetDirectoryName).ToArray();
			Assert.That(
				BizTalkAssemblyResolver.RefineProbingPaths(probingPaths),
				Is.EqualTo(new[] { @"c:\folder\one", @"c:\folder\two" }));
		}
	}
}

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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	[TestFixture]
	public class MapCustomXsltPathResolverFixture
	{
		#region Setup/Teardown

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_projectFolder = ComputeProjectFolder();
		}

		#endregion

		[Test]
		public void TryResolveBtmClassSourceFilePath()
		{
			string path;
			Assert.That(typeof(IdentityTransform).TryResolveBtmClassSourceFilePath(out path));
			Assert.That(path, Is.EqualTo(Path.Combine(_projectFolder, @"BizTalk.Unit\Unit\Transform\IdentityTransform.cs")).IgnoreCase);
		}

		[Test]
		public void TryResolveCustomXsltPath()
		{
			string path;
			Assert.That(typeof(IdentityTransform).TryResolveCustomXsltPath(out path));
			Assert.That(path, Is.EqualTo(Path.Combine(_projectFolder, @"BizTalk.Unit\Unit\Transform\IdentityTransform.xslt")).IgnoreCase);
		}

		[Test]
		public void TryResolveEmbeddedXsltResourceSourceFilePath()
		{
			string path;
			Assert.That(typeof(IdentityTransform).TryResolveEmbeddedXsltResourceSourceFilePath("IdentityTransform.xslt", out path));
			Assert.That(path, Is.EqualTo(Path.Combine(_projectFolder, @"BizTalk.Unit\Unit\Transform\IdentityTransform.xslt")).IgnoreCase);
		}

		private string _projectFolder;

		[SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
		private static string ComputeProjectFolder([CallerFilePath] string sourceFilePath = "")
		{
			return sourceFilePath.Substring(0, sourceFilePath.IndexOf(@"\BizTalk.Unit.Tests"));
		}
	}
}

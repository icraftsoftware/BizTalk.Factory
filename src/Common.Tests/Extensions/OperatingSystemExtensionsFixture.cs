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
using NUnit.Framework;

namespace Be.Stateless.Extensions
{
	[TestFixture]
	public class OperatingSystemExtensionsFixture
	{
		[Test]
		public void TransactionalFileSystemSupported()
		{
			Assert.That(Windows7.SupportTransactionalFileSystem());
		}

		[Test]
		public void TransactionalFileSystemUnsupported()
		{
			Assert.That(WindowsXP.SupportTransactionalFileSystem(), Is.False);
		}

		public static readonly OperatingSystem Windows7 = new OperatingSystem(PlatformID.Win32NT, new Version("6.1.7601.65536"));
		public static readonly OperatingSystem WindowsXP = new OperatingSystem(PlatformID.Win32NT, new Version("5.2.7601.65536"));
	}
}

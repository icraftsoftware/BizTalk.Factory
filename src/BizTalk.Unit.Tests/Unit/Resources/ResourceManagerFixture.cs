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

using System.IO;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Resources
{
	[TestFixture]
	public class ResourceManagerFixture
	{
		[Test]
		public void LoadNestedResourceWithCompositeName()
		{
			Assert.That(ResourceManager.LoadString("Data.Resource.Composite.Name.txt"), Is.EqualTo("This is a nested resource with a composite name."));
		}

		[Test]
		public void LoadNestedResourceWithSimpleName()
		{
			Assert.That(ResourceManager.LoadString("Data.Resource.txt"), Is.EqualTo("This is a nested resource with a simple name."));
		}

		[Test]
		public void LoadResourceThrowsWhenNotFound()
		{
			Assert.That(
				() => ResourceManager.LoadString("Nonexistent.txt"),
				Throws.TypeOf<FileNotFoundException>()
					.With.Message.EqualTo(string.Format("Cannot find resource 'Nonexistent.txt' in assembly {0}.", typeof(ResourceManagerFixture).Assembly.FullName)));
		}

		[Test]
		public void LoadResourceWithAbsoluteName()
		{
			Assert.That(
				ResourceManager.LoadString("Be.Stateless.BizTalk.Unit.Transform.Data.Message.xml"),
				Does.StartWith(
					"<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
						"<ns0:services ns1:mustUnderstand='1' xmlns:ns1='http://schemas.xmlsoap.org/soap/envelope/' xmlns:ns0='http://schemas.biztalk.org/btf-2-0/services'>"));
		}

		[Test]
		public void LoadResourceWithCompositeName()
		{
			Assert.That(ResourceManager.LoadString("Resource.Composite.Name.txt"), Is.EqualTo("This is a resource with a composite name."));
		}

		[Test]
		public void LoadResourceWithSimpleName()
		{
			Assert.That(ResourceManager.LoadString("Resource.txt"), Is.EqualTo("This is a resource with a simple name."));
		}
	}
}

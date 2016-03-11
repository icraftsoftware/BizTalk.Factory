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

using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class Pop3AdapterFixture
	{
		[Test]
		public void ProtocolTypeSettingsAreReadFromRegistry()
		{
			var mock = new Mock<Pop3Adapter> { CallBase = true };
			var fa = mock.Object as IAdapter;
			Assert.That(fa.ProtocolType.Name, Is.EqualTo("POP3"));
			Assert.That(fa.ProtocolType.Capabilities, Is.EqualTo(71689));
			Assert.That(fa.ProtocolType.ConfigurationClsid, Is.EqualTo("1787fcc1-9aaa-4bbd-9096-7eb77e3d9d9b"));
		}
	}
}

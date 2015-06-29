#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
	public class FileAdapterFixture
	{
		[Test]
		public void FileTransportSettingsAreReadFromRegistry()
		{
			var mock = new Mock<FileAdapter> { CallBase = true };
			var ft = mock.Object as IAdapter;
			Assert.That(ft.ProtocolType.Name, Is.EqualTo("FILE"));
			Assert.That(ft.ProtocolType.Capabilities, Is.EqualTo(11));
			Assert.That(ft.ProtocolType.ConfigurationClsid, Is.EqualTo("5e49e3a6-b4fc-4077-b44c-22f34a242fdb"));
		}
	}
}

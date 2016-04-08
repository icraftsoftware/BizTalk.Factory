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

using Microsoft.BizTalk.Adapter.SBMessaging;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class SBMessagingAdapterFixture
	{
		[Test]
		public void ProtocolTypeSettingsAreReadFromRegistry()
		{
			var mock = new Mock<SBMessagingAdapter<SBMessagingRLConfig>> { CallBase = true };
			var fa = mock.Object as IAdapter;
			Assert.That(fa.ProtocolType.Name, Is.EqualTo("SB-Messaging"));
			Assert.That(fa.ProtocolType.Capabilities, Is.EqualTo(523));
			Assert.That(fa.ProtocolType.ConfigurationClsid, Is.EqualTo("9c458d4a-a73c-4cb3-89c4-86ae0103de2f"));
		}
	}
}

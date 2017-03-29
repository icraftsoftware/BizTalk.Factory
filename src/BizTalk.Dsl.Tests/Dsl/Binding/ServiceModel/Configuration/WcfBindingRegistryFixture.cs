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

using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration
{
	[TestFixture]
	public class WcfBindingRegistryFixture
	{
		[Test]
		public void GetBindingNameForDecoratedNetMsmqBindingElement()
		{
			var bindingElement = new NetMsmqBindingElement();
			Assert.That(WcfBindingRegistry.GetBindingName(bindingElement), Is.EqualTo("netMsmqBinding"));
		}

		[Test]
		public void GetBindingNameForStandardCustomBindingElement()
		{
			var bindingElement = new System.ServiceModel.Configuration.CustomBindingElement();
			Assert.That(WcfBindingRegistry.GetBindingName(bindingElement), Is.EqualTo("customBinding"));
		}

		[Test]
		public void GetBindingNameForStandardNetMsmqBindingElement()
		{
			var bindingElement = new System.ServiceModel.Configuration.NetMsmqBindingElement();
			Assert.That(WcfBindingRegistry.GetBindingName(bindingElement), Is.EqualTo("netMsmqBinding"));
		}
	}
}

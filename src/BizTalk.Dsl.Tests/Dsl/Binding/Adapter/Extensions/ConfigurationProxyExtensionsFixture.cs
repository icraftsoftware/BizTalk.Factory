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

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions
{
	[TestFixture]
	public class ConfigurationProxyExtensionsFixture
	{
		[Test]
		public void GetBindingElementXmlForDecoratedBindingElement()
		{
			var binding = new ServiceModel.Configuration.NetMsmqBindingElement();
			Assert.That(() => binding.GetBindingElementXml("netMsmqBinding"), Throws.Nothing);
		}

		[Test]
		public void GetBindingElementXmlForUndecoratedBindingElement()
		{
			var binding = new System.ServiceModel.Configuration.NetMsmqBindingElement();
			Assert.That(() => binding.GetBindingElementXml("netMsmqBinding"), Throws.Nothing);
		}
	}
}

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

using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration
{
	[TestFixture]
	public class NetMsmqBindingElementFixture
	{
		[Test]
		public void NetMsmqBindingElementDecoratorCanBeUsedAsWcfCustomAdapterTypeParameter()
		{
			Assert.That(
				() => new WcfCustomAdapter.Inbound<NetMsmqBindingElement>(a => { a.Binding.ExactlyOnce = true; }),
				Throws.Nothing);
		}

		[Test]
		public void SerializeToXml()
		{
			var binding = new NetMsmqBindingElement { ExactlyOnce = false };
			Assert.That(
				binding.GetBindingElementXml("netMsmqBinding"),
				Is.EqualTo("<binding name=\"netMsmqBinding\" exactlyOnce=\"false\" retryCycleDelay=\"00:10:00\" />"));
		}
	}
}

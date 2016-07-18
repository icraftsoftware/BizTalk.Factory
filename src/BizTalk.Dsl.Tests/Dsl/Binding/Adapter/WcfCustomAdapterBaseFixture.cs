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

using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using Be.Stateless.Reflection;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfCustomAdapterBaseFixture
	{
		[Test]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<NetMsmqBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides("ACC");

			adapterMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<NetMsmqBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides(string.Empty);

			adapterMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToBindingElement()
		{
			var bindingMock = new Mock<NetMsmqBindingElement> { CallBase = true };
			var environmentSensitiveBindingMock = bindingMock.As<ISupportEnvironmentOverride>();

			var adapterMock = new Mock<WcfCustomAdapterBase<NetMsmqBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };
			Reflector.SetField(adapterMock.Object, "_bindingConfigurationElement", bindingMock.Object);

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides("ACC");

			environmentSensitiveBindingMock.Verify(b => b.ApplyEnvironmentOverrides("ACC"), Times.Once());
		}
	}
}

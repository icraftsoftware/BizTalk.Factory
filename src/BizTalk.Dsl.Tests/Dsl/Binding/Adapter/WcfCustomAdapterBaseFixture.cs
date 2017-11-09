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

using System;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Be.Stateless.Reflection;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using NetMsmqBindingElement = Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration.NetMsmqBindingElement;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfCustomAdapterBaseFixture
	{
		[Test]
		public void BasicHttpBindingElementIsSupported()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };
			Assert.That(() => adapterMock.Object, Throws.Nothing);
		}

		[Test]
		public void BasicHttpsBindingElementIsNotSupported()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpsBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };
			Assert.That(
				() => adapterMock.Object,
				Throws.TypeOf<TypeInitializationException>()
					.With.InnerException.TypeOf<BindingException>()
					.And.InnerException.Message.EqualTo("BasicHttpBindingElement has to be used for https addresses as well."));
		}

		[Test]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, NetMsmqBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides("ACC");

			adapterMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, NetMsmqBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides(string.Empty);

			adapterMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToBindingElement()
		{
			var bindingMock = new Mock<NetMsmqBindingElement> { CallBase = true };
			var environmentSensitiveBindingMock = bindingMock.As<ISupportEnvironmentOverride>();

			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, NetMsmqBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };
			Reflector.SetField(adapterMock.Object, "_bindingConfigurationElement", bindingMock.Object);

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides("ACC");

			environmentSensitiveBindingMock.Verify(b => b.ApplyEnvironmentOverrides("ACC"), Times.Once());
		}

		[Test]
		public void ValidateCustomBasicHttpBindingWithoutTransportSecurityThrowsWhenSchemeIsHttps()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomTLConfig>>(new ProtocolType()) { CallBase = true };
			adapterMock.Object.Address = new EndpointAddress("https://services.stateless.be/soap/default");
			adapterMock.Object.Binding.Security.Mode = BasicHttpSecurityMode.None;
			Assert.That(
				() => ((ISupportValidation) adapterMock.Object).Validate(),
				Throws.TypeOf<ArgumentException>().With.InnerException.Message.EqualTo("Invalid address scheme; expecting \"http\" scheme."));
		}

		[Test]
		public void ValidateCustomBasicHttpBindingWithTransportSecurityDoesNotThrowWhenSchemeIsHttps()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomTLConfig>>(new ProtocolType()) { CallBase = true };
			adapterMock.Object.Address = new EndpointAddress("https://services.stateless.be/soap/default");
			adapterMock.Object.Binding.Security.Mode = BasicHttpSecurityMode.Transport;
			Assert.That(() => ((ISupportValidation) adapterMock.Object).Validate(), Throws.Nothing);
		}

		[Test]
		public void ValidateCustomBasicHttpBindingWithTransportSecurityThrowsWhenSchemeIsHttp()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomTLConfig>>(new ProtocolType()) { CallBase = true };
			adapterMock.Object.Address = new EndpointAddress("http://services.stateless.be/soap/default");
			adapterMock.Object.Binding.Security.Mode = BasicHttpSecurityMode.Transport;
			Assert.That(
				() => ((ISupportValidation) adapterMock.Object).Validate(),
				Throws.TypeOf<ArgumentException>().With.InnerException.Message.EqualTo("Invalid address scheme; expecting \"https\" scheme."));
		}
	}
}

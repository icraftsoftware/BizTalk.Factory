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

using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Microsoft.Adapters.OracleDB;
using Microsoft.Adapters.Sql;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory
{
	[TestFixture]
	public class NamingConventionFixture
	{
		[Test]
		public void WcfCustomAdapterNameContainsActualProtocolTypeName()
		{
			IAdapter adapter = new CustomAdapterFake<NetTcpBindingElement, CustomRLConfig>();
			Assert.That(ConventionDummy.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomNetTcp"));

			adapter = new CustomAdapterFake<NetMsmqBindingElement, CustomRLConfig>();
			Assert.That(ConventionDummy.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomNetMsmq"));

			adapter = new CustomAdapterFake<OracleDBBindingConfigurationElement, CustomRLConfig>();
			Assert.That(ConventionDummy.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomOracleDB"));

			adapter = new CustomAdapterFake<SqlAdapterBindingConfigurationElement, CustomRLConfig>();
			Assert.That(ConventionDummy.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomSql"));
		}

		[Test]
		public void WcfCustomIsolatedAdapterNameContainsActualProtocolTypeName()
		{
			IAdapter adapter = new CustomIsolatedAdapterFake<NetTcpBindingElement, CustomRLConfig>();
			Assert.That(ConventionDummy.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomIsolatedNetTcp"));

			adapter = new CustomIsolatedAdapterFake<WSHttpBindingElement, CustomRLConfig>();
			Assert.That(ConventionDummy.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomIsolatedWsHttp"));

			adapter = new CustomIsolatedAdapterFake<BasicHttpBindingElement, CustomRLConfig>();
			Assert.That(ConventionDummy.Instance.ComputeAdapterName(adapter), Is.EqualTo("WCF-CustomIsolatedBasicHttp"));
		}

		private class ConventionDummy : NamingConvention<string, string>
		{
			public new string ComputeAdapterName(IAdapter adapter)
			{
				return base.ComputeAdapterName(adapter);
			}

			internal static readonly ConventionDummy Instance = new ConventionDummy();
		}

		private class CustomAdapterFake<TBinding, TConfig> : WcfCustomAdapter<TBinding, TConfig>
			where TBinding : StandardBindingElement, new()
			where TConfig : AdapterConfig,
				IAdapterConfigAddress,
				IAdapterConfigIdentity,
				IAdapterConfigBinding,
				IAdapterConfigEndpointBehavior,
				IAdapterConfigInboundMessageMarshalling,
				IAdapterConfigOutboundMessageMarshalling,
				new() { }

		private class CustomIsolatedAdapterFake<TBinding, TConfig> : WcfCustomIsolatedAdapter<TBinding, TConfig>
			where TBinding : StandardBindingElement, new()
			where TConfig : RLConfig,
				IAdapterConfigBinding,
				IAdapterConfigEndpointBehavior,
				IAdapterConfigInboundMessageMarshalling,
				IAdapterConfigOutboundMessageMarshalling,
				new() { }
	}
}

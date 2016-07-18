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

using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public abstract class WcfCustomAdapterBase<TBinding, TConfig> : WcfTwoWayExtensibleAdapterBase<EndpointAddress, TBinding, TConfig>
		where TBinding : StandardBindingElement, new()
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			IAdapterConfigIdentity,
			IAdapterConfigBinding,
			IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new()
	{
		protected WcfCustomAdapterBase(ProtocolType protocolType) : base(protocolType) { }

		#region Base Class Member Overrides

		protected override void ApplyEnvironmentOverrides(string environment)
		{
			// ReSharper disable once SuspiciousTypeConversion.Global
			(Binding as ISupportEnvironmentOverride).IfNotNull(b => b.ApplyEnvironmentOverrides(environment));
		}

		#endregion

		public TBinding Binding
		{
			get { return _bindingConfigurationElement; }
		}
	}
}

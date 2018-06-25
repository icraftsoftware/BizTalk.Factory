#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public abstract class WcfCustomAdapterBase<TAddress, TBinding, TConfig> : WcfBindingCentricAdapterBase<TAddress, TBinding, TConfig>, IAdapterConfigBinding<TBinding>
		where TBinding : StandardBindingElement, new()
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
			IAdapterConfigBinding,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new()
	{
		static WcfCustomAdapterBase()
		{
			// BizTalk Server does only depend on BasicHttpBinding even for https
			if (typeof(TBinding) == typeof(BasicHttpsBindingElement))
				throw new BindingException(
					string.Format(
						"{0} has to be used for https addresses as well.",
						typeof(BasicHttpBindingElement).Name));
		}

		protected WcfCustomAdapterBase(ProtocolType protocolType) : base(protocolType) { }

		#region IAdapterConfigBinding<TBinding> Members

		public TBinding Binding
		{
			get { return _bindingConfigurationElement; }
		}

		#endregion

		#region Base Class Member Overrides

		protected override void Validate()
		{
			// ensure binding is configured before validation
			_adapterConfig.BindingConfiguration = _bindingConfigurationElement.GetBindingElementXml(_bindingConfigurationElement.Name);
			base.Validate();
		}

		#endregion

		#region Base Class Member Overrides

		protected override void ApplyEnvironmentOverrides(string environment)
		{
			// ReSharper disable once SuspiciousTypeConversion.Global
			(Binding as ISupportEnvironmentOverride).IfNotNull(b => b.ApplyEnvironmentOverrides(environment));
		}

		#endregion
	}
}

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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public abstract class WcfTwoWayExtensibleAdapterBase<TAddress, TBinding, TConfig> : WcfTwoWayAdapterBase<TAddress, TBinding, TConfig>, IAdapterConfigEndpointBehavior
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
		protected WcfTwoWayExtensibleAdapterBase(ProtocolType protocolType) : base(protocolType)
		{
			_adapterConfig.BindingType = _bindingConfigurationElement.Name = WcfBindingRegistry.GetBindingName(_bindingConfigurationElement);
			EndpointBehaviors = Enumerable.Empty<BehaviorExtensionElement>();
		}

		#region IAdapterConfigEndpointBehavior Members

		public IEnumerable<BehaviorExtensionElement> EndpointBehaviors { get; set; }

		#endregion

		#region Base Class Member Overrides

		protected override void Save(IPropertyBag propertyBag)
		{
			_adapterConfig.BindingConfiguration = _bindingConfigurationElement.GetBindingElementXml(_bindingConfigurationElement.Name);
			_adapterConfig.EndpointBehaviorConfiguration = EndpointBehaviors.GetEndpointBehaviorElementXml();
			base.Save(propertyBag);
		}

		#endregion
	}
}

#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using System.Linq;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Adapter.Wcf.Converters;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfCustomAdapterBase<TAddress, TBinding, TConfig> : WcfAdapterBase<TAddress, TBinding, TConfig>
		where TBinding : StandardBindingElement,
			new()
		where TConfig : AdapterConfig,
			IAdapterConfigIdentity,
			IAdapterConfigBinding,
			IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new()
	{
		protected WcfCustomAdapterBase(ProtocolType protocolType, string bindingType) : base(protocolType, bindingType)
		{
			_configurationProxy = new ConfigurationProxy();
			_adapterConfig.BindingType = bindingType;
		}

		#region Base Class Member Overrides

		protected override void Save(IPropertyBag propertyBag)
		{
			_adapterConfig.BindingConfiguration = GetBindingElementXml();
			_adapterConfig.EndpointBehaviorConfiguration = GetEndpointBehaviorElementXml(EndpointBehaviors);
			base.Save(propertyBag);
		}

		#endregion

		#region Behavior Tab - EndpointBehavior Settings

		public IEnumerable<IEndpointBehavior> EndpointBehaviors { get; set; }

		#endregion

		private string GetBindingElementXml()
		{
			_configurationProxy.SetBindingElement(_bindingConfigurationElement);
			return _configurationProxy.GetBindingElementXml(_bindingConfigurationElement.Name);
		}

		protected string GetEndpointBehaviorElementXml(IEnumerable<IEndpointBehavior> endpointBehaviors)
		{
			var endpointBehaviorElement = new EndpointBehaviorElement("EndpointBehavior");
			endpointBehaviors.Cast<BehaviorExtensionElement>().Each(b => endpointBehaviorElement.Add(b));
			_configurationProxy.SetEndpointBehaviorElement(endpointBehaviorElement);
			return _configurationProxy.GetEndpointBehaviorElementXml();
		}

		protected string GetServiceBehaviorElementXml(IEnumerable<IServiceBehavior> serviceBehaviors)
		{
			var serviceBehaviorElement = new ServiceBehaviorElement("ServiceBehavior");
			serviceBehaviors.Cast<BehaviorExtensionElement>().Each(b => serviceBehaviorElement.Add(b));
			_configurationProxy.SetServiceBehaviorElement(serviceBehaviorElement);
			return _configurationProxy.GetServiceBehaviorElementXml();
		}

		private readonly ConfigurationProxy _configurationProxy;
	}
}

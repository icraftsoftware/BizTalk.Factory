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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
	public abstract class WcfAdapterBase<TBinding, TConfig> : AdapterBase, IAdapter, IAdapterBindingSerializerFactory
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
		static WcfAdapterBase()
		{
			_configurationProxy = new ConfigurationProxy();
		}

		protected WcfAdapterBase(string bindingConfigurationElementName)
		{
			_bindingConfigurationElement = new TBinding { Name = bindingConfigurationElementName };
			_adapterConfig = new TConfig { BindingType = _bindingConfigurationElement.Name };
		}

		#region IAdapter Members

		string IAdapter.Address
		{
			get { return GetAddress().ToString(); }
		}

		ProtocolType IAdapter.ProtocolType
		{
			get { return GetProtocolType(); }
		}

		void IAdapter.Load(IPropertyBag propertyBag)
		{
			throw new NotSupportedException();
		}

		void IAdapter.Save(IPropertyBag propertyBag)
		{
			Save(propertyBag);
		}

		#endregion

		#region IAdapterBindingSerializerFactory Members

		public IDslSerializer GetAdapterBindingSerializer()
		{
			return new AdapterBindingSerializer(this);
		}

		#endregion

		protected abstract Uri GetAddress();

		protected abstract ProtocolType GetProtocolType();

		protected virtual void Save(IPropertyBag propertyBag)
		{
			_adapterConfig.BindingConfiguration = GetBindingConfiguration();
			_adapterConfig.Save(propertyBag as Microsoft.BizTalk.ExplorerOM.IPropertyBag);
		}

		private string GetBindingConfiguration()
		{
			_configurationProxy.SetBindingElement(_bindingConfigurationElement);
			return _configurationProxy.GetBindingElementXml(_bindingConfigurationElement.Name);
		}

		protected string GetEndpointBehaviorConfiguration(IEnumerable<IEndpointBehavior> endpointBehaviors)
		{
			var endpointBehaviorElement = new EndpointBehaviorElement("EndpointBehavior");
			endpointBehaviors.Cast<BehaviorExtensionElement>().Each(b => endpointBehaviorElement.Add(b));
			_configurationProxy.SetEndpointBehaviorElement(endpointBehaviorElement);
			return _configurationProxy.GetEndpointBehaviorElementXml();
		}

		protected string GetServiceBehaviorConfiguration(IEnumerable<IServiceBehavior> serviceBehaviors)
		{
			var cp = new ConfigurationProxy();
			var serviceBehaviorElement = new ServiceBehaviorElement("ServiceBehavior");
			serviceBehaviors.Cast<BehaviorExtensionElement>().Each(b => serviceBehaviorElement.Add(b));
			cp.SetServiceBehaviorElement(serviceBehaviorElement);
			return cp.GetServiceBehaviorElementXml();
		}

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ConfigurationProxy _configurationProxy;

		protected readonly TConfig _adapterConfig;
		protected readonly TBinding _bindingConfigurationElement;
	}
}

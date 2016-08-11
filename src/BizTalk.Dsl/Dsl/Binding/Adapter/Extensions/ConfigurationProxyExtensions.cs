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

using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.Adapter.Wcf.Converters;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions
{
	public static class ConfigurationProxyExtensions
	{
		public static string GetBindingElementXml(this ConfigurationElement configurationElement, string bindingName)
		{
			var bindingElementDecorator = configurationElement as IBindingElementDecorator;
			if (bindingElementDecorator != null) configurationElement = (ConfigurationElement) bindingElementDecorator.DecoratedBindingElement;

			var configurationProxy = new ConfigurationProxy();
			configurationProxy.SetBindingElement(configurationElement);
			return configurationProxy.GetBindingElementXml(bindingName);
		}

		public static string GetEndpointBehaviorElementXml(this IEnumerable<BehaviorExtensionElement> endpointBehaviors)
		{
			var endpointBehaviorElement = new EndpointBehaviorElement("EndpointBehavior");
			endpointBehaviors.Each(b => endpointBehaviorElement.Add(b));

			var configurationProxy = new ConfigurationProxy();
			configurationProxy.SetEndpointBehaviorElement(endpointBehaviorElement);
			return configurationProxy.GetEndpointBehaviorElementXml();
		}

		public static string GetServiceBehaviorElementXml(this IEnumerable<BehaviorExtensionElement> serviceBehaviors)
		{
			var serviceBehaviorElement = new ServiceBehaviorElement("ServiceBehavior");
			serviceBehaviors.Each(b => serviceBehaviorElement.Add(b));

			var configurationProxy = new ConfigurationProxy();
			configurationProxy.SetServiceBehaviorElement(serviceBehaviorElement);
			return configurationProxy.GetServiceBehaviorElementXml();
		}
	}
}

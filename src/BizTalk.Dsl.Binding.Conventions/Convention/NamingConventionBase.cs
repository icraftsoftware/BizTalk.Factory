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

using System;
using System.Linq;
using System.ServiceModel.Channels;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Extensions;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public abstract class NamingConventionBase<TNamingConvention, TParty, TMessageName> :
			IApplicationNameMemento<string>,
			IPartyMemento<TParty>,
			IMessageNameMemento<TMessageName>,
			IMessageFormatMemento<string>
		where TNamingConvention : NamingConventionBase<TNamingConvention, TParty, TMessageName>, INamingConvention<TNamingConvention>
	{
		#region IApplicationNameMemento<string> Members

		public string ApplicationName { get; set; }

		#endregion

		#region IMessageFormatMemento<string> Members

		public string MessageFormat { get; set; }

		#endregion

		#region IMessageNameMemento<TMessageName> Members

		public TMessageName MessageName { get; set; }

		#endregion

		#region IPartyMemento<TParty> Members

		public TParty Party { get; set; }

		#endregion

		protected string ComputeApplicationName(IApplicationBinding<TNamingConvention> application)
		{
			return ApplicationName.IsNullOrEmpty() ? application.GetType().Name : ApplicationName;
		}

		protected string ComputeReceivePortName(IReceivePort<TNamingConvention> receivePort)
		{
			if (Equals(Party, default(TParty))) throw new NamingConventionException("Party is required.");

			var area = ComputeArea(receivePort.GetType());
			return string.Format(
				"{0}{1}.RP{2}.{3}",
				((ISupportNamingConvention) receivePort.ApplicationBinding).Name,
				area.IsNullOrEmpty() ? string.Empty : string.Format(".{0}", area),
				receivePort.IsTwoWay ? "2" : "1",
				Party);
		}

		protected string ComputeReceiveLocationName(IReceiveLocation<TNamingConvention> receiveLocation)
		{
			if (Equals(Party, default(TParty))) Party = receiveLocation.ReceivePort.Name.Party;
			if (Equals(Party, default(TParty))) throw new NamingConventionException("Party is required.");
			if (!Equals(Party, receiveLocation.ReceivePort.Name.Party))
				throw new NamingConventionException(
					string.Format(
						"ReceiveLocation's Party '{0}' does not match its ReceivePort's one '{1}'.",
						Party,
						receiveLocation.ReceivePort.Name.Party));
			if (Equals(MessageName, default(TMessageName))) throw new NamingConventionException("MessageName is required.");
			if (MessageFormat == null) throw new NamingConventionException("A non null MessageFormat is required.");

			var area = ComputeArea(receiveLocation.GetType());
			var receivePortArea = ComputeArea(receiveLocation.ReceivePort.GetType());
			if (area.IsNullOrEmpty() && !receivePortArea.IsNullOrEmpty()) area = receivePortArea;
			if (receivePortArea != area)
				throw new NamingConventionException(
					string.Format(
						"ReceiveLocation's Area '{0}' does not match its ReceivePort's one '{1}'.",
						area,
						receivePortArea));

			return string.Format(
				"{0}{1}.RL{2}.{3}.{4}.{5}{6}",
				((ISupportNamingConvention) receiveLocation.ReceivePort.ApplicationBinding).Name,
				area.IsNullOrEmpty() ? string.Empty : string.Format(".{0}", area),
				receiveLocation.ReceivePort.IsTwoWay ? "2" : "1",
				Party,
				MessageName,
				ComputeAdapterName(receiveLocation.Transport.Adapter),
				MessageFormat.IsNullOrEmpty() ? string.Empty : string.Format(".{0}", MessageFormat));
		}

		protected string ComputeSendPortName(ISendPort<TNamingConvention> sendPort)
		{
			if (Equals(Party, default(TParty))) throw new NamingConventionException("Party is required.");
			if (Equals(MessageName, default(TMessageName))) throw new NamingConventionException("MessageName is required.");
			if (MessageFormat == null) throw new NamingConventionException("A non null MessageFormat is required.");

			var area = ComputeArea(sendPort.GetType());
			return string.Format(
				"{0}{1}.SP{2}.{3}.{4}.{5}{6}",
				((ISupportNamingConvention) sendPort.ApplicationBinding).Name,
				area.IsNullOrEmpty() ? string.Empty : string.Format(".{0}", area),
				sendPort.IsTwoWay ? "2" : "1",
				Party,
				MessageName,
				ComputeAdapterName(sendPort.Transport.Adapter),
				MessageFormat.IsNullOrEmpty() ? string.Empty : string.Format(".{0}", MessageFormat));
		}

		protected virtual string ComputeAdapterName(IAdapter adapter)
		{
			var name = adapter.ProtocolType.Name;
			if (adapter.GetType().IsSubclassOfOpenGenericType(typeof(WcfCustomAdapterBase<,>)))
			{
				// cast to dynamic in order to access Binding property which is declared by WcfCustomAdapterBase<,>
				dynamic dynamicAdapter = adapter;
				var binding = dynamicAdapter.Binding;

				var customBindingElement = binding as IBindingElementDecorator;
				if (customBindingElement != null)
				{
					var actualBindingElement = (System.ServiceModel.Configuration.CustomBindingElement) customBindingElement.DecoratedBindingElement;
					var transportElementType = actualBindingElement
						.Select(be => be.BindingElementType)
						.Single(bet => typeof(TransportBindingElement).IsAssignableFrom(bet));
					return typeof(Microsoft.ServiceModel.Channels.Common.Adapter).IsAssignableFrom(transportElementType)
						? name + transportElementType.Name
							.TrimSuffix(typeof(Microsoft.ServiceModel.Channels.Common.Adapter).Name)
							.Capitalize()
						: name + transportElementType.Name
							.TrimSuffix(typeof(TransportBindingElement).Name)
							.Capitalize();
				}

				var bindingName = ((string) binding.Name)
					.TrimSuffix("Binding")
					.Capitalize();
				return name + bindingName;
			}
			return name;
		}

		protected virtual string ComputeArea(Type type)
		{
			if (type.IsGenericType) type = type.GetGenericTypeDefinition();
			var tokens = type.FullName.Split('.');
			return tokens.Length == 5 ? tokens[3] : null;
		}
	}
}

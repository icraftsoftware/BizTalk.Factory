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
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory
{
	public class NamingConvention<TParty, TMessageName> :
		INamingConvention<NamingConvention<TParty, TMessageName>>,
		IApplicationNameMemento<string>,
		IPartyMemento<TParty>,
		IMessageNameMemento<TMessageName>,
		IMessageFormatMemento<string>
	{
		#region Operators

		public static implicit operator string(NamingConvention<TParty, TMessageName> convention)
		{
			throw new NotSupportedException("In order to support Be.Stateless.BizTalk.Dsl.Binding.Subscription.FilterTranslator.");
		}

		#endregion

		#region IApplicationNameMemento<string> Members

		public string ApplicationName { get; set; }

		#endregion

		#region IMessageFormatMemento<string> Members

		public string MessageFormat { get; set; }

		#endregion

		#region IMessageNameMemento<TMessageName> Members

		TMessageName IMessageNameMemento<TMessageName>.MessageName
		{
			get { return MessageName; }
			set { MessageName = value; }
		}

		#endregion

		#region INamingConvention<NamingConvention<TParty,TMessageName>> Members

		public string ComputeApplicationName(IApplicationBinding<NamingConvention<TParty, TMessageName>> application)
		{
			return ApplicationName.IsNullOrEmpty() ? application.GetType().Name : ApplicationName;
		}

		public string ComputeReceivePortName(IReceivePort<NamingConvention<TParty, TMessageName>> receivePort)
		{
			if (Area == null) Area = ComputeArea(receivePort.GetType());
			if (Equals(Party, default(TParty))) throw new NamingConventionException("Party is required.");

			return string.Format(
				"{0}{1}.RP{2}.{3}",
				((ISupportNamingConvention) receivePort.ApplicationBinding).Name,
				Area.IsNullOrEmpty() ? string.Empty : string.Format(".{0}", Area),
				receivePort.IsTwoWay ? "2" : "1",
				Party);
		}

		public string ComputeReceiveLocationName(IReceiveLocation<NamingConvention<TParty, TMessageName>> receiveLocation)
		{
			if (Area == null) Area = receiveLocation.ReceivePort.Name.Area;
			if (Area == null) Area = ComputeArea(receiveLocation.GetType());
			if (Area != receiveLocation.ReceivePort.Name.Area) throw new NamingConventionException("ReceiveLocation's Area does not match its ReceivePort's one.");
			if (Equals(Party, default(TParty))) Party = receiveLocation.ReceivePort.Name.Party;
			if (Equals(Party, default(TParty))) throw new NamingConventionException("Party is required.");
			if (!Equals(Party, receiveLocation.ReceivePort.Name.Party)) throw new NamingConventionException("ReceiveLocation's Party does not match its ReceivePort's one.");
			if (Equals(MessageName, default(TMessageName))) throw new NamingConventionException("MessageName is required.");
			if (MessageFormat == null) throw new NamingConventionException("MessageFormat is required.");

			return string.Format(
				"{0}{1}.RL{2}.{3}.{4}.{5}{6}",
				((ISupportNamingConvention) receiveLocation.ReceivePort.ApplicationBinding).Name,
				Area.IsNullOrEmpty() ? string.Empty : string.Format(".{0}", Area),
				receiveLocation.ReceivePort.IsTwoWay ? "2" : "1",
				Party,
				MessageName,
				ComputeAdapterName(receiveLocation.Transport.Adapter),
				MessageFormat.IsNullOrEmpty() ? string.Empty : string.Format(".{0}", MessageFormat));
		}

		public string ComputeSendPortName(ISendPort<NamingConvention<TParty, TMessageName>> sendPort)
		{
			if (Area == null) Area = ComputeArea(sendPort.GetType());
			if (Equals(Party, default(TParty))) throw new NamingConventionException("Party is required.");
			if (MessageName.Equals(default(TMessageName))) throw new NamingConventionException("MessageName is required.");
			if (MessageFormat == null) throw new NamingConventionException("MessageFormat is required.");

			return string.Format(
				"{0}{1}.SP{2}.{3}.{4}.{5}{6}",
				((ISupportNamingConvention) sendPort.ApplicationBinding).Name,
				Area.IsNullOrEmpty() ? string.Empty : string.Format(".{0}", Area),
				sendPort.IsTwoWay ? "2" : "1",
				Party,
				MessageName,
				ComputeAdapterName(sendPort.Transport.Adapter),
				MessageFormat.IsNullOrEmpty() ? string.Empty : string.Format(".{0}", MessageFormat));
		}

		#endregion

		#region IPartyMemento<TParty> Members

		TParty IPartyMemento<TParty>.Party
		{
			get { return Party; }
			set { Party = value; }
		}

		#endregion

		private string Area { get; set; }

		private TMessageName MessageName { get; set; }

		private TParty Party { get; set; }

		protected virtual string ComputeAdapterName(IAdapter adapter)
		{
			var name = adapter.ProtocolType.Name;
			if (adapter.GetType().IsSubclassOfOpenGenericType(typeof(WcfCustomAdapterBase<,>)))
			{
				const string bindingSuffix = "Binding";
				// to be able able to access Binding property which is declared by WcfCustomAdapterBase<,>
				dynamic dynamicAdapter = adapter;
				var bindingName = dynamicAdapter.Binding.Name;
				if (bindingName.EndsWith(bindingSuffix)) bindingName = bindingName.Substring(0, bindingName.Length - bindingSuffix.Length);
				bindingName = char.ToUpper(bindingName[0]) + bindingName.Substring(1);
				return name + bindingName;
			}
			return name;
		}

		protected virtual string ComputeArea(Type type)
		{
			if (type.IsGenericType) type = type.GetGenericTypeDefinition();
			var tokens = type.FullName.Split('.');
			return tokens.Length == 5 ? tokens[3] : string.Empty;
		}
	}
}

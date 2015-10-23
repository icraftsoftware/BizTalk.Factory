﻿#region Copyright & License

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
using System.Linq;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfLobAdapterBase<TAddress, TBinding, TConfig> : WcfStandardAdapterBase<TAddress, TBinding, TConfig>
		where TBinding : StandardBindingElement,
			new()
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			IAdapterConfigIdentity,
			IAdapterConfigBinding,
			IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new()
	{
		protected WcfLobAdapterBase(ProtocolType protocolType) : base(protocolType)
		{
			_adapterConfig.BindingType = _bindingConfigurationElement.Name;
			EndpointBehaviors = Enumerable.Empty<IEndpointBehavior>();
		}

		#region Base Class Member Overrides

		protected override void Save(IPropertyBag propertyBag)
		{
			_adapterConfig.BindingConfiguration = _bindingConfigurationElement.GetBindingElementXml(_bindingConfigurationElement.Name);
			_adapterConfig.EndpointBehaviorConfiguration = EndpointBehaviors.GetEndpointBehaviorElementXml();
			base.Save(propertyBag);
		}

		#endregion

		public IEnumerable<IEndpointBehavior> EndpointBehaviors { get; set; }

		#region Binding Tab - General Settings

		/// <summary>
		/// Gets or sets the interval of time after which the receive method, invoked by a communication object, times
		/// out.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The interval of time that a connection can remain inactive, during which no application messages are received,
		/// before it is dropped. The default value is 10 minute.
		/// </para>
		/// <para>
		/// For inbound operations such as receiving IDOCs, it is recommend to set the timeout to the maximum possible
		/// value, which is 24.20:31:23.6470000 (24 days). When using the adapter with BizTalk Server, setting the timeout
		/// to a large value does not impact the functionality of the adapter.
		/// </para>
		/// </remarks>
		/// <returns>
		/// The <see cref="T:Timespan"/> that specifies the interval of time to wait for the receive method to time out.
		/// </returns>
		/// <exception cref="T:ArgumentOutOfRangeException">
		/// The value is less than zero or too large.
		/// </exception>
		public TimeSpan ReceiveTimeout
		{
			get { return _bindingConfigurationElement.ReceiveTimeout; }
			set { _bindingConfigurationElement.ReceiveTimeout = value; }
		}

		#endregion
	}
}
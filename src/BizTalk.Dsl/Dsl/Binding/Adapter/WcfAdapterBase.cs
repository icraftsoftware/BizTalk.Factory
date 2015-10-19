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
using System.ServiceModel.Configuration;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfAdapterBase<TBinding, TConfig> : AdapterBase, IAdapter, IAdapterBindingSerializerFactory, IAdapterConfigTimeouts
		where TBinding : StandardBindingElement,
			new()
		where TConfig : AdapterConfig,
			IAdapterConfigIdentity,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new()
	{
		protected WcfAdapterBase(string bindingConfigurationElementName)
		{
			_bindingConfigurationElement = new TBinding { Name = bindingConfigurationElementName };
			_adapterConfig = new TConfig();
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

		#region IAdapterConfigTimeouts Members

		/// <summary>
		/// Gets or sets the interval of time after which the close method, invoked by a communication object, times out.
		/// </summary>
		/// <remarks>
		/// The interval of time provided for a connection to close before the transport raises an exception. The default
		/// value is 1 minute.
		/// </remarks>
		/// <returns>
		/// The <see cref="T:Timespan"/> that specifies the interval of time to wait for the close method to time out.
		/// </returns>
		/// <exception cref="T:ArgumentOutOfRangeException">
		/// The value is less than zero or too large.
		/// </exception>
		public TimeSpan CloseTimeout
		{
			get { return _bindingConfigurationElement.CloseTimeout; }
			set { _bindingConfigurationElement.CloseTimeout = value; }
		}

		/// <summary>
		/// Gets or sets the interval of time after which the open method, invoked by a communication object, times out.
		/// </summary>
		/// <remarks>
		/// The interval of time provided for a connection to open before the transport raises an exception. The default
		/// value is 1 minute.
		/// </remarks>
		/// <returns>
		/// The <see cref="T:Timespan"/> that specifies the interval of time to wait for the open method to time out.
		/// </returns>
		/// <exception cref="T:ArgumentOutOfRangeException">
		/// The value is less than zero or too large.
		/// </exception>
		public TimeSpan OpenTimeout
		{
			get { return _bindingConfigurationElement.OpenTimeout; }
			set { _bindingConfigurationElement.OpenTimeout = value; }
		}

		/// <summary>
		/// Gets or sets the interval of time after which the send method, invoked by a communication object, times out.
		/// </summary>
		/// <remarks>
		/// The interval of time provided for a write operation to complete before the transport raises an exception. The
		/// default value is 1 minute.
		/// </remarks>
		/// <returns>
		/// The <see cref="T:Timespan"/> that specifies the interval of time to wait for the send method to time out.
		/// </returns>
		/// <exception cref="T:ArgumentOutOfRangeException">
		/// The value is less than zero or too large.
		/// </exception>
		public TimeSpan SendTimeout
		{
			get { return _bindingConfigurationElement.SendTimeout; }
			set { _bindingConfigurationElement.SendTimeout = value; }
		}

		#endregion

		protected abstract Uri GetAddress();

		protected abstract ProtocolType GetProtocolType();

		protected virtual void Save(IPropertyBag propertyBag)
		{
			_adapterConfig.Save(propertyBag as Microsoft.BizTalk.ExplorerOM.IPropertyBag);
		}

		protected readonly TConfig _adapterConfig;
		protected readonly TBinding _bindingConfigurationElement;
	}
}

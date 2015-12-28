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
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Adapter.Wcf.Converters;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;
using Microsoft.ServiceModel.Channels.Common;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	/// <summary>
	/// Binding DSL base class for all WCF-based BizTalk Server Adapters.
	/// </summary>
	/// <typeparam name="TAddress">
	/// The address configuration type to which the actual adpater will be connected to. It typically is either <see
	/// cref="EndpointAddress"/> or derived from <see cref="ConnectionUri"/>.
	/// </typeparam>
	/// <typeparam name="TBinding">
	/// The <see cref="StandardBindingElement"/>-derived class that matches the adapter to be configured.
	/// </typeparam>
	/// <typeparam name="TConfig">
	/// The <see cref="AdapterConfig"/>-derived class that matches the adapter to be configured.
	/// </typeparam>
	/// <seealso href="https://msdn.microsoft.com/en-us/library/bb259952.aspx">WCF Adapters</seealso>
	/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245975.aspx">What Are the WCF Adapters?</seealso>
	/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public abstract class WcfAdapterBase<TAddress, TBinding, TConfig> : AdapterBase, IAdapterConfigTimeouts
		where TBinding : StandardBindingElement, new()
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			IAdapterConfigIdentity,
			new()
	{
		static WcfAdapterBase()
		{
			_bindingName = WcfBindingRegistry.GetBindingName<TBinding>();
		}

		protected WcfAdapterBase(ProtocolType protocolType) : base(protocolType)
		{
			_adapterConfig = new TConfig();
			_bindingConfigurationElement = new TBinding { Name = _bindingName };
		}

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

		#region Base Class Member Overrides

		protected override string GetAddress()
		{
			// because EndpointAddress or ConnectionUri's ToString() internally calls Uri.ToString()
			return Address.ToString();
		}

		protected override void Save(IPropertyBag propertyBag)
		{
			_adapterConfig.Save(propertyBag as Microsoft.BizTalk.ExplorerOM.IPropertyBag);
		}

		protected override void Validate()
		{
			// TODO identity
			// TODO outbound & inbound msg config (body location, ...): outbound only for SP, inbound only for RL, both for 2-way
			// TODO PropagateFaultMessage for two-way only
			// TODO IsolationLevel iif EnableTransaction
			// TODO Proxy Settings
			// TODO see Microsoft.BizTalk.Adapter.Wcf.Metadata.BtsActionMapping and Microsoft.BizTalk.Adapter.Wcf.Metadata.BtsActionMappingHelper.CreateXml(BtsActionMapping btsActionMapping)
			// TODO validate BtsActionMapping against orchestration ports' actions
			_adapterConfig.Address = GetAddress();
			_adapterConfig.Validate();
			_adapterConfig.Address = null;
		}

		#endregion

		#region General Tab - Endpoint Address Settings

		public TAddress Address { get; set; }

		/// <summary>
		/// Specify the identity of the service that this receive location provides.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The values that can be specified for the Identity property differ according to the security configuration.
		/// These settings enable the client to authenticate this receive location. In the handshake process between the
		/// client and service, the Windows Communication Foundation (WCF) infrastructure will ensure that the identity of
		/// the expected service matches the values of this element.
		/// </para>
		/// <para>
		/// It defaults to <see cref="string.Empty"/>.
		/// </para>
		/// </remarks>
		/// <example>
		/// XML Blob example:
		/// <code><![CDATA[&lt;identity&gt;
		///   &lt;userPrincipalName value="username@contoso.com" /&gt;
		/// &lt;/identity&gt;]]>
		/// </code>
		/// </example>
		public EndpointIdentity Identity
		{
			get { return _identity; }
			set
			{
				_adapterConfig.Identity = IdentityFactory.CreateIdentity(value);
				_identity = value;
			}
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly string _bindingName;

		protected readonly TConfig _adapterConfig;
		protected readonly TBinding _bindingConfigurationElement;
		private EndpointIdentity _identity;
	}
}

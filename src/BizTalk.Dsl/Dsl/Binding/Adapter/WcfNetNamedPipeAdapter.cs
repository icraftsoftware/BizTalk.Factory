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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public abstract class WcfNetNamedPipeAdapter<TConfig>
		: WcfTwoWayAdapterBase<EndpointAddress, NetNamedPipeBindingElement, TConfig>,
			IAdapterConfigMaxReceivedMessageSize,
			IAdapterConfigSecurityMode<NetNamedPipeSecurityMode>,
			IAdapterConfigTransactions
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigNetNamedPipeBinding,
			IAdapterConfigNetNamedPipeSecurity,
			IAdapterConfigNetNamedPipeTransactions,
			IAdapterConfigOutboundMessageMarshalling,
			IAdapterConfigTimeouts,
			new()
	{
		static WcfNetNamedPipeAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("148d2e28-d634-4127-aa9e-7d6298156bf1"));
		}

		protected WcfNetNamedPipeAdapter() : base(_protocolType)
		{
			// Binding Tab - General Settings
			MaxReceivedMessageSize = ushort.MaxValue;

			// Binding Tab - Transactions Settings
			EnableTransaction = false;
			TransactionProtocol = TransactionProtocolValue.OleTransactions;

			// Security Tab - Security Mode Settings
			SecurityMode = NetNamedPipeSecurityMode.Transport;

			// Security Tab - Transport Security Settings
			TransportProtectionLevel = ProtectionLevel.EncryptAndSign;
		}

		#region IAdapterConfigMaxReceivedMessageSize Members

		public int MaxReceivedMessageSize
		{
			get { return _adapterConfig.MaxReceivedMessageSize; }
			set { _adapterConfig.MaxReceivedMessageSize = value; }
		}

		#endregion

		#region IAdapterConfigSecurityMode<NetNamedPipeSecurityMode> Members

		/// <summary>
		/// Specify the type of security that is used.
		/// </summary>
		/// <remarks>
		/// <list type="bullet">
		/// <item>
		/// <see cref="NetNamedPipeSecurityMode.None"/> &#8212; This disables security.
		/// </item>
		/// <item>
		/// <see cref="NetNamedPipeSecurityMode.Transport"/> &#8212; Security is provided using underlying transport-based
		/// security. It is possible to control the protection level with this mode.
		/// </item>
		/// </list>
		/// </remarks>
		public NetNamedPipeSecurityMode SecurityMode
		{
			get { return _adapterConfig.SecurityMode; }
			set { _adapterConfig.SecurityMode = value; }
		}

		#endregion

		#region IAdapterConfigTransactions Members

		/// <summary>
		/// Specify whether a message is send under transaction scope.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For outbound NetNamedPipe adapters, it specifies whether a message is transmitted to the destination service
		/// and deleted from the MessageBox database in a transactional context using the transaction protocol specified
		/// in the <see cref="TransactionProtocol"/> property.
		/// </para>
		/// <para>
		/// For inbound NetNamedPipe adapters, it specifies whether a message is submitted to the MessageBox database
		/// using the transaction flowed from clients. If this property is set to <c>True</c>, the clients are required to
		/// submit messages using the transaction protocol specified in the <see cref="TransactionProtocol"/> property. If
		/// the clients submit messages outside the transactional scope then the receive location returns an exception
		/// back to the clients, and no messages are suspended.
		/// </para>
		/// <para>
		/// The option is available only for one-way receive locations. If the clients submit messages in a transactional
		/// context for request-response receive locations, then an exception is returned back to the clients and no
		/// messages are suspended.
		/// </para>
		/// <para>
		/// It defaults to <c>False</c>.
		/// </para>
		/// </remarks>
		public bool EnableTransaction
		{
			get { return _adapterConfig.EnableTransaction; }
			set { _adapterConfig.EnableTransaction = value; }
		}

		#endregion

		#region Binding Tab - Transactions Settings

		/// <summary>
		/// Specify the transaction protocol to be used with this binding.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="TransactionProtocolValue.OleTransactions"/>.
		/// </remarks>
		public TransactionProtocolValue TransactionProtocol
		{
			get { return _adapterConfig.TransactionProtocol; }
			set { _adapterConfig.TransactionProtocol = value; }
		}

		#endregion

		#region Security Tab - Transport Security Settings

		/// <summary>
		/// Define security at the level of the TCP transport. Signing messages mitigates the risk of a third party
		/// tampering with the message while it is being transferred. Encryption provides data-level privacy during
		/// transport.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="ProtectionLevel.EncryptAndSign"/>.
		/// </remarks>
		public ProtectionLevel TransportProtectionLevel
		{
			get { return _adapterConfig.TransportProtectionLevel; }
			set { _adapterConfig.TransportProtectionLevel = value; }
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}

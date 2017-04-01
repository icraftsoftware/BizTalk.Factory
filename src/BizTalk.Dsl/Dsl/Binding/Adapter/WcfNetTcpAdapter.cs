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
	public abstract class WcfNetTcpAdapter<TConfig>
		: WcfTwoWayAdapterBase<EndpointAddress, NetTcpBindingElement, TConfig>,
			IAdapterConfigMaxReceivedMessageSize,
			IAdapterConfigMessageSecurity<MessageCredentialType>,
			IAdapterConfigSecurityMode<SecurityMode>,
			IAdapterConfigTransactions,
			IAdapterConfigTransportSecurity<TcpClientCredentialType>
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			IAdapterConfigNetTcpBinding,
			IAdapterConfigNetTcpSecurity,
			IAdapterConfigNetTcpTransactions,
			new()
	{
		static WcfNetTcpAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("7fd2dfcd-6a7b-44f9-8387-29457fd2eaaf"));
		}

		protected WcfNetTcpAdapter() : base(_protocolType)
		{
			// Binding Tab - General Settings
			MaxReceivedMessageSize = ushort.MaxValue;

			// Binding Tab - Transactions Settings
			EnableTransaction = false;
			TransactionProtocol = TransactionProtocolValue.OleTransactions;

			// Security Tab - Security Mode Settings
			SecurityMode = SecurityMode.Transport;

			// Security Tab - Transport Security Settings
			TransportClientCredentialType = TcpClientCredentialType.Windows;
			TransportProtectionLevel = ProtectionLevel.EncryptAndSign;

			// Security Tab - Message Security Settings
			MessageClientCredentialType = MessageCredentialType.Windows;
			AlgorithmSuite = SecurityAlgorithmSuiteValue.Basic256;
		}

		#region IAdapterConfigMaxReceivedMessageSize Members

		/// <summary>
		/// Specify the maximum size, in bytes, for a message (including headers) that can be received on the wire. The
		/// size of the messages is bounded by the amount of memory allocated for each message. You can use this property
		/// to limit exposure to denial of service (DoS) attacks. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// The WCF-NetTcp adapter leverages the <see cref="NetTcpBinding"/> class in the buffered transfer mode to
		/// communicate with an endpoint. For the buffered transport mode, the <see cref="NetTcpBinding"/>.<see
		/// cref="NetTcpBinding.MaxBufferSize"/> property is always equal to the value of this property.
		/// </para>
		/// <para>
		/// It defaults to roughly <see cref="ushort"/>.<see cref="ushort.MaxValue"/>, 65536.
		/// </para>
		/// </remarks>
		public int MaxReceivedMessageSize
		{
			get { return _adapterConfig.MaxReceivedMessageSize; }
			set { _adapterConfig.MaxReceivedMessageSize = value; }
		}

		#endregion

		#region IAdapterConfigMessageSecurity<MessageCredentialType> Members

		/// <summary>
		/// Specify the type of credential to be used when performing client authentication using message-based
		/// security.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="MessageCredentialType"/> property, see the
		/// Message client credential type property in <see
		/// href="https://msdn.microsoft.com/en-us/library/bb246097.aspx">WCF-NetTcp Transport Properties Dialog Box,
		/// Receive, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="MessageCredentialType.Windows"/>.
		/// </para>
		/// </remarks>
		public MessageCredentialType MessageClientCredentialType
		{
			get { return _adapterConfig.MessageClientCredentialType; }
			set { _adapterConfig.MessageClientCredentialType = value; }
		}

		/// <summary>
		/// Specify the message encryption and key-wrap algorithms. These algorithms map to those specified in the
		/// Security Policy Language (WS-SecurityPolicy) specification.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="AlgorithmSuite"/> property, see the
		/// Algorithm suite property in <see href="https://msdn.microsoft.com/en-us/library/bb246097.aspx">WCF-NetTcp
		/// Transport Properties Dialog Box, Receive, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="SecurityAlgorithmSuiteValue.Basic256"/>.
		/// </para>
		/// </remarks>
		public SecurityAlgorithmSuiteValue AlgorithmSuite
		{
			get { return _adapterConfig.AlgorithmSuite; }
			set { _adapterConfig.AlgorithmSuite = value; }
		}

		#endregion

		#region IAdapterConfigSecurityMode<SecurityMode> Members

		/// <summary>
		/// Specify the type of security that is used.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="SecurityMode"/> property, see the Security
		/// mode property in <see href="https://msdn.microsoft.com/en-us/library/bb226379.aspx">WCF-NetTcp Transport
		/// Properties Dialog Box, Send, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="System.ServiceModel.SecurityMode.Transport"/>.
		/// </para>
		/// </remarks>
		public SecurityMode SecurityMode
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
		/// For outbound NetTcp adapters, it specifies whether a message is transmitted to the destination service and
		/// deleted from the MessageBox database in a transactional context using the transaction protocol specified in
		/// the <see cref="TransactionProtocol"/> property.
		/// </para>
		/// <para>
		/// For inbound NetTcp adapters, it specifies whether a message is submitted to the MessageBox database using
		/// the transaction flowed from clients. If this property is set to <c>True</c>, the clients are required to
		/// submit messages using the transaction protocol specified in the <see cref="TransactionProtocol"/> property.
		/// If the clients submit messages outside the transactional scope then the receive location returns an
		/// exception back to the clients, and no messages are suspended.
		/// </para>
		/// <para>
		/// The option is available only for one-way receive locations. If the clients submit messages in a
		/// transactional context for request-response receive locations, then an exception is returned back to the
		/// clients and no messages are suspended.
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

		#region IAdapterConfigTransportSecurity<TcpClientCredentialType> Members

		/// <summary>
		/// Specify the type of credential to be used when performing the client authentication.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="TransportClientCredentialType"/> property,
		/// see the Transport client credential type property in <see
		/// href="https://msdn.microsoft.com/en-us/library/bb246097.aspx">WCF-NetTcp Transport Properties Dialog Box,
		/// Receive, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="TcpClientCredentialType.Windows"/>.
		/// </para>
		/// </remarks>
		public TcpClientCredentialType TransportClientCredentialType
		{
			get { return _adapterConfig.TransportClientCredentialType; }
			set { _adapterConfig.TransportClientCredentialType = value; }
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

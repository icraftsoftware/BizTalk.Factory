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
using System.Net.Security;
using System.ServiceModel;
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfNetTcpAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// The WCF-NetTcp adapter provides connected cross-computer or cross-process communication in an environment in
		/// which both services and clients are WCF based. It provides full access to SOAP security, reliability, and
		/// transaction features. This adapter uses the TCP transport, and messages have binary encoding.
		/// </summary>
		/// <remarks>
		/// You use the WCF-NetTcp send adapter to call a WCF service through the typeless contract by using the TCP
		/// protocol.
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226419.aspx">What Is the WCF-NetTcp Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx">How to Configure a WCF-NetTcp Send Port</seealso>.
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226379.aspx">WCF-NetTcp Transport Properties Dialog Box, Send, Security Tab</seealso>.
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Outbound : WcfNetTcpAdapter<NetTcpTLConfig>,
			IOutboundAdapter,
			IAdapterConfigOutboundAction,
			IAdapterConfigOutboundPropagateFaultMessage,
			IAdapterConfigOutboundCredentials
		{
			public Outbound()
			{
				// Binding Tab - General Settings
				MaxReceivedMessageSize = ushort.MaxValue;

				// Binding Tab - Transactions Settings
				EnableTransaction = false;
				TransactionProtocol = TransactionProtocolValue.OleTransactions;

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

				// Messages Tab - Error Handling Settings
				PropagateFaultMessage = true;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region IAdapterConfigOutboundAction Members

			public string StaticAction
			{
				get { return _adapterConfig.StaticAction; }
				set { _adapterConfig.StaticAction = value; }
			}

			#endregion

			#region IAdapterConfigOutboundCredentials Members

			public bool UseSSO
			{
				get { return _adapterConfig.UseSSO; }
				set { _adapterConfig.UseSSO = value; }
			}

			public string AffiliateApplicationName
			{
				get { return _adapterConfig.AffiliateApplicationName; }
				set { _adapterConfig.AffiliateApplicationName = value; }
			}

			public string UserName
			{
				get { return _adapterConfig.UserName; }
				set { _adapterConfig.UserName = value; }
			}

			public string Password
			{
				get { return _adapterConfig.Password; }
				set { _adapterConfig.Password = value; }
			}

			#endregion

			#region IAdapterConfigOutboundPropagateFaultMessage Members

			public bool PropagateFaultMessage
			{
				get { return _adapterConfig.PropagateFaultMessage; }
				set { _adapterConfig.PropagateFaultMessage = value; }
			}

			#endregion

			#region Binding Tab - General Settings

			/// <summary>
			/// Specify the maximum size, in bytes, for a message (including headers) that can be received on the wire. The
			/// size of the messages is bounded by the amount of memory allocated for each message. You can use this
			/// property to limit exposure to denial of service (DoS) attacks. 
			/// </summary>
			/// <remarks>
			/// <para>
			/// The WCF-NetTcp adapter leverages the <see cref="NetTcpBinding"/> class in the buffered transfer mode to
			/// communicate with an endpoint. For the buffered transport mode, the <see cref="NetTcpBinding"/>.<see
			/// cref="NetTcpBinding.MaxBufferSize"/> property is always equal to the value of this property.
			/// </para>
			/// <para>
			/// It defaults to <see cref="ushort"/>.<see cref="ushort.MaxValue"/>, 65536.
			/// </para>
			/// </remarks>
			public int MaxReceivedMessageSize
			{
				get { return _adapterConfig.MaxReceivedMessageSize; }
				set { _adapterConfig.MaxReceivedMessageSize = value; }
			}

			#endregion

			#region Binding Tab - Transactions Settings

			/// <summary>
			/// Specify whether a message is submitted to the MessageBox database using the transaction flowed from
			/// clients.
			/// </summary>
			/// <remarks>
			/// <para>
			/// If this property is set to True, the clients are required to submit messages using the transaction protocol
			/// specified in the TransactionProtocol property. If the clients submit messages outside the transactional
			/// scope then this receive location returns an exception back to the clients, and no messages are suspended.
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

			/// <summary>
			/// Specify the transaction protocol to be used with this receive location.
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

			#region Security Tab - Security Mode Settings

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

			#region Security Tab - Client Certificate Settings

			/// <summary>
			/// Specify the thumbprint of the X.509 certificate for authenticating this send port to services. This
			/// property is required if the ClientCredentialsType property is set to Certificate.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The certificate to be used for this property must be installed into the My store in the Current User
			/// location.
			/// </para>
			/// <para>
			/// It defaults to an <see cref="string.Empty"/> string.
			/// </para>
			/// </remarks>
			public string ClientCertificateThumbprint
			{
				get { return _adapterConfig.ClientCertificate; }
				set { _adapterConfig.ClientCertificate = value; }
			}

			#endregion

			#region Security Tab - Transport Security Settings

			/// <summary>
			/// Specify the type of credential to be used when performing the send port authentication.
			/// </summary>
			/// <remarks>
			/// <para>
			/// For more information about the member names for the <see cref="TransportClientCredentialType"/> property,
			/// see the Transport client credential type property in <see
			/// href="https://msdn.microsoft.com/en-us/library/bb226379.aspx">WCF-NetTcp Transport Properties Dialog Box,
			/// Send, Security Tab</see>.
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

			/// <summary>
			/// Specify security at the level of the TCP transport. Signing messages mitigates the risk of a third party
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

			#region Security Tab - Message Security Settings

			/// <summary>
			/// Specify the type of credential to be used when performing client authentication using message-based
			/// security.
			/// </summary>
			/// <remarks>
			/// <para>
			/// For more information about the member names for the <see cref="MessageCredentialType"/> property, see the
			/// Message client credential type property in <see
			/// href="https://msdn.microsoft.com/en-us/library/bb226379.aspx">WCF-NetTcp Transport Properties Dialog Box,
			/// Send, Security Tab</see>.
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
			/// Algorithm suite property in <see href="https://msdn.microsoft.com/en-us/library/bb226379.aspx">WCF-NetTcp
			/// Transport Properties Dialog Box, Send, Security Tab</see>.
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
		}

		#endregion
	}
}

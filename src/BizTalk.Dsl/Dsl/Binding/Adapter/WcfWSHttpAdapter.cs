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
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public abstract class WcfWSHttpAdapter<TAddress, TConfig> : WcfTwoWayAdapterBase<TAddress, WSHttpBindingElement, TConfig>,
		IAdapterConfigMaxReceivedMessageSize,
		IAdapterConfigServiceCertificate
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			IAdapterConfigIdentity,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigServiceCertificate,
			IAdapterConfigTransactions,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			IAdapterConfigWSHttpBinding,
			IAdapterConfigWSHttpSecurity,
			new()
	{
		static WcfWSHttpAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("2b219014-ba04-4b70-a66b-a8c418b109fd"));
		}

		protected WcfWSHttpAdapter() : base(_protocolType)
		{
			// Binding Tab - General Settings
			MaxReceivedMessageSize = ushort.MaxValue;

			// Binding Tab - Encoding Settings
			MessageEncoding = WSMessageEncoding.Text;
			TextEncoding = Encoding.UTF8;

			// Binding Tab - Transactions
			EnableTransaction = false;

			// Security Tab - Security Mode Settings
			SecurityMode = SecurityMode.Message;

			// Security Tab - Transport Security Settings
			TransportClientCredentialType = HttpClientCredentialType.Windows;

			// Security Tab - Message Security Settings
			MessageClientCredentialType = MessageCredentialType.Windows;
			AlgorithmSuite = SecurityAlgorithmSuiteValue.Basic256;
			NegotiateServiceCredential = true;
			EstablishSecurityContext = true;
		}

		#region IAdapterConfigMaxReceivedMessageSize Members

		public int MaxReceivedMessageSize
		{
			get { return _adapterConfig.MaxReceivedMessageSize; }
			set { _adapterConfig.MaxReceivedMessageSize = value; }
		}

		#endregion

		#region IAdapterConfigServiceCertificate Members

		public string ServiceCertificate
		{
			get { return _adapterConfig.ServiceCertificate; }
			set { _adapterConfig.ServiceCertificate = value; }
		}

		#endregion

		#region Binding Tab - Transactions

		/// <summary>
		/// Specify whether a message is submitted to the MessageBox database using the transaction flowed from
		/// clients.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If this property is <c>True</c>, the clients are required to submit messages using the WS-AtomicTransaction
		/// protocol. If the clients submit messages outside the transactional scope then this receive location returns
		/// an exception back to the clients and no messages are suspended.
		/// </para>
		/// <para>
		/// </para>
		/// The option is available only for one-way receive locations. If the clients submit messages in a
		/// transactional context for request-response receive locations, then an exception is returned back to the
		/// clients and no messages are suspended.
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

		#region Security Tab - Security Mode Settings

		/// <summary>
		/// Specify the type of security that is used.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="SecurityMode"/> property, see the Security mode
		/// property in <see href="https://msdn.microsoft.com/en-us/library/bb226411.aspx">WCF-WSHttp Transport Properties
		/// Dialog Box, Receive, Security Tab</see> and <see
		/// href="https://msdn.microsoft.com/en-us/library/bb226397.aspx">WCF-WSHttp Transport Properties Dialog Box,
		/// Send, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="System.ServiceModel.SecurityMode.Message"/>.
		/// </para>
		/// </remarks>
		public SecurityMode SecurityMode
		{
			get { return _adapterConfig.SecurityMode; }
			set { _adapterConfig.SecurityMode = value; }
		}

		#endregion

		#region Security Tab - Transport Security Settings

		/// <summary>
		/// Specify the type of credential to be used when performing the client authentication.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="TransportClientCredentialType"/> property, see
		/// the Transport client credential type property in <see
		/// href="https://msdn.microsoft.com/en-us/library/bb226411.aspx">WCF-WSHttp Transport Properties Dialog Box,
		/// Receive, Security Tab</see> and <see href="https://msdn.microsoft.com/en-us/library/bb226397.aspx">WCF-WSHttp
		/// Transport Properties Dialog Box, Send, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="HttpClientCredentialType.Windows"/>.
		/// </para>
		/// </remarks>
		public HttpClientCredentialType TransportClientCredentialType
		{
			get { return _adapterConfig.TransportClientCredentialType; }
			set { _adapterConfig.TransportClientCredentialType = value; }
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;

		#region Binding Tab - Encoding Settings

		/// <summary>
		/// Specify the encoder used to encode the SOAP message.
		/// </summary>
		/// <remarks>
		/// <list type="bullet">
		/// <item>
		/// <see cref="WSMessageEncoding.Text"/> &#8212; Use a text message encoder.</item>
		/// <item>
		/// <see cref="WSMessageEncoding.Mtom"/> &#8212; Use a Message Transmission Optimization Mechanism 1.0 (MTOM)
		/// encoder.
		/// </item>
		/// </list>
		/// It defaults to <see cref="WSMessageEncoding.Text"/>.
		/// </remarks>
		public WSMessageEncoding MessageEncoding
		{
			get { return _adapterConfig.MessageEncoding; }
			set { _adapterConfig.MessageEncoding = value; }
		}

		/// <summary>
		/// Specify the character set encoding to be used for emitting messages on the binding when the <see
		/// cref="MessageEncoding"/> property is set to <see cref="WSMessageEncoding.Text"/>.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="Encoding.UTF8"/>.
		/// </remarks>
		public Encoding TextEncoding
		{
			get { return Encoding.GetEncoding(_adapterConfig.TextEncoding); }
			set { _adapterConfig.TextEncoding = value.WebName; }
		}

		#endregion

		#region Security Tab - Message Security Settings

		/// <summary>
		/// Specify the type of credential to be used when performing client authentication using message-based security.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="MessageClientCredentialType"/> property, see
		/// the Message client credential type property in <see
		/// href="https://msdn.microsoft.com/en-us/library/bb226411.aspx">WCF-WSHttp Transport Properties Dialog Box,
		/// Receive, Security Tab</see> and <see href="https://msdn.microsoft.com/en-us/library/bb226397.aspx">WCF-WSHttp
		/// Transport Properties Dialog Box, Send, Security Tab</see>.
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
		/// For more information about the member names for the <see cref="AlgorithmSuite"/> property, see the Algorithm
		/// suite property in <see href="https://msdn.microsoft.com/en-us/library/bb226411.aspx">WCF-WSHttp Transport
		/// Properties Dialog Box, Receive, Security Tab</see> and <see
		/// href="https://msdn.microsoft.com/en-us/library/bb226397.aspx">WCF-WSHttp Transport Properties Dialog Box,
		/// Send, Security Tab</see>.
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

		/// <summary>
		/// Specify whether the service credential is provisioned at the client out of band, or is obtained from the
		/// service to the client through a process of negotiation. Such a negotiation is a precursor to the usual message
		/// exchange.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the <see cref="MessageClientCredentialType"/> property equals <see cref="MessageCredentialType.None"/>,
		/// <see cref="MessageCredentialType.UserName"/>, or <see cref="MessageCredentialType.Certificate"/>, setting this
		/// property to <c>False</c> implies that the service certificate is available at the client out of band and that
		/// the client needs to specify the service certificate. This mode is interoperable with SOAP stacks that
		/// implement WS-Trust and WS-SecureConversation.
		/// </para>
		/// <para>
		/// If the <see cref="MessageClientCredentialType"/> property is set to <see
		/// cref="MessageCredentialType.Windows"/>, setting this property to <c>False</c> specifies Kerberos-based
		/// authentication. This means that the client and service must be part of the same Kerberos domain. This mode is
		/// interoperable with SOAP stacks that implement the Kerberos token profile (as defined at OASIS WSS TC) as well
		/// as WS-Trust and WS-SecureConversation.
		/// </para>
		/// <para>
		/// When this property is <c>True</c>, it causes a .NET SOAP negotiation that tunnels SPNego exchange over SOAP
		/// messages.
		/// </para>
		/// <para>
		/// It defaults to <c>True</c>.
		/// </para>
		/// </remarks>
		public bool NegotiateServiceCredential
		{
			get { return _adapterConfig.NegotiateServiceCredential; }
			set { _adapterConfig.NegotiateServiceCredential = value; }
		}

		/// <summary>
		/// Specify whether the security channel establishes a secure session. A secure session establishes a Security
		/// Context Token (SCT) before exchanging the application messages.
		/// </summary>
		/// <remarks>
		/// It defaults to <c>True</c>.
		/// </remarks>
		public bool EstablishSecurityContext
		{
			get { return _adapterConfig.EstablishSecurityContext; }
			set { _adapterConfig.EstablishSecurityContext = value; }
		}

		#endregion
	}
}

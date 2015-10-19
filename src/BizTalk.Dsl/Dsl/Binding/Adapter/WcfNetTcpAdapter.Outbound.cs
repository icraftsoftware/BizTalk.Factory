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

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Outbound : WcfNetTcpAdapter<NetTcpTLConfig>
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

				// Security Tab - User Name Credentials Settings
				UseSSO = false;

				// Messages Tab - Error Handling Settings
				PropagateFaultMessage = true;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region General Tab - SOAP Action Header Settings

			/// <summary>
			/// Specify the SOAPAction header field for outgoing messages. This property can also be set through the
			/// message context property <see cref="WCF.Action"/> in a pipeline or orchestration.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You can specify this value in two different ways: the single action format and the action mapping format.
			/// </para>
			/// <para>
			/// If you set this property in the single action format &#8212; for example,
			/// <![CDATA[http://contoso.com/svc/operation1]]>&#8212;the SOAPAction header for outgoing messages is always
			/// set to the value specified in this property.
			/// </para>
			/// <para>
			/// If you set this property in the action mapping format, the outgoing SOAPAction header is determined by the
			/// <see cref="BTS.Operation"/> context property. For example, if this property is set to the following XML
			/// format and the <see cref="BTS.Operation"/> property is set to <c>operation1</c>, the WCF send adapter uses
			/// <![CDATA[http://contoso.com/svc/operation1]]> for the outgoing SOAPAction header.
			/// <code><![CDATA[<BtsActionMapping>
			///   <Operation Name="operation1" Action="http://contoso.com/svc/operation1" />
			///   <Operation Name="operation2" Action="http://contoso.com/svc/operation2" />
			/// </BtsActionMapping>]]></code>
			/// </para>
			/// <para>
			/// If outgoing messages come from an orchestration port, orchestration instances dynamically set the
			/// BTS.Operation property with the operation name of the port. If outgoing messages are routed with
			/// content-based routing, you can set the <see cref="BTS.Operation"/> property in pipeline components.
			/// </para>
			/// <para>
			/// It defaults to an <see cref="string.Empty"/> string.
			/// </para>
			/// </remarks>
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
			public string StaticAction
			{
				get { return _adapterConfig.StaticAction; }
				set { _adapterConfig.StaticAction = value; }
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
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226412.aspx"/>
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
			/// It defaults to <see cref="bool.False"/>.
			/// </para>
			/// </remarks>
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
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
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
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
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226379.aspx"/>
			public SecurityMode SecurityMode
			{
				get { return _adapterConfig.SecurityMode; }
				set { _adapterConfig.SecurityMode = value; }
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
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226379.aspx"/>
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
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
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
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226379.aspx"/>
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
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226379.aspx"/>
			public SecurityAlgorithmSuiteValue AlgorithmSuite
			{
				get { return _adapterConfig.AlgorithmSuite; }
				set { _adapterConfig.AlgorithmSuite = value; }
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
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
			public string ClientCertificateThumbprint
			{
				get { return _adapterConfig.ClientCertificate; }
				set { _adapterConfig.ClientCertificate = value; }
			}

			#endregion

			#region Security Tab - User Name Credentials Settings

			/// <summary>
			/// Specify whether to use Single Sign-On to retrieve client credentials for authentication with the
			/// destination server.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="bool.False"/>.
			/// </remarks>
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
			public bool UseSSO
			{
				get { return _adapterConfig.UseSSO; }
				set { _adapterConfig.UseSSO = value; }
			}

			/// <summary>
			/// Specify the affiliate application to use for Enterprise Single Sign-On (SSO).
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="string.Empty"/>.
			/// </remarks>
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
			public string AffiliateApplicationName
			{
				get { return _adapterConfig.AffiliateApplicationName; }
				set { _adapterConfig.AffiliateApplicationName = value; }
			}

			/// <summary>
			/// Specify the user name to use for authentication with the destination server when the <see cref="UseSSO"/>
			/// property is set to <see cref="bool.False"/>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You do not have to use the domain\user format for this property.
			/// </para>
			/// <para>
			/// It defaults to <see cref="string.Empty"/>.
			/// </para>
			/// </remarks>
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
			public string UserName
			{
				get { return _adapterConfig.UserName; }
				set { _adapterConfig.UserName = value; }
			}

			/// <summary>
			/// Specify the password to use for authentication with the destination server when the <see cref="UseSSO"/>
			/// property is set to <see cref="bool.False"/>.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="string.Empty"/>.
			/// </remarks>
			/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
			public string Password
			{
				get { return _adapterConfig.Password; }
				set { _adapterConfig.Password = value; }
			}

			#endregion

			#region Messages Tab - Error Handling Settings

			/// <summary>
			/// Specify whether to route or suspend messages failed in outbound processing.
			/// </summary>
			/// <remarks>
			/// <para>
			/// <list type="bullet">
			/// <item>
			/// <see cref="bool.True"/> &#8212; Route the message that fails outbound processing to a subscribing
			/// application (such as another receive port or orchestration schedule).
			/// </item>
			/// <item>
			/// <see cref="bool.False"/> &#8212; Suspend failed messages and generate a negative acknowledgment (NACK).
			/// </item>
			/// </list>
			/// </para>
			/// <para>
			/// This property is valid only for solicit-response ports.
			/// </para>
			/// <para>
			/// It defauts to <see cref="bool.True"/>.
			/// </para>
			/// </remarks>
			/// <see href="https://msdn.microsoft.com/en-us/library/bb226460.aspx"/>
			public bool PropagateFaultMessage
			{
				get { return _adapterConfig.PropagateFaultMessage; }
				set { _adapterConfig.PropagateFaultMessage = value; }
			}

			#endregion
		}

		#endregion
	}
}

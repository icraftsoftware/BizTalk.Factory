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
using System.Net.Security;
using System.ServiceModel;
using Microsoft.BizTalk.Adapter.ServiceBus;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Configuration;
using WCF;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfNetTcpRelayAdapter<TConfig> : WcfTwoWayAdapterBase<EndpointAddress, NetTcpRelayBindingElement, TConfig>,
		IAdapterConfigMaxReceivedMessageSize
		where TConfig : AdapterConfig,
			IAdapterConfigAcsCredentials,
			IAdapterConfigAddress,
			IAdapterConfigIdentity,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			IAdapterConfigNetTcpBinding,
			IAdapterConfigNetTcpRelaySecurity,
			new()
	{
		static WcfNetTcpRelayAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("b0a7e20b-9519-4b8e-9137-3a0dec2792b0"));
		}

		protected WcfNetTcpRelayAdapter() : base(_protocolType)
		{
			// Binding Tab - General Settings
			MaxReceivedMessageSize = ushort.MaxValue;

			// Security Tab - Security Mode Settings
			SecurityMode = EndToEndSecurityMode.Transport;
		}

		#region IAdapterConfigMaxReceivedMessageSize Members

		/// <summary>
		/// Specify the maximum size, in bytes, for a message including headers, which can be received on the wire. The
		/// size of the messages is bounded by the amount of memory allocated for each message. You can use this property
		/// to limit exposure to denial of service (DoS) attacks.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The WCF-NetTcpRelay adapter leverages the <see cref="NetTcpRelayBinding"/> class in the buffered transfer mode
		/// to communicate with an endpoint. For the buffered transport mode, the <see cref="NetTcpBinding"/>.<see
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

		#region Security Tab - Security Mode Settings

		/// <summary>
		/// Specify the type of security that is used.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Valid values include the following:
		/// <list type="bullet">
		/// <item>
		/// <term><see cref="EndToEndSecurityMode.None"/></term>
		/// <description>
		/// Messages are not secured during transfer.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="EndToEndSecurityMode.Message"/></term>
		/// <description>
		/// Security is provided using SOAP message security. By default, the SOAP Body is encrypted and signed. This mode
		/// offers a variety of features, such as whether the service credentials are available at the client out of band,
		/// and the algorithm suite to use. If you select <see cref="MessageCredentialType.None"/>, <see
		/// cref="MessageCredentialType.UserName"/>, or <see cref="MessageCredentialType.Certificate"/> for the <see
		/// cref="MessageClientCredentialType"/> property in this security mode, you must supply the service certificate
		/// for this receive location through the <see cref="ServiceCertificate"/> thumbprint property.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="EndToEndSecurityMode.Transport"/></term>
		/// <description>
		/// Transport security is provided using TLS over TCP or SPNego. It is possible to control the protection level
		/// with this mode. If you select If you select <see cref="MessageCredentialType.None"/> or <see
		/// cref="MessageCredentialType.Certificate"/> for the <see cref="MessageClientCredentialType"/> property in this
		/// security mode, you must supply the service certificate for this receive location through the <see
		/// cref="ServiceCertificate"/> thumbprint property.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="EndToEndSecurityMode.TransportWithMessageCredential"/></term>
		/// <description>
		/// Transport security is coupled with message security. Transport security is provided by TLS over TCP or SPNego
		/// and ensures integrity, confidentiality, and server authentication. If you select <see
		/// cref="MessageCredentialType.Windows"/>, <see cref="MessageCredentialType.UserName"/>, or <see
		/// cref="MessageCredentialType.Certificate"/> for the <see cref="MessageClientCredentialType"/> property in this
		/// security mode, you must supply the service certificate for this receive location through the <see
		/// cref="ServiceCertificate"/> thumbprint property.
		/// </description>
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// It defaults to <see cref="EndToEndSecurityMode.Transport"/>.
		/// </para>
		/// </remarks>
		public EndToEndSecurityMode SecurityMode
		{
			get { return _adapterConfig.SecurityMode; }
			set { _adapterConfig.SecurityMode = value; }
		}

		#endregion

		#region Security Tab - Transport Security Settings

		/// <summary>
		/// Define security at the level of the TCP transport.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Signing messages mitigates the risk of a third party tampering with the message while it is being transferred.
		/// Encryption provides data-level privacy during transport.
		/// </para>
		/// <para>
		/// Valid values include the following:
		/// <list type="bullet">
		/// <item>
		/// <term><see cref="ProtectionLevel.None"/></term>
		/// <description>
		/// No protection.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="ProtectionLevel.Sign"/></term>
		/// <description>
		/// Messages are signed.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="ProtectionLevel.EncryptAndSign"/></term>
		/// <description>
		/// Messages are encrypted and signed.
		/// </description>
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// It defaults to <see cref="ProtectionLevel.EncryptAndSign"/>.
		/// </para>
		/// </remarks>
		public ProtectionLevel TransportProtectionLevel
		{
			get { return _adapterConfig.TransportProtectionLevel; }
			set { _adapterConfig.TransportProtectionLevel = value; }
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;

		#region Security Tab - Message Security Settings

		/// <summary>
		/// Specify the type of credential to be used when performing client authentication using message-based security.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is required only if the Security mode is set to Message or TransportWithMessageCredential.
		/// </para>
		/// <para>
		/// Valid values include the following:
		/// <list type="bullet">
		/// <item>
		/// <term><see cref="MessageCredentialType.None"/></term>
		/// <description>
		/// This allows the service to interact with anonymous clients. This indicates that this client does not provide
		/// any client credential.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="MessageCredentialType.Certificate"/></term>
		/// <description>
		/// Clients are authenticated to this receive location using the client certificate specified through <see
		/// cref="ServiceCertificate"/> thumbprint property. The credential is passed through the SOAP Header element
		/// using the WSS SOAP Message Security X509 Token Profile 1.0 protocol. To authenticate the client certificates,
		/// the CA certificate chain for the client certificates must be installed in the Trusted Root Certification
		/// Authorities certificate store of this computer.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="MessageCredentialType.UserName"/></term>
		/// <description>
		/// Clients are authenticated to this receive location with a UserName credential. The credential is passed
		/// through the SOAP Header element using the WSS SOAP Message Security UsernameToken Profile 1.0 protocol. You
		/// must create the domain or local user accounts corresponding to client credentials.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="MessageCredentialType.Windows"/></term>
		/// <description>
		/// Allow the SOAP exchanges to be under the authenticated context of a Windows credential. The client credential
		/// is passed through the SOAP Header element using the WSS SOAP Message Security Kerberos Token Profile 1.0
		/// protocol. You must create the domain or local user accounts corresponding to client credentials. In addition,
		/// the client's userPrincipalName element must be configured with the user account name running this receive
		/// handler.
		/// </description>
		/// </item>
		/// </list>
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
		/// suite property in <see href="https://msdn.microsoft.com/en-us/library/bb246097.aspx">WCF-NetTcp Transport
		/// Properties Dialog Box, Receive, Security Tab</see> and <see
		/// href="https://msdn.microsoft.com/en-us/library/bb226527.aspx">WCF-NetTcp Transport Properties Dialog Box,
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

		#endregion

		#region Security Tab - Access Control Service Settings

		/// <summary>
		/// Access Control Service (ACS) must be configured to issue token in Simple Web Token (SWT) format using a
		/// service identity symmetric key. The SWT token will be sent in the HTTP Authorization header.
		/// </summary>
		public bool UseAcsAuthentication
		{
			get { return _adapterConfig.UseAcsAuthentication; }
			set { _adapterConfig.UseAcsAuthentication = value; }
		}

		/// <summary>
		/// Access Control Service STS URI.
		/// </summary>
		public Uri StsUri
		{
			get { return new Uri(_adapterConfig.StsUri); }
			set { _adapterConfig.StsUri = value.ToString(); }
		}

		/// <summary>
		/// Specify the issuer name.
		/// </summary>
		/// <remarks>
		/// Typically this is set to owner.
		/// </remarks>
		public string IssuerName
		{
			get { return _adapterConfig.IssuerName; }
			set { _adapterConfig.IssuerName = value; }
		}

		/// <summary>
		/// Specify the issuer key.
		/// </summary>
		public string IssuerSecret
		{
			get { return _adapterConfig.IssuerSecret; }
			set { _adapterConfig.IssuerSecret = value; }
		}

		#endregion
	}
}

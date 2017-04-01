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
using System.ServiceModel;
using System.Text;
using Microsoft.BizTalk.Adapter.ServiceBus;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Configuration;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfBasicHttpRelayAdapter<TConfig>
		: WcfTwoWayAdapterBase<EndpointAddress, BasicHttpRelayBindingElement, TConfig>,
			IAdapterConfigMaxReceivedMessageSize,
			IAdapterConfigMessageEncoding,
			IAdapterConfigSecurityMode<EndToEndBasicHttpSecurityMode>,
			IAdapterConfigMessageSecurity<BasicHttpMessageCredentialType>,
			IAdapterConfigServiceCertificate
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			IAdapterConfigBasicHttpBinding,
			IAdapterConfigBasicHttpRelaySecurity,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigServiceCertificate,
			new()
	{
		static WcfBasicHttpRelayAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("f15097a3-283a-40b2-aca7-6b7bae0a0955"));
		}

		protected WcfBasicHttpRelayAdapter() : base(_protocolType)
		{
			// Binding Tab - General Settings
			MaxReceivedMessageSize = ushort.MaxValue;

			// Security Tab
			SecurityMode = EndToEndBasicHttpSecurityMode.Transport;
		}

		#region IAdapterConfigMaxReceivedMessageSize Members

		/// <summary>
		/// Specify the maximum size, in bytes, for a message including headers, which can be received on the wire. The
		/// size of the messages is bounded by the amount of memory allocated for each message. You can use this property
		/// to limit exposure to denial of service (DoS) attacks.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The WCF-BasicHttpRelay adapter leverages the <see cref="BasicHttpRelayBinding"/> class in the buffered
		/// transfer mode to communicate with an endpoint. For the buffered transport mode, the <see
		/// cref="BasicHttpRelayBinding"/>.<see cref="BasicHttpRelayBinding.MaxBufferSize"/> property is always equal to
		/// the value of this property.
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

		#region IAdapterConfigMessageEncoding Members

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
		/// <para>
		/// Valid values include the following:
		/// <list type="bullet">
		/// <item>
		/// <see cref="Encoding.BigEndianUnicode"/>
		/// </item>
		/// <item>
		/// <see cref="Encoding.Unicode"/>
		/// </item>
		/// <item>
		/// <see cref="Encoding.UTF8"/>
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// It defaults to <see cref="Encoding.UTF8"/>.
		/// </para>
		/// </remarks>
		public Encoding TextEncoding
		{
			get { return Encoding.GetEncoding(_adapterConfig.TextEncoding); }
			set { _adapterConfig.TextEncoding = value.BodyName; }
		}

		#endregion

		#region IAdapterConfigMessageSecurity<BasicHttpMessageCredentialType> Members

		/// <summary>
		/// Specify the message-level security options only if you set the Security mode above to Message or
		/// TransportWithMessageCredential.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Valid values include the following:
		/// <list type="bullet">
		/// <item>
		/// <term>UserName</term>
		/// <description>
		/// Enables this receive location to require that clients be authenticated using the UserName credential. You
		/// must create the domain or local user accounts corresponding to the client credentials.
		/// </description>
		/// </item>
		/// <item>
		/// <term>Certificate</term>
		/// <description>
		/// Clients are authenticated to this receive location using the client certificate. The CA certificate chain
		/// for the client X.509 certificates must be installed in the Trusted Root Certification Authorities
		/// certificate store of this computer so that the clients can be authenticated to this receive
		/// </description>
		/// location. 
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// It defaults to <see cref="BasicHttpMessageCredentialType.UserName"/>.
		/// </para>
		/// </remarks>
		public BasicHttpMessageCredentialType MessageClientCredentialType
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
		/// Algorithm suite property in <see
		/// href="https://msdn.microsoft.com/en-us/library/bb226322.aspx">WCF-BasicHttp Transport Properties Dialog
		/// Box, Receive, Security Tab</see> and <see
		/// href="https://msdn.microsoft.com/en-us/library/bb226514.aspx">WCF-BasicHttp Transport Properties Dialog
		/// Box, Send, Security Tab</see>.
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

		#region IAdapterConfigSecurityMode<EndToEndBasicHttpSecurityMode> Members

		/// <summary>
		/// Specify the type of security that is used.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Valid values include the following:
		/// <list type="bullet">
		/// <item>
		/// <term><see cref="EndToEndBasicHttpSecurityMode.None"/></term>
		/// <description>
		/// Messages are not secured during transfer.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="EndToEndBasicHttpSecurityMode.Transport"/></term>
		/// <description>
		/// Security is provided using the HTTPS transport. The SOAP messages are secured using HTTPS. To use this
		/// mode, you must set up Secure Sockets Layer (SSL) in Microsoft Internet Information Services (IIS).
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="EndToEndBasicHttpSecurityMode.Message"/></term>
		/// <description>
		/// Security is provided using SOAP message security over the HTTP transport. By default, the SOAP Body is
		/// encrypted and signed. The only valid Message client credential type for the WCF-Basic adapter is
		/// Certificate. This mode requires the HTTP transport. When using this security mode, the service certificate
		/// for this receive location needs to be provided through the Service certificate - Thumbprint property.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="EndToEndBasicHttpSecurityMode.TransportWithMessageCredential"/></term>
		/// <description>
		/// Integrity, confidentiality, and service authentication are provided by the HTTPS transport. To use this
		/// mode, you must set up Secure Sockets Layer (SSL) in Microsoft Internet Information Services (IIS). 
		/// </description>
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// It defaults to <see cref="EndToEndBasicHttpSecurityMode.Transport"/>.
		/// </para>
		/// </remarks>
		public EndToEndBasicHttpSecurityMode SecurityMode
		{
			get { return _adapterConfig.SecurityMode; }
			set { _adapterConfig.SecurityMode = value; }
		}

		#endregion

		#region IAdapterConfigServiceCertificate Members

		public string ServiceCertificate
		{
			get { return _adapterConfig.ServiceCertificate; }
			set { _adapterConfig.ServiceCertificate = value; }
		}

		#endregion

		#region Security Tab - Client Security Settings

		/// <summary>
		/// Specify the option to authenticate with the Service Bus relay endpoint from where the message is received.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Valid values include the following:
		/// <list type="definition">
		/// <item>
		/// <term><see cref="Microsoft.ServiceBus.RelayClientAuthenticationType.None"/></term>
		/// <description>No authentication is required.</description>
		/// </item>
		/// <item>
		/// <term><see cref="Microsoft.ServiceBus.RelayClientAuthenticationType.RelayAccessToken"/></term>
		/// <description>Specify this to use a security token to authorize with the Service Bus Relay endpoint.</description>
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// It defaults to <see cref="Microsoft.ServiceBus.RelayClientAuthenticationType.RelayAccessToken"/>.
		/// </para>
		/// </remarks>
		public RelayClientAuthenticationType RelayClientAuthenticationType
		{
			get { return _adapterConfig.RelayClientAuthenticationType; }
			set { _adapterConfig.RelayClientAuthenticationType = value; }
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}

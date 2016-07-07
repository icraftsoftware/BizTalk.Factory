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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Metadata;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfWebHttpAdapter<TAddress, TConfig> : WcfAdapterBase<TAddress, WebHttpBindingElement, TConfig>,
		IAdapterConfigMaxReceivedMessageSize,
		IAdapterConfigServiceCertificate
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			IAdapterConfigEndpointBehavior,
			IAdapterConfigIdentity,
			IAdapterConfigOutboundHttpProperty,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigServiceCertificate,
			IAdapterConfigRequestHttpProperty,
			IAdapterConfigVariablePropertyMapping,
			IAdapterConfigWebHttpBinding,
			IAdapterConfigWebHttpSecurity,
			new()
	{
		static WcfWebHttpAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("e5b2de81-de67-4559-869b-20925949a1e0"));
		}

		protected WcfWebHttpAdapter() : base(_protocolType)
		{
			// Binding Tab
			MaxReceivedMessageSize = ushort.MaxValue;

			// Behavior Tab
			EndpointBehaviors = Enumerable.Empty<BehaviorExtensionElement>();
		}

		#region IAdapterConfigMaxReceivedMessageSize Members

		/// <summary>
		/// Specify the maximum size, in bytes, for a message including headers, which can be received on the wire. The
		/// size of the messages is bounded by the amount of memory allocated for each message. You can use this
		/// property to limit exposure to denial of service (DoS) attacks.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The WCF-WebHttp adapter leverages the <see cref="WebHttpBinding"/> class in the buffered transfer mode to
		/// communicate with an endpoint. For the buffered transport mode, the <see
		/// cref="WebHttpBinding.MaxBufferSize"/> property is always equal to the value of this property.
		/// </para>
		/// <para>
		/// It defaults to <see cref="ushort.MaxValue"/> and cannot exceed <see cref="int.MaxValue"/>.
		/// </para>
		/// </remarks>
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

		#region Base Class Member Overrides

		protected override void Save(IPropertyBag propertyBag)
		{
			_adapterConfig.EndpointBehaviorConfiguration = EndpointBehaviors.GetEndpointBehaviorElementXml();
			base.Save(propertyBag);
		}

		#endregion

		#region Behavior Tab

		public IEnumerable<BehaviorExtensionElement> EndpointBehaviors { get; set; }

		#endregion

		#region Message Tab - Outbound HTTP Headers Settings

		/// <summary>
		/// Specifies the HTTP headers that are stamped on the response message.
		/// </summary>
		public string HttpHeaders
		{
			get { return _adapterConfig.HttpHeaders; }
			set { _adapterConfig.HttpHeaders = value; }
		}

		#endregion

		#region General Tab - HTTP Method and URL Mapping Settings

		/// <summary>
		/// BTS Operation Mapping allows users to map incoming HTTP requests to BTS Operation in the message context,
		/// based on the incoming HTTP Method and the URL sub-path.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The incoming HTTP Method and the URL sub-path are matched against a set of HTTP method and the URI
		/// Template. If a match is found, the adapter promotes the <see cref="BTS"/>.<see cref="BTS.Operation"/>
		/// property to the BizTalk Message Context with the value specified in the message.
		/// </para>
		/// <para>
		/// You can specify HTTP method to URL mapping as a singular format or a multi-mapping format. The
		/// multi-mapping format resembles the following: <code><![CDATA[
		/// <BtsHttpUrlMapping>
		///   <Operation Name = "DeleteCustomer" Method="DELETE" Url="/Customer/12345" />
		/// </BtsHttpUrlMapping>
		/// ]]></code>
		/// </para>
		/// <para>
		/// In the above snippet, notice that the customer ID is provided as a constant value, which is 12345. However,
		/// there could be scenarios when the customer ID, or any other query variable, must be determined at runtime.
		/// To enable such scenarios, you must provide the variable component of the URL within curly brackets <c>{</c>
		/// <c>}</c>. For example, in the above snippet, if you specify the customer ID as a variable, it would look
		/// like: <code><![CDATA[
		/// <BtsHttpUrlMapping>
		///   <Operation Name = "DeleteCustomer" Method="DELETE" Url="/Customer/{ID}" />
		/// </BtsHttpUrlMapping>
		/// ]]></code>
		/// </para>
		/// <para>
		/// In such a case, you must also specify where the value for the variable <c>ID</c> must be picked from at
		/// runtime. You specify that using <see cref="VariableMapping"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="Be.Stateless.BizTalk.Dsl.Binding.Adapter.Metadata.HttpUrlMapping"/>
		/// <seealso cref="HttpUrlMappingOperation"/>
		public string HttpUrlMapping
		{
			get { return _adapterConfig.HttpMethodAndUrl; }
			set { _adapterConfig.HttpMethodAndUrl = value; }
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
		/// <see cref="Microsoft.BizTalk.Adapter.Wcf.Config.WebHttpSecurityMode.None"/>: Messages are not secured during
		/// transfer.
		/// </item>
		/// <item>
		/// <see cref="Microsoft.BizTalk.Adapter.Wcf.Config.WebHttpSecurityMode.Transport"/>: Security is provided using
		/// the HTTPS transport. The messages are secured using HTTPS. To use this mode, you must set up Secure Sockets
		/// Layer (SSL) in Microsoft Internet Information Services (IIS).
		/// </item>
		/// <item>
		/// <see cref="Microsoft.BizTalk.Adapter.Wcf.Config.WebHttpSecurityMode.TransportCredentialOnly"/>: Integrity,
		/// confidentiality, and service authentication are provided by the HTTPS transport. To use this mode, you must
		/// set up Secure Sockets Layer (SSL) in Microsoft Internet Information Services (IIS).
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// It defaults to <see cref="Microsoft.BizTalk.Adapter.Wcf.Config.WebHttpSecurityMode.Transport"/>.
		/// </para>
		/// </remarks>
		public Microsoft.BizTalk.Adapter.Wcf.Config.WebHttpSecurityMode SecurityMode
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
		/// Valid values include the following:
		/// <list type="bullet">
		/// <item>
		/// <see cref="HttpClientCredentialType.None"/>: No authentication occurs at the transport level.
		/// </item>
		/// <item>
		/// <see cref="HttpClientCredentialType.Basic"/>: Basic authentication. In Basic authentication, user names and
		/// passwords are sent in plain text over the network. You must create the domain or local user accounts
		/// corresponding to the credentials.
		/// </item>
		/// <item>
		/// <see cref="HttpClientCredentialType.Digest"/>: Digest authentication. This authentication method operates much
		/// like Basic authentication, except that passwords are sent across the network as a hash value for additional
		/// security. Digest authentication is available only on domains with domain controllers running Windows Server
		/// operating systems authentication. You must create the domain or local user accounts corresponding to client
		/// credentials.
		/// </item>
		/// <item>
		/// <see cref="HttpClientCredentialType.Ntlm"/>: NTLM authentication. Clients can send the credentials without
		/// sending a password to this receive location. You must create the domain or local user accounts corresponding
		/// to client credentials.
		/// </item>
		/// <item>
		/// <see cref="HttpClientCredentialType.Windows"/>: Windows integrated authentication. Windows Communication
		/// Foundation negotiates Kerberos or NTLM, preferring Kerberos if a domain is present. If you want to use
		/// Kerberos it is important to have the client identify the service with a service principal name (SPN). You must
		/// create the domain or local user accounts corresponding to client credentials.
		/// </item>
		/// <item>
		/// <see cref="HttpClientCredentialType.Certificate"/>: Client authentication using the client certificate. The CA
		/// certificate chain for the client X.509 certificates must be installed in the Trusted Root Certification
		/// Authorities certificate store of this computer so that the clients can be authenticated to this receive
		/// location.
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// The Transport client credential type property must match the authentication scheme of the IIS virtual
		/// directory hosting this receive location. For example, if the property is set to Windows, you also need to
		/// enable Integrated Windows authentication for the virtual directory that hosts it. Similarly if the property is
		/// set to None, you must allow anonymous access to the virtual directory that hosts this receive location.
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

		#region General Tab - Variable Mapping Settings

		/// <summary>
		/// Allows to specify what the variable maps to at runtime if variables have been declared in the HTTP Method
		/// URL Mappings.
		/// </summary>
		/// <remarks>
		/// You must specify the name of the property that provides the value to be associated to the variable. You
		/// must have already defined/promoted this property as part of your solution. You must also provide the
		/// namespace for the property in the Property Namespace field.
		/// </remarks>
		/// <seealso cref="Be.Stateless.BizTalk.Dsl.Binding.Adapter.Metadata.HttpUrlMapping"/>
		/// <seealso cref="HttpUrlMappingOperation"/>
		public string VariableMapping
		{
			get { return _adapterConfig.VariablePropertyMapping; }
			set { _adapterConfig.VariablePropertyMapping = value; }
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}

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
using System.ServiceModel;
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfWebHttpAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// Microsoft BizTalk Server uses the WCF-WebHttp adapter to send messages to RESTful services.
		/// </summary>
		/// <remarks>
		/// The WCF-WebHttp send adapter sends HTTP messages to a service from a BizTalk message. The receive location
		/// receives messages from a RESTful service. For GET and DELETE request, the adapter does not use any payload.
		/// For POST and PUT request, the adapter uses the BizTalk message body part to the HTTP content/payload.
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/jj572846.aspx">WCF-WebHttp Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/jj572853.aspx">How to Configure a WCF-WebHttp Send Port</seealso>
		public class Outbound : WcfWebHttpAdapter<EndpointAddress, WebHttpTLConfig>, IAdapterConfigAccessControlService
		{
			public Outbound()
			{
				ProxyToUse = ProxySelection.None;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region IAdapterConfigAccessControlService Members

			public Uri StsUri
			{
				get { return new Uri(_adapterConfig.StsUri); }
				set { _adapterConfig.StsUri = value.ToString(); }
			}

			public string IssuerName
			{
				get { return _adapterConfig.IssuerName; }
				set { _adapterConfig.IssuerName = value; }
			}

			public string IssuerSecret
			{
				get { return _adapterConfig.IssuerSecret; }
				set { _adapterConfig.IssuerSecret = value; }
			}

			#endregion

			#region Security Tab - Client Certificate Settings

			/// <summary>
			/// Specify the thumbprint of the X.509 certificate for authenticating this send port to the endpoint.
			/// </summary>
			/// <remarks>
			/// You must install the client certificate into the Current User location of the user account for the send
			/// handler hosting this send port.
			/// </remarks>
			public string ClientCertificate
			{
				get { return _adapterConfig.ClientCertificate; }
				set { _adapterConfig.ClientCertificate = value; }
			}

			#endregion

			#region Proxy Tab - General Settings

			/// <summary>
			/// Specify which proxy server to use for outgoing HTTP traffic.
			/// </summary>
			/// <remarks>
			/// <list type="bullet">
			/// <item>
			/// <see cref="ProxySelection.None"/> &#8212; Do not use a proxy server for this send port.
			/// </item>
			/// <item>
			/// <see cref="ProxySelection.Default"/> &#8212; Use the proxy settings in the send handler hosting this send
			/// port.
			/// </item>
			/// <item>
			/// <see cref="ProxySelection.UserSpecified"/> &#8212; Use the proxy server specified in the <see cref="ProxyAddress"/>
			/// property.
			/// </item>
			/// </list>
			/// It defaults to <see cref="ProxySelection.None"/>.
			/// </remarks>
			public ProxySelection ProxyToUse
			{
				get { return _adapterConfig.ProxyToUse; }
				set { _adapterConfig.ProxyToUse = value; }
			}

			#endregion

			#region Message Tab - Outbound Message Settings

			/// <summary>
			/// Specify whether to remove the message payload for outgoing HTTP request made for some HTTP verbs.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Based on the verb you use to invoke a REST endpoint, you may or may not require a message payload. For
			/// example, you may not need a message payload while using the GET or DELETE verbs. However, to trigger a call
			/// to the REST endpoint using the send port, you may use a dummy message that includes a message payload.
			/// Before the message is sent to the REST endpoint, the message payload from the dummy message must be
			/// removed. You can specify the verbs for which the message payload must be removed using the Suppress Body
			/// for Verbs property.
			/// </para>
			/// <para>
			/// For example, if you want to remove the message payload while using a GET verb, specify the value for this
			/// property as GET.
			/// </para>
			/// </remarks>
			public string SuppressMessageBodyForHttpVerbs
			{
				get { return _adapterConfig.SuppressMessageBodyForHttpVerbs; }
				set { _adapterConfig.SuppressMessageBodyForHttpVerbs = value; }
			}

			#endregion

			#region Security Tab - Access Control Service Settings

			/// <summary>
			/// Specify whether to authenticate with the Service Bus.
			/// </summary>
			/// <remarks>
			/// This is required only when invoking a REST interface for Service Bus related entities.
			/// </remarks>
			public bool UseAcsAuthentication
			{
				get { return _adapterConfig.UseAcsAuthentication; }
				set { _adapterConfig.UseAcsAuthentication = value; }
			}

			#endregion

			#region Security Tab - User Name Credentials Settings

			/// <summary>
			/// Specify the affiliate application to use for Enterprise Single Sign-On (SSO).
			/// </summary>
			/// <remarks>
			/// <para>
			/// You must set the credentials if you selected the <see cref="HttpClientCredentialType.Basic"/> or <see
			/// cref="HttpClientCredentialType.Digest"/> option for <see
			/// cref="WcfWebHttpAdapter{TAddress,TConfig}.TransportClientCredentialType"/> and <see cref="UseSSO"/> is set
			/// to <c>True</c>.
			/// </para>
			/// <para>
			/// It defaults to <see cref="string.Empty"/>.
			/// </para>
			/// </remarks>
			public string AffiliateApplicationName
			{
				get { return _adapterConfig.AffiliateApplicationName; }
				set { _adapterConfig.AffiliateApplicationName = value; }
			}

			/// <summary>
			/// Specify the password to use for authentication with the destination server when the <see cref="UseSSO"/>
			/// property is set to <c>False</c>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You must set the credentials if you selected the <see cref="HttpClientCredentialType.Basic"/> or <see
			/// cref="HttpClientCredentialType.Digest"/> option for <see
			/// cref="WcfWebHttpAdapter{TAddress,TConfig}.TransportClientCredentialType"/> and <see cref="UseSSO"/> is set
			/// to <c>False</c>.
			/// </para>
			/// <para>
			/// It defaults to <see cref="string.Empty"/>.
			/// </para>
			/// </remarks>
			public string Password
			{
				get { return _adapterConfig.Password; }
				set { _adapterConfig.Password = value; }
			}

			/// <summary>
			/// Specify the user name to use for authentication with the destination server when the <see cref="UseSSO"/>
			/// property is set to <c>False</c>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You must set the credentials if you selected the <see cref="HttpClientCredentialType.Basic"/> or <see
			/// cref="HttpClientCredentialType.Digest"/> option for <see
			/// cref="WcfWebHttpAdapter{TAddress,TConfig}.TransportClientCredentialType"/> and <see cref="UseSSO"/> is set
			/// to <c>False</c>.
			/// </para>
			/// <para>
			/// You do not have to use the domain\user format for this property.
			/// </para>
			/// <para>
			/// It defaults to <see cref="string.Empty"/>.
			/// </para>
			/// </remarks>
			public string UserName
			{
				get { return _adapterConfig.UserName; }
				set { _adapterConfig.UserName = value; }
			}

			/// <summary>
			/// Specify whether to use Single Sign-On to retrieve client credentials for authentication with the
			/// destination server.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You must set the credentials if you selected the <see cref="HttpClientCredentialType.Basic"/> or <see
			/// cref="HttpClientCredentialType.Digest"/> option for <see
			/// cref="WcfWebHttpAdapter{TAddress,TConfig}.TransportClientCredentialType"/>.
			/// </para>
			/// <para>
			/// It defaults to <c>False</c>.
			/// </para>
			/// </remarks>
			public bool UseSSO
			{
				get { return _adapterConfig.UseSSO; }
				set { _adapterConfig.UseSSO = value; }
			}

			#endregion

			#region Proxy Tab - Proxy Settings

			/// <summary>
			/// Specify the address of the proxy server.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Use the https or the http scheme depending on the security configuration. This address can be followed by a
			/// colon and the port number. For example, <c>http://127.0.0.1:8080</c>.
			/// </para>
			/// <para>
			/// It defaults to an <see cref="string.Empty"/> string.
			/// </para>
			/// </remarks>
			public string ProxyAddress
			{
				get { return _adapterConfig.ProxyAddress; }
				set { _adapterConfig.ProxyAddress = value; }
			}

			/// <summary>
			/// Specify the user name to use for the proxy.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The WCF-BasicHttp adapter leverages the <see cref="BasicHttpBinding"/> in the buffered transfer mode to
			/// communicate with an endpoint. Proxy credentials of <see cref="BasicHttpBinding"/> are applicable only when
			/// the <see cref="WcfBasicHttpAdapter{TAddress,TConfig}.SecurityMode"/> is <see
			/// cref="BasicHttpSecurityMode.Transport"/>, <see cref="BasicHttpSecurityMode.None"/>, or <see
			/// cref="BasicHttpSecurityMode.TransportCredentialOnly"/>. If you set the <see
			/// cref="WcfBasicHttpAdapter{TAddress,TConfig}.SecurityMode"/> property to <see
			/// cref="BasicHttpSecurityMode.Message"/> or <see
			/// cref="BasicHttpSecurityMode.TransportWithMessageCredential"/>, the WCF-BasicHttp adapter does not use the
			/// credential specified in the <see cref="ProxyUserName"/> and <see cref="ProxyPassword"/> properties for
			/// authentication against the proxy.
			/// </para>
			/// <para>
			/// The WCF-BasicHttp send adapter uses Basic authentication for the proxy.
			/// </para>
			/// <para>
			/// It defaults to an <see cref="string.Empty"/> string.
			/// </para>
			/// </remarks>
			public string ProxyUserName
			{
				get { return _adapterConfig.ProxyUserName; }
				set { _adapterConfig.ProxyUserName = value; }
			}

			/// <summary>
			/// Specify the password to use for the proxy.
			/// </summary>
			/// <remarks>
			/// It defaults to an <see cref="string.Empty"/> string.
			/// </remarks>
			public string ProxyPassword
			{
				get { return _adapterConfig.ProxyPassword; }
				set { _adapterConfig.ProxyPassword = value; }
			}

			#endregion
		}

		#endregion
	}
}

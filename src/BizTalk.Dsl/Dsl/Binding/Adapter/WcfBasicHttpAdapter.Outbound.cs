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
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfBasicHttpAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// You can use the WCF-BasicHttp adapter to do cross-computer communication with legacy ASMX-based Web services
		/// and clients that conform to the WS-I Basic Profile 1.1, using either the HTTP or HTTPS transport with text
		/// encoding. However, you will not be able to take advantage of features that are supported by WS-* protocols.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb246055.aspx">What Is the WCF-BasicHttp Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226467.aspx">How to Configure a WCF-BasicHttp Send Port</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226514.aspx">WCF-BasicHttp Transport Properties Dialog Box, Send, Security Tab</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Outbound : WcfBasicHttpAdapter<BasicHttpTLConfig>,
			IOutboundAdapter,
			IAdapterConfigOutboundAction,
			IAdapterConfigOutboundPropagateFaultMessage,
			IAdapterConfigOutboundCredentials
		{
			public Outbound()
			{
				// Security Tab - Access Control Service Settings
				UseAcsAuthentication = false;

				// Proxy Tab - General Settings
				ProxyToUse = ProxySelection.None;
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

			#region Security Tab - Client Certificate Settings

			/// <summary>
			/// Specify the thumbprint of the X.509 certificate for authenticating this send port to services. This
			/// property is required if the <see cref="WcfBasicHttpAdapter{TConfig}.MessageClientCredentialType"/> property
			/// is set to <see cref="BasicHttpMessageCredentialType.Certificate"/>.
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
			public string ClientCertificate
			{
				get { return _adapterConfig.ClientCertificate; }
				set { _adapterConfig.ClientCertificate = value; }
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
			public string StsUri
			{
				get { return _adapterConfig.StsUri; }
				set { _adapterConfig.StsUri = value; }
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
			/// the <see cref="WcfBasicHttpAdapter{TConfig}.SecurityMode"/> is <see
			/// cref="BasicHttpSecurityMode.Transport"/>, <see cref="BasicHttpSecurityMode.None"/>, or <see
			/// cref="BasicHttpSecurityMode.TransportCredentialOnly"/>. If you set the <see
			/// cref="WcfBasicHttpAdapter{TConfig}.SecurityMode"/> property to <see cref="BasicHttpSecurityMode.Message"/>
			/// or <see cref="BasicHttpSecurityMode.TransportWithMessageCredential"/>, the WCF-BasicHttp adapter does not
			/// use the credential specified in the <see cref="ProxyUserName"/> and <see cref="ProxyPassword"/> properties
			/// for authentication against the proxy.
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

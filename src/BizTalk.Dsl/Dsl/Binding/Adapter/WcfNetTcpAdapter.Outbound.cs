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
			public string ClientCertificate
			{
				get { return _adapterConfig.ClientCertificate; }
				set { _adapterConfig.ClientCertificate = value; }
			}

			#endregion
		}

		#endregion
	}
}

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
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfNetTcpAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The WCF-NetTcp adapter provides connected cross-computer or cross-process communication in an environment in
		/// which both services and clients are WCF based. It provides full access to SOAP security, reliability, and
		/// transaction features. This adapter uses the TCP transport, and messages have binary encoding.
		/// </summary>
		/// <remarks>
		/// You use the WCF-NetTcp receive adapter to receive WCF service requests through the TCP protocol. A receive
		/// location that uses the WCF-NetTcp receive adapter can be configured as one-way or request-response (two-way).
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226419.aspx">What Is the WCF-NetTcp Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226412.aspx">How to Configure a WCF-NetTcp Receive Location</seealso>.
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb246097.aspx">WCF-NetTcp Transport Properties Dialog Box, Receive, Security Tab</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Inbound : WcfNetTcpAdapter<NetTcpRLConfig>,
			IInboundAdapter,
			IAdapterConfigInboundIncludeExceptionDetailInFaults,
			IAdapterConfigInboundSuspendRequestMessageOnFailure,
			IAdapterConfigMaxConcurrentCalls,
			IAdapterConfigServiceCertificate,
			IAdapterConfigSSO
		{
			public Inbound()
			{
				// Binding Tab - Connection Pool Settings
				LeaseTimeout = TimeSpan.FromMinutes(5);

				// Binding Tab - Service Throttling Behavior Settings
				MaxConcurrentCalls = 200;

				// Messages Tab - Error Handling Settings
				SuspendRequestMessageOnFailure = true;
				IncludeExceptionDetailInFaults = true;
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region IAdapterConfigInboundIncludeExceptionDetailInFaults Members

			public bool IncludeExceptionDetailInFaults
			{
				get { return _adapterConfig.IncludeExceptionDetailInFaults; }
				set { _adapterConfig.IncludeExceptionDetailInFaults = value; }
			}

			#endregion

			#region IAdapterConfigInboundSuspendRequestMessageOnFailure Members

			public bool SuspendRequestMessageOnFailure
			{
				get { return _adapterConfig.SuspendMessageOnFailure; }
				set { _adapterConfig.SuspendMessageOnFailure = value; }
			}

			#endregion

			#region IAdapterConfigMaxConcurrentCalls Members

			public int MaxConcurrentCalls
			{
				get { return _adapterConfig.MaxConcurrentCalls; }
				set { _adapterConfig.MaxConcurrentCalls = value; }
			}

			#endregion

			#region IAdapterConfigServiceCertificate Members

			/// <summary>
			/// Specify the thumbprint of the X.509 certificate for this receive location that the clients use to
			/// authenticate the service. The certificate to be used for this property must be installed into the My store
			/// in the Current User location.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You must install the service certificate into the Current User location of the user account for the receive
			/// handler hosting this receive location.
			/// </para>
			/// <para>
			/// It defaults to an <see cref="string.Empty"/> string.
			/// </para>
			/// </remarks>
			public string ServiceCertificate
			{
				get { return _adapterConfig.ServiceCertificate; }
				set { _adapterConfig.ServiceCertificate = value; }
			}

			#endregion

			#region IAdapterConfigSSO Members

			/// <summary>
			/// Specify whether to use Enterprise Single Sign-On (SSO) to retrieve client credentials to issue an SSO
			/// ticket.
			/// </summary>
			/// <remarks>
			/// For more information about the security configurations supporting SSO, see the section, "Enterprise Single
			/// Sign-On Supportability for the WCF-NetTcp Receive Adapter" in <see
			/// href="https://msdn.microsoft.com/en-us/library/bb246097.aspx">WCF-NetTcp Transport Properties Dialog Box,
			/// Receive, Security Tab</see>. Box, Receive, Security Tab.
			/// </remarks>
			public bool UseSSO
			{
				get { return _adapterConfig.UseSSO; }
				set { _adapterConfig.UseSSO = value; }
			}

			#endregion

			#region Binding Tab - Connection Pool Settings

			/// <summary>
			/// Specify the maximum lifetime of an active pooled connection. After the specified time elapses, the
			/// connection closes after the current request is serviced.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The WCF-NetTcp adapter leverages the <see cref="NetTcpBinding"/> class to communicate with an endpoint.
			/// When using the <see cref="NetTcpBinding"/> in load-balanced scenarios, consider reducing the default lease
			/// timeout.
			/// </para>
			/// <para>
			/// It defaults to 00:05:00.
			/// </para>
			/// </remarks>
			public TimeSpan LeaseTimeout
			{
				get { return _adapterConfig.LeaseTimeout; }
				set { _adapterConfig.LeaseTimeout = value; }
			}

			#endregion
		}

		#endregion
	}
}

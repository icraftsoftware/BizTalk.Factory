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
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfWSHttpAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// You can use the WCF-WSHttp adapter to do cross-computer communication with services and clients that can
		/// understand the next-generation Web service standards, using either the HTTP or HTTPS transport with text or
		/// Message Transmission Optimization Mechanism (MTOM) encoding. The WCF-WSHttp adapter provides full access to
		/// the SOAP security, reliability, and transaction features.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245971.aspx">What Is the WCF-WSHttp Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226482.aspx">How to Configure a WCF-WSHttp Receive Location</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226411.aspx">WCF-WSHttp Transport Properties Dialog Box, Receive, Security Tab</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Inbound : WcfWSHttpAdapter<WSHttpRLConfig>,
			IInboundAdapter,
			IAdapterConfigMaxConcurrentCalls,
			IAdapterConfigInboundIncludeExceptionDetailInFaults,
			IAdapterConfigInboundSuspendRequestMessageOnFailure
		{
			public Inbound()
			{
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

			#region Security Tab - Security Mode Settings

			/// <summary>
			/// Specify whether to use Enterprise Single Sign-On (SSO) to retrieve client credentials to issue an SSO
			/// ticket.
			/// </summary>
			/// <remarks>
			/// <para>
			/// For more information about the security configurations supporting SSO, see the section "Enterprise Single
			/// Sign-On Supportability for the WCF-WSHttp Receive Adapter" in <see
			/// href="https://msdn.microsoft.com/en-us/library/bb226411.aspx">WCF-WSHttp Transport Properties Dialog Box,
			/// Receive, Security Tab</see>.
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
		}

		#endregion
	}
}

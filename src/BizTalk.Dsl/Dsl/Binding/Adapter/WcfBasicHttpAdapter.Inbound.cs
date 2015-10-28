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
	public abstract partial class WcfBasicHttpAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// You can use the WCF-BasicHttp adapter to do cross-computer communication with legacy ASMX-based Web services
		/// and clients that conform to the WS-I Basic Profile 1.1, using either the HTTP or HTTPS transport with text
		/// encoding. However, you will not be able to take advantage of features that are supported by WS-* protocols.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb246055.aspx">What Is the WCF-BasicHttp Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb246064.aspx">How to Configure a WCF-BasicHttp Receive Location</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226322.aspx">WCF-BasicHttp Transport Properties Dialog Box, Receive, Security Tab</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Inbound : WcfBasicHttpAdapter<BasicHttpRLConfig>,
			IInboundAdapter,
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

			#region Binding Tab - Service Throttling Behaviour Settings

			/// <summary>
			/// Specify the number of concurrent calls to a single service instance. Calls in excess of the limit are
			/// queued.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The range of this property is from 1 to <see cref="int.MaxValue"/>.
			/// </para>
			/// <para>
			/// It defaults to 200.
			/// </para>
			/// </remarks>
			public int MaxConcurrentCalls
			{
				get { return _adapterConfig.MaxConcurrentCalls; }
				set { _adapterConfig.MaxConcurrentCalls = value; }
			}

			#endregion

			#region Security Tab - Security Mode Settings

			/// <summary>
			/// Specify whether to use Enterprise Single Sign-On (SSO) to retrieve client credentials to issue an SSO
			/// ticket. For more information about the security configurations supporting SSO, see the section "Enterprise
			/// Single Sign-On Supportability for the WCF-BasicHttp Receive Adapter" in <see
			/// href="https://msdn.microsoft.com/en-us/library/bb226322.aspx">WCF-BasicHttp Transport Properties Dialog
			/// Box, Receive, Security Tab</see>.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
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

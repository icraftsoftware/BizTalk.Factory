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
	public abstract partial class WcfNetNamedPipeAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The WCF-NetNamedPipe adapter provides cross-process communication on the same computer in an environment in
		/// which both services and clients are WCF based. It provides full access to SOAP reliability and transaction
		/// features. The adapter uses the named pipe transport, and messages have binary encoding. This adapter cannot be
		/// used in cross-computer communication.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226493.aspx">What Is the WCF-NetNamedPipe Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb259943.aspx">How to Configure a WCF-NetNamedPipe Receive Location</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226347.aspx">WCF-NetNamedPipe Transport Properties Dialog Box, Receive, Security Tab</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Inbound : WcfNetNamedPipeAdapter<NetNamedPipeRLConfig>,
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
			/// ticket.
			/// </summary>
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

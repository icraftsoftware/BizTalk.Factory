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
		#region Nested Type: Outbound

		/// <summary>
		/// The WCF-NetNamedPipe adapter provides cross-process communication on the same computer in an environment in
		/// which both services and clients are WCF based. It provides full access to SOAP reliability and transaction
		/// features. The adapter uses the named pipe transport, and messages have binary encoding. This adapter cannot be
		/// used in cross-computer communication.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226493.aspx">What Is the WCF-NetNamedPipe Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb246110.aspx">How to Configure a WCF-NetNamedPipe Send Port</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb259936.aspx">WCF-NetNamedPipe Transport Properties Dialog Box, Send, Security Tab</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Outbound : WcfNetNamedPipeAdapter<NetNamedPipeTLConfig>,
			IOutboundAdapter,
			IAdapterConfigOutboundAction,
			IAdapterConfigOutboundPropagateFaultMessage
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

			#region IAdapterConfigOutboundPropagateFaultMessage Members

			public bool PropagateFaultMessage
			{
				get { return _adapterConfig.PropagateFaultMessage; }
				set { _adapterConfig.PropagateFaultMessage = value; }
			}

			#endregion
		}

		#endregion
	}
}

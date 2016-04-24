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
using Microsoft.BizTalk.Adapter.ServiceBus;
using Microsoft.ServiceBus;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfNetTcpRelayAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// Microsoft BizTalk Server uses the WCF-NetTcpRelay adapter when receiving and sending WCF service requests
		/// through the <see cref="NetTcpRelayBinding"/>. The WCF-NetTcpRelay adapter enables you to send and receive
		/// messages from the Service Bus relay endpoints using the <see cref="NetTcpRelayBinding"/>.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/jj572847.aspx">WCF-NetTcpRelay Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/jj572849.aspx">How to Configure a WCF-NetTcpRelay Send Port</seealso>
		public class Outbound : WcfNetTcpRelayAdapter<NetTcpRelayTLConfig>,
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

			#region Security Tab - Client Certificate Settings

			/// <summary>
			/// Specify the thumbprint of the X.509 certificate for authenticating this send port to services. This
			/// property is required if the <see cref="WcfNetTcpRelayAdapter{TConfig}.MessageClientCredentialType"/>
			/// property is set to <see cref="MessageCredentialType.Certificate"/>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The certificate to be used for this property must be installed into the My store in the Current User
			/// location of the user account for the send handler hosting this send port.
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

			#endregion	}
		}

		#endregion
	}
}

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
	public abstract partial class WcfBasicHttpRelayAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// Microsoft BizTalk Server uses the WCF-BasicHttpRelay adapter when receiving and sending WCF service requests
		/// through the <see cref="BasicHttpRelayBinding"/>. The WCF-BasicHttpRelay adapter enables you to send and
		/// receive messages from the Service Bus relay endpoints using the <see cref="BasicHttpRelayBinding"/>.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/jj572839.aspx">WCF-BasicHttpRelay Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/jj572837.aspx">How to Configure a WCF-BasicHttpRelay Receive Location</seealso>
		public class Inbound : WcfBasicHttpRelayAdapter<BasicHttpRelayRLConfig>,
			IAdapterConfigAccessControlService,
			IAdapterConfigInboundSuspendRequestMessageOnFailure,
			IAdapterConfigInboundIncludeExceptionDetailInFaults,
			IAdapterConfigMaxConcurrentCalls
		{
			public Inbound()
			{
				// Binding Tab - Encoding Settings
				MessageEncoding = WSMessageEncoding.Text;

				// Binding Tab - Service Throttling Behavior Settings
				MaxConcurrentCalls = 200;

				// Security Tab - Client Security Settings
				RelayClientAuthenticationType = RelayClientAuthenticationType.RelayAccessToken;

				// Messages Tab - Error Handling Settings
				SuspendRequestMessageOnFailure = true;
				IncludeExceptionDetailInFaults = true;
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
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

			#region Security Tab - Service Discovery

			/// <summary>
			/// Specify whether the behavior of the service is published in the Service Registry.
			/// </summary>
			public bool EnableServiceDiscovery
			{
				get { return _adapterConfig.EnableServiceDiscovery; }
				set { _adapterConfig.EnableServiceDiscovery = value; }
			}

			/// <summary>
			/// Specify the name with which the service is published to the Service Registry.
			/// </summary>
			public string ServiceDisplayName
			{
				get { return _adapterConfig.ServiceDisplayName; }
				set { _adapterConfig.ServiceDisplayName = value; }
			}

			/// <summary>
			/// Set the discovery mode for the service published in the Service Registry.
			/// </summary>
			/// <remarks>
			/// For more information about the discovery modes, see <see cref="DiscoveryType"/>.
			/// </remarks>
			public DiscoveryType DiscoveryMode
			{
				get { return _adapterConfig.DiscoveryMode; }
				set { _adapterConfig.DiscoveryMode = value; }
			}

			#endregion
		}

		#endregion
	}
}

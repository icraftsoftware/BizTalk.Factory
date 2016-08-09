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
using Microsoft.BizTalk.Adapter.SBMessaging;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class SBMessagingAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The SB-Messaging adapter allows to send and receive messages from Service Bus entities like Queues, Topics,
		/// and Relays. You can use the SB-Messaging adapters to bridge the connectivity between Windows Azure and
		/// on-premises BizTalk Server, thereby enabling users to create a typical hybrid application.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/jj572852.aspx">SB-Messaging Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/jj572840.aspx">How to Configure an SB-Messaging Receive Location</seealso>
		public class Inbound : SBMessagingAdapter<SBMessagingRLConfig>, IInboundAdapter
		{
			public Inbound() { }

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region General Tab

			/// <summary>
			/// Specifies a time span value that indicates the time for a receive operation to complete.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>10</c> minutes.
			/// </remarks>
			public TimeSpan ReceiveTimeout
			{
				get { return _adapterConfig.ReceiveTimeout; }
				set { _adapterConfig.ReceiveTimeout = value; }
			}

			/// <summary>
			/// Specifies the number of messages that are received simultaneously from the Service Bus Queue or a topic.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Prefetching enables the queue or subscription client to load additional messages from the service when it
			/// performs a receive operation. The client stores these messages in a local cache. The size of the cache is
			/// determined by the value for the Prefetch Count property you specify here.
			/// </para>
			/// <para>
			/// For more information, refer to the section "Prefetching" at <see
			/// href="http://msdn.microsoft.com/library/windowsazure/hh528527.aspx#_prefetching">Best Practices for
			/// performance improvements using Service Bus brokered messaging</see>.
			/// </para>
			/// <para>
			/// It defaults to <c>-1</c>.
			/// </para>
			/// </remarks>
			public int PrefetchCount
			{
				get { return _adapterConfig.PrefetchCount; }
				set { _adapterConfig.PrefetchCount = value; }
			}

			/// <summary>
			/// Whether to use a Service Bus session to receive messages from a queue or a subscription.
			/// </summary>
			public bool IsSessionful
			{
				get { return _adapterConfig.IsSessionful; }
				set { _adapterConfig.IsSessionful = value; }
			}

			#endregion

			#region Properties Tab

			/// <summary>
			/// Specify the namespace that the adapter uses to write the brokered message properties as message context
			/// properties on the message received by BizTalk Server.
			/// </summary>
			public string CustomBrokeredPropertyNamespace
			{
				get { return _adapterConfig.CustomBrokeredPropertyNamespace; }
				set { _adapterConfig.CustomBrokeredPropertyNamespace = value; }
			}

			/// <summary>
			/// Specify whether to promote the brokered message properties.
			/// </summary>
			public bool PromoteCustomProperties
			{
				get { return _adapterConfig.PromoteCustomProperties; }
				set { _adapterConfig.PromoteCustomProperties = value; }
			}

			#endregion
		}

		#endregion
	}
}

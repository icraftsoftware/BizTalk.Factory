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
using Microsoft.ServiceBus.Messaging;
using SBMessaging;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class SBMessagingAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// The SB-Messaging adapter allows to send and receive messages from Service Bus entities like Queues, Topics,
		/// and Relays. You can use the SB-Messaging adapters to bridge the connectivity between Windows Azure and
		/// on-premises BizTalk Server, thereby enabling users to create a typical hybrid application.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/jj572852.aspx">SB-Messaging Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/jj572838.aspx">How to Configure an SB-Messaging Send Port</seealso>
		public class Outbound : SBMessagingAdapter<SBMessagingTLConfig>
		{
			public Outbound() { }

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region Properties Tab

			/// <summary>
			/// Specify the namespace that contains the BizTalk message context properties that you want to write as
			/// user-defined Brokered Message properties on the outgoing message sent to the Service Bus Queue.
			/// </summary>
			/// <remarks>
			/// All the properties that belong to the namespace are written to the message as user-defined Brokered Message
			/// properties. The adapter ignores the namespace while writing the properties as Brokered Message properties.
			/// It uses the namespace only to ascertain what properties to write.
			/// </remarks>
			public string CustomBrokeredPropertyNamespace
			{
				get { return _adapterConfig.CustomBrokeredPropertyNamespace; }
				set { _adapterConfig.CustomBrokeredPropertyNamespace = value; }
			}

			/// <summary>
			/// Specify the identifier of the correlation.
			/// </summary>
			/// <seealso href="https://msdn.microsoft.com/library/azure/microsoft.servicebus.messaging.brokeredmessage_properties.aspx">BrokeredMessage Properties</seealso>
			public string DefaultCorrelationId
			{
				get { return _adapterConfig.DefaultCorrelationId; }
				set { _adapterConfig.DefaultCorrelationId = value; }
			}

			/// <summary>
			/// Specify the application specific label.
			/// </summary>
			/// <seealso href="https://msdn.microsoft.com/library/azure/microsoft.servicebus.messaging.brokeredmessage_properties.aspx">BrokeredMessage Properties</seealso>
			public string DefaultLabel
			{
				get { return _adapterConfig.DefaultLabel; }
				set { _adapterConfig.DefaultLabel = value; }
			}

			/// <summary>
			/// Specify the identifier of the message.
			/// </summary>
			/// <remarks>
			/// This is a user-defined value that Service Bus can use to identify duplicate messages, if enabled.
			/// </remarks>
			/// <seealso href="https://msdn.microsoft.com/library/azure/microsoft.servicebus.messaging.brokeredmessage_properties.aspx">BrokeredMessage Properties</seealso>
			public string DefaultMessageId
			{
				get { return _adapterConfig.DefaultMessageId; }
				set { _adapterConfig.DefaultMessageId = value; }
			}

			/// <summary>
			/// Specify
			/// </summary>
			/// <seealso href="https://msdn.microsoft.com/library/azure/microsoft.servicebus.messaging.brokeredmessage_properties.aspx">BrokeredMessage Properties</seealso>
			public string DefaultReplyTo
			{
				get { return _adapterConfig.DefaultReplyTo; }
				set { _adapterConfig.DefaultReplyTo = value; }
			}

			/// <summary>
			/// Specify the address of the queue to reply to.
			/// </summary>
			/// <seealso href="https://msdn.microsoft.com/library/azure/microsoft.servicebus.messaging.brokeredmessage_properties.aspx">BrokeredMessage Properties</seealso>
			public string DefaultReplyToSessionId
			{
				get { return _adapterConfig.DefaultReplyToSessionId; }
				set { _adapterConfig.DefaultReplyToSessionId = value; }
			}

			/// <summary>
			/// Specify the date and time in UTC at which the message will be enqueued.
			/// </summary>
			/// <remarks>
			/// This property returns the time in UTC; when setting the property, the supplied <see cref="DateTime"/> value
			/// must also be in UTC.
			/// </remarks>
			/// <seealso href="https://msdn.microsoft.com/library/azure/microsoft.servicebus.messaging.brokeredmessage_properties.aspx">BrokeredMessage Properties</seealso>
			public DateTime DefaultScheduledEnqueueTimeUtc
			{
				get { return _adapterConfig.DefaultScheduledEnqueueTimeUtc; }
				set { _adapterConfig.DefaultScheduledEnqueueTimeUtc = value; }
			}

			/// <summary>
			/// Specify the identifier of the session.
			/// </summary>
			/// <seealso href="https://msdn.microsoft.com/library/azure/microsoft.servicebus.messaging.brokeredmessage_properties.aspx">BrokeredMessage Properties</seealso>
			public string DefaultSessionId
			{
				get { return _adapterConfig.DefaultSessionId; }
				set { _adapterConfig.DefaultSessionId = value; }
			}

			/// <summary>
			/// Specify the message's time to live value.
			/// </summary>
			/// <remarks>
			/// <para>
			/// This is the duration after which the message expires, starting from when the message is sent to the Service
			/// Bus.
			/// </para>
			/// <para>
			/// Messages older than their <see cref="TimeToLive"/> value will expire and no longer be retained in the
			/// message store. Subscribers will be unable to receive expired messages.
			/// </para>
			/// <para>
			/// <see cref="TimeToLive"/> is the maximum lifetime that a message can receive, but its value cannot exceed
			/// the specified <see cref="QueueDescription.DefaultMessageTimeToLive"/> value on the destination queue or
			/// subscription. If a lower <see cref="TimeToLive"/> value is specified, it will be applied to the individual
			/// message. However, a larger value specified on the message will be overridden by the entity's <see
			/// cref="QueueDescription.DefaultMessageTimeToLive"/> value.
			/// </para>
			/// <para>
			/// It defaults to <see cref="TimeSpan.MaxValue"/>.
			/// </para>
			/// </remarks>
			/// <seealso href="https://msdn.microsoft.com/library/azure/microsoft.servicebus.messaging.brokeredmessage_properties.aspx">BrokeredMessage Properties</seealso>
			public TimeSpan DefaultTimeToLive
			{
				get { return _adapterConfig.DefaultTimeToLive; }
				set { _adapterConfig.DefaultTimeToLive = value; }
			}

			#endregion

			#region General Tab

			/// <summary>
			/// Specifies a time span value that indicates the interval when the message batches being sent to a Queue or a
			/// Topic are flushed.
			/// </summary>
			/// <remarks>
			/// <para>
			/// For more information about batching with respect to Service Bus Queues and Topics, see the Client-side
			/// batching section at <see
			/// href="http://msdn.microsoft.com/library/windowsazure/hh528527.aspx#_client-side-batching">Best Practices
			/// for performance improvements using Service Bus brokered messaging</see>.
			/// </para>
			/// <para>
			/// It defaults to <c>20</c> milliseconds.
			/// </para>
			/// </remarks>
			public TimeSpan BatchFlushInterval
			{
				get { return _adapterConfig.BatchFlushInterval; }
				set { _adapterConfig.BatchFlushInterval = value; }
			}

			/// <summary>
			/// Specifies a time span value that indicates the time for a send operation to complete.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>10</c> minutes.
			/// </remarks>
			public TimeSpan SendTimeout
			{
				get { return _adapterConfig.SendTimeout; }
				set { _adapterConfig.SendTimeout = value; }
			}

			#endregion
		}

		#endregion
	}
}

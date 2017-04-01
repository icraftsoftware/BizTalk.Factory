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
	public abstract partial class WcfNetMsmqAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// The WCF-NetMsmq adapter provides disconnected cross-computer communication by using queuing technology in an
		/// environment where both the services and clients are WCF based. It uses the Message Queuing (MSMQ) transport,
		/// and messages have binary encoding.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb246091.aspx">What Is the WCF-NetMsmq Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245965.aspx">How to Configure a WCF-NetMsmq Send Port</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb246035.aspx">WCF-NetMsmq Transport Properties Dialog Box, Send, Security Tab</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Outbound : WcfNetMsmqAdapter<NetMsmqTLConfig>,
			IOutboundAdapter,
			IAdapterConfigClientCertificate,
			IAdapterConfigNetMsmqBinding,
			IAdapterConfigOutboundAction,
			IAdapterConfigOutboundCredentials,
			IAdapterConfigOutboundMessageMarshalling
		{
			public Outbound()
			{
				// Binding Tab - Queue Settings
				TimeToLive = TimeSpan.FromDays(1);
				UseSourceJournal = false;
				DeadLetterQueue = DeadLetterQueue.System;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region IAdapterConfigClientCertificate Members

			/// <summary>
			/// Specify the thumbprint of the X.509 certificate for authenticating this send port to services. This
			/// property is required if the <see cref="WcfNetMsmqAdapter{TConfig}.MessageClientCredentialType"/> property
			/// is set to <see cref="MessageCredentialType.Certificate"/>.
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

			#endregion

			#region IAdapterConfigNetMsmqBinding Members

			/// <summary>
			/// Specify a time span for how long the messages are valid before they are expired and put into the
			/// dead-letter queue.
			/// </summary>
			/// <remarks>
			/// <para>
			/// This property is set to ensure that time-sensitive messages do not become stale before they are processed
			/// by this send port. A message in a queue that is not consumed by this send port within the time interval
			/// specified is said to be expired. Expired messages are sent to special queue called the dead-letter queue.
			/// The location of the dead-letter queue is set with the <see cref="DeadLetterQueue"/> property.
			/// </para>
			/// <para>
			/// It defaults to 1 day.
			/// </para>
			/// </remarks>
			public TimeSpan TimeToLive
			{
				get { return _adapterConfig.TimeToLive; }
				set { _adapterConfig.TimeToLive = value; }
			}

			/// <summary>
			/// Specify whether copies of messages processed by this send port should be stored in the source journal queue.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			public bool UseSourceJournal
			{
				get { return _adapterConfig.UseSourceJournal; }
				set { _adapterConfig.UseSourceJournal = value; }
			}

			/// <summary>
			/// Specify the dead-letter queue where messages that have failed to be delivered to the application will be
			/// transferred.
			/// </summary>
			/// <remarks>
			/// <para>
			/// <list type="bullet">
			/// <item>
			/// <see cref="System.ServiceModel.DeadLetterQueue.None"/> &#8212; No dead-letter queue is to be used.
			/// </item>
			/// <item>
			/// <see cref="System.ServiceModel.DeadLetterQueue.System"/> &#8212; Use the system-wide dead-letter queue.
			/// </item>
			/// <item>
			/// <see cref="System.ServiceModel.DeadLetterQueue.Custom"/> &#8212; Use a custom dead-letter queue.
			/// </item>
			/// </list>
			/// For more information about the messages delivered to the dead-letter queue, see <see
			/// href="https://msdn.microsoft.com/en-us/library/bb246101.aspx">WCF-NetMsmq Transport Properties Dialog Box,
			/// Send, Binding Tab</see>.
			/// </para>
			/// <para>
			/// The custom dead-letter queue is supported only in Message Queuing (MSMQ) 4.0, released with Windows Vista.
			/// </para>
			/// <para>
			/// It defaults to <see cref="System.ServiceModel.DeadLetterQueue.System"/>
			/// </para>
			/// </remarks>
			public DeadLetterQueue DeadLetterQueue
			{
				get { return _adapterConfig.DeadLetterQueue; }
				set { _adapterConfig.DeadLetterQueue = value; }
			}

			/// <summary>
			/// Specify the fully qualified URI with the net.msmq scheme for the location of the per-application
			/// dead-letter queue, where messages that have expired or that have failed transfer or delivery are placed.
			/// </summary>
			/// <remarks>
			/// <para>
			/// For example, <c>net.msmq://localhost/deadLetterQueueName</c>. The dead-letter queue is a queue on the queue
			/// manager of the sending application for expired messages that have failed to be delivered. This property is
			/// required if the <see cref="DeadLetterQueue"/> property is set to <see
			/// cref="System.ServiceModel.DeadLetterQueue.Custom"/>.
			/// </para>
			/// <para>
			/// It defaults to <see cref="string.Empty"/>
			/// </para>
			/// </remarks>
			public string CustomDeadLetterQueue
			{
				get { return _adapterConfig.CustomDeadLetterQueue; }
				set { _adapterConfig.CustomDeadLetterQueue = value; }
			}

			#endregion

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

			#region IAdapterConfigOutboundMessageMarshalling Members

			/// <summary>
			/// Specify the data selection for the SOAP Body element of outgoing WCF messages.
			/// </summary>
			/// <remarks>
			/// <para>
			/// <list type="bullet">
			/// <item>
			/// <see cref="OutboundMessageBodySelection.UseBodyElement"/> &#8212; Use the BizTalk message body part to create
			/// the content of the SOAP Body element for an outgoing message.
			/// </item>
			/// <item>
			/// <see cref="OutboundMessageBodySelection.UseTemplate"/> &#8212; Use the template supplied in the <see
			/// cref="OutboundXmlTemplate"/> property to create the content of the SOAP Body element for an outgoing message.
			/// </item>
			/// </list>
			/// </para>
			/// <para>
			/// For more information about how to use the <see cref="OutboundBodyLocation"/> property, see <see
			/// href="https://msdn.microsoft.com/en-us/library/bb226478.aspx">Specifying the Message Body for the WCF
			/// Adapters</see>.
			/// </para>
			/// <para>
			/// For send port, this property is valid only for solicit-response ports.
			/// </para>
			/// <para>
			/// It defaults to <see cref="OutboundMessageBodySelection.UseBodyElement"/>.
			/// </para>
			/// </remarks>
			public OutboundMessageBodySelection OutboundBodyLocation
			{
				get { return _adapterConfig.OutboundBodyLocation; }
				set { _adapterConfig.OutboundBodyLocation = value; }
			}

			/// <summary>
			/// Specify the XML-formatted template for the content of the SOAP Body element of an outgoing response message.
			/// This property is required if the <see cref="OutboundBodyLocation"/> property is set to <see
			/// cref="OutboundMessageBodySelection.UseTemplate"/>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// For more information about how to use the <see cref="OutboundXmlTemplate"/> property, see <see
			/// href="https://msdn.microsoft.com/en-us/library/bb226478.aspx">Specifying the Message Body for the WCF
			/// Adapters</see>.
			/// </para>
			/// <para>
			/// For send port, this property is valid only for solicit-response ports.
			/// </para>
			/// <para>
			/// It defaults to <see cref="string.Empty"/>.
			/// </para>
			/// </remarks>
			public string OutboundXmlTemplate
			{
				get { return _adapterConfig.OutboundXmlTemplate; }
				set { _adapterConfig.OutboundXmlTemplate = value; }
			}

			#endregion
		}

		#endregion
	}
}

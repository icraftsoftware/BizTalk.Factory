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
	public abstract partial class WcfNetMsmqAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The WCF-NetMsmq adapter provides disconnected cross-computer communication by using queuing technology in an
		/// environment where both the services and clients are WCF based. It uses the Message Queuing (MSMQ) transport,
		/// and messages have binary encoding.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb246091.aspx">What Is the WCF-NetMsmq Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb259976.aspx">How to Configure a WCF-NetMsmq Receive Location</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226546.aspx">WCF-NetMsmq Transport Properties Dialog Box, Receive, Security Tab</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Inbound : WcfNetMsmqAdapter<NetMsmqRLConfig>,
			IInboundAdapter,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigInboundDisableLocationOnFailure,
			IAdapterConfigInboundSuspendRequestMessageOnFailure,
			IAdapterConfigMaxConcurrentCalls,
			IAdapterConfigMaxReceivedMessageSize
		{
			public Inbound()
			{
				// Binding Tab - General Settings
				MaxReceivedMessageSize = ushort.MaxValue;

				// Binding Tab - Message Order Settings
				OrderedProcessing = false;

				// Binding Tab - Service Throttling Behavior Settings
				MaxConcurrentCalls = 200;

				// Messages Tab - Error Handling Settings
				DisableLocationOnFailure = false;
				SuspendRequestMessageOnFailure = true;
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region IAdapterConfigInboundDisableLocationOnFailure Members

			public bool DisableLocationOnFailure
			{
				get { return _adapterConfig.DisableLocationOnFailure; }
				set { _adapterConfig.DisableLocationOnFailure = value; }
			}

			#endregion

			#region IAdapterConfigInboundMessageMarshalling Members

			/// <summary>
			/// Specify the data selection for the SOAP Body element of incoming WCF messages.
			/// </summary>
			/// <remarks>
			/// <para>
			/// <list type="bullet">
			/// <item>
			/// <see cref="InboundMessageBodySelection.UseBodyElement"/> &#8212; Use the content of the SOAP Body element of
			/// an incoming message to create the BizTalk message body part. If the Body element has more than one child
			/// element, only the first element becomes the BizTalk message body part.
			/// </item>
			/// <item>
			/// <see cref="InboundMessageBodySelection.UseBodyPath"/> &#8212; Use the body path expression in the <see
			/// cref="InboundBodyPathExpression"/> property to create the BizTalk message body part. The body path expression
			/// is evaluated against the immediate child element of the SOAP Body element of an incoming message. This
			/// property is valid only for solicit-response ports.
			/// </item>
			/// <item>
			/// <see cref="InboundMessageBodySelection.UseEnvelope"/> &#8212; Create the BizTalk message body part from the
			/// entire SOAP Envelope of an incoming message.
			/// </item>
			/// </list>
			/// </para>
			/// <para>
			/// For more information about how to use the <see cref="InboundBodyLocation"/> property, see <see
			/// href="https://msdn.microsoft.com/en-us/library/bb226478.aspx">Specifying the Message Body for the WCF
			/// Adapters</see>.
			/// </para>
			/// <para>
			/// It defaults to <see cref="InboundMessageBodySelection.UseBodyElement"/>.
			/// </para>
			/// </remarks>
			public InboundMessageBodySelection InboundBodyLocation
			{
				get { return _adapterConfig.InboundBodyLocation; }
				set { _adapterConfig.InboundBodyLocation = value; }
			}

			/// <summary>
			/// Specify the body path expression to identify a specific part of an incoming message used to create the BizTalk
			/// message body part. This body path expression is evaluated against the immediate child element of the SOAP Body
			/// node of an incoming message. If this body path expression returns more than one node, only the first node is
			/// chosen for the BizTalk message body part. This property is required if the <see cref="InboundBodyLocation"/>
			/// property is set to <see cref="InboundMessageBodySelection.UseBodyPath"/>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// For more information about how to use the <see cref="InboundBodyPathExpression"/> property, see <see
			/// href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and
			/// Properties</see>.
			/// </para>
			/// <para></para>
			/// <para>
			/// For send port, this property is valid only for solicit-response ports.
			/// </para>
			/// <para>
			/// It defaults to <see cref="string.Empty"/>.
			/// </para>
			/// </remarks>
			public string InboundBodyPathExpression
			{
				get { return _adapterConfig.InboundBodyPathExpression; }
				set { _adapterConfig.InboundBodyPathExpression = value; }
			}

			/// <summary>
			/// Specify the type of encoding that the WCF-NetTcp send adapter uses to decode for the node identified by the
			/// XPath specified in <see cref="InboundBodyPathExpression"/>. This property is required if the <see
			/// cref="InboundBodyLocation"/> property is set to <see cref="InboundMessageBodySelection.UseBodyPath"/>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// <list type="bullet">
			/// <item>
			/// <see cref="MessageBodyFormat.Base64"/> &#8212; Base64 encoding.
			/// </item>
			/// <item>
			/// <see cref="MessageBodyFormat.Hex"/> &#8212; Hexadecimal encoding.
			/// </item>
			/// <item>
			/// <see cref="MessageBodyFormat.String"/> &#8212; UTF-8 Text encoding.
			/// </item>
			/// <item>
			/// <see cref="MessageBodyFormat.Xml"/> &#8212; The WCF adapters create the BizTalk message body with the outer
			/// XML of the node selected by the body path expression in <see cref="InboundBodyPathExpression"/>.
			/// </item>
			/// </list>
			/// </para>
			/// <para>
			/// For send port, this property is valid only for solicit-response ports.
			/// </para>
			/// <para>
			/// It defaults to <see cref="MessageBodyFormat.Xml"/>.
			/// </para>
			/// </remarks>
			public MessageBodyFormat InboundNodeEncoding
			{
				get { return _adapterConfig.InboundNodeEncoding; }
				set { _adapterConfig.InboundNodeEncoding = value; }
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

			#region IAdapterConfigMaxReceivedMessageSize Members

			public int MaxReceivedMessageSize
			{
				get { return _adapterConfig.MaxReceivedMessageSize; }
				set { _adapterConfig.MaxReceivedMessageSize = value; }
			}

			#endregion

			#region Binding Tab - Message Order Settings

			/// <summary>
			/// Specify whether to process messages serially.
			/// </summary>
			/// <remarks>
			/// <para>
			/// When this property is selected, this receive location accommodates ordered message delivery when used in
			/// conjunction with a BizTalk messaging or orchestration send port that has the <c>Ordered Delivery</c> option
			/// set to <c>True</c>. You can select this only when the <see
			/// cref="WcfNetMsmqAdapter{TConfig}.EnableTransaction"/> property is set to <c>True</c>.
			/// </para>
			/// <para>
			/// When this property is set to <c>True</c>, the WCF-NetMsmq receive location optimizes resource usage when
			/// handling large messages by making the adapter single-threaded.
			/// </para>
			/// <para>
			/// It defaults to <c>False</c>.
			/// </para>
			/// </remarks>
			public bool OrderedProcessing
			{
				get { return _adapterConfig.OrderedProcessing; }
				set { _adapterConfig.OrderedProcessing = value; }
			}

			#endregion
		}

		#endregion
	}
}

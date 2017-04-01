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
using System.Net.Mime;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class HttpAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// You use the HTTP adapter to exchange information between Microsoft BizTalk Server and an application by means
		/// of the HTTP protocol. HTTP is the primary protocol for interbusiness message exchange. Applications can send
		/// messages to a server by sending HTTP POST or HTTP GET requests to a specified HTTP URL. The HTTP adapter
		/// receives the HTTP requests and submits them to BizTalk Server for processing. Similarly, BizTalk Server can
		/// transmit messages to remote applications by sending HTTP POST requests to a specified HTTP URL.
		/// </summary>
		/// <remarks>
		/// The HTTP receive adapter is a Microsoft Internet Information Services (IIS) Internet Server Application
		/// Programming Interface (ISAPI) extension that the IIS process hosts, and controls the receive locations that
		/// use the HTTP adapter.
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-US/library/aa577953.aspx">HTTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-US/library/aa560119.aspx">Configuring the HTTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-US/library/aa561368.aspx">HTTP Receive Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-US/library/aa547842.aspx">How to Configure an HTTP Receive Handler</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-US/library/aa559072.aspx">How to Configure IIS for an HTTP Receive Location</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-US/library/aa561370.aspx">How to Configure an HTTP Receive Location</seealso>
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		public class Inbound : HttpAdapter, IInboundAdapter, IAdapterConfigInboundSuspendRequestMessageOnFailure, IAdapterConfigSSO
		{
			public Inbound()
			{
				ResponseContentType = MediaTypeNames.Text.Xml;
				ReturnCorrelationHandle = true;
				SuspendRequestMessageOnFailure = true;
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region IAdapterConfigInboundSuspendRequestMessageOnFailure Members

			/// <summary>
			/// Indicate whether or not to suspend HTTP requests that fail inbound processing.
			/// </summary>
			/// <remarks>
			/// <para>
			/// A value of <c>False</c> indicates to discard the failed request and send an error status code (401 or 500)
			/// to the client.
			/// </para>
			/// <para>
			/// A value of <c>True</c> indicates to suspend the failed request and send an "Accepted" status code (202) to
			/// the client for one-way receive ports or an "Error" status code (500) to the client for two-way receive
			/// ports.
			/// </para>
			/// <para>
			/// It defaults to <c>True</c>.
			/// </para>
			/// </remarks>
			public bool SuspendRequestMessageOnFailure { get; set; }

			#endregion

			#region IAdapterConfigSSO Members

			/// <summary>
			/// Specifies whether the HTTP adapter will issue the SSO ticket to messages that arrive on this receive
			/// location.
			/// </summary>
			/// <remarks>
			/// <para>
			/// If this option is enabled then you must also enable the option to Allow Tickets at the SSO System level.
			/// The Allow Tickets option is configurable on the Options tab of the SSO System Properties dialog box
			/// available in the SSO Administration MMC interface. If this option is enabled and the Allow Tickets option
			/// at the SSO System level is not enabled then any messages received by this receive location will be
			/// suspended.
			/// </para>
			/// <para>
			/// It defaults to <c>False</c>.
			/// </para>
			/// </remarks>
			public bool UseSSO { get; set; }

			#endregion

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				return Path;
			}

			protected override string GetPublicAddress()
			{
				return Url.IfNotNull(u => u.ToString());
			}

			protected override void Save(IPropertyBag propertyBag)
			{
				propertyBag.WriteAdapterCustomProperty("LoopBack", LoopBack);
				propertyBag.WriteAdapterCustomProperty("ResponseContentType", ResponseContentType);
				propertyBag.WriteAdapterCustomProperty("ReturnCorrelationHandle", ReturnCorrelationHandle);
				propertyBag.WriteAdapterCustomProperty("SuspendFailedRequests", SuspendRequestMessageOnFailure);
				propertyBag.WriteAdapterCustomProperty("UseSSO", UseSSO);
			}

			protected override void Validate() { }

			#endregion

			/// <summary>
			/// Specifies that the request message received on this location will be routed either to a send port or back
			/// to the receive location to be sent as a response. This property is valid only for request-response receive
			/// ports. It is ignored for one-way receive ports.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			public bool LoopBack { get; set; }

			/// <summary>
			/// Specify the name of the virtual directory where you post the messages received by the HTTP/HTTPS receive
			/// location. The virtual directory includes the name of the receive location DLL and an optional query string.
			/// </summary>
			/// <remarks>
			/// This location must not contain more than one BTSHTTPReceive.dll ISAPI extension, including all subfolders.
			/// </remarks>
			/// <example>
			/// <para>
			/// Examples of virtual directory names are:
			/// </para>
			/// <para>
			/// <![CDATA[/<virtual directory>/BTSHTTPReceive.dll]]>
			/// </para>
			/// <para>
			/// <![CDATA[/<virtual directory>/BTSHTTPReceive.dll?Purchase%20Order]]>
			/// </para>
			/// </example>
			public string Path { get; set; }

			/// <summary>
			/// Content type of the HTTP response messages that the HTTP adapter sends back to clients from this receive
			/// location. This property is valid only for request-response receive ports and is ignored for one-way receive
			/// ports.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="MediaTypeNames.Text.Xml"/>.
			/// </remarks>
			public string ResponseContentType { get; set; }

			/// <summary>
			/// Specifies that the correlation token of submitted message that the HTTP adapter sends on HTTP response to
			/// the client if the submission is successful. This property is valid only for one-way receive ports and is
			/// ignored for request-response receive ports.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>True</c>.
			/// </remarks>
			public bool ReturnCorrelationHandle { get; set; }

			/// <summary>
			/// Specify the fully qualified URI for this receive location. The value for this property is a combination of
			/// the server name and the virtual directory. The BizTalk Messaging Engine exposes this address to external
			/// partners. The specified URI should designate the public Web site URL for trading partners to connect to
			/// when sending messages to BizTalk Server.
			/// </summary>
			/// <remarks>
			/// This information is optional and is not used by BizTalk Server. This parameter is available to allow
			/// administrators to document the public URL that the receive location is tied to.
			/// </remarks>
			public Uri Url { get; set; }
		}

		#endregion
	}
}

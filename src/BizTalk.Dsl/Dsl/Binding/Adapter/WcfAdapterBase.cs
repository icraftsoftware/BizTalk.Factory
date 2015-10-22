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

using System.Diagnostics.CodeAnalysis;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	// see What Are the WCF Adapters?, https://msdn.microsoft.com/en-us/library/bb245975.aspx
	// see WCF Adapters Property Schema and Properties, https://msdn.microsoft.com/en-us/library/bb245991.aspx
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public abstract class WcfAdapterBase<TAddress, TConfig> : AdapterBase
		where TConfig : AdapterConfig, IAdapterConfigIdentity, IAdapterConfigInboundMessageMarshalling, IAdapterConfigOutboundMessageMarshalling, new()
	{
		protected WcfAdapterBase(ProtocolType protocolType) : base(protocolType)
		{
			_adapterConfig = new TConfig();
		}

		#region Base Class Member Overrides

		protected override string GetAddress()
		{
			// because EndpointAddress or ConnectionUri's ToString() internally calls Uri.ToString()
			return Address.ToString();
		}

		protected override void Save(IPropertyBag propertyBag)
		{
			_adapterConfig.Save(propertyBag as Microsoft.BizTalk.ExplorerOM.IPropertyBag);
		}

		protected override void Validate()
		{
			// TODO identity
			// TODO outbound & inbound msg config (body location, ...): outbound only for SP, inbound only for RL, both for 2-way
			_adapterConfig.Validate();
		}

		#endregion

		protected readonly TConfig _adapterConfig;

		#region General Tab - Endpoint Address Settings

		public TAddress Address { get; set; }

		// TODO ?? could we use a strong type instead of a string ??
		public string Identity
		{
			get { return _adapterConfig.Identity; }
			set { _adapterConfig.Identity = value; }
		}

		#endregion

		#region Messages Tab - Inbound BizTalk Message Body Settings

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

		#region Messages Tab - Outbound WCF Message Body Settings

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
}

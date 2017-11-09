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

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public interface IAdapterConfigOutboundAction
	{
		#region General Tab - SOAP Action Header Settings

		/// <summary>
		/// Specify the SOAPAction header field for outgoing messages. This property can also be set through the message
		/// context property <see cref="WCF.Action"/> in a pipeline or orchestration.
		/// </summary>
		/// <remarks>
		/// <para>
		/// You can specify this value in two different ways: the single action format and the action mapping format.
		/// </para>
		/// <para>
		/// If you set this property in the single action format &#8212; for example,
		/// <![CDATA[http://contoso.com/svc/operation1]]>&#8212;the SOAPAction header for outgoing messages is always set
		/// to the value specified in this property.
		/// </para>
		/// <para>
		/// If you set this property in the action mapping format, the outgoing SOAPAction header is determined by the
		/// <see cref="BTS.Operation"/> context property. For example, if this property is set to the following XML format
		/// and the <see cref="BTS.Operation"/> property is set to <c>operation1</c>, the WCF send adapter uses
		/// <![CDATA[http://contoso.com/svc/operation1]]> for the outgoing SOAPAction header.
		/// <code><![CDATA[<BtsActionMapping>
		///   <Operation Name="operation1" Action="http://contoso.com/svc/operation1" />
		///   <Operation Name="operation2" Action="http://contoso.com/svc/operation2" />
		/// </BtsActionMapping>]]></code>
		/// </para>
		/// <para>
		/// If outgoing messages come from an orchestration port, orchestration instances dynamically set the
		/// BTS.Operation property with the operation name of the port. If outgoing messages are routed with content-based
		/// routing, you can set the <see cref="BTS.Operation"/> property in pipeline components.
		/// </para>
		/// <para>
		/// It defaults to an <see cref="string.Empty"/> string.
		/// </para>
		/// </remarks>
		/// <example>
		/// <list type="number">
		/// <item>
		/// <term>
		/// How to configure a static action corresponding to single constant SOAP action:
		/// </term>
		/// <description>
		/// <code><![CDATA[
		/// adapter.StaticAction = "http://services.stateless.be/biztalk/factory/mail/1.1/IMailService/SendMessage";
		/// ]]></code>
		/// </description>
		/// </item>
		/// <item>
		/// <term>
		/// How to configure action mapping from operations to SOAP actions:
		/// </term>
		/// <description>
		/// <code><![CDATA[
		/// using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Metadata;
		/// 
		/// ...
		/// 
		/// adapter.StaticAction = new ActionMapping {
		///   new ActionMappingOperation("UpdateIntervention", "http://Microsoft.LobServices.OracleDB/2007/03/TICKETING/Procedure/UPDATE_INTERVENTION"),
		///   new ActionMappingOperation("UpdateOperation", "http://Microsoft.LobServices.OracleDB/2007/03/TICKETING/Procedure/UPDATE_OPERATION")
		/// }; ]]></code>
		/// </description>
		/// </item>
		/// </list>
		/// </example>
		string StaticAction { get; set; }

		#endregion
	}
}

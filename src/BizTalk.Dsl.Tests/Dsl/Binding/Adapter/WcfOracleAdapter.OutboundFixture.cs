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

using System.Transactions;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Metadata;
using Microsoft.Adapters.OracleDB;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfOracleAdapterOutboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var ooa = new WcfOracleAdapter.Outbound(
				a => {
					a.Address = new OracleDBConnectionUri { DataSourceName = "TNS" };
					a.IsolationLevel = IsolationLevel.ReadCommitted;
					a.OutboundBodyLocation = OutboundMessageBodySelection.UseBodyElement;
					a.PropagateFaultMessage = true;
					a.StaticAction = new ActionMapping {
						new ActionMappingOperation("CreateTicket", "http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/CREATE_TICKET"),
						new ActionMappingOperation("UpdateTicket", "http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/UPDATE_TICKET")
					};
					a.UserName = "Scott";
					a.Password = "Tiger";
				});
			var xml = ((IAdapterBindingSerializerFactory) ooa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<BindingType vt=\"8\">oracleDBBinding</BindingType>" +
						"<BindingConfiguration vt=\"8\">" +
						"&lt;binding name=\"oracleDBBinding\" " +
						"dataFetchSize=\"65535\" " +
						"enableBizTalkCompatibilityMode=\"true\" /&gt;" +
						"</BindingConfiguration>" +
						"<EndpointBehaviorConfiguration vt=\"8\">&lt;behavior name=\"EndpointBehavior\" /&gt;" + "</EndpointBehaviorConfiguration>" +
						"<StaticAction vt=\"8\">" +
						"&lt;BtsActionMapping&gt;" +
						"&lt;Operation Name=\"CreateTicket\" Action=\"http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/CREATE_TICKET\" /&gt;" +
						"&lt;Operation Name=\"UpdateTicket\" Action=\"http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/UPDATE_TICKET\" /&gt;" +
						"&lt;/BtsActionMapping&gt;" +
						"</StaticAction>" +
						"<UseSSO vt=\"11\">0</UseSSO>" +
						"<UserName vt=\"8\">Scott</UserName>" +
						"<Password vt=\"8\">Tiger</Password>" +
						"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
						"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
						"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
						"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
						"<PropagateFaultMessage vt=\"11\">-1</PropagateFaultMessage>" +
						"<EnableTransaction vt=\"11\">-1</EnableTransaction>" +
						"<IsolationLevel vt=\"8\">ReadCommitted</IsolationLevel>" +
						"</CustomProps>"));
		}

		[Test]
		[Ignore("TODO")]
		public void Validate()
		{
			Assert.Fail("TODO");
		}

		[Test]
		public void ValidateDoesNotThrow()
		{
			var ooa = new WcfOracleAdapter.Outbound(
				a => {
					a.Address = new OracleDBConnectionUri { DataSourceName = "TNS" };
					a.IsolationLevel = IsolationLevel.ReadCommitted;
					a.OutboundBodyLocation = OutboundMessageBodySelection.UseBodyElement;
					a.PropagateFaultMessage = true;
					a.StaticAction = new ActionMapping {
						new ActionMappingOperation("CreateTicket", "http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/CREATE_TICKET"),
						new ActionMappingOperation("UpdateTicket", "http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/UPDATE_TICKET")
					};
					a.UserName = "Scott";
					a.Password = "Tiger";
				});

			Assert.That(() => ((ISupportValidation) ooa).Validate(), Throws.Nothing);
		}
	}
}

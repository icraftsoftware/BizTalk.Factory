#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.Transactions;
using Microsoft.Adapters.Sql;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfSqlAdapterOutboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var osa = new WcfSqlAdapter.Outbound(
				a => {
					a.Address = new SqlAdapterConnectionUri { InboundId = "AvailableBatches", Server = "localhost", InitialCatalog = "BizTalkFactoryTransientStateDb" };
					a.IsolationLevel = IsolationLevel.ReadCommitted;
					a.OutboundBodyLocation = OutboundMessageBodySelection.UseBodyElement;
					a.PropagateFaultMessage = true;
					a.SendTimeout = TimeSpan.FromMinutes(2);
					a.StaticAction = "TypedProcedure/dbo/usp_batch_AddPart";
				});
			var xml = ((IAdapterBindingSerializerFactory) osa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<BindingType vt=\"8\">sqlBinding</BindingType>" +
						"<BindingConfiguration vt=\"8\">"
						+ "&lt;binding name=\"sqlBinding\" sendTimeout=\"00:02:00\" /&gt;"
						+ "</BindingConfiguration>" +
						"<EndpointBehaviorConfiguration vt=\"8\">&lt;behavior name=\"EndpointBehavior\" /&gt;" + "</EndpointBehaviorConfiguration>" +
						"<StaticAction vt=\"8\">TypedProcedure/dbo/usp_batch_AddPart</StaticAction>" +
						"<UseSSO vt=\"11\">0</UseSSO>" +
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
			var osa = new WcfSqlAdapter.Outbound(
				a => {
					a.Address = new SqlAdapterConnectionUri { InboundId = "AvailableBatches", Server = "localhost", InitialCatalog = "BizTalkFactoryTransientStateDb" };
					a.IsolationLevel = IsolationLevel.ReadCommitted;
					a.OutboundBodyLocation = OutboundMessageBodySelection.UseBodyElement;
					a.PropagateFaultMessage = true;
					a.StaticAction = "TypedProcedure/dbo/usp_batch_AddPart";
				});

			Assert.That(() => ((ISupportValidation) osa).Validate(), Throws.Nothing);
		}
	}
}

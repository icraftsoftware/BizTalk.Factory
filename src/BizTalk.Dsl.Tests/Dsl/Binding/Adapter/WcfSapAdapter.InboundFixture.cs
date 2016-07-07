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
using System.Data.SqlClient;
using Microsoft.Adapters.SAP;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfSapAdapterInboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var isa = new WcfSapAdapter.Inbound(
				a => {
					a.Address = new SAPConnectionUri {
						ApplicationServerHost = "appHost",
						ConnectionType = OutboundConnectionType.A,
						Client = "100",
						Language = "FR",
						ListenerGwHost = "gwHost",
						ListenerGwServ = "gwServer",
						ProgramId = "listenerProgramId"
					};
					a.CredentialType = CredentialSelection.UserAccount;
					a.Password = "p@ssw0rd";
					a.UserName = "BTS_USER";
					a.MaxConnectionsPerSystem = 30;
					a.ReceiveTimeout = TimeSpan.MaxValue;
					a.TidDatabaseConnectionString = new SqlConnectionStringBuilder {
						DataSource = "localhost",
						InitialCatalog = "BizTalkFactoryTransientStateDb",
						IntegratedSecurity = true
					}.ToString();
				});
			var xml = ((IAdapterBindingSerializerFactory) isa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<BindingType vt=\"8\">sapBinding</BindingType>" +
						"<BindingConfiguration vt=\"8\">" +
						"&lt;binding name=\"sapBinding\" " +
						"receiveTimeout=\"Infinite\" " +
						"enableBizTalkCompatibilityMode=\"true\" " +
						"tidDatabaseConnectionString=\"Data Source=localhost;Initial Catalog=BizTalkFactoryTransientStateDb;Integrated Security=True\" " +
						"maxConnectionsPerSystem=\"30\" /&gt;" +
						"</BindingConfiguration>" +
						"<ServiceBehaviorConfiguration vt=\"8\">&lt;behavior name=\"ServiceBehavior\" /&gt;</ServiceBehaviorConfiguration>" +
						"<EndpointBehaviorConfiguration vt=\"8\">&lt;behavior name=\"EndpointBehavior\" /&gt;" + "</EndpointBehaviorConfiguration>" +
						"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
						"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
						"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
						"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
						"<DisableLocationOnFailure vt=\"11\">0</DisableLocationOnFailure>" +
						"<SuspendMessageOnFailure vt=\"11\">-1</SuspendMessageOnFailure>" +
						"<IncludeExceptionDetailInFaults vt=\"11\">-1</IncludeExceptionDetailInFaults>" +
						"<CredentialType vt=\"8\">UserAccount</CredentialType>" +
						"<UserName vt=\"8\">BTS_USER</UserName>" +
						"<Password vt=\"8\">p@ssw0rd</Password>" +
						"<OrderedProcessing vt=\"11\">0</OrderedProcessing>" +
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
			var isa = new WcfSapAdapter.Inbound(
				a => {
					a.Address = new SAPConnectionUri {
						ApplicationServerHost = "appHost",
						ConnectionType = OutboundConnectionType.A,
						Client = "100",
						Language = "FR",
						ListenerGwHost = "gwHost",
						ListenerGwServ = "gwServer",
						ProgramId = "listenerProgramId"
					};
					a.CredentialType = CredentialSelection.UserAccount;
					a.Password = "p@ssw0rd";
					a.UserName = "BTS_USER";
					a.MaxConnectionsPerSystem = 30;
					a.ReceiveTimeout = TimeSpan.MaxValue;
					a.TidDatabaseConnectionString = new SqlConnectionStringBuilder {
						DataSource = "localhost",
						InitialCatalog = "BizTalkFactoryTransientStateDb",
						IntegratedSecurity = true
					}.ToString();
				});

			Assert.That(() => ((ISupportValidation) isa).Validate(), Throws.Nothing);
		}
	}
}

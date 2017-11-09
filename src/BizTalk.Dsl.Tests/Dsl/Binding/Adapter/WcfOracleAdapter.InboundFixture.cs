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
using System.Transactions;
using Microsoft.Adapters.OracleDB;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfOracleAdapterInboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var ioa = new WcfOracleAdapter.Inbound(
				a => {
					a.Address = new OracleDBConnectionUri { DataSourceName = "TNS", PollingId = "ticket" };
					a.InboundOperationType = InboundOperation.Polling;
					a.PolledDataAvailableStatement = "SELECT COUNT(1) FROM EVENTS";
					a.PollingInterval = TimeSpan.FromHours(2);
					a.PollingStatement = "SELECT KEY FROM EVENTS FOR UPDATE";
					a.PostPollStatement = "DELETE FROM EVENTS";
					a.UserName = "Scott";
					a.Password = "Tiger";
					a.ServiceBehaviors = new[] {
						new OracleDBInboundTransactionBehavior {
							TransactionIsolationLevel = IsolationLevel.ReadCommitted
						}
					};
				});
			var xml = ((IAdapterBindingSerializerFactory) ioa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<BindingType vt=\"8\">oracleDBBinding</BindingType>" +
						"<BindingConfiguration vt=\"8\">" +
						"&lt;binding name=\"oracleDBBinding\" " +
						"pollingStatement=\"SELECT KEY FROM EVENTS FOR UPDATE\" " +
						"postPollStatement=\"DELETE FROM EVENTS\" " +
						"pollingInterval=\"7200\" " +
						"polledDataAvailableStatement=\"SELECT COUNT(1) FROM EVENTS\" " +
						"enableBizTalkCompatibilityMode=\"true\" /&gt;" +
						"</BindingConfiguration>" +
						"<ServiceBehaviorConfiguration vt=\"8\">" +
						"&lt;behavior name=\"ServiceBehavior\"&gt;" +
						"&lt;oracleDBAdapterInboundTransactionBehavior transactionIsolationLevel=\"ReadCommitted\" /&gt;" +
						"&lt;/behavior&gt;" +
						"</ServiceBehaviorConfiguration>" +
						"<EndpointBehaviorConfiguration vt=\"8\">&lt;behavior name=\"EndpointBehavior\" /&gt;" + "</EndpointBehaviorConfiguration>" +
						"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
						"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
						"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
						"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
						"<DisableLocationOnFailure vt=\"11\">0</DisableLocationOnFailure>" +
						"<SuspendMessageOnFailure vt=\"11\">-1</SuspendMessageOnFailure>" +
						"<IncludeExceptionDetailInFaults vt=\"11\">-1</IncludeExceptionDetailInFaults>" +
						"<CredentialType vt=\"8\">None</CredentialType>" +
						"<UserName vt=\"8\">Scott</UserName>" +
						"<Password vt=\"8\">Tiger</Password>" +
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
			var ioa = new WcfOracleAdapter.Inbound(
				a => {
					a.Address = new OracleDBConnectionUri { DataSourceName = "TNS", PollingId = "ticket" };
					a.InboundOperationType = InboundOperation.Polling;
					a.PolledDataAvailableStatement = "SELECT COUNT(1) FROM EVENTS";
					a.PollingInterval = TimeSpan.FromHours(2);
					a.PollingStatement = "SELECT KEY FROM EVENTS FOR UPDATE";
					a.PostPollStatement = "DELETE FROM EVENTS";
					a.UserName = "Scott";
					a.Password = "Tiger";
					a.ServiceBehaviors = new[] {
						new OracleDBInboundTransactionBehavior {
							TransactionIsolationLevel = IsolationLevel.ReadCommitted
						}
					};
				});

			Assert.That(() => ((ISupportValidation) ioa).Validate(), Throws.Nothing);
		}
	}
}

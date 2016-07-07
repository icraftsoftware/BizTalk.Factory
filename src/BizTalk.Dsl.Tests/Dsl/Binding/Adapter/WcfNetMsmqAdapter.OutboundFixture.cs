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

using System.ServiceModel;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfNetMsmqAdapterOutboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var nma = new WcfNetMsmqAdapter.Outbound(
				a => {
					a.Address = new EndpointAddress("net.msmq://localhost/private/service_queue");
					a.SecurityMode = NetMsmqSecurityMode.Message;
					a.UseSourceJournal = true;
					a.DeadLetterQueue = DeadLetterQueue.Custom;
					a.CustomDeadLetterQueue = "net.msmq://localhost/deadLetterQueueName";
					a.StaticAction = "http://biztalk.stateless.be/action";
				});
			var xml = ((IAdapterBindingSerializerFactory) nma).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<TimeToLive vt=\"8\">1.00:00:00</TimeToLive>" +
						"<UseSourceJournal vt=\"11\">-1</UseSourceJournal>" +
						"<DeadLetterQueue vt=\"8\">Custom</DeadLetterQueue>" +
						"<CustomDeadLetterQueue vt=\"8\">net.msmq://localhost/deadLetterQueueName</CustomDeadLetterQueue>" +
						"<EnableTransaction vt=\"11\">-1</EnableTransaction>" +
						"<SecurityMode vt=\"8\">Message</SecurityMode>" +
						"<MessageClientCredentialType vt=\"8\">Windows</MessageClientCredentialType>" +
						"<AlgorithmSuite vt=\"8\">Basic256</AlgorithmSuite>" +
						"<MsmqAuthenticationMode vt=\"8\">WindowsDomain</MsmqAuthenticationMode>" +
						"<MsmqProtectionLevel vt=\"8\">Sign</MsmqProtectionLevel>" +
						"<MsmqSecureHashAlgorithm vt=\"8\">Sha1</MsmqSecureHashAlgorithm>" +
						"<MsmqEncryptionAlgorithm vt=\"8\">RC4Stream</MsmqEncryptionAlgorithm>" +
						"<UseSSO vt=\"11\">0</UseSSO>" +
						"<StaticAction vt=\"8\">http://biztalk.stateless.be/action</StaticAction>" +
						"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
						"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
						"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
						"<SendTimeout vt=\"8\">00:01:00</SendTimeout>" +
						"<CloseTimeout vt=\"8\">00:01:00</CloseTimeout>" +
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
			var nma = new WcfNetMsmqAdapter.Outbound(
				a => {
					a.Address = new EndpointAddress("net.msmq://localhost/private/service_queue");
					a.SecurityMode = NetMsmqSecurityMode.Message;
					a.UseSourceJournal = true;
					a.DeadLetterQueue = DeadLetterQueue.Custom;
					a.CustomDeadLetterQueue = "net.msmq://localhost/deadLetterQueueName";
					a.StaticAction = "http://biztalk.stateless.be/action";
				});

			Assert.That(() => ((ISupportValidation) nma).Validate(), Throws.Nothing);
		}
	}
}

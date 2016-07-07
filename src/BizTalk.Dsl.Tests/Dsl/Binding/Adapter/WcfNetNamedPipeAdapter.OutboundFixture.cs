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

using System.Net.Security;
using System.ServiceModel;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfNetNamedPipeAdapterOutboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var npa = new WcfNetNamedPipeAdapter.Outbound(
				a => {
					a.Address = new EndpointAddress("net.pipe://localhost/biztalk.factory/service.svc");
					a.SecurityMode = NetNamedPipeSecurityMode.Transport;
					a.TransportProtectionLevel = ProtectionLevel.EncryptAndSign;
				});
			var xml = ((IAdapterBindingSerializerFactory) npa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<MaxReceivedMessageSize vt=\"3\">65535</MaxReceivedMessageSize>" +
						"<EnableTransaction vt=\"11\">0</EnableTransaction>" +
						"<TransactionProtocol vt=\"8\">OleTransactions</TransactionProtocol>" +
						"<SecurityMode vt=\"8\">Transport</SecurityMode>" +
						"<TransportProtectionLevel vt=\"8\">EncryptAndSign</TransportProtectionLevel>" +
						"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
						"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
						"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
						"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
						"<PropagateFaultMessage vt=\"11\">-1</PropagateFaultMessage>" +
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
			var npa = new WcfNetNamedPipeAdapter.Outbound(
				a => {
					a.Address = new EndpointAddress("net.pipe://localhost/biztalk.factory/service.svc");
					a.SecurityMode = NetNamedPipeSecurityMode.Transport;
					a.TransportProtectionLevel = ProtectionLevel.EncryptAndSign;
				});

			Assert.That(() => ((ISupportValidation) npa).Validate(), Throws.Nothing);
		}
	}
}

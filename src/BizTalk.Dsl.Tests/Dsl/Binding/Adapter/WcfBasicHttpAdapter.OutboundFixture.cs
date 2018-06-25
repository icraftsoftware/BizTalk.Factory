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
using System.ServiceModel;
using System.Text;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfBasicHttpAdapterOutboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var bha = new WcfBasicHttpAdapter.Outbound(
				a => {
					a.Address = new EndpointAddress("http://localhost/biztalk.factory/service.svc");
					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("service_spn");
					a.SecurityMode = BasicHttpSecurityMode.Message;
					a.SendTimeout = TimeSpan.FromMinutes(2);
					a.TextEncoding = Encoding.Unicode;
				});
			var xml = ((IAdapterBindingSerializerFactory) bha).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<MaxReceivedMessageSize vt=\"3\">65535</MaxReceivedMessageSize>" +
						"<MessageEncoding vt=\"8\">Text</MessageEncoding>" +
						"<TextEncoding vt=\"8\">utf-16</TextEncoding>" +
						"<SecurityMode vt=\"8\">Message</SecurityMode>" +
						"<MessageClientCredentialType vt=\"8\">UserName</MessageClientCredentialType>" +
						"<AlgorithmSuite vt=\"8\">Basic256</AlgorithmSuite>" +
						"<TransportClientCredentialType vt=\"8\">None</TransportClientCredentialType>" +
						"<UseAcsAuthentication vt=\"11\">0</UseAcsAuthentication>" +
						"<UseSSO vt=\"11\">0</UseSSO>" +
						"<ProxyToUse vt=\"8\">None</ProxyToUse>" +
						"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
						"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
						"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
						"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
						"<PropagateFaultMessage vt=\"11\">-1</PropagateFaultMessage>" +
						"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
						"<SendTimeout vt=\"8\">00:02:00</SendTimeout>" +
						"<CloseTimeout vt=\"8\">00:01:00</CloseTimeout>" +
						"<Identity vt=\"8\">&lt;identity&gt;\r\n  &lt;servicePrincipalName value=\"service_spn\" /&gt;\r\n&lt;/identity&gt;</Identity>" +
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
			var bha = new WcfBasicHttpAdapter.Outbound(
				a => {
					a.Address = new EndpointAddress("http://localhost/biztalk.factory/service.svc");
					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("service_spn");
					a.MessageClientCredentialType = BasicHttpMessageCredentialType.Certificate;
					a.SecurityMode = BasicHttpSecurityMode.Message;
					a.TextEncoding = Encoding.Unicode;
				});

			Assert.That(() => ((ISupportValidation) bha).Validate(), Throws.Nothing);
		}
	}
}

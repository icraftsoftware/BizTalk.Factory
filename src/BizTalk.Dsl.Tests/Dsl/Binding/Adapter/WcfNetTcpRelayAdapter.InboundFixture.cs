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
using System.ServiceModel;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfNetTcpRelayAdapterInboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var wnt = new WcfNetTcpRelayAdapter.Inbound(
				a => {
					a.Identity = EndpointIdentity.CreateSpnIdentity("spn_name");
					a.MaxConcurrentCalls = 201;
					a.MaxReceivedMessageSize = 64512;

					a.SuspendRequestMessageOnFailure = true;
					a.IncludeExceptionDetailInFaults = true;

					a.StsUri = new Uri("https://biztalk.factory-sb.accesscontrol.windows.net/");
					a.IssuerName = "issuer_name";
					a.IssuerSecret = "issuer_secret";

					a.EnableServiceDiscovery = true;
					a.ServiceDisplayName = "display_name";
				});
			var xml = ((IAdapterBindingSerializerFactory) wnt).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<MaxReceivedMessageSize vt=\"3\">64512</MaxReceivedMessageSize>" +
						"<SecurityMode vt=\"8\">Transport</SecurityMode>" +
						"<MessageClientCredentialType vt=\"8\">Windows</MessageClientCredentialType>" +
						"<AlgorithmSuite vt=\"8\">Basic256</AlgorithmSuite>" +
						"<TransportProtectionLevel vt=\"8\">EncryptAndSign</TransportProtectionLevel>" +
						"<RelayClientAuthenticationType vt=\"8\">RelayAccessToken</RelayClientAuthenticationType>" +
						"<LeaseTimeout vt=\"8\">00:00:00</LeaseTimeout>" +
						"<UseSSO vt=\"11\">0</UseSSO>" +
						"<MaxConcurrentCalls vt=\"3\">201</MaxConcurrentCalls>" +
						"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
						"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
						"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
						"<OutboundXmlTemplate vt=\"8\">" + (
							"&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;") +
						"</OutboundXmlTemplate>" +
						"<SuspendMessageOnFailure vt=\"11\">-1</SuspendMessageOnFailure>" +
						"<IncludeExceptionDetailInFaults vt=\"11\">-1</IncludeExceptionDetailInFaults>" +
						"<StsUri vt=\"8\">https://biztalk.factory-sb.accesscontrol.windows.net/</StsUri>" +
						"<IssuerName vt=\"8\">issuer_name</IssuerName>" +
						"<IssuerSecret vt=\"8\">issuer_secret</IssuerSecret>" +
						"<EnableServiceDiscovery vt=\"11\">-1</EnableServiceDiscovery>" +
						"<DiscoveryMode vt=\"8\">Public</DiscoveryMode>" +
						"<ServiceDisplayName vt=\"8\">display_name</ServiceDisplayName>" +
						"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
						"<SendTimeout vt=\"8\">00:01:00</SendTimeout>" +
						"<CloseTimeout vt=\"8\">00:01:00</CloseTimeout>" +
						"<Identity vt=\"8\">" + (
							"&lt;identity&gt;\r\n  &lt;servicePrincipalName value=\"spn_name\" /&gt;\r\n&lt;/identity&gt;") +
						"</Identity>" +
						"</CustomProps>"))
				;
		}

		[Test]
		public void Validate()
		{
			var wnt = new WcfNetTcpRelayAdapter.Inbound(
				a => { a.Address = new EndpointAddress("https://biztalk.factory.servicebus.windows.net/batch-queue"); });
			Assert.That(
				() => ((ISupportValidation) wnt).Validate(),
				Throws.TypeOf<ArgumentException>()
					.With.InnerException.TypeOf<ArgumentException>()
					.With.InnerException.Message.StartsWith("Invalid address scheme; expecting \"sb\" scheme."));
		}
	}
}

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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using NUnit.Framework;
using CustomBindingElement = Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration.CustomBindingElement;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfCustomAdapterOutboundFixture
	{
		[Test]
		public void SerializeCustomBindingToXml()
		{
			var wca = new WcfCustomAdapter.Outbound<CustomBindingElement>(
				a => {
					a.OpenTimeout = TimeSpan.FromMinutes(33);
					a.SendTimeout = TimeSpan.FromMinutes(2);
					a.Binding.Add(
						new MtomMessageEncodingElement {
							MessageVersion = MessageVersion.Soap11,
							ReaderQuotas = { MaxStringContentLength = 7340032 }
						},
						new HttpsTransportElement {
							MaxReceivedMessageSize = 7340032,
							MaxBufferSize = 7340032,
							UseDefaultWebProxy = false,
							RequireClientCertificate = true
						});
				});
			var xml = ((IAdapterBindingSerializerFactory) wca).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<BindingType vt=\"8\">customBinding</BindingType>" +
						"<BindingConfiguration vt=\"8\">" + (
							"&lt;binding name=\"customDslBinding\" openTimeout=\"00:33:00\" sendTimeout=\"00:02:00\"&gt;" +
								"&lt;mtomMessageEncoding messageVersion=\"Soap11\"&gt;" +
								"&lt;readerQuotas maxStringContentLength=\"7340032\" /&gt;" +
								"&lt;/mtomMessageEncoding&gt;" +
								"&lt;httpsTransport maxReceivedMessageSize=\"7340032\" maxBufferSize=\"7340032\" useDefaultWebProxy=\"false\" requireClientCertificate=\"true\" /&gt;" +
								"&lt;/binding&gt;") +
						"</BindingConfiguration>" +
						"<EndpointBehaviorConfiguration vt=\"8\">&lt;behavior name=\"EndpointBehavior\" /&gt;" + "</EndpointBehaviorConfiguration>" +
						"<UseSSO vt=\"11\">0</UseSSO>" +
						"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
						"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
						"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
						"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
						"<PropagateFaultMessage vt=\"11\">-1</PropagateFaultMessage>" +
						"<EnableTransaction vt=\"11\">0</EnableTransaction>" +
						"<IsolationLevel vt=\"8\">Serializable</IsolationLevel>" +
						"</CustomProps>"));
		}

		[Test]
		public void SerializeToXml()
		{
			var wca = new WcfCustomAdapter.Outbound<NetTcpBindingElement>(
				a => {
					const int tenMegaBytes = 1024 * 1024 * 10;
					a.Address = new EndpointAddress("net.tcp://localhost/biztalk.factory/service.svc");
					a.Binding.MaxReceivedMessageSize = tenMegaBytes;
					a.Binding.ReaderQuotas.MaxArrayLength = tenMegaBytes;
					a.Binding.ReaderQuotas.MaxStringContentLength = tenMegaBytes;
					a.Binding.Security.Mode = SecurityMode.Transport;
					a.Binding.Security.Transport.ProtectionLevel = ProtectionLevel.Sign;
					a.Binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
					a.StaticAction = "http://services.biztalk.net/mail/2011/11/IMailService/SendMessage";
				});
			var xml = ((IAdapterBindingSerializerFactory) wca).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<BindingType vt=\"8\">netTcpBinding</BindingType>" +
						"<BindingConfiguration vt=\"8\">" +
						"&lt;binding name=\"netTcpBinding\" maxReceivedMessageSize=\"10485760\"&gt;" +
						"&lt;readerQuotas maxStringContentLength=\"10485760\" maxArrayLength=\"10485760\" /&gt;" +
						"&lt;security&gt;&lt;transport protectionLevel=\"Sign\" /&gt;&lt;/security&gt;" +
						"&lt;/binding&gt;" +
						"</BindingConfiguration>" +
						"<EndpointBehaviorConfiguration vt=\"8\">&lt;behavior name=\"EndpointBehavior\" /&gt;" + "</EndpointBehaviorConfiguration>" +
						"<StaticAction vt=\"8\">http://services.biztalk.net/mail/2011/11/IMailService/SendMessage" +
						"</StaticAction><UseSSO vt=\"11\">0</UseSSO>" +
						"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
						"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
						"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
						"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
						"<PropagateFaultMessage vt=\"11\">-1</PropagateFaultMessage>" +
						"<EnableTransaction vt=\"11\">0</EnableTransaction>" +
						"<IsolationLevel vt=\"8\">Serializable</IsolationLevel>" +
						"</CustomProps>"));
		}

		[Test]
		[Ignore("TODO")]
		public void Validate()
		{
			Assert.Fail("TODO");
		}

		[Test]
		public void ValidateCustomBasicHttpBindingWithTransportSecurity()
		{
			var wca = new WcfCustomAdapter.Outbound<BasicHttpBindingElement>(
				a => {
					a.Address = new EndpointAddress("https://services.stateless.be/soap/default");
					a.Binding.Security.Mode = BasicHttpSecurityMode.Transport;
					a.Binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
					a.EndpointBehaviors = new[] {
						new ClientCredentialsElement {
							ClientCertificate = {
								FindValue = "*.stateless.be",
								StoreLocation = StoreLocation.LocalMachine,
								StoreName = StoreName.My,
								X509FindType = X509FindType.FindBySubjectName
							}
						}
					};
					a.Identity = EndpointIdentityFactory.CreateCertificateIdentity(
						StoreLocation.LocalMachine,
						StoreName.TrustedPeople,
						X509FindType.FindBySubjectDistinguishedName,
						"*.services.party.be");
				});
			Assert.That(() => ((ISupportValidation) wca).Validate(), Throws.Nothing);
		}

		[Test]
		public void ValidateCustomBinding()
		{
			var wca = new WcfCustomAdapter.Outbound<CustomBindingElement>(
				a => {
					a.Address = new EndpointAddress("https://localhost/biztalk.factory/service.svc");
					a.Binding.Add(
						new MtomMessageEncodingElement { MessageVersion = MessageVersion.Soap11 },
						new HttpsTransportElement { RequireClientCertificate = true });
				});
			Assert.That(() => ((ISupportValidation) wca).Validate(), Throws.Nothing);
		}

		[Test]
		public void ValidateDoesNotThrow()
		{
			var wca = new WcfCustomAdapter.Outbound<NetTcpBindingElement>(
				a => {
					const int tenMegaBytes = 1024 * 1024 * 10;
					a.Address = new EndpointAddress("net.tcp://localhost/biztalk.factory/service.svc");
					a.Binding.MaxReceivedMessageSize = tenMegaBytes;
					a.Binding.ReaderQuotas.MaxArrayLength = tenMegaBytes;
					a.Binding.ReaderQuotas.MaxStringContentLength = tenMegaBytes;
					a.Binding.Security.Mode = SecurityMode.Transport;
					a.Binding.Security.Transport.ProtectionLevel = ProtectionLevel.Sign;
					a.Binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
					a.StaticAction = "http://services.biztalk.net/mail/2011/11/IMailService/SendMessage";
				});

			Assert.That(() => ((ISupportValidation) wca).Validate(), Throws.Nothing);
		}
	}
}

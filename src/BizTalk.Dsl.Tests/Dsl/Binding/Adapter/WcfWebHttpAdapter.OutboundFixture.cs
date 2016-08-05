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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Metadata;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using NUnit.Framework;
using WebHttpSecurityMode = Microsoft.BizTalk.Adapter.Wcf.Config.WebHttpSecurityMode;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfWebHttpAdapterOutboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var wha = new WcfWebHttpAdapter.Outbound(
				a => {
					a.Address = new EndpointAddress("https://localhost/dummy.svc");

					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("spn_name");
					a.SecurityMode = WebHttpSecurityMode.Transport;
					a.ServiceCertificate = "thumbprint";
					a.TransportClientCredentialType = HttpClientCredentialType.Basic;
					a.UserName = "username";
					a.Password = "p@ssw0rd";

					a.UseAcsAuthentication = true;
					a.StsUri = new Uri("https://localhost/swt_token_issuer");
					a.IssuerName = "issuer_name";
					a.IssuerSecret = "secret";

					a.SuppressMessageBodyForHttpVerbs = "GET";
					a.HttpHeaders = "Content-Type: application/json";
					a.HttpUrlMapping = new HttpUrlMapping {
						new HttpUrlMappingOperation("AddCustomer", "POST", "/Customer/{id}"),
						new HttpUrlMappingOperation("DeleteCustomer", "DELETE", "/Customer/{id}")
					};
					a.VariableMapping = new VariableMapping {
						new VariablePropertyMapping("id", BizTalkFactoryProperties.ReceiverName)
					};

					a.MaxReceivedMessageSize = 2048;
				});
			var xml = ((IAdapterBindingSerializerFactory) wha).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<MaxReceivedMessageSize vt=\"3\">2048</MaxReceivedMessageSize>" +
						"<SecurityMode vt=\"8\">Transport</SecurityMode>" +
						"<TransportClientCredentialType vt=\"8\">Basic</TransportClientCredentialType>" +
						"<ServiceCertificate vt=\"8\">thumbprint</ServiceCertificate>" +
						"<UseSSO vt=\"11\">0</UseSSO>" +
						"<UserName vt=\"8\">username</UserName>" +
						"<Password vt=\"8\">p@ssw0rd</Password>" +
						"<ProxyToUse vt=\"8\">None</ProxyToUse>" +
						"<SuppressMessageBodyForHttpVerbs vt=\"8\">GET</SuppressMessageBodyForHttpVerbs>" +
						"<HttpHeaders vt=\"8\">Content-Type: application/json</HttpHeaders>" +
						"<HttpMethodAndUrl vt=\"8\">" + (
							"&lt;BtsHttpUrlMapping&gt;" +
								"&lt;Operation Name=\"AddCustomer\" Method=\"POST\" Url=\"/Customer/{id}\" /&gt;" +
								"&lt;Operation Name=\"DeleteCustomer\" Method=\"DELETE\" Url=\"/Customer/{id}\" /&gt;" +
								"&lt;/BtsHttpUrlMapping&gt;") +
						"</HttpMethodAndUrl>" +
						"<VariablePropertyMapping vt=\"8\">" + (
							"&lt;BtsVariablePropertyMapping&gt;" +
								string.Format(
									"&lt;Variable Name=\"id\" PropertyName=\"{0}\" PropertyNamespace=\"{1}\" /&gt;",
									BizTalkFactoryProperties.ReceiverName.Name,
									BizTalkFactoryProperties.ReceiverName.Namespace) +
								"&lt;/BtsVariablePropertyMapping&gt;") +
						"</VariablePropertyMapping>" +
						"<EndpointBehaviorConfiguration vt=\"8\">" + (
							"&lt;behavior name=\"EndpointBehavior\" /&gt;") +
						"</EndpointBehaviorConfiguration>" +
						"<StsUri vt=\"8\">https://localhost/swt_token_issuer</StsUri>" +
						"<IssuerName vt=\"8\">issuer_name</IssuerName>" +
						"<IssuerSecret vt=\"8\">secret</IssuerSecret>" +
						"<UseAcsAuthentication vt=\"11\">-1</UseAcsAuthentication>" +
						"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
						"<SendTimeout vt=\"8\">00:01:00</SendTimeout>" +
						"<CloseTimeout vt=\"8\">00:01:00</CloseTimeout>" +
						"<Identity vt=\"8\">" + (
							"&lt;identity&gt;\r\n" +
								"  &lt;servicePrincipalName value=\"spn_name\" /&gt;\r\n" +
								"&lt;/identity&gt;") +
						"</Identity>" +
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
			var wha = new WcfWebHttpAdapter.Outbound(
				a => {
					a.Address = new EndpointAddress("https://localhost/dummy.svc");

					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("spn_name");
					a.SecurityMode = WebHttpSecurityMode.Transport;
					a.ServiceCertificate = "thumbprint";
					a.TransportClientCredentialType = HttpClientCredentialType.Basic;
					a.UserName = "username";
					a.Password = "p@ssw0rd";

					a.UseAcsAuthentication = true;
					a.StsUri = new Uri("https://localhost/swt_token_issuer");
					a.IssuerName = "issuer_name";
					a.IssuerSecret = "secret";

					a.SuppressMessageBodyForHttpVerbs = "GET";
					a.HttpHeaders = "Content-Type: application/json";
					a.HttpUrlMapping = new HttpUrlMapping {
						new HttpUrlMappingOperation("AddCustomer", "POST", "/Customer/{id}"),
						new HttpUrlMappingOperation("DeleteCustomer", "DELETE", "/Customer/{id}")
					};
					a.VariableMapping = new VariableMapping {
						new VariablePropertyMapping("id", BizTalkFactoryProperties.ReceiverName)
					};

					a.MaxReceivedMessageSize = 2048;
				});

			Assert.That(() => ((ISupportValidation) wha).Validate(), Throws.Nothing);
		}
	}
}

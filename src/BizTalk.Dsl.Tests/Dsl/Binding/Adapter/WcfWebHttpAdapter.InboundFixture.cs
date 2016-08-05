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
	public class WcfWebHttpAdapterInboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var wha = new WcfWebHttpAdapter.Inbound(
				a => {
					a.Address = new Uri("/dummy.svc", UriKind.Relative);

					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("spn_name");
					a.SecurityMode = WebHttpSecurityMode.Transport;
					a.ServiceCertificate = "thumbprint";
					a.TransportClientCredentialType = HttpClientCredentialType.Windows;

					a.AddMessageBodyForHttpVerbs = "GET,HEAD";
					a.HttpHeaders = "Content-Type: application/json\r\nReferer: http://www.my.org/";
					a.HttpUrlMapping = new HttpUrlMapping {
						new HttpUrlMappingOperation("AddCustomer", "POST", "/Customer/{id}"),
						new HttpUrlMappingOperation("DeleteCustomer", "DELETE", "/Customer/{id}")
					};
					a.VariableMapping = new VariableMapping {
						new VariablePropertyMapping("id", BizTalkFactoryProperties.ReceiverName)
					};

					a.MaxConcurrentCalls = 400;
				});
			var xml = ((IAdapterBindingSerializerFactory) wha).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<MaxReceivedMessageSize vt=\"3\">65535</MaxReceivedMessageSize>" +
						"<SecurityMode vt=\"8\">Transport</SecurityMode>" +
						"<TransportClientCredentialType vt=\"8\">Windows</TransportClientCredentialType>" +
						"<ServiceCertificate vt=\"8\">thumbprint</ServiceCertificate>" +
						"<UseSSO vt=\"11\">0</UseSSO>" +
						"<MaxConcurrentCalls vt=\"3\">400</MaxConcurrentCalls>" +
						"<SuspendMessageOnFailure vt=\"11\">-1</SuspendMessageOnFailure>" +
						"<IncludeExceptionDetailInFaults vt=\"11\">-1</IncludeExceptionDetailInFaults>" +
						"<DisableLocationOnFailure vt=\"11\">0</DisableLocationOnFailure>" +
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
						"<AddMessageBodyForHttpVerbs vt=\"8\">" + (
							"GET,HEAD") +
						"</AddMessageBodyForHttpVerbs>" +
						"<HttpHeaders vt=\"8\">" + (
							"Content-Type: application/json\r\nReferer: http://www.my.org/") +
						"</HttpHeaders>" +
						"<ServiceBehaviorConfiguration vt=\"8\">" + (
							"&lt;behavior name=\"ServiceBehavior\" /&gt;") +
						"</ServiceBehaviorConfiguration>" +
						"<EndpointBehaviorConfiguration vt=\"8\">" + (
							"&lt;behavior name=\"EndpointBehavior\" /&gt;") +
						"</EndpointBehaviorConfiguration>" +
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
			var wha = new WcfWebHttpAdapter.Inbound(
				a => {
					a.Address = new Uri("/dummy.svc", UriKind.Relative);

					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("spn_name");
					a.SecurityMode = WebHttpSecurityMode.Transport;
					a.ServiceCertificate = "thumbprint";
					a.TransportClientCredentialType = HttpClientCredentialType.Windows;

					a.AddMessageBodyForHttpVerbs = "GET,HEAD";
					a.HttpHeaders = "Content-Type: application/json\r\nReferer: http://www.my.org/";
					a.HttpUrlMapping = new HttpUrlMapping {
						new HttpUrlMappingOperation("AddCustomer", "POST", "/Customer/{id}"),
						new HttpUrlMappingOperation("DeleteCustomer", "DELETE", "/Customer/{id}")
					};
					a.VariableMapping = new VariableMapping {
						new VariablePropertyMapping("id", BizTalkFactoryProperties.ReceiverName)
					};

					a.MaxConcurrentCalls = 400;
				});

			Assert.That(() => ((ISupportValidation) wha).Validate(), Throws.Nothing);
		}
	}
}

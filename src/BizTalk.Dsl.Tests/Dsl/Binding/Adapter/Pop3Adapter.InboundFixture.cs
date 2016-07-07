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
using System.Security;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class Pop3AdapterInboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var ipa = new Pop3Adapter.Inbound(
				a => {
					a.MailServer = "pop3.world.com";
					a.AuthenticationScheme = Pop3Adapter.AuthenticationScheme.SecurePasswordAuthentication;
					a.UserName = "domain\\reader/owner";
					a.Password = "p@ssw0rd";
					a.UseSSL = true;
					a.BodyPartContentType = "text/";
					a.ErrorThreshold = 50;
					a.PollingInterval = TimeSpan.FromSeconds(15);
				});
			var xml = ((IAdapterBindingSerializerFactory) ipa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<AdapterConfig vt=\"8\">" + SecurityElement.Escape(
							"<Config>" +
								"<uri>POP3://pop3.world.com#domain\\reader/owner</uri>" +
								"<applyMIME>true</applyMIME>" +
								"<bodyPartContentType>text/</bodyPartContentType>" +
								"<bodyPartIndex>0</bodyPartIndex>" +
								"<mailServer>pop3.world.com</mailServer>" +
								"<serverPort>0</serverPort>" +
								"<authenticationScheme>SPA</authenticationScheme>" +
								"<userName>domain\\reader/owner</userName>" +
								"<password>p@ssw0rd</password>" +
								"<sslRequired>true</sslRequired>" +
								"<errorThreshold>50</errorThreshold>" +
								"<pollingInterval>15</pollingInterval>" +
								"<pollingUnitOfMeasure>Seconds</pollingUnitOfMeasure>" +
								"</Config>") +
						"</AdapterConfig>" +
						"</CustomProps>"));
		}

		[Test]
		public void Validate()
		{
			var ipa = new Pop3Adapter.Inbound(
				a => {
					a.MailServer = "pop3.world.com";
					a.AuthenticationScheme = Pop3Adapter.AuthenticationScheme.SecurePasswordAuthentication;
					a.UserName = "owner";
					a.Password = "p@ssw0rd";
					a.UseSSL = true;
					a.BodyPartContentType = "text/";
					a.ErrorThreshold = 50;
					a.PollingInterval = TimeSpan.FromSeconds(15);
				});
			Assert.That(
				() => ((ISupportValidation) ipa).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo(
						@"The format of the user name property is invalid for SPA authentication scheme. Make sure that the user name is specified as either <domain-name>\<user-name> or <machine-name>\<user-name>."));
		}

		[Test]
		public void ValidateDoesNotThrow()
		{
			var ipa = new Pop3Adapter.Inbound(
				a => {
					a.MailServer = "pop3.world.com";
					a.AuthenticationScheme = Pop3Adapter.AuthenticationScheme.SecurePasswordAuthentication;
					a.UserName = "domain\\reader/owner";
					a.Password = "p@ssw0rd";
					a.UseSSL = true;
					a.BodyPartContentType = "text/";
					a.ErrorThreshold = 50;
					a.PollingInterval = TimeSpan.FromSeconds(15);
				});
			Assert.That(() => ((ISupportValidation) ipa).Validate(), Throws.Nothing);
		}
	}
}

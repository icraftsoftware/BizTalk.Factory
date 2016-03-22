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

using System.Security;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class FtpAdapterOutboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var pfa = new FtpAdapter.Outbound(
				a => {
					//a.MailServer = "pop3.world.com";
					//a.AuthenticationScheme = Pop3Adapter.AuthenticationScheme.SecurePasswordAuthentication;
					//a.UserName = "domain\\reader/owner";
					//a.Password = "p@ssw0rd";
					//a.UseSSL = true;
					//a.BodyPartContentType = "text/";
					//a.ErrorThreshold = 50;
					//a.PollingInterval = TimeSpan.FromSeconds(15);
				});
			var xml = ((IAdapterBindingSerializerFactory) pfa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<AdapterConfig vt=\"8\">" + SecurityElement.Escape(
							"<Config>" +
								"<uri>ftp://ftp.server.com:21//in/from_bts/%MessageID%.xml</uri>" +
								"<serverAddress>ftp.server.com</serverAddress>" +
								"<serverPort>21</serverPort>" +
								"<password><![CDATA[******]]></password>" +
								"<accountName>ftpaccount</accountName>" +
								"<targetFolder>/in/from_bts</targetFolder>" +
								"<targetFileName>%MessageID%.xml</targetFileName>" +
								"<representationType>binary</representationType>" +
								"<beforePut>cd /</beforePut>" +
								"<afterPut>cd /</afterPut>" +
								"<allocateStorage>True</allocateStorage>" +
								"<appendIfExists>False</appendIfExists>" +
								"<spoolingFolder>/in/from_bts/temp</spoolingFolder>" +
								"<connectionLimit>10</connectionLimit>" +
								"<passiveMode>False</passiveMode>" +
								"<firewallType>Socks5</firewallType>" +
								"<firewallAddress>firewall.com</firewallAddress>" +
								"<firewallPort>21</firewallPort>" +
								"<firewallUserName>fwuser</firewallUserName>" +
								"<firewallPassword>******</firewallPassword>" +
								"<useSsl>True</useSsl>" +
								"<useDataProtection>True</useDataProtection>" +
								"<ftpsConnMode>Explicit</ftpsConnMode>" +
								"<clientCertificateHash>hash</clientCertificateHash>" +
								"</Config>") +
						"</AdapterConfig>" +
						"</CustomProps>"));
		}

		[Test]
		public void Validate()
		{
			var ofa = new FtpAdapter.Outbound(
				a => {
					//a.MailServer = "pop3.world.com";
					//a.AuthenticationScheme = Pop3Adapter.AuthenticationScheme.SecurePasswordAuthentication;
					//a.UserName = "owner";
					//a.Password = "p@ssw0rd";
					//a.UseSSL = true;
					//a.BodyPartContentType = "text/";
					//a.ErrorThreshold = 50;
					//a.PollingInterval = TimeSpan.FromSeconds(15);
				});
			Assert.That(
				() => ((ISupportValidation) ofa).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo(
						@"The format of the user name property is invalid for SPA authentication scheme. Make sure that the user name is specified as either <domain-name>\<user-name> or <machine-name>\<user-name>."));
		}
	}
}

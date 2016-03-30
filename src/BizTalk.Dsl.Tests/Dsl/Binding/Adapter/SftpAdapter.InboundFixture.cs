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
using Microsoft.BizTalk.Adapter.Sftp;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class SftpAdapterInboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var isa = new SftpAdapter.Inbound(
				a => {
					a.ServerAddress = "sftp.server.com";
					a.Port = 23;
					a.FolderPath = "/out/to_bts/";
					a.FileMask = "*.xml";
					a.PollingInterval = TimeSpan.FromMinutes(10);
					a.ConnectionLimit = 6;

					a.ClientAuthenticationMode = ClientAuthenticationMode.MultiFactorAuthentication;
					a.PrivateKeyFile = @"c:\files\key.ppk";
					a.PrivateKeyPassword = "p@ssw0rd";
					a.UserName = "user";
					a.Password = "p@ssw0rd";
					a.SshServerHostKeyFingerPrint = "fingerprint";
					a.EncryptionCipher = EncryptionCipher.AES;

					a.ProxyAddress = "proxy.server.com";
					a.ProxyPort = 1082;
					a.ProxyType = ProxyType.SOCKS4;
					a.ProxyUserName = "proxyuser";
					a.ProxyPassword = "p@ssw0rd";
				});
			var xml = ((IAdapterBindingSerializerFactory) isa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<ServerAddress vt=\"8\">sftp.server.com</ServerAddress>" +
						"<Port vt=\"3\">23</Port>" +
						"<FolderPath vt=\"8\">/out/to_bts/</FolderPath>" +
						"<FileMask vt=\"8\">*.xml</FileMask>" +
						"<AccessAnySSHServerHostKey vt=\"11\">0</AccessAnySSHServerHostKey>" +
						"<SSHServerHostKey vt=\"8\">fingerprint</SSHServerHostKey>" +
						"<ClientAuthenticationMode vt=\"8\">MultiFactorAuthentication</ClientAuthenticationMode>" +
						"<PrivateKey vt=\"8\">c:\\files\\key.ppk</PrivateKey>" +
						"<PrivateKeyPassword vt=\"8\">p@ssw0rd</PrivateKeyPassword>" +
						"<UserName vt=\"8\">user</UserName>" +
						"<Password vt=\"8\">p@ssw0rd</Password>" +
						"<PollingInterval vt=\"3\">10</PollingInterval>" +
						"<PollingIntervalUnit vt=\"8\">Minutes</PollingIntervalUnit>" +
						"<ConnectionLimit vt=\"3\">6</ConnectionLimit>" +
						"<ProxyPassword vt=\"8\">p@ssw0rd</ProxyPassword>" +
						"<ProxyUserName vt=\"8\">proxyuser</ProxyUserName>" +
						"<ProxyType vt=\"8\">SOCKS4</ProxyType>" +
						"<ProxyAddress vt=\"8\">proxy.server.com</ProxyAddress>" +
						"<ProxyPort vt=\"3\">1082</ProxyPort>" +
						"<EncryptionCipher vt=\"8\">AES</EncryptionCipher>" +
						"</CustomProps>"));
		}

		[Test]
		[Ignore("TODO")]
		public void Validate()
		{
			Assert.Fail("TODO");
		}
	}
}

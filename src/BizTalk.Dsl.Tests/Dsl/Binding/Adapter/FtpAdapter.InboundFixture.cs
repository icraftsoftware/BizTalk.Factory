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
	public class FtpAdapterInboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var ifa = new FtpAdapter.Inbound(
				a => {
					a.MaximumNumberOfFiles = 1;
					a.MaximumBatchSize = 2;

					a.FirewallAddress = "firewall.com";
					a.FirewallType = FtpAdapter.FirewallType.Socks4;
					a.FirewallUserName = "fireuser";
					a.FirewallPassword = "p@ssw0rd";

					a.Server = "ftp.server.com";
					a.Folder = "/out/to_bts/";
					a.FileMask = "*.*.csv";
					a.UserName = "ftpuser";
					a.Password = "p@ssw0rd";
					a.AfterGet = a.BeforeGet = "cd /";
					a.ErrorThreshold = 11;
					a.Log = "c:\\windows\\temp\\ftp.log";
					a.MaximumFileSize = 100;
					a.UseNameList = false;

					a.DeleteAfterDownload = true;
					a.EnableTimestampComparison = true;
					a.PollingInterval = TimeSpan.FromSeconds(120);

					a.ClientCertificate = "hash";
					a.FtpsConnectionMode = FtpAdapter.FtpsConnectionMode.Implicit;
					a.UseSSL = true;

					a.ReceiveTimeout = TimeSpan.FromMinutes(1);
					a.TemporaryFolder = "c:\\windows\\temp";
				});
			var xml = ((IAdapterBindingSerializerFactory) ifa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<AdapterConfig vt=\"8\">" + SecurityElement.Escape(
							"<Config>" +
								"<uri>ftp://ftp.server.com:21//out/to_bts//*.*.csv</uri>" +
								"<maximumNumberOfFiles>1</maximumNumberOfFiles>" +
								"<maximumBatchSize>2</maximumBatchSize>" +
								"<firewallAddress>firewall.com</firewallAddress>" +
								"<firewallPort>21</firewallPort>" +
								"<firewallUserName>fireuser</firewallUserName>" +
								"<firewallPassword>p@ssw0rd</firewallPassword>" +
								"<firewallType>Socks4</firewallType>" +
								"<passiveMode>False</passiveMode>" +
								"<serverAddress>ftp.server.com</serverAddress>" +
								"<serverPort>21</serverPort>" +
								"<targetFolder>/out/to_bts/</targetFolder>" +
								"<fileMask>*.*.csv</fileMask>" +
								"<userName>ftpuser</userName>" +
								"<password>p@ssw0rd</password>" +
								"<afterGet>cd /</afterGet>" +
								"<beforeGet>cd /</beforeGet>" +
								"<errorThreshold>11</errorThreshold>" +
								"<commandLogFilename>c:\\windows\\temp\\ftp.log</commandLogFilename>" +
								"<maxFileSize>100</maxFileSize>" +
								"<representationType>binary</representationType>" +
								"<useNLST>False</useNLST>" +
								"<deleteAfterDownload>True</deleteAfterDownload>" +
								"<enableTimeComparison>True</enableTimeComparison>" +
								"<pollingInterval>2</pollingInterval>" +
								"<pollingUnitOfMeasure>Minutes</pollingUnitOfMeasure>" +
								"<redownloadInterval>-1</redownloadInterval>" +
								"<clientCertificateHash>hash</clientCertificateHash>" +
								"<ftpsConnMode>Implicit</ftpsConnMode>" +
								"<useDataProtection>True</useDataProtection>" +
								"<useSsl>True</useSsl>" +
								"<receiveDataTimeout>60000</receiveDataTimeout>" +
								"<spoolingFolder>c:\\windows\\temp</spoolingFolder>" +
								"</Config>") +
						"</AdapterConfig>" +
						"</CustomProps>"));
		}

		[Test]
		public void Validate()
		{
			var ifa = new FtpAdapter.Inbound();
			Assert.That(
				() => ((ISupportValidation) ifa).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo(@"The Server Address is not defined"));
		}

		[Test]
		public void ValidateDoesNotThrow()
		{
			var ifa = new FtpAdapter.Inbound(
				a => {
					a.MaximumNumberOfFiles = 1;
					a.MaximumBatchSize = 2;

					a.FirewallAddress = "firewall.com";
					a.FirewallType = FtpAdapter.FirewallType.Socks4;
					a.FirewallUserName = "fireuser";
					a.FirewallPassword = "p@ssw0rd";

					a.Server = "ftp.server.com";
					a.Folder = "/out/to_bts/";
					a.FileMask = "*.*.csv";
					a.UserName = "ftpuser";
					a.Password = "p@ssw0rd";
					a.AfterGet = a.BeforeGet = "cd /";
					a.ErrorThreshold = 11;
					a.Log = "c:\\windows\\temp\\ftp.log";
					a.MaximumFileSize = 100;
					a.UseNameList = false;

					a.DeleteAfterDownload = true;
					a.EnableTimestampComparison = true;
					a.PollingInterval = TimeSpan.FromSeconds(120);

					a.ClientCertificate = "hash";
					a.FtpsConnectionMode = FtpAdapter.FtpsConnectionMode.Implicit;
					a.UseSSL = true;

					a.ReceiveTimeout = TimeSpan.FromMinutes(1);
					a.TemporaryFolder = "c:\\windows\\temp";
				});
			Assert.That(() => ((ISupportValidation) ifa).Validate(), Throws.Nothing);
		}
	}
}

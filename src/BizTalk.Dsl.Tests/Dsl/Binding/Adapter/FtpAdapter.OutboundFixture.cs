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
					a.Server = "ftp.server.com";
					a.Folder = "/in/from_bts/";
					a.UserName = "ftpuser";
					a.Password = "p@ssw0rd";
					a.AfterPut = "cd /";
					a.BeforePut = "cd /";
					a.AllocateStorage = true;

					a.ConnectionLimit = 10;
				});
			var xml = ((IAdapterBindingSerializerFactory) pfa).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<AdapterConfig vt=\"8\">" + SecurityElement.Escape(
							"<Config>" +
								"<uri>ftp://ftp.server.com:21//in/from_bts//%MessageID%.xml</uri>" +
								"<firewallPort>21</firewallPort>" +
								"<firewallType>NoFirewall</firewallType>" +
								"<passiveMode>False</passiveMode>" +
								"<serverAddress>ftp.server.com</serverAddress>" +
								"<serverPort>21</serverPort>" +
								"<targetFolder>/in/from_bts/</targetFolder>" +
								"<targetFileName>%MessageID%.xml</targetFileName>" +
								"<userName>ftpuser</userName>" +
								"<password>p@ssw0rd</password>" +
								"<afterPut>cd /</afterPut>" +
								"<beforePut>cd /</beforePut>" +
								"<allocateStorage>True</allocateStorage>" +
								"<representationType>binary</representationType>" +
								"<ftpsConnMode>Explicit</ftpsConnMode>" +
								"<useDataProtection>True</useDataProtection>" +
								"<connectionLimit>10</connectionLimit>" +
								"</Config>") +
						"</AdapterConfig>" +
						"</CustomProps>"));
		}

		[Test]
		public void Validate()
		{
			var ofa = new FtpAdapter.Outbound();
			Assert.That(
				() => ((ISupportValidation) ofa).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo("The Server Address is not defined"));
		}
	}
}

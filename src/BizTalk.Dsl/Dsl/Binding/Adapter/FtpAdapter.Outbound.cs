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
using System.ComponentModel;
using System.Xml.Serialization;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class FtpAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// The FTP adapter exchanges data between an FTP server and Microsoft BizTalk Server, and allows for the
		/// integration of vital data stored on a variety of platforms with line-of-business applications. The adapter can
		/// connect to the FTP server via SOCKS4 or SOCKS5 proxy server.
		/// </summary>
		/// <remarks>
		/// The FTP adapter can transfer a large number of files from an FTP server to BizTalk Server. It can also
		/// transfer files as part of an orchestration.
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561215.aspx">FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa559267.aspx">What Is the FTP Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561032.aspx">Configuring the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa578371.aspx">FTP Commands that are Required by the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561727.aspx">FTP Adapter Security Recommendations</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561990.aspx">Best Practices for the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/ff629768.aspx">Enhancements to the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa546802.aspx">How to Configure an FTP Send Port</seealso>
		[XmlRoot("Config")]
		public class Outbound : FtpAdapter, IOutboundAdapter
		{
			public Outbound()
			{
				// Firewall Settings
				FirewallPort = 21;
				FirewallType = FirewallType.None;
				Mode = Mode.Active;

				// FTP Settings
				Port = 21;
				TargetFileName = "%MessageID%.xml";
				Representation = RepresentationType.Binary;

				// SSL Settings
				FtpsConnectionMode = FtpsConnectionMode.Explicit;
				UseDataProtection = true;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				return "ftp://" + Server + ":" + Port + "/" + Folder + "/" + TargetFileName;
			}

			#endregion

			#region Firewall Settings

			/// <summary>
			/// Specify the address of the firewall, either a DNS name or an IP address.
			/// </summary>
			[XmlElement("firewallAddress")]
			public string FirewallAddress { get; set; }

			/// <summary>
			/// Specify the port for the firewall.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Values range from <c>1</c> to <c>65535</c>.
			/// </para>
			/// <para>
			/// It defaults to <c>21</c>.
			/// </para>
			/// </remarks>
			[XmlElement("firewallPort")]
			public uint FirewallPort { get; set; }

			/// <summary>
			/// Specify the user name for the firewall.
			/// </summary>
			[XmlElement("firewallUserName")]
			public string FirewallUserName { get; set; }

			/// <summary>
			/// Specify the password for the firewall.
			/// </summary>
			[XmlElement("firewallPassword")]
			public string FirewallPassword { get; set; }

			/// <summary>
			/// Specify the type of firewall deployed.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="FtpAdapter.FirewallType.None"/>.
			/// </remarks>
#pragma warning disable 108
			[XmlElement("firewallType")]
			public FirewallType FirewallType { get; set; }
#pragma warning restore 108

			/// <summary>
			/// Specify the mode in which the adapter connects to the FTP server.
			/// </summary>
			/// <remarks>
			/// <para>
			/// In active mode, the FTP server connects to a port opened by the FTP adapter. In passive mode, the FTP
			/// adapter connects to a port opened by the FTP server.
			/// </para>
			/// <para>
			/// It defaults to <see cref="FtpAdapter.Mode.Active"/>;
			/// </para>
			/// </remarks>
#pragma warning disable 108
			[XmlIgnore]
			public Mode Mode
#pragma warning restore 108
			{
				get { return XmlAliasedPassiveMode == Boolean.True ? Mode.Passive : Mode.Active; }
				set { XmlAliasedPassiveMode = value == Mode.Passive ? Boolean.True : Boolean.False; }
			}

			[EditorBrowsable(EditorBrowsableState.Never)]
			[XmlElement("passiveMode")]
			public Boolean XmlAliasedPassiveMode { get; set; }

			#endregion

			#region FTP Settings

			/// <summary>
			/// Specify the server name or IP address of the FTP server.
			/// </summary>
			[XmlElement("serverAddress")]
			public string Server { get; set; }

			/// <summary>
			/// Specify the port address for this FTP server.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>21</c>.
			/// </remarks>
			[XmlElement("serverPort")]
			public int Port { get; set; }

			/// <summary>
			/// Specify the location to move the files to on the FTP server.
			/// </summary>
			[XmlElement("targetFolder")]
			public string Folder { get; set; }

			/// <summary>
			/// Specify an alternative name for the file. Retaining the default name guarantees unique message names for
			/// each message sent.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>%MessageID%.xm</c>.
			/// </remarks>
			[XmlElement("targetFileName")]
			public string TargetFileName { get; set; }

			/// <summary>
			/// Specify the user name to log on to the FTP server.
			/// </summary>
			[XmlElement("userName")]
			public string UserName { get; set; }

			/// <summary>
			/// Specify the user password to log on to the FTP server.
			/// </summary>
			[XmlElement("password")]
			public string Password { get; set; }

			/// <summary>
			/// Specify the FTP commands to run after the file <c>PUT</c>. Separate commands with a semicolon (<c>;</c>).
			/// </summary>
			[XmlElement("afterPut")]
			public string AfterPut { get; set; }

			/// <summary>
			/// Specify the FTP commands to run before the file <c>PUT</c>. Separate commands with a semicolon (<c>;</c>).
			/// </summary>
			/// <remarks>
			/// <c>QUIT</c> command is not supported before the file <c>PUT</c>.
			/// </remarks>
			[XmlElement("beforePut")]
			public string BeforePut { get; set; }

			/// <summary>
			/// Specify whether to allocate storage space for legacy host systems. This option is provided for backward
			/// compatibility.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			[XmlElement("allocateStorage")]
			public Boolean AllocateStorage { get; set; }

			/// <summary>
			/// Specify to append data to file if it exists.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			[XmlElement("appendIfExists")]
			public Boolean AppendIfExists { get; set; }

			/// <summary>
			/// Specify the location to save a copy of the log file. Use this file to diagnose error conditions when
			/// sending or receiving files through FTP adapter.
			/// </summary>
			[XmlElement("commandLogFilename")]
			public string Log { get; set; }

			/// <summary>
			/// Select how FTP receives the data.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="FtpAdapter.RepresentationType.Binary"/>.
			/// </remarks>
			[XmlElement("representationType")]
			public RepresentationType Representation { get; set; }

			/// <summary>
			/// Specify the Enterprise Single Sign-On affiliate application.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="string.Empty"/>.
			/// </remarks>
			[XmlElement("ssoAffiliateApplication")]
			public string AffiliateApplicationName { get; set; }

			#endregion

			#region SSL Settings

			/// <summary>
			/// Specify the SHA1 hash of the client certificate that must be used in the Secure Sockets Layer (SSL)
			/// negotiation.
			/// </summary>
			/// <remarks>
			/// Based on this hash, the client certificate is picked up from the personal store of the user account under
			/// which the BizTalk host instance is running.
			/// </remarks>
			[XmlElement("clientCertificateHash")]
			public string ClientCertificate { get; set; }

			/// <summary>
			/// Specify the mode of SSL connection made to the FTPS server.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="FtpAdapter.FtpsConnectionMode.Explicit"/>.
			/// </remarks>
#pragma warning disable 108
			[XmlElement("ftpsConnMode")]
			public FtpsConnectionMode FtpsConnectionMode { get; set; }
#pragma warning restore 108

			/// <summary>
			/// Specify if the adapter must use SSL encryption or plain text when it sends and receives data files from the
			/// FTPS server.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>True</c>.
			/// </remarks>
			[XmlElement("useDataProtection")]
			public Boolean UseDataProtection { get; set; }

			/// <summary>
			/// Specify whether the FTP adapter must use SSL to communicate with the FTPS server.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			[XmlElement("useSsl")]
			public Boolean UseSSL { get; set; }

			#endregion

			#region Tuning Parameters

			/// <summary>
			/// Specify the maximum number of concurrent FTP connections that can be opened to the server. A value of 0 means no limit.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>0</c> seconds.
			/// </remarks>
			[XmlElement("connectionLimit")]
			public int ConnectionLimit { get; set; }

			/// <summary>
			/// Specify the location for a temporary folder.
			/// </summary>
			/// <remarks>
			/// The file is first uploaded here and then moved to the destination FTP folder. In case of transfer failure,
			/// the adapter restarts the file upload in ASCII mode of transfer and resumes in binary mode of transfer.
			/// </remarks>
			[XmlElement("spoolingFolder")]
			public string TemporaryFolder { get; set; }

			#endregion
		}

		#endregion
	}
}

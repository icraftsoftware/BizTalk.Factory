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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using Microsoft.BizTalk.Adapter.Sftp;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class FtpAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The FTP adapter exchanges data between an FTP server and Microsoft BizTalk Server, and allows for the
		/// integration of vital data stored on a variety of platforms with line-of-business applications. The adapter can
		/// connect to the FTP server via SOCKS4 or SOCKS5 proxy server.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The FTP adapter can transfer a large number of files from an FTP server to BizTalk Server. It can also
		/// transfer files as part of an orchestration.
		/// </para>
		/// <para>
		/// Do not configure multiple FTP receive locations to poll the same FTP URL. If multiple FTP receive locations
		/// are polling the same URL concurrently then each receive location can receive a copy of the file, which can
		/// cause data duplication. This behavior occurs because the FTP protocol has no provision for locking files when
		/// reading them from the target URL.
		/// </para>
		/// <para>
		/// To provide high availability for the FTP receive adapter you should configure the FTP receive adapter to run
		/// in a clustered BizTalk Host instance. For more information about how to run an FTP receive handler in a
		/// clustered BizTalk Host see <see href="https://msdn.microsoft.com/en-us/library/aa561801.aspx">Considerations
		/// for Running Adapter Handlers within a Clustered Host</see>.
		/// </para>
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561215.aspx">FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa559267.aspx">What Is the FTP Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561032.aspx">Configuring the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa578371.aspx">FTP Commands that are Required by the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561727.aspx">FTP Adapter Security Recommendations</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa561990.aspx">Best Practices for the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/ff629768.aspx">Enhancements to the FTP Adapter</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/aa559095.aspx">How to Configure an FTP Receive Location</seealso>
		[XmlRoot("Config")]
		public class Inbound : FtpAdapter, IInboundAdapter
		{
			public Inbound()
			{
				// Firewall Settings
				FirewallPort = 21;
				FirewallType = FirewallType.None;
				Mode = Mode.Active;

				// FTP Settings
				Port = 21;
				ErrorThreshold = 10;
				Representation = RepresentationType.Binary;

				// Polling Settings
				DeleteAfterDownload = true;
				PollingInterval = TimeSpan.FromSeconds(60);
				RedownloadInterval = TimeSpan.MinValue;

				// SSL Settings
				FtpsConnectionMode = FtpsConnectionMode.Explicit;
				UseDataProtection = true;

				// Tuning Parameters
				ReceiveTimeout = TimeSpan.FromSeconds(90);
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				return "ftp://" + Server + ":" + Port + "/" + Folder + "/" + FileMask;
			}

			#endregion

			#region Batch Settings

			/// <summary>
			/// Specify the maximum number of files per BizTalk Server batch.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Zero (<c>0</c>) indicates no limit.
			/// </para>
			/// <para>
			/// It defaults to zero (<c>0</c>).
			/// </para>
			/// </remarks>
			[XmlElement("maximumNumberOfFiles")]
			public uint MaximumNumberOfFiles { get; set; }

			/// <summary>
			/// Specify the maximum number of bytes per BizTalk Server batch.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Zero (<c>0</c>) indicates no limit.
			/// </para>
			/// <para>
			/// It defaults to zero (<c>0</c>).
			/// </para>
			/// </remarks>
			[XmlElement("maximumBatchSize")]
			public uint MaximumBatchSize { get; set; }

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
			/// Specify the polling location on the FTP server.
			/// </summary>
			[XmlElement("targetFolder")]
			public string Folder { get; set; }

			/// <summary>
			/// Specify the file mask filter to use when transmitting files.
			/// </summary>
			[XmlElement("fileMask")]
			public string FileMask { get; set; }

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
			/// Specify the FTP commands to run after the file <c>GET</c>. Separate commands with a semicolon (<c>;</c>).
			/// </summary>
			[XmlElement("afterGet")]
			public string AfterGet { get; set; }

			/// <summary>
			/// Specify the FTP commands to run before the file <c>GET</c>. Separate commands with a semicolon (<c>;</c>).
			/// </summary>
			/// <remarks>
			/// <c>QUIT</c> command is not supported before the file <c>GET</c>.
			/// </remarks>
			[XmlElement("beforeGet")]
			public string BeforeGet { get; set; }

			/// <summary>
			/// Specify the number of errors that BizTalk Server can encounter before the location is disabled.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>10</c>.
			/// </remarks>
			[XmlElement("errorThreshold")]
			public uint ErrorThreshold { get; set; }

			/// <summary>
			/// Specify the location to save a copy of the log file. You use this file to diagnose error conditions when
			/// sending or receiving files through FTP.
			/// </summary>
			[XmlElement("commandLogFilename")]
			public string Log { get; set; }

			/// <summary>
			/// Specify the maximum downloadable file size, in megabytes.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Zero (<c>0</c>) indicates no limit on the file size.
			/// </para>
			/// <para>
			/// It defaults to <c>100</c>.
			/// </para>
			/// </remarks>
			[XmlElement("maxFileSize")]
			public uint MaximumFileSize { get; set; }

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

			/// <summary>
			/// Specify how the adapter lists files. To view file names instead of the system-defined file listing, set
			/// this value to <c>true</c>.
			/// </summary>
			[XmlElement("useNLST")]
			public Boolean UseNameList { get; set; }

			#endregion

			#region Polling Settings

			/// <summary>
			/// Specify whether the adapter deletes a file from the FTP server after downloading it.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>True</c>.
			/// </remarks>
			[XmlElement("deleteAfterDownload")]
			public Boolean DeleteAfterDownload { get; set; }

			/// <summary>
			/// Specify whether the adapter downloads a file again based on its modified timestamp. In cases when the
			/// adapter does not have delete permissions on the FTP server, the MDTM (Modification Time) command allows the
			/// adapter to know whether a file has been modified since last download. Based on the value of this property,
			/// the file is downloaded again.
			/// </summary>
			/// <remarks>
			/// <para>
			/// In case the FTP server does not support MDTM, set the <see cref="RedownloadInterval"/> property.
			/// </para>
			/// <para>
			/// This property is applicable only when <see cref="DeleteAfterDownload"/> is set to <c>False</c>.
			/// </para>
			/// <para>
			/// It defaults to <c>False</c>.
			/// </para>
			/// </remarks>
			[XmlElement("enableTimeComparison")]
			public Boolean EnableTimestampComparison { get; set; }

			/// <summary>
			/// Specify the interval number for polling this location.
			/// </summary>
			/// <remarks>
			/// <para>
			/// To poll continuously, set this value to zero (0).
			/// </para>
			/// <para>
			/// It defaults to <c>60</c> seconds.
			/// </para>
			/// </remarks>
			[XmlIgnore]
			public TimeSpan PollingInterval
			{
				get { return BuildTimeSpan(XmlAliasedPollingInterval, XmlAliasedPollingUnitOfMeasure); }
				set
				{
					UnbuildTimeSpan(
						value,
						(q, u) => {
							XmlAliasedPollingInterval = q;
							XmlAliasedPollingUnitOfMeasure = u;
						});
				}
			}

			[EditorBrowsable(EditorBrowsableState.Never)]
			[XmlElement("pollingInterval")]
			public int XmlAliasedPollingInterval { get; set; }

			[EditorBrowsable(EditorBrowsableState.Never)]
			[XmlElement("pollingUnitOfMeasure")]
			public PollingIntervalUnit XmlAliasedPollingUnitOfMeasure { get; set; }

			/// <summary>
			/// Specify the interval after which the adapter downloads files again.
			/// </summary>
			/// <remarks>
			/// <para>
			/// This property is applicable only when both <see cref="DeleteAfterDownload"/> and <see
			/// cref="EnableTimestampComparison"/> are set to <c>False</c>.
			/// </para>
			/// <para>
			/// A negative <see cref="TimeSpan"/> indicates that the adapter will not download files again.
			/// </para>
			/// <para>
			/// A zero <see cref="TimeSpan"/>, <see cref="TimeSpan.Zero"/>, indicates that the adapter will download the
			/// file in each polling cycle.
			/// </para>
			/// <para>
			/// It defaults to a negative <see cref="TimeSpan"/>.
			/// </para>
			/// </remarks>
			[XmlIgnore]
			public TimeSpan RedownloadInterval { get; set; }

			[EditorBrowsable(EditorBrowsableState.Never)]
			[SuppressMessage("ReSharper", "RedundantCaseLabel")]
			[XmlElement("redownloadInterval")]
			public int XmlAliasedRedownloadInterval
			{
				get
				{
					if (RedownloadInterval < TimeSpan.Zero) return -1;
					if (RedownloadInterval == TimeSpan.Zero) return 0;
					return (int) RedownloadInterval.TotalSeconds;
				}
				set
				{
					if (value < 0) RedownloadInterval = TimeSpan.MinValue;
					if (value == 0) RedownloadInterval = TimeSpan.Zero;
					RedownloadInterval = TimeSpan.FromSeconds(value);
				}
			}

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
			/// Specify the time before the receive call will abort.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You use this property to prevent a slow server from causing the receive location to stop responding.
			/// </para>
			/// <para>
			/// It defaults to <c>90</c> seconds.
			/// </para>
			/// </remarks>
			[XmlIgnore]
			public TimeSpan ReceiveTimeout { get; set; }

			[EditorBrowsable(EditorBrowsableState.Never)]
			[XmlElement("receiveDataTimeout")]
			public int XmlAliasedReceiveTimeout
			{
				get { return (int) ReceiveTimeout.TotalMilliseconds; }
				set { ReceiveTimeout = TimeSpan.FromMilliseconds(value); }
			}

			/// <summary>
			/// Specify the location for a temporary folder.
			/// </summary>
			/// <remarks>
			/// You use this location to guarantee recovery from a transfer failure.
			/// </remarks>
			[XmlElement("spoolingFolder")]
			public string TemporaryFolder { get; set; }

			#endregion
		}

		#endregion
	}
}

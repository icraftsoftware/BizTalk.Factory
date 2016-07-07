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

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class SftpAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// Use the SFTP adapter to send and receive messages from a secure FTP server using the SSH file transfer
		/// protocol.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/dn281088.aspx">SFTP Adapter Frequently Asked Questions</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/jj735584.aspx">How to Configure an SFTP Send Port</seealso>
		public class Outbound : SftpAdapter<SftpTLConfig>, IOutboundAdapter
		{
			public Outbound()
			{
				// Proxy Settings
				ProxyPort = 1080;
				ProxyType = ProxyType.None;

				// Security Settings
				AccessAnySshServerHostKey = false;
				ClientAuthenticationMode = ClientAuthenticationMode.Password;
				EncryptionCipher = EncryptionCipher.Auto;

				// SSH Server Settings
				Port = 22;
				TargetFileName = "%MessageID%.xml";
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				var uriBuilder = new UriBuilder("sftp", ServerAddress, Port, FolderPath + "/" + TargetFileName);
				return uriBuilder.Uri.AbsoluteUri;
			}

			#endregion

			#region Other Settings

			/// <summary>
			/// Specify the maximum number of concurrent connections that can be opened to the server.
			/// </summary>
			public int ConnectionLimit
			{
				get { return _adapterConfig.ConnectionLimit; }
				set { _adapterConfig.ConnectionLimit = value; }
			}

			/// <summary>
			/// A temporary folder on the SFTP server to upload large files to, before they can be atomically moved to the
			/// required location on the same server.
			/// </summary>
			public string TemporaryFolder
			{
				get { return _adapterConfig.TemporaryFolder; }
				set { _adapterConfig.TemporaryFolder = value; }
			}

			#endregion

			#region Proxy Settings

			/// <summary>
			/// Specifies either the DNS name or the IP address of the proxy server.
			/// </summary>
			public string ProxyAddress
			{
				get { return _adapterConfig.ProxyAddress; }
				set { _adapterConfig.ProxyAddress = value; }
			}

			/// <summary>
			/// Specifies the port for the proxy server.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>1080</c>.
			/// </remarks>
			public int ProxyPort
			{
				get { return _adapterConfig.ProxyPort; }
				set { _adapterConfig.ProxyPort = value; }
			}

			/// <summary>
			/// Specifies the username for the proxy server.
			/// </summary>
			public string ProxyUserName
			{
				get { return _adapterConfig.ProxyUserName; }
				set { _adapterConfig.ProxyUserName = value; }
			}

			/// <summary>
			/// Specifies the password for the proxy server.
			/// </summary>
			public string ProxyPassword
			{
				get { return _adapterConfig.ProxyPassword; }
				set { _adapterConfig.ProxyPassword = value; }
			}

			/// <summary>
			/// Specifies the protocol used by the proxy server.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="Microsoft.BizTalk.Adapter.Sftp.ProxyType.None"/>.
			/// </remarks>
			public ProxyType ProxyType
			{
				get { return _adapterConfig.ProxyType; }
				set { _adapterConfig.ProxyType = value; }
			}

			#endregion

			#region Security Settings

			/// <summary>
			/// If set to <c>True</c>, the receive location accepts any SSH public host key from the server. If set to
			/// <c>False</c>, the receive location uses the fingerprint of the server for authentication. You specify the
			/// fingerprint in the <see cref="SshServerHostKeyFingerPrint"/> property.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			public bool AccessAnySshServerHostKey
			{
				get { return _adapterConfig.AccessAnySSHServerHostKey; }
				set { _adapterConfig.AccessAnySSHServerHostKey = value; }
			}

			/// <summary>
			/// Specifies the fingerprint of the server used by the adapter to authenticate the server if the <see
			/// cref="AccessAnySshServerHostKey"/> property is set to <c>False</c>. If the fingerprints do not match, the
			/// connection fails.
			/// </summary>
			public string SshServerHostKeyFingerPrint
			{
				get { return _adapterConfig.SSHServerHostKeyFingerPrint; }
				set { _adapterConfig.SSHServerHostKeyFingerPrint = value; }
			}

			/// <summary>
			/// Specifies the authentication method that the receive location uses for authenticating the client to the SSH
			/// Server.
			/// </summary>
			/// <remarks>
			/// <para>
			/// If set to Password, you must specify the value in the <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.Password"/> property. If set to <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.PublicKeyAuthentication"/>, you must specify
			/// the private key of the user in the <see cref="PrivateKeyFile"/> property. If set to <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.MultiFactorAuthentication"/> you must provide
			/// <see cref="UserName"/> with its <see cref="Password"/> and <see cref="PrivateKeyFile"/>. Additionally, if
			/// the private key is protected by a password, specify the password as well for the <see
			/// cref="PrivateKeyPassword"/> property.
			/// </para>
			/// <para>
			/// It defaults to <see cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.Password"/>.
			/// </para>
			/// </remarks>
			public ClientAuthenticationMode ClientAuthenticationMode
			{
				get { return _adapterConfig.ClientAuthenticationMode; }
				set { _adapterConfig.ClientAuthenticationMode = value; }
			}

			/// <summary>
			/// Specify the kind of encryption cipher.
			/// </summary>
			public EncryptionCipher EncryptionCipher
			{
				get { return _adapterConfig.EncryptionCipher; }
				set { _adapterConfig.EncryptionCipher = value; }
			}

			/// <summary>
			/// Specify the private key for the SFTP user if you set the <see cref="ClientAuthenticationMode"/> to <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.PublicKeyAuthentication"/>.
			/// </summary>
			public string PrivateKeyFile
			{
				get { return _adapterConfig.PrivateKeyFile; }
				set { _adapterConfig.PrivateKeyFile = value; }
			}

			/// <summary>
			/// Specify a private key password, if required for the key specified in the <see cref="PrivateKeyFile"/>
			/// property.
			/// </summary>
			public string PrivateKeyPassword
			{
				get { return _adapterConfig.PrivateKeyPassword; }
				set { _adapterConfig.PrivateKeyPassword = value; }
			}

			/// <summary>
			/// Specifies a username to log on to the SFTP server.
			/// </summary>
			public string UserName
			{
				get { return _adapterConfig.UserName; }
				set { _adapterConfig.UserName = value; }
			}

			/// <summary>
			/// Specify the SFTP user password if you set the <see cref="ClientAuthenticationMode"/> to <see
			/// cref="Microsoft.BizTalk.Adapter.Sftp.ClientAuthenticationMode.Password"/>.
			/// </summary>
			public string Password
			{
				get { return _adapterConfig.Password; }
				set { _adapterConfig.Password = value; }
			}

			#endregion

			#region SSH Server Settings

			/// <summary>
			/// Specifies the server name or IP address of the secure FTP server.
			/// </summary>
			public string ServerAddress
			{
				get { return _adapterConfig.ServerAddress; }
				set { _adapterConfig.ServerAddress = value; }
			}

			/// <summary>
			/// Specifies the port address for the secure FTP server on which the file transfer takes place.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>22</c>.
			/// </remarks>
			public int Port
			{
				get { return _adapterConfig.Port; }
				set { _adapterConfig.Port = value; }
			}

			/// <summary>
			/// Specifies the folder path on the secure FTP server where the file is copied.
			/// </summary>
			public string FolderPath
			{
				get { return _adapterConfig.FolderPath; }
				set { _adapterConfig.FolderPath = value; }
			}

			/// <summary>
			/// Specifies the name with which the file is transferred to the secure FTP server. You can also use macros for
			/// the target file name.
			/// </summary>
			public string TargetFileName
			{
				get { return _adapterConfig.TargetFileName; }
				set { _adapterConfig.TargetFileName = value; }
			}

			/// <summary>
			/// If the file being transferred to the secure FTP server already exists at the destination, this property
			/// specifies whether the data from the file being transferred should be appended to the existing file. If set
			/// to <c>True</c>, the data is appended. If set to <c>False</c>, the file at the destination server is
			/// overwritten.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			public bool AppendIfExists
			{
				get { return _adapterConfig.AppendIfExists; }
				set { _adapterConfig.AppendIfExists = value; }
			}

			#endregion
		}

		#endregion
	}
}

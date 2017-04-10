#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public abstract class WcfNetMsmqAdapter<TConfig>
		: WcfAdapterBase<EndpointAddress, NetMsmqBindingElement, TConfig>,
			IAdapterConfigMessageSecurity<MessageCredentialType>,
			IAdapterConfigNetMsmqSecurity,
			IAdapterConfigSecurityMode<NetMsmqSecurityMode>,
			IAdapterConfigServiceCertificate,
			IAdapterConfigTransactions
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigServiceCertificate,
			IAdapterConfigNetMsmqSecurity,
			IAdapterConfigTransactions,
			new()
	{
		static WcfNetMsmqAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("36f48beb-64aa-4c80-b396-1f2ba53bed84"));
		}

		protected WcfNetMsmqAdapter() : base(_protocolType)
		{
			// Binding Tab - Transactions Settings
			EnableTransaction = true;

			// Security Tab - Security Mode Settings
			SecurityMode = NetMsmqSecurityMode.Transport;

			// Security Tab - Transport Security Settings
			MsmqAuthenticationMode = MsmqAuthenticationMode.WindowsDomain;
			MsmqProtectionLevel = ProtectionLevel.Sign;
			MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.Sha1;
			MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.RC4Stream;

			// Security Tab - Message Security Settings
			MessageClientCredentialType = MessageCredentialType.Windows;
			AlgorithmSuite = SecurityAlgorithmSuiteValue.Basic256;
		}

		#region IAdapterConfigMessageSecurity<MessageCredentialType> Members

		/// <summary>
		/// Specify the type of credential to be used when performing client authentication using message-based
		/// security.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="MessageClientCredentialType"/> property, see
		/// the Message client credential type property in <see
		/// href="https://msdn.microsoft.com/en-us/library/bb226546.aspx">WCF-NetMsmq Transport Properties Dialog Box,
		/// Receive, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="MessageCredentialType.Windows"/>.
		/// </para>
		/// </remarks>
		public MessageCredentialType MessageClientCredentialType
		{
			get { return _adapterConfig.MessageClientCredentialType; }
			set { _adapterConfig.MessageClientCredentialType = value; }
		}

		/// <summary>
		/// Specify the message encryption and key-wrap algorithms. These algorithms map to those specified in the
		/// Security Policy Language (WS-SecurityPolicy) specification.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="SecurityAlgorithmSuiteValue"/> property, see
		/// the Message client credential type property in <see
		/// href="https://msdn.microsoft.com/en-us/library/bb226546.aspx">WCF-NetMsmq Transport Properties Dialog Box,
		/// Receive, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="SecurityAlgorithmSuiteValue.Basic256"/>.
		/// </para>
		/// </remarks>
		public SecurityAlgorithmSuiteValue AlgorithmSuite
		{
			get { return _adapterConfig.AlgorithmSuite; }
			set { _adapterConfig.AlgorithmSuite = value; }
		}

		#endregion

		#region IAdapterConfigNetMsmqSecurity Members

		/// <summary>
		/// Specify the type of security that is used.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="SecurityMode"/> property, see the Security
		/// mode property in <see href="https://msdn.microsoft.com/en-us/library/bb226546.aspx">WCF-NetMsmq Transport
		/// Properties Dialog Box, Receive, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="NetMsmqSecurityMode.Transport"/>.
		/// </para>
		/// </remarks>
		public NetMsmqSecurityMode SecurityMode
		{
			get { return _adapterConfig.SecurityMode; }
			set { _adapterConfig.SecurityMode = value; }
		}

		/// <summary>
		/// Specify how the message must be authenticated by the MSMQ transport.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="MsmqAuthenticationMode"/> property, see the
		/// MSMQ authentication mode property in <see
		/// href="https://msdn.microsoft.com/en-us/library/bb226546.aspx">WCF-NetMsmq Transport Properties Dialog Box,
		/// Receive, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="System.ServiceModel.MsmqAuthenticationMode.WindowsDomain"/>.
		/// </para>
		/// </remarks>
		public MsmqAuthenticationMode MsmqAuthenticationMode
		{
			get { return _adapterConfig.MsmqAuthenticationMode; }
			set { _adapterConfig.MsmqAuthenticationMode = value; }
		}

		/// <summary>
		/// Specify the way messages are secured at the level of the MSMQ transport. Encryption ensures message
		/// integrity, while sign and encrypt ensures both message integrity and non-repudiation.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <list type="bullet">
		/// <item>
		/// <see cref="ProtectionLevel.None"/> &#8212; No protection.
		/// </item>
		/// <item>
		/// <see cref="ProtectionLevel.Sign"/> &#8212; Messages are signed.
		/// </item>
		/// <item>
		/// <see cref="ProtectionLevel.EncryptAndSign"/> &#8212; Messages are encrypted and signed. To use this
		/// protection level, you must enable Active Directory Integration for MSMQ.
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// It defaults to <see cref="ProtectionLevel.Sign"/>.
		/// </para>
		/// </remarks>
		public ProtectionLevel MsmqProtectionLevel
		{
			get { return _adapterConfig.MsmqProtectionLevel; }
			set { _adapterConfig.MsmqProtectionLevel = value; }
		}

		/// <summary>
		/// Specify the hash algorithm to be used for computing the message digest. This property is not available if
		/// the <see cref="MsmqProtectionLevel"/> property is set to <see cref="ProtectionLevel.None"/>.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="System.ServiceModel.MsmqSecureHashAlgorithm.Sha1"/>.
		/// </remarks>
		public MsmqSecureHashAlgorithm MsmqSecureHashAlgorithm
		{
			get { return _adapterConfig.MsmqSecureHashAlgorithm; }
			set { _adapterConfig.MsmqSecureHashAlgorithm = value; }
		}

		/// <summary>
		/// Specify the algorithm to be used for message encryption on the wire when transferring messages between
		/// message queue managers. This property is available only if the <see cref="MsmqProtectionLevel"/> property
		/// is set to <see cref="ProtectionLevel.EncryptAndSign"/>.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="System.ServiceModel.MsmqEncryptionAlgorithm.RC4Stream"/>.
		/// </remarks>
		public MsmqEncryptionAlgorithm MsmqEncryptionAlgorithm
		{
			get { return _adapterConfig.MsmqEncryptionAlgorithm; }
			set { _adapterConfig.MsmqEncryptionAlgorithm = value; }
		}

		#endregion

		#region IAdapterConfigServiceCertificate Members

		public string ServiceCertificate
		{
			get { return _adapterConfig.ServiceCertificate; }
			set { _adapterConfig.ServiceCertificate = value; }
		}

		#endregion

		#region IAdapterConfigTransactions Members

		/// <summary>
		/// Specify whether the message queue is transactional or non-transactional.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If this property is selected, each message is delivered only once, and the sender is notified of delivery
		/// failures. To send messages through transactional send ports, both the <c>durable</c> and <c>exactlyOnce</c>
		/// binding elements of the client must be set to <c>True</c>. If this property is cleared, messages are
		/// transferred without delivery assurance.
		/// </para>
		/// <para>
		/// </para>
		/// If you use a transactional queue under this receive location, this property must be selected.
		/// <para>
		/// It defaults to <c>False</c>.
		/// </para>
		/// </remarks>
		public bool EnableTransaction
		{
			get { return _adapterConfig.EnableTransaction; }
			set { _adapterConfig.EnableTransaction = value; }
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}

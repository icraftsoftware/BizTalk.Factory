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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class SBMessagingAdapter<TConfig> : AdapterBase, IAdapterConfigAccessControlService
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			IAdapterConfigTimeouts,
			IAdapterConfigAcsCredentials,
			IAdapterConfigSasCredentials,
			new()
	{
		static SBMessagingAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("9c458d4a-a73c-4cb3-89c4-86ae0103de2f"));
		}

		protected SBMessagingAdapter() : base(_protocolType)
		{
			_adapterConfig = new TConfig();
		}

		#region IAdapterConfigAccessControlService Members

		public Uri StsUri
		{
			get { return new Uri(_adapterConfig.StsUri); }
			set { _adapterConfig.StsUri = value.ToString(); }
		}

		public string IssuerName
		{
			get { return _adapterConfig.IssuerName; }
			set { _adapterConfig.IssuerName = value; }
		}

		public string IssuerSecret
		{
			get { return _adapterConfig.IssuerSecret; }
			set { _adapterConfig.IssuerSecret = value; }
		}

		#endregion

		#region Base Class Member Overrides

		protected override string GetAddress()
		{
			return Address.IfNotNull(a => a.ToString());
		}

		protected override void Save(IPropertyBag propertyBag)
		{
			_adapterConfig.Save(propertyBag as Microsoft.BizTalk.ExplorerOM.IPropertyBag);
		}

		protected override void Validate()
		{
			_adapterConfig.Address = GetAddress();
			_adapterConfig.Validate();
			_adapterConfig.Address = null;
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;

		protected readonly TConfig _adapterConfig;

		#region General Tab

		/// <summary>
		/// Specify the URL where the Service Bus queue is deployed.
		/// </summary>
		/// <remarks>
		/// Typically the URL is in the following format:
		/// <![CDATA[sb://<namespace>.servicebus.windows.net/<queue_name>/]]>
		/// </remarks>
		public Uri Address { get; set; }

		/// <summary>
		/// Specifies a time span value that indicates the time for a channel open operation to complete.
		/// </summary>
		/// <remarks>
		/// It defaults to <c>1</c> minute.
		/// </remarks>
		public TimeSpan OpenTimeout
		{
			get { return _adapterConfig.OpenTimeout; }
			set { _adapterConfig.OpenTimeout = value; }
		}

		/// <summary>
		/// Specifies a time span value that indicates the time for a channel close operation to complete.
		/// </summary>
		/// <remarks>
		/// It defaults to <c>1</c> minute.
		/// </remarks>
		public TimeSpan CloseTimeout
		{
			get { return _adapterConfig.CloseTimeout; }
			set { _adapterConfig.CloseTimeout = value; }
		}

		#endregion

		#region Authentication Tab

		/// <summary>
		/// Whether to use an Access Control Service for authentication.
		/// </summary>
		public bool UseAcsAuthentication
		{
			get { return _adapterConfig.UseAcsAuthentication; }
			set { _adapterConfig.UseAcsAuthentication = value; }
		}

		/// <summary>
		/// Whether to use Shared Access Signature for authentication.
		/// </summary>
		public bool UseSasAuthentication
		{
			get { return _adapterConfig.UseSasAuthentication; }
			set { _adapterConfig.UseSasAuthentication = value; }
		}

		/// <summary>
		/// Specify the SAS key name.
		/// </summary>
		public string SharedAccessKeyName
		{
			get { return _adapterConfig.SharedAccessKeyName; }
			set { _adapterConfig.SharedAccessKeyName = value; }
		}

		/// <summary>
		/// Specify the SAS key value.
		/// </summary>
		public string SharedAccessKey
		{
			get { return _adapterConfig.SharedAccessKey; }
			set { _adapterConfig.SharedAccessKey = value; }
		}

		#endregion
	}
}

#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using System.Transactions;
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfSqlAdapter
	{
		#region Nested Type: Outbound

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Outbound : WcfSqlAdapter<CustomTLConfig>, IOutboundAdapter
		{
			public Outbound()
			{
				// Binding Tab - Buffering Settings
				BatchSize = 20;
				ChunkSize = 4096 * 1024;

				// Binding Tab - Miscellaneous Settings
				AllowIdentityInsert = false;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override void Validate()
			{
				// TODO PropagateFaultMessage for two-way only
				// TODO IsolationLevel iif EnableTransaction
				// TODO Proxy Settings
				// TODO see Microsoft.BizTalk.Adapter.Wcf.Metadata.BtsActionMapping and Microsoft.BizTalk.Adapter.Wcf.Metadata.BtsActionMappingHelper.CreateXml(BtsActionMapping btsActionMapping)
				// TODO validate BtsActionMapping against orchestration ports' actions
			}

			#endregion

			#region General Tab - SOAP Action Header Settings

			public string StaticAction
			{
				get { return _adapterConfig.StaticAction; }
				set { _adapterConfig.StaticAction = value; }
			}

			#endregion

			#region Binding Tab - Buffering Settings

			/// <summary>
			/// The number of rows to buffer in memory before attempting an Insert, Update or Delete operation. A higher value
			/// can improve performance, but requires more memory consumption.
			/// </summary>
			public int BatchSize
			{
				get { return _bindingConfigurationElement.BatchSize; }
				set { _bindingConfigurationElement.BatchSize = value; }
			}

			/// <summary>
			/// The size of the internal buffer used by the adapter during the SetXXX() operations.
			/// </summary>
			public int ChunkSize
			{
				get { return _bindingConfigurationElement.ChunkSize; }
				set { _bindingConfigurationElement.ChunkSize = value; }
			}

			#endregion

			#region Binding Tab - Miscellaneous Settings

			/// <summary>
			/// Determines whether the adapter does a "SET IDENTITY_INSERT ON" before an INSERT or UPDATE operation.
			/// </summary>
			public bool AllowIdentityInsert
			{
				get { return _bindingConfigurationElement.AllowIdentityInsert; }
				set { _bindingConfigurationElement.AllowIdentityInsert = value; }
			}

			#endregion

			#region Credentials Tab - User Name Credentials Settings

			public string AffiliateApplicationName
			{
				get { return _adapterConfig.AffiliateApplicationName; }
				set { _adapterConfig.AffiliateApplicationName = value; }
			}

			public string Password
			{
				get { return _adapterConfig.Password; }
				set { _adapterConfig.Password = value; }
			}

			public CredentialUse CredentialUse
			{
				get { return _adapterConfig.UseCredentials; }
			}

			public string UserName
			{
				get { return _adapterConfig.UserName; }
				set { _adapterConfig.UserName = value; }
			}

			public bool UseSSO
			{
				get { return _adapterConfig.UseSSO; }
				set { _adapterConfig.UseSSO = value; }
			}

			#endregion

			#region Credentials Tab - Proxy Settings

			public string ProxyAddress
			{
				get { return _adapterConfig.ProxyAddress; }
				set { _adapterConfig.ProxyAddress = value; }
			}

			public string ProxyPassword
			{
				get { return _adapterConfig.ProxyPassword; }
				set { _adapterConfig.ProxyPassword = value; }
			}

			public ProxySelection ProxyToUse
			{
				get { return _adapterConfig.ProxyToUse; }
				set { _adapterConfig.ProxyToUse = value; }
			}

			public string ProxyUserName
			{
				get { return _adapterConfig.ProxyUserName; }
				set { _adapterConfig.ProxyUserName = value; }
			}

			public bool UseProxy
			{
				get { return _adapterConfig.UseProxy; }
				set { _adapterConfig.UseProxy = value; }
			}

			#endregion

			#region Messages Tab - Error Handling Settings

			public bool PropagateFaultMessage
			{
				get { return _adapterConfig.PropagateFaultMessage; }
				set { _adapterConfig.PropagateFaultMessage = value; }
			}

			#endregion

			#region Messages Tab - Transactions Settings

			public bool EnableTransaction
			{
				get { return _adapterConfig.EnableTransaction; }
				set { _adapterConfig.EnableTransaction = value; }
			}

			public IsolationLevel IsolationLevel
			{
				get { return _adapterConfig.IsolationLevel; }
				set { _adapterConfig.IsolationLevel = value; }
			}

			#endregion
		}

		#endregion
	}
}

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

		/// <summary>
		/// The Microsoft BizTalk Adapter for SQL Server exposes the SQL Server database as a WCF service. Adapter clients
		/// can perform operations on the SQL Server database by exchanging SOAP messages with the adapter. The adapter
		/// consumes the SOAP message and makes appropriate ADO.NET calls to perform the operation. The adapter returns
		/// the response from the SQL Server database back to the client in the form of SOAP messages.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/dd788149.aspx">Overview of BizTalk Adapter for SQL Server</seealso>.
		/// <seealso href="https://msdn.microsoft.com/en-us/library/dd787981.aspx">Working with BizTalk Adapter for SQL Server Binding Properties</seealso>.
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Outbound : WcfSqlAdapter<CustomTLConfig>,
			IOutboundAdapter,
			IAdapterConfigOutboundAction,
			IAdapterConfigOutboundCredentials,
			IAdapterConfigOutboundPropagateFaultMessage,
			IAdapterConfigOutboundTransactionIsolation
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

			#region IAdapterConfigOutboundAction Members

			public string StaticAction
			{
				get { return _adapterConfig.StaticAction; }
				set { _adapterConfig.StaticAction = value; }
			}

			#endregion

			#region IAdapterConfigOutboundCredentials Members

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

			#region IAdapterConfigOutboundPropagateFaultMessage Members

			public bool PropagateFaultMessage
			{
				get { return _adapterConfig.PropagateFaultMessage; }
				set { _adapterConfig.PropagateFaultMessage = value; }
			}

			#endregion

			#region IAdapterConfigOutboundTransactionIsolation Members

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
		}

		#endregion
	}
}

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
	public partial class WcfOracleAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// The Microsoft BizTalk Adapter for Oracle Database exposes the Oracle database as a WCF service. Adapter
		/// clients can perform operations on the Oracle database by exchanging SOAP messages with the adapter. The
		/// adapter consumes the WCF message and makes appropriate ODP.NET calls to perform the operation. The adapter
		/// returns the response from the Oracle database back to the client in the form of SOAP messages.
		/// </summary>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/dd788161.aspx">Overview of BizTalk Adapter for Oracle Database</seealso>.
		/// <seealso href="https://msdn.microsoft.com/en-us/library/dd788467.aspx">Working with BizTalk Adapter for Oracle Database Binding Properties</seealso>.
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Outbound : WcfOracleAdapter<CustomTLConfig>,
			IInboundAdapter,
			IAdapterConfigOutboundAction,
			IAdapterConfigOutboundCredentials,
			IAdapterConfigOutboundPropagateFaultMessage
		{
			public Outbound() { }

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

			#region Messages Tab - Transactions Settings

			/// <summary>
			/// Specify whether a message is submitted to the MessageBox database using the transaction flowed from
			/// clients.
			/// </summary>
			/// <remarks>
			/// <para>
			/// If this property is set to <c>True</c>, the clients are required to submit messages using the transaction
			/// protocol specified in the TransactionProtocol property. If the clients submit messages outside the
			/// transactional scope then this receive location returns an exception back to the clients, and no messages
			/// are suspended.
			/// </para>
			/// <para>
			/// The option is available only for one-way receive locations. If the clients submit messages in a
			/// transactional context for request-response receive locations, then an exception is returned back to the
			/// clients and no messages are suspended.
			/// </para>
			/// <para>
			/// It defaults to <c>False</c>.
			/// </para>
			/// </remarks>
			public bool EnableTransaction
			{
				get { return _adapterConfig.EnableTransaction; }
				set { _adapterConfig.EnableTransaction = value; }
			}

			/// <summary>
			/// Specify the transaction protocol to be used with this receive location.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="TransactionProtocolValue.OleTransactions"/>.
			/// </remarks>
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

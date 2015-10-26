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

using System.Transactions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226442.aspx">WCF-Custom Transport Properties Dialog Box, Send, Messages Tab</seealso>
	public interface IAdapterConfigOutboundTransactionIsolation
	{
		#region Messages Tab - Transactions Settings

		/// <summary>
		/// Specify whether a message is send under <see cref="Transaction"/> scope.
		/// </summary>
		bool EnableTransaction { get; set; }

		/// <summary>
		/// Specify the Isolation Level for the transaction.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="System.Transactions.IsolationLevel.Serializable"/>.
		/// </remarks>
		IsolationLevel IsolationLevel { get; set; }

		#endregion
	}
}

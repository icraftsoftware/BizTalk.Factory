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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Tracking.Messaging;

namespace Be.Stateless.BizTalk
{
	namespace Factory
	{
		/// <summary>
		/// Default names for the messaging-only processes/flows tracked by BizTalk Factory.
		/// </summary>
		/// <remarks>
		/// <para>
		/// By convention, the 3rd and 4th tokens of the CLR namespace respectively denote the LOB/project and business
		/// area/domain to which a process/flow belongs. For instance <c>Be.Stateless.Shopping.Customer</c>,
		/// <c>Be.Stateless.Shopping.Invoicing</c>, and <c>Be.Stateless.Shopping.Shipping</c> cover 3 different business
		/// areas (<c>Customer</c>, <c>Invoicing</c>, and <c>Shipping</c>) of the <c>Shopping</c> LOB/project.
		/// </para>
		/// <para>
		/// The 5th token of the CLR namespace can be used to subdivide the business area/domain further, as in
		/// <c>Be.Stateless.Shopping.Invoicing.Billing</c> or <c>Be.Stateless.Shopping.Invoicing.Payments</c>.
		/// </para>
		/// <para>
		/// These tokens are of importance for the monitoring web site accompanying BizTalk Factory.
		/// </para>
		/// </remarks>
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		public class GlobalArea : ProcessName<GlobalArea>
		{
			/// <summary>
			/// Default name for failed messaging-only flows.
			/// </summary>
			public string Failed { get; private set; }

			/// <summary>
			/// Default name for unidentified messaging-only flows.
			/// </summary>
			public string Unidentified { get; private set; }
		}

		namespace ServiceArea
		{
			/// <summary>
			/// BizTalk Factory's process names.
			/// </summary>
			/// <remarks>
			/// <para>
			/// By convention, the 3rd and 4th tokens of the CLR namespace respectively denote the LOB/project and business
			/// area/domain to which a process/flow belongs. For instance <c>Be.Stateless.Shopping.Customer</c>,
			/// <c>Be.Stateless.Shopping.Invoicing</c>, and <c>Be.Stateless.Shopping.Shipping</c> cover 3 different
			/// business areas (<c>Customer</c>, <c>Invoicing</c>, and <c>Shipping</c>) of the <c>Shopping</c> LOB/project.
			/// </para>
			/// <para>
			/// The 5th token of the CLR namespace can be used to subdivide the business area/domain further, as in
			/// <c>Be.Stateless.Shopping.Invoicing.Billing</c> or <c>Be.Stateless.Shopping.Invoicing.Payments</c>.
			/// </para>
			/// <para>
			/// These tokens are of importance for the monitoring web site accompanying BizTalk Factory.
			/// </para>
			/// </remarks>
			[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
			public class Batch : ProcessName<Batch>
			{
				/// <summary>
				/// Name of the batch aggregating messaging-only process/flow.
				/// </summary>
				public string Aggregate { get; private set; }

				/// <summary>
				/// Name of the batch releasing messaging-only process/flow.
				/// </summary>
				public string Release { get; private set; }
			}

			/// <summary>
			/// BizTalk Factory's process names.
			/// </summary>
			/// <remarks>
			/// <para>
			/// By convention, the 3rd and 4th tokens of the CLR namespace respectively denote the LOB/project and business
			/// area/domain to which a process/flow belongs. For instance <c>Be.Stateless.Shopping.Customer</c>,
			/// <c>Be.Stateless.Shopping.Invoicing</c>, and <c>Be.Stateless.Shopping.Shipping</c> cover 3 different
			/// business areas (<c>Customer</c>, <c>Invoicing</c>, and <c>Shipping</c>) of the <c>Shopping</c> LOB/project.
			/// </para>
			/// <para>
			/// The 5th token of the CLR namespace can be used to subdivide the business area/domain further, as in
			/// <c>Be.Stateless.Shopping.Invoicing.Billing</c> or <c>Be.Stateless.Shopping.Invoicing.Payments</c>.
			/// </para>
			/// <para>
			/// These tokens are of importance for the monitoring web site accompanying BizTalk Factory.
			/// </para>
			/// </remarks>
			[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
			public class Claim : ProcessName<Claim>
			{
				/// <summary>
				/// Name of the claim check messaging-only process/flow.
				/// </summary>
				public string Check { get; private set; }
			}
		}
	}
}

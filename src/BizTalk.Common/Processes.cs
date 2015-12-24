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
		public class Default : ProcessName<Default>
		{
			/// <summary>
			/// Default name for failed messaging-only flows.
			/// </summary>
			public string Failed { get; private set; }

			/// <summary>
			/// Default name for unidentified messaging-only flows.
			/// </summary>
			/// <remarks>
			/// This property has only been declared so as to be automatically deployed in the BizTalkFactoryMgmtDb with
			/// other messaging-only flow names. It is never used anywhere but in testing code.
			/// </remarks>
			public string Unidentified { get; private set; }
		}

		namespace Services
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

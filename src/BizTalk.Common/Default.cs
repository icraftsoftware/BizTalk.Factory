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
		namespace Areas
		{
			/// <summary>
			/// Default names for the messaging-only processes/flows tracked by BizTalk Factory.
			/// </summary>
			/// <remarks>
			/// <para>
			/// By convention, the token that precedes the 'Areas' or the 'Orchestrations' token of the CLR namespace 
			/// denotes the LOB/project to which a process belongs. The area is represented by the token that follows 
			/// 'Areas' or 'Orchestrations', and the process name itself is the next token.
			/// For instance <c>Be.Stateless.ECommerce.Areas.Shopping.CustomerRegistration</c>,
			/// <c>Be.Stateless.ECommerce.Areas.Shopping.Invoicing</c>, and <c>Be.Stateless.ECommerce.Areas.Shopping.Shipping</c>
			/// cover 3 different business processes (<c>CustomerRegistration</c>, <c>Invoicing</c>, and <c>Shipping</c>) 
			/// in the <c>Shopping</c> area if the <c>ECommerce</c> LOB/project.
			/// </para>
			/// <para>
			/// These tokens are of importance for the monitoring web site accompanying BizTalk Factory. 
			/// An area named <see cref="Default"/> can be used when one does not wish to use
			/// areas to partition processes. The default area will not appear in the monitoring site.
			/// </para>
			/// </remarks>
			[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
			public class Default : ProcessName<Default>
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

			/// <summary>
			/// BizTalk Factory's process names.
			/// </summary>
			/// <remarks>
			/// <para>
			/// By convention, the token that precedes the 'Areas' or the 'Orchestrations' token of the CLR namespace 
			/// denotes the LOB/project to which a process belongs. The area is represented by the token that follows 
			/// 'Areas' or 'Orchestrations', and the process name itself is the next token.
			/// For instance <c>Be.Stateless.ECommerce.Areas.Shopping.CustomerRegistration</c>,
			/// <c>Be.Stateless.ECommerce.Areas.Shopping.Invoicing</c>, and <c>Be.Stateless.ECommerce.Areas.Shopping.Shipping</c>
			/// cover 3 different business processes (<c>CustomerRegistration</c>, <c>Invoicing</c>, and <c>Shipping</c>) 
			/// in the <c>Shopping</c> area if the <c>ECommerce</c> LOB/project.
			/// <seealso cref="Default"/>
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
			/// By convention, the token that precedes the 'Areas' or the 'Orchestrations' token of the CLR namespace 
			/// denotes the LOB/project to which a process belongs. The area is represented by the token that follows 
			/// 'Areas' or 'Orchestrations', and the process name itself is the next token.
			/// For instance <c>Be.Stateless.ECommerce.Areas.Shopping.CustomerRegistration</c>,
			/// <c>Be.Stateless.ECommerce.Areas.Shopping.Invoicing</c>, and <c>Be.Stateless.ECommerce.Areas.Shopping.Shipping</c>
			/// cover 3 different business processes (<c>CustomerRegistration</c>, <c>Invoicing</c>, and <c>Shipping</c>) 
			/// in the <c>Shopping</c> area if the <c>ECommerce</c> LOB/project.
			/// <seealso cref="Default"/>
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

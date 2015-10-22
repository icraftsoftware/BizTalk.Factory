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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfCustomAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The WCF-Custom adapter is used to enable the use of WCF extensibility components in BizTalk Server. The
		/// adapter enables complete flexibility of the WCF framework. It allows users to select and configure a WCF
		/// binding for the receive location and send port. It also allows users to set the endpoint behaviors and
		/// security settings.
		/// </summary>
		/// You use the WCF-Custom receive adapter to receive WCF service requests through the bindings, service behavior,
		/// endpoint behavior, security mechanism, and the source of the inbound message body that you selected and
		/// configured in the transport properties dialog in the receive location. A receive location that uses the
		/// WCF-Custom receive adapter can be configured as one-way or request-response (two-way).
		/// <remarks>
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb226367.aspx">What Is the WCF-Custom Adapter?</seealso>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb259941.aspx">How to Configure a WCF-Custom Receive Location</seealso>.
		/// <seealso href="https://msdn.microsoft.com/en-us/library/bb245991.aspx">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Inbound<TBinding> : WcfCustomAdapter<TBinding, CustomRLConfig>,
			IInboundAdapter,
			IAdapterConfigInboundCredentials,
			IAdapterConfigInboundDisableLocationOnFailure,
			IAdapterConfigInboundIncludeExceptionDetailInFaults,
			IAdapterConfigInboundSuspendRequestMessageOnFailure
			where TBinding : StandardBindingElement, new()
		{
			public Inbound()
			{
				// Other Tab - Credentials Settings
				CredentialType = CredentialSelection.None;

				// Messages Tab - Error Handling Settings
				DisableLocationOnFailure = false;
				SuspendRequestMessageOnFailure = true;
				IncludeExceptionDetailInFaults = true;

				ServiceBehaviors = Enumerable.Empty<IServiceBehavior>();
			}

			public Inbound(Action<Inbound<TBinding>> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region IAdapterConfigInboundCredentials Members

			public CredentialSelection CredentialType
			{
				get { return _adapterConfig.CredentialType; }
				set { _adapterConfig.CredentialType = value; }
			}

			public string UserName
			{
				get { return _adapterConfig.UserName; }
				set { _adapterConfig.UserName = value; }
			}

			public string Password
			{
				get { return _adapterConfig.Password; }
				set { _adapterConfig.Password = value; }
			}

			public string AffiliateApplicationName
			{
				get { return _adapterConfig.AffiliateApplicationName; }
				set { _adapterConfig.AffiliateApplicationName = value; }
			}

			#endregion

			#region IAdapterConfigInboundDisableLocationOnFailure Members

			public bool DisableLocationOnFailure
			{
				get { return _adapterConfig.DisableLocationOnFailure; }
				set { _adapterConfig.DisableLocationOnFailure = value; }
			}

			#endregion

			#region IAdapterConfigInboundIncludeExceptionDetailInFaults Members

			public bool IncludeExceptionDetailInFaults
			{
				get { return _adapterConfig.IncludeExceptionDetailInFaults; }
				set { _adapterConfig.IncludeExceptionDetailInFaults = value; }
			}

			#endregion

			#region IAdapterConfigInboundSuspendRequestMessageOnFailure Members

			public bool SuspendRequestMessageOnFailure
			{
				get { return _adapterConfig.SuspendMessageOnFailure; }
				set { _adapterConfig.SuspendMessageOnFailure = value; }
			}

			#endregion

			#region Base Class Member Overrides

			protected override void Save(IPropertyBag propertyBag)
			{
				_adapterConfig.ServiceBehaviorConfiguration = ServiceBehaviors.GetServiceBehaviorElementXml();
				base.Save(propertyBag);
			}

			#endregion

			#region Base Class Member Overrides

			protected override void Validate()
			{
				_adapterConfig.Address = Address.Uri.ToString();
				base.Validate();
				_adapterConfig.Address = null;
			}

			#endregion

			/// <summary>
			/// Specify whether to preserve message order when processing messages received over the NetMsmq binding.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>
			/// </remarks>
			public bool OrderedProcessing
			{
				get { return _adapterConfig.OrderedProcessing; }
				set { _adapterConfig.OrderedProcessing = value; }
			}

			public IEnumerable<IServiceBehavior> ServiceBehaviors { get; set; }
		}

		#endregion
	}
}

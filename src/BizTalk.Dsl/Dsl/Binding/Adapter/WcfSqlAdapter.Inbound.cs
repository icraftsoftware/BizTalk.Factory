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
using System.ServiceModel.Description;
using Microsoft.Adapters.Sql;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfSqlAdapter
	{
		#region Nested Type: Inbound

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Inbound : WcfSqlAdapter<CustomRLConfig>, IInboundAdapter
		{
			public Inbound()
			{
				// Binding Tab - Inbound Settings
				InboundOperationType = InboundOperation.Polling;

				// Binding Tab - Notification Settings
				NotifyOnListenerStart = true;

				// Binding Tab - Polling Settings
				PollingInterval = TimeSpan.FromSeconds(30);
				PollWhileDataFound = false;

				// Other Tab - Polling Credentials Settings
				CredentialType = CredentialSelection.None;

				// Messages Tab - Error Handling Settings
				DisableLocationOnFailure = false;
				SuspendRequestMessageOnFailure = true;
				IncludeExceptionDetailInFaults = true;

				ServiceBehaviors = Enumerable.Empty<IServiceBehavior>();
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override void Save(IPropertyBag propertyBag)
			{
				_adapterConfig.ServiceBehaviorConfiguration = GetServiceBehaviorConfiguration(ServiceBehaviors);
				base.Save(propertyBag);
			}

			protected override void Validate()
			{
				//base.Validate();
				// TODO validate Notification Settings if InboundOperation.Notification 
				// TODO validate Polling Settings if different than InboundOperation.Notification
				// TODO validate Polling Credentials Settings, i.e. user name + password if CredentialSelection.UserAccount
				// TODO validate Polling Credentials Settings, i.e. AffiliateApplicationName if CredentialSelection.GetCredentials
			}

			#endregion

			#region Binding Tab - Inbound Settings

			/// <summary>
			/// The inbound operation which needs to be performed.
			/// </summary>
			public InboundOperation InboundOperationType
			{
				get { return _bindingConfigurationElement.InboundOperationType; }
				set { _bindingConfigurationElement.InboundOperationType = value; }
			}

			#endregion

			#region Behavior Tab - ServiceBehavior Settings

			public IEnumerable<IServiceBehavior> ServiceBehaviors { get; set; }

			#endregion

			#region Binding Tab - Notification Settings

			/// <summary>
			/// The SQL SELECT or EXEC statement against which notifications will be registered.
			/// </summary>
			public string NotificationStatement
			{
				get { return _bindingConfigurationElement.NotificationStatement; }
				set { _bindingConfigurationElement.NotificationStatement = value; }
			}

			/// <summary>
			/// Determines whether the adapter should send a notification message when the Listener is started.
			/// </summary>
			public bool NotifyOnListenerStart
			{
				get { return _bindingConfigurationElement.NotifyOnListenerStart; }
				set { _bindingConfigurationElement.NotifyOnListenerStart = value; }
			}

			#endregion

			#region Binding Tab - Polling Settings

			/// <summary>
			/// This statement is executed to determine whether data is available to be polled. Execution of this statement
			/// should return a single result set consisting of one row and one column, the value in which should reflect
			/// the number of rows available to be read.
			/// </summary>
			public string PolledDataAvailableStatement
			{
				get { return _bindingConfigurationElement.PolledDataAvailableStatement; }
				set { _bindingConfigurationElement.PolledDataAvailableStatement = value; }
			}

			/// <summary>
			/// The SQL statement used to retrieve data from SQL, and optionally update the database. This statement will
			/// be executed within a transaction. In BizTalk Server, this same transaction will be used to insert the
			/// message into the Message Box.
			/// </summary>
			public string PollingStatement
			{
				get { return _bindingConfigurationElement.PollingStatement; }
				set { _bindingConfigurationElement.PollingStatement = value; }
			}

			/// <summary>
			/// The interval at which the adapter will execute the Polled Data Available Statement to determine whether
			/// data is available to be polled.
			/// </summary>
			public TimeSpan PollingInterval
			{
				get { return TimeSpan.FromSeconds(_bindingConfigurationElement.PollingIntervalInSeconds); }
				set { _bindingConfigurationElement.PollingIntervalInSeconds = (int) value.TotalSeconds; }
			}

			/// <summary>
			/// Controls whether the adapter should execute the Polled Data Available Statement even before the polling
			/// interval has elapsed, if the previous execution of the polling statement returned data.
			/// </summary>
			public bool PollWhileDataFound
			{
				get { return _bindingConfigurationElement.PollWhileDataFound; }
				set { _bindingConfigurationElement.PollWhileDataFound = value; }
			}

			#endregion

			#region Other Tab - Credentials Settings

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

			#region Messages Tab - Error Handling Settings

			public bool DisableLocationOnFailure
			{
				get { return _adapterConfig.DisableLocationOnFailure; }
				set { _adapterConfig.DisableLocationOnFailure = value; }
			}

			public bool IncludeExceptionDetailInFaults
			{
				get { return _adapterConfig.IncludeExceptionDetailInFaults; }
				set { _adapterConfig.IncludeExceptionDetailInFaults = value; }
			}

			public bool SuspendRequestMessageOnFailure
			{
				get { return _adapterConfig.SuspendMessageOnFailure; }
				set { _adapterConfig.SuspendMessageOnFailure = value; }
			}

			#endregion
		}

		#endregion
	}
}

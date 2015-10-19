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
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfSqlAdapter<TConfig> : WcfLobAdapterBase<SqlAdapterBindingConfigurationElement, TConfig>
		where TConfig : AdapterConfig,
			IAdapterConfigIdentity,
			IAdapterConfigBinding,
			IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new()
	{
		static WcfSqlAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("59b35d03-6a06-4734-a249-ef561254ecf7"));
		}

		protected WcfSqlAdapter() : base("sqlBinding")
		{
			// Binding Tab - BizTalk Settings
			EnableBizTalkCompatibilityMode = true;

			// Binding Tab - Connection Settings
			Encrypt = false;
			MaxConnectionPoolSize = 100;

			// Binding Tab - Diagnostics Settings
			EnablePerformanceCounters = false;

			// Binding Tab - Metadata Settings
			UseDatabaseNameInXsdNamespace = false;

			// Binding Tab - Transaction Settings
			UseAmbientTransaction = true;

			EndpointBehaviors = Enumerable.Empty<IEndpointBehavior>();
		}

		#region Base Class Member Overrides

		protected override Uri GetAddress()
		{
			return Address.Uri;
		}

		protected override ProtocolType GetProtocolType()
		{
			return _protocolType;
		}

		protected override void Save(IPropertyBag propertyBag)
		{
			_adapterConfig.EndpointBehaviorConfiguration = GetEndpointBehaviorElementXml(EndpointBehaviors);
			base.Save(propertyBag);
		}

		#endregion

		#region Base Class Member Overrides

		protected override void Validate()
		{
			// TODO _adapterConfig.Identity
			_adapterConfig.Validate();
		}

		#endregion

		#region Binding Tab - BizTalk Settings

		/// <summary>
		/// Whether the adapter will be used with BizTalk Server.
		/// </summary>
		public bool EnableBizTalkCompatibilityMode
		{
			get { return _bindingConfigurationElement.EnableBizTalkCompatibilityMode; }
			set { _bindingConfigurationElement.EnableBizTalkCompatibilityMode = value; }
		}

		#endregion

		#region Binding Tab - Diagnostics Settings

		/// <summary>
		/// Determines whether performance counters are enabled or not.
		/// </summary>
		public bool EnablePerformanceCounters
		{
			get { return _bindingConfigurationElement.EnablePerformanceCounters; }
			set { _bindingConfigurationElement.EnablePerformanceCounters = value; }
		}

		#endregion

		#region Behavior Tab - EndpointBehavior Settings

		public IEnumerable<IEndpointBehavior> EndpointBehaviors { get; set; }

		#endregion

		#region Binding Tab - General Settings

		/// <summary>
		/// Gets or sets the interval of time after which the receive method, invoked by a communication object, times
		/// out.
		/// </summary>
		/// <remarks>
		/// The interval of time that a connection can remain inactive, during which no application messages are received,
		/// before it is dropped. The default value is 10 minute.
		/// </remarks>
		/// <returns>
		/// The <see cref="T:Timespan"/> that specifies the interval of time to wait for the receive method to time out.
		/// </returns>
		/// <exception cref="T:ArgumentOutOfRangeException">
		/// The value is less than zero or too large.
		/// </exception>
		public TimeSpan ReceiveTimeout
		{
			get { return _bindingConfigurationElement.ReceiveTimeout; }
			set { _bindingConfigurationElement.ReceiveTimeout = value; }
		}

		#endregion

		#region Binding Tab - Transaction Settings

		/// <summary>
		/// Determines whether the adapter performs the operations on the SQL Server within the context of the ambient
		/// transaction. In BizTalk Server, the same transaction is used to publish/delete messages from the MessageBox.
		/// </summary>
		public bool UseAmbientTransaction
		{
			get { return _bindingConfigurationElement.UseAmbientTransaction; }
			set { _bindingConfigurationElement.UseAmbientTransaction = value; }
		}

		#endregion

		#region Binding Tab - Metadata Settings

		/// <summary>
		/// Determines whether the database name should be used in the XSD namespaces.
		/// </summary>
		public bool UseDatabaseNameInXsdNamespace
		{
			get { return _bindingConfigurationElement.UseDatabaseNameInXsdNamespace; }
			set { _bindingConfigurationElement.UseDatabaseNameInXsdNamespace = value; }
		}

		#endregion

		#region General Tab - Endpoint Address Settings

		public SqlAdapterConnectionUri Address { get; set; }

		public string Identity
		{
			get { return _adapterConfig.Identity; }
			set { _adapterConfig.Identity = value; }
		}

		#endregion

		#region Binding Tab - Connection Settings

		/// <summary>
		/// Determines whether SQL Server uses SSL encryption for all data sent between the client and server if the
		/// server has a certificate installed.
		/// </summary>
		public bool Encrypt
		{
			get { return _bindingConfigurationElement.Encrypt; }
			set { _bindingConfigurationElement.Encrypt = value; }
		}

		/// <summary>
		/// The maximum number of connections allowed in the connection pool for a particular connection URI.
		/// </summary>
		public int MaxConnectionPoolSize
		{
			get { return _bindingConfigurationElement.MaxConnectionPoolSize; }
			set { _bindingConfigurationElement.MaxConnectionPoolSize = value; }
		}

		/// <summary>
		/// Gets or sets the value for WorkstationId which is the name of the workstation connecting to SQL Server.
		/// </summary>
		public string WorkstationId
		{
			get { return _bindingConfigurationElement.WorkstationId; }
			set { _bindingConfigurationElement.WorkstationId = value; }
		}

		#endregion

		#region Binding Tab - FOR XML Settings

		/// <summary>
		/// Gets or sets the name of the root node which will be used for executing FOR XML stored procedures.
		/// </summary>
		public string XmlStoredProcedureRootNodeName
		{
			get { return _bindingConfigurationElement.XmlStoredProcedureRootNodeName; }
			set { _bindingConfigurationElement.XmlStoredProcedureRootNodeName = value; }
		}

		/// <summary>
		/// Gets or sets the namespace of the root node which will be used for executing FOR XML stored procedures.
		/// </summary>
		public string XmlStoredProcedureRootNodeNamespace
		{
			get { return _bindingConfigurationElement.XmlStoredProcedureRootNodeNamespace; }
			set { _bindingConfigurationElement.XmlStoredProcedureRootNodeNamespace = value; }
		}

		#endregion

		#region Messages Tab - Inbound BizTalk Message Body Settings

		public InboundMessageBodySelection InboundBodyLocation
		{
			get { return _adapterConfig.InboundBodyLocation; }
			set { _adapterConfig.InboundBodyLocation = value; }
		}

		public string InboundBodyPathExpression
		{
			get { return _adapterConfig.InboundBodyPathExpression; }
			set { _adapterConfig.InboundBodyPathExpression = value; }
		}

		public MessageBodyFormat InboundNodeEncoding
		{
			get { return _adapterConfig.InboundNodeEncoding; }
			set { _adapterConfig.InboundNodeEncoding = value; }
		}

		#endregion

		#region Messages Tab - Outbound WCF Message Body Settings

		public OutboundMessageBodySelection OutboundBodyLocation
		{
			get { return _adapterConfig.OutboundBodyLocation; }
			set { _adapterConfig.OutboundBodyLocation = value; }
		}

		public string OutboundXmlTemplate
		{
			get { return _adapterConfig.OutboundXmlTemplate; }
			set { _adapterConfig.OutboundXmlTemplate = value; }
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}

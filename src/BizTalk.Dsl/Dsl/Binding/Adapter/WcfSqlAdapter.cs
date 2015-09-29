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
using Microsoft.Adapters.Sql;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Adapter.Wcf.Converters;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfSqlAdapter<T> : AdapterBase, IAdapter, IAdapterBindingSerializerFactory
		where T : AdapterConfig,
			// TODO ?? IAdapterConfigAddress, ??
			IAdapterConfigIdentity,
			IAdapterConfigBinding,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new()
	{
		static WcfSqlAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("59b35d03-6a06-4734-a249-ef561254ecf7"));
		}

		protected WcfSqlAdapter()
		{
			_bindingConfigurationElement = new SqlAdapterBindingConfigurationElement { Name = "sqlBinding" };
			_adapterConfig = new T { BindingType = _bindingConfigurationElement.Name };

			// BizTalk Settings
			EnableBizTalkCompatibilityMode = true;

			// Connection Settings
			Encrypt = false;
			MaxConnectionPoolSize = 100;

			// Diagnostics Settings
			EnablePerformanceCounters = false;

			// Metadata Settings
			UseDatabaseNameInXsdNamespace = false;

			// Transaction Settings
			UseAmbientTransaction = true;
		}

		#region IAdapter Members

		string IAdapter.Address
		{
			get { return Address.Uri.ToString(); }
		}

		ProtocolType IAdapter.ProtocolType
		{
			get { return _protocolType; }
		}

		void IAdapter.Load(IPropertyBag propertyBag)
		{
			throw new NotImplementedException();
		}

		void IAdapter.Save(IPropertyBag propertyBag)
		{
			Save(propertyBag);
		}

		#endregion

		#region IAdapterBindingSerializerFactory Members

		IDslSerializer IAdapterBindingSerializerFactory.GetAdapterBindingSerializer()
		{
			return new AdapterBindingSerializer(this);
		}

		#endregion

		#region Base Class Member Overrides

		protected override void Validate()
		{
			// TODO _adapterConfig.Identity
		}

		#endregion

		public SqlAdapterConnectionUri Address { get; set; }

		#region General Settings

		/// <summary>
		/// Gets or sets the interval of time after which the close method, invoked by a communication object, times out.
		/// </summary>
		/// <remarks>
		/// The interval of time provided for a connection to close before the transport raises an exception. The default
		/// value is 1 minute.
		/// </remarks>
		/// <returns>
		/// The <see cref="T:Timespan"/> that specifies the interval of time to wait for the close method to time out.
		/// </returns>
		/// <exception cref="T:ArgumentOutOfRangeException">
		/// The value is less than zero or too large.
		/// </exception>
		public TimeSpan CloseTimeout
		{
			get { return _bindingConfigurationElement.CloseTimeout; }
			set { _bindingConfigurationElement.CloseTimeout = value; }
		}

		/// <summary>
		/// Gets the interval of time after which the open method, invoked by a communication object, times out.
		/// </summary>
		/// <remarks>
		/// The interval of time provided for a connection to open before the transport raises an exception. The default
		/// value is 1 minute.
		/// </remarks>
		/// <returns>
		/// The <see cref="T:Timespan"/> that specifies the interval of time to wait for the open method to time out.
		/// </returns>
		/// <exception cref="T:ArgumentOutOfRangeException">
		/// The value is less than zero or too large.
		/// </exception>
		public TimeSpan OpenTimeout
		{
			get { return _bindingConfigurationElement.OpenTimeout; }
			set { _bindingConfigurationElement.OpenTimeout = value; }
		}

		///// <summary>
		///// Gets the name of the binding.
		///// </summary>
		///// <returns>
		///// The name of the binding.
		///// </returns>
		//TODO ?? public string Name ??
		//{
		//	get { return _bindingConfigurationElement.Name; }
		//	private set { _bindingConfigurationElement.Name = value; }
		//}

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

		/// <summary>
		/// Gets or sets the interval of time after which the send method, invoked by a communication object, times out.
		/// </summary>
		/// <remarks>
		/// The interval of time provided for a write operation to complete before the transport raises an exception. The
		/// default value is 1 minute.
		/// </remarks>
		/// <returns>
		/// The <see cref="T:Timespan"/> that specifies the interval of time to wait for the send method to time out.
		/// </returns>
		/// <exception cref="T:ArgumentOutOfRangeException">
		/// The value is less than zero or too large.
		/// </exception>
		public TimeSpan SendTimeout
		{
			get { return _bindingConfigurationElement.SendTimeout; }
			set { _bindingConfigurationElement.SendTimeout = value; }
		}

		#endregion

		#region BizTalk Settings

		/// <summary>
		/// Whether the adapter will be used with BizTalk Server.
		/// </summary>
		public bool EnableBizTalkCompatibilityMode
		{
			get { return _bindingConfigurationElement.EnableBizTalkCompatibilityMode; }
			set { _bindingConfigurationElement.EnableBizTalkCompatibilityMode = value; }
		}

		#endregion

		#region Connection Settings

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

		#region Diagnostics Settings

		/// <summary>
		/// Determines whether performance counters are enabled or not.
		/// </summary>
		public bool EnablePerformanceCounters
		{
			get { return _bindingConfigurationElement.EnablePerformanceCounters; }
			set { _bindingConfigurationElement.EnablePerformanceCounters = value; }
		}

		#endregion

		#region Endpoint Identity

		public string Identity
		{
			get { return _adapterConfig.Identity; }
			set { _adapterConfig.Identity = value; }
		}

		#endregion

		#region FOR XML Settings

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

		#region Inbound Message Marshalling Settings

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

		#region Outbound Message Marshalling Settings

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

		#region Metadata Settings

		/// <summary>
		/// Determines whether the database name should be used in the XSD namespaces.
		/// </summary>
		public bool UseDatabaseNameInXsdNamespace
		{
			get { return _bindingConfigurationElement.UseDatabaseNameInXsdNamespace; }
			set { _bindingConfigurationElement.UseDatabaseNameInXsdNamespace = value; }
		}

		#endregion

		#region Transaction Settings

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

		protected virtual void Save(IPropertyBag propertyBag)
		{
			var p = new ConfigurationProxy();
			p.SetBindingElement(_bindingConfigurationElement);
			_adapterConfig.BindingConfiguration = p.GetBindingElementXml(_bindingConfigurationElement.Name);
			_adapterConfig.Save(propertyBag as Microsoft.BizTalk.ExplorerOM.IPropertyBag);
		}

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;

		protected readonly T _adapterConfig;
		protected readonly SqlAdapterBindingConfigurationElement _bindingConfigurationElement;
	}
}

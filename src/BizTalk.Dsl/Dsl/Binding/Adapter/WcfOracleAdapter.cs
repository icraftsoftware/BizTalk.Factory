#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using Microsoft.Adapters.OracleDB;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;
using Microsoft.ServiceModel.Channels.Common;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfOracleAdapter<TConfig>
		: WcfLobAdapterBase<OracleDBConnectionUri, OracleDBBindingConfigurationElement, TConfig>,
			IAdapterConfigBizTalkCompatibilityMode,
			IAdapterConfigPerformanceCounters
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
			IAdapterConfigBinding,
			IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new()
	{
		static WcfOracleAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("d7127586-e851-412e-8a8a-2428aeddc219"));
		}

		protected WcfOracleAdapter() : base(_protocolType)
		{
			// Binding Tab - BizTalk Settings
			EnableBizTalkCompatibilityMode = true;

			// Binding Tab - Diagnostics Settings
			EnablePerformanceCounters = false;

			// Binding Tab - Buffer Management Settings
			DataFetchSize = ushort.MaxValue;
			InsertBatchSize = 1;
			MaxOutputAssociativeArrayElements = 32;
			MetadataPooling = true;
			StatementCachePurge = false;
			StatementCacheSize = 10;

			// Binding Tab - Metadata Settings
			EnableSafeTyping = false;
			UseSchemaInNameSpace = true;

			// Binding Tab - Oracle Connection Pool Settings
			DecrPoolSize = 1;
			IncrPoolSize = 5;
			MaxPoolSize = 100;
			MinPoolSize = 1;
			UseOracleConnectionPool = true;

			// Binding Tab - Run Time Behavior Settings
			SkipNilNodes = true;

			// Binding Tab - Transactions Settings
			UseAmbientTransaction = true;
		}

		#region IAdapterConfigBizTalkCompatibilityMode Members

		public bool EnableBizTalkCompatibilityMode
		{
			get { return _bindingConfigurationElement.EnableBizTalkCompatibilityMode; }
			set { _bindingConfigurationElement.EnableBizTalkCompatibilityMode = value; }
		}

		#endregion

		#region IAdapterConfigPerformanceCounters Members

		public bool EnablePerformanceCounters
		{
			get { return _bindingConfigurationElement.EnablePerformanceCounters; }
			set { _bindingConfigurationElement.EnablePerformanceCounters = value; }
		}

		#endregion

		#region Binding Tab - Run Time Behavior Settings

		/// <summary>
		/// Specifies whether the Oracle Database adapter will skip inserting or updating values for nodes that are
		/// marked as 'nil' in the request XML.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property is applicable for inserting or updating records in a table and for RECORD type parameters in
		/// stored procedures. Default is <c>True</c>, which means the adapter will skip passing values for nodes that
		/// are marked as 'nil'. In this case, the default value in Oracle (if specified) is taken into account for
		/// nodes that are marked as 'nil'. If set to <c>False</c>, the adapter explicitly passes a null value for
		/// these nodes.
		/// </para>
		/// <para>
		/// Notice that:
		/// <list type="bullet">
		/// <item>
		/// For nodes that are not present in the request XML, the adapter always skips passing values, irrespective of
		/// the value of the <see cref="SkipNilNodes"/> property.
		/// </item>
		/// <item>
		/// For PL/SQL tables of RECORDS, the adapter always passes a null value for nodes that are either marked as
		/// 'nil' or not present in the request XML, irrespective of the value of the <see cref="SkipNilNodes"/>
		/// property.
		/// </item>
		/// </list>
		/// The following example explains the difference in the adapter configuration based on the value you set for
		/// this property. Assume an XML request as follows: <code><![CDATA[
		/// <EMPNO>1000</EMPNO>
		/// <ENAME>John</ENAME>
		/// <SAL nil='true'></SAL>
		/// ]]></code> If <see cref="SkipNilNodes"/> is set to <c>True</c>, the adapter executes the following command:
		/// <code>INSERT INTO EMP (EMPNO, ENAME) VALUES (1000, 'John');</code>
		/// </para>
		/// <para>
		/// If <see cref="SkipNilNodes"/> is set to <c>False</c>, the adapter executes the following command:
		/// <code>INSERT INTO EMP (EMPNO, ENAME, SAL) VALUES (1000, 'John', null);</code>
		/// Note that in the second statement, the adapter explicitly inserts a null value for the parameter 'SAL'.
		/// </para>
		/// </remarks>
		public bool SkipNilNodes
		{
			get { return _bindingConfigurationElement.SkipNilNodes; }
			set { _bindingConfigurationElement.SkipNilNodes = value; }
		}

		#endregion

		#region Binding Tab - Transactions Settings

		/// <summary>
		/// Specifies whether the Oracle Database adapter performs the operations using the transaction context
		/// provided by the caller.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default value is <c>True</c>, which means that the adapter always performs the operations in a
		/// transaction context, assuming that the client is providing the transactional context. If there are other
		/// resources participating in the transaction, the connections created enlist in <see cref="Transaction"/> and
		/// are elevated to an MSDTC transaction.
		/// </para>
		/// <para>
		/// However, there can be scenarios where you do not want the adapter to perform operations in a transactional
		/// context. For example:
		/// <list type="bullet">
		/// <item>
		/// While performing a simple SELECT operation on the Oracle database (on a send port).
		/// </item>
		/// <item>
		/// While specify a polling statement that performs a SELECT operation and does not involve any changes to the
		/// table either through a DELETE statement or by invoking a stored procedure (on a receive port).
		/// </item>
		/// </list>
		/// Both these operations do not make any updates to the database table and hence, elevating these operations
		/// to use an MSDTC transaction can be a performance overhead. In such scenarios, you can set the binding
		/// property to <c>False</c> so that the Oracle Database adapter does not perform the operations in a
		/// transaction context.
		/// </para>
		/// <para>
		/// Notice that not performing operations in a transactional context is advisable only for operations that do
		/// not make changes to the database. For operations that update data in the database, we recommend setting the
		/// property to <c>True</c> otherwise you might either experience message loss or duplicate messages depending on
		/// whether you are performing inbound or outbound operations. 
		/// </para>
		/// </remarks>
		public bool UseAmbientTransaction
		{
			get { return _bindingConfigurationElement.UseAmbientTransaction; }
			set { _bindingConfigurationElement.UseAmbientTransaction = value; }
		}

		#endregion

		#region Binding Tab - UDT .NET Type Generation - Run Time Settings

		/// <summary>
		/// Specifies the name of the DLLs, separated by a semi-colon, which the adapter creates while generating
		/// metadata.
		/// </summary>
		/// <remarks>
		/// These DLLs are saved at the location you specified for the <see
		/// cref="OracleDBBindingConfigurationElement.GeneratedUserTypesAssemblyFilePath"/> property while generating
		/// metadata. You must manually copy these DLLs to the following locations:
		/// <list type="bullet">
		/// <item>
		/// For BizTalk projects, copy the DLLs at the same location as BTSNTSvc.exe. For BizTalk Server 2013 R2, this
		/// is available typically under &lt;installation drive&gt;:\Program Files\Microsoft BizTalk Server 2013 R2.
		/// </item>
		/// <item>
		/// For .NET Projects, copy the DLLs to the \bin\Development folder within your .NET project folder.
		/// </item>
		/// </list>
		/// This property is required only while sending and receiving messages to perform operations on the Oracle
		/// database.
		/// </remarks>
		public string UserAssembliesLoadPath
		{
			get { return _bindingConfigurationElement.UserAssembliesLoadPath; }
			set { _bindingConfigurationElement.UserAssembliesLoadPath = value; }
		}

		#endregion

		#region Binding Tab - Buffer Management Settings

		/// <summary>
		/// Specifies the amount of data in bytes that ODP.NET fetches from the result set in one server roundtrip.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is an ODP.NET property that is used for performance tuning.
		/// </para>
		/// <para>
		/// It defaults to 65536, roughly <see cref="ushort.MaxValue"/>.
		/// </para>
		/// </remarks>
		public long DataFetchSize
		{
			get { return _bindingConfigurationElement.DataFetchSize; }
			set { _bindingConfigurationElement.DataFetchSize = value; }
		}

		/// <summary>
		/// Specifies the batch size for multiple record Insert operations.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default is one. For values of <see cref="InsertBatchSize"/> greater than one, the Oracle Database
		/// adapter batches the specified number of records into a single ODP.NET call. If the number of records in the
		/// Insert operation is not a multiple of the batch size, the final batch will contain fewer records than the
		/// batch size value. For example, if the insert message has 10 records and the <see cref="InsertBatchSize"/>
		/// is set to 1, the adapter reads individual records and writes them into the Oracle database. So, the adapter
		/// performs 10 separate operations on the Oracle database. Similarly, if the insert message has 10 records and
		/// the <see cref="InsertBatchSize"/> is set to 5, the adapter will read and write 5 records at a time into the
		/// Oracle database, therefore performing only 2 insert operations.
		/// </para>
		/// <para>
		/// If the structure of the records is not the same across a batch, a <see cref="XmlReaderParsingException"/>
		/// exception is thrown and the transaction is rolled back for the entire insert operation. A well-chosen value
		/// for <see cref="InsertBatchSize"/> can greatly improve adapter performance for multiple record Insert
		/// operations.
		/// </para>
		/// </remarks>
		public int InsertBatchSize
		{
			get { return _bindingConfigurationElement.InsertBatchSize; }
			set { _bindingConfigurationElement.InsertBatchSize = value; }
		}

		/// <summary>
		/// Specifies the maximum size in bytes (32512) of an Oracle long data type column.
		/// </summary>
		/// <remarks>
		/// The default is 0. You must use the default value if you are not performing operation on long data type. To
		/// prefetch the data, you must specify -1 as the value for this binding property. You must explicitly set an
		/// appropriate value for this binding property if you are:
		/// <list type="bullet">
		/// <item>
		/// Executing a stored procedure that contains parameters of long data type.
		/// </item>
		/// <item>
		/// Performing a Select operation on a table that contains columns with long data type, and the SELECT
		/// statement does not include the primary key column.
		/// </item>
		/// </list>
		/// This binding property is deprecated and should be set only if Long data type is used.
		/// </remarks>
		public long LongDatatypeColumnSize
		{
#pragma warning disable 618
			get { return _bindingConfigurationElement.LongDatatypeColumnSize; }
			set { _bindingConfigurationElement.LongDatatypeColumnSize = value; }
#pragma warning restore 618
		}

		/// <summary>
		/// Specifies the size of the associate array that the adapter creates when performing operations that return
		/// an associative array in the response. The adapter communicates the size of the array to ODP.NET, which in
		/// turn creates a buffer depending on the array size.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This binding property is useful when performing operations involving PL/SQL table types.
		/// </para>
		/// <para>
		/// It defaults to 32.
		/// </para>
		/// </remarks>
		public int MaxOutputAssociativeArrayElements
		{
			get { return _bindingConfigurationElement.MaxOutputAssociativeArrayElements; }
			set { _bindingConfigurationElement.MaxOutputAssociativeArrayElements = value; }
		}

		/// <summary>
		/// Specifies whether ODP.NET caches metadata information for executed queries.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is an ODP.NET property that is used for performance tuning. Caching metadata information improves
		/// performance; however, if changes to the underlying Oracle artifacts occur on the Oracle system, this pooled
		/// metadata will be out of sync. This might cause operations performed on the Oracle system to return
		/// unexpected exceptions.
		/// </para>
		/// <para>
		/// The default is <c>True</c>, which enables metadata pooling.
		/// </para>
		/// </remarks>
		public bool MetadataPooling
		{
			get { return _bindingConfigurationElement.MetadataPooling; }
			set { _bindingConfigurationElement.MetadataPooling = value; }
		}

		/// <summary>
		/// Specifies whether the ODP.NET statement cache associated with a connection is purged when the connection is
		/// returned to the connection pool.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is an ODP.NET property that is used for performance tuning.
		/// </para>
		/// <para>
		/// The default is <c>False</c>, which disables statement cache purging.
		/// </para>
		/// </remarks>
		public bool StatementCachePurge
		{
			get { return _bindingConfigurationElement.StatementCachePurge; }
			set { _bindingConfigurationElement.StatementCachePurge = value; }
		}

		/// <summary>
		/// Specifies the maximum number of statements that can be cached by each ODP.NET connection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is an ODP.NET property that is used for performance tuning.
		/// </para>
		/// <para>
		/// The default is 10. Setting this property to zero disables statement caching for connections.
		/// </para>
		/// </remarks>
		public int StatementCacheSize
		{
			get { return _bindingConfigurationElement.StatementCacheSize; }
			set { _bindingConfigurationElement.StatementCacheSize = value; }
		}

		#endregion

		#region Binding Tab - Metadata Settings

		/// <summary>
		/// Enables or disables safe typing.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This feature controls how the adapter surfaces certain Oracle data types. For more information about safe
		/// typing, see <see href="https://msdn.microsoft.com/en-us/library/dd787930.aspx">Basic Oracle Data
		/// Types</see>.
		/// </para>
		/// <para>
		/// The default is False; safe typing is disabled.
		/// </para>
		/// </remarks>
		public bool EnableSafeTyping
		{
			get { return _bindingConfigurationElement.EnableSafeTyping; }
			set { _bindingConfigurationElement.EnableSafeTyping = value; }
		}

		/// <summary>
		/// Specifies whether the schema name (SCOTT, HR, and so on) is included in the xml namespace for operations
		/// and their associated types.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The advantage of not having scheme name included in the namespace is that if there is a table with same
		/// name (for example, EMP) in two different schemas then the same XML can be used to perform the simple SQL
		/// operations (Insert, Update, Delete, Select) on both tables. 
		/// </para>
		/// <para>
		/// For example, if the <see cref="UseSchemaInNameSpace"/> property is <c>True</c>, the namespace for these
		/// operations on the SCOTT.EMP table is
		/// "<c>http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Table/EMP</c>"; if it is <c>False</c>, the
		/// namespace is "<c>http://Microsoft.LobServices.OracleDB/2007/03/Table/EMP</c>".
		/// </para>
		/// <para>
		/// The message action is not affected by the <see cref="UseSchemaInNameSpace"/> binding property; it always
		/// includes the schema name.
		/// </para>
		/// <para>
		/// </para>
		/// It is strongly recommended to set this binding property to <c>True</c> while generating metadata. If you
		/// set this property to <c>False</c>, the Oracle schema names (for example, SCOTT) will not be available in
		/// the XML namespace of the generated schema. So, if there are two tables with the same name in two different
		/// Oracle schemas, and they are added to the same BizTalk project, the BizTalk project will fail to build and
		/// deploy. If you want to include such schemas in the same BizTalk project, you must manually edit them to
		/// include the Oracle schema name in the XML namespace. 
		/// <para>
		/// The default is True; the schema name is included in the namespace.
		/// </para>
		/// </remarks>
		public bool UseSchemaInNameSpace
		{
			get { return _bindingConfigurationElement.UseSchemaInNameSpace; }
			set { _bindingConfigurationElement.UseSchemaInNameSpace = value; }
		}

		#endregion

		#region Binding Tab - Oracle Connection Pool Settings

		/// <summary>
		/// Specifies the maximum duration in seconds of a connection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is an ODP.NET property that is used for performance tuning.
		/// </para>
		/// <para>
		/// The default is 0.
		/// </para>
		/// </remarks>
		public int ConnectionLifetime
		{
			get { return _bindingConfigurationElement.ConnectionLifetime; }
			set { _bindingConfigurationElement.ConnectionLifetime = value; }
		}

		/// <summary>
		/// Specifies the number of connections that are closed when an excessive amount of established connections are
		/// not in use.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is an ODP.NET property that is used for performance tuning.
		/// </para>
		/// <para>
		/// The default is 1.
		/// </para>
		/// </remarks>
		public int DecrPoolSize
		{
			get { return _bindingConfigurationElement.DecrPoolSize; }
			set { _bindingConfigurationElement.DecrPoolSize = value; }
		}

		/// <summary>
		/// Specifies the number of new connections to be created when a new connection is requested and there are no
		/// available connections in the ODP.NET connection pool.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is an ODP.NET property that is used for performance tuning.
		/// </para>
		/// <para>
		/// The default is 5.
		/// </para>
		/// </remarks>
		public int IncrPoolSize
		{
			get { return _bindingConfigurationElement.IncrPoolSize; }
			set { _bindingConfigurationElement.IncrPoolSize = value; }
		}

		/// <summary>
		/// Specifies the maximum number of connections in an ODP.NET connection pool.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is an ODP.NET property that is used for performance tuning. You must set MaxPoolSize judiciously. It
		/// is possible to exhaust the number of connections available from ODP.NET, if this value is set too large.
		/// </para>
		/// <para>
		/// The default is 100.
		/// </para>
		/// </remarks>
		public int MaxPoolSize
		{
			get { return _bindingConfigurationElement.MaxPoolSize; }
			set { _bindingConfigurationElement.MaxPoolSize = value; }
		}

		/// <summary>
		/// Specifies the minimum number of connections in an ODP.NET connection pool.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is an ODP.NET property that is used for performance tuning.
		/// </para>
		/// <para>
		/// The default is 1.
		/// </para>
		/// </remarks>
		public int MinPoolSize
		{
			get { return _bindingConfigurationElement.MinPoolSize; }
			set { _bindingConfigurationElement.MinPoolSize = value; }
		}

		/// <summary>
		/// Specifies whether to use the ODP.NET connection pool. The Oracle Database adapter implements connection
		/// pooling by using the ODP.NET connection pool.
		/// </summary>
		/// <remarks>
		/// The default is <c>True</c>, which enables connection pooling.
		/// </remarks>
		public bool UseOracleConnectionPool
		{
			get { return _bindingConfigurationElement.UseOracleConnectionPool; }
			set { _bindingConfigurationElement.UseOracleConnectionPool = value; }
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}

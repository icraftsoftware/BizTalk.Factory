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
using Microsoft.Adapters.SAP;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	// https://msdn.microsoft.com/en-us/library/dd787907.aspx
	public abstract class WcfSapAdapter<TConfig>
		: WcfLobAdapterBase<SAPConnectionUri, SAPAdapterBindingConfigurationElement, TConfig>,
			IAdapterConfigBizTalkCompatibilityMode,
			IAdapterConfigPerformanceCounters
		where TConfig : AdapterConfig,
			IAdapterConfigAddress,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
			IAdapterConfigBinding,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new()
	{
		static WcfSapAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("a5f15999-8879-472d-8c62-3b5ea9406504"));
		}

		protected WcfSapAdapter() : base(_protocolType)
		{
			// Binding Tab - BizTalk Settings
			EnableBizTalkCompatibilityMode = true;

			// Binding Tab - Connection Settings
			EnableConnectionPooling = true;
			IdleConnectionTimeout = TimeSpan.FromMinutes(15);
			MaxConnectionsPerSystem = 50;

			// Binding Tab - Diagnostics Settings
			EnablePerformanceCounters = false;
		}

		#region IAdapterConfigBizTalkCompatibilityMode Members

		/// <summary>
		/// Specifies whether the BizTalk Layered Channel Binding Element should be loaded. The BizTalk Layered Channel
		/// Binding Element is loaded to enable BizTalk transactions to flow through the SAP adapter to the SAP system.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Set this to <c>True</c> to load the binding element. Otherwise, set this to <c>False</c>.
		/// </para>
		/// <para>
		/// When using the adapters from BizTalk Server, you must always set the property to true. When using the adapters
		/// from Visual Studio, you must always set the property to false.
		/// </para>
		/// </remarks>
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

		#region Binding Tab - General Settings

		/// <summary>
		/// <para>
		/// The SAP system does not enforce correct values to be specified for DATS, TIMS, and NUMC fields. So, if invalid
		/// values are present in the SAP data store for DATS, TIMS, and NUMC fields and a client program tries to read
		/// the values using the SAP adapter, the adapter throws an exception.
		/// </para>
		/// <para>
		/// Also, the SAP system has special values for representing minimum and maximum values for the DATS, TIMS, and
		/// NUMC fields for which there is no equivalent .NET type. For example, the minimum and maximum values for a DATS
		/// field are 00000000 and 99999999 respectively, for which there is no equivalent .NET type <see
		/// cref="DateTime"/>. Moreover, converting the minimum and maximum values for DATS fields to <see
		/// cref="DateTime"/>.<see cref="DateTime.MinValue"/> and <see cref="DateTime"/>.<see cref="DateTime.MaxValue"/>
		/// is not feasible because the minimum or maximum value for DATS field and minimum or maximum value for a .NET
		/// <see cref="DateTime"/> type are not the same.
		/// </para>
		/// </summary>
		/// <remarks>
		/// To enable adapter clients to control the adapter behavior when special values are encountered in the SAP
		/// system, you can set the DataTypesBehavior binding property. This is a complex binding property that has the
		/// following sub-properties:
		/// <list type="table">
		/// <listheader>
		/// <term><see cref="SapDataTypesBehavior"/></term>
		/// <description>Description</description>
		/// </listheader>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.DateTimeMaxToDats"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to send a DATS value when the adapter client sends the value
		/// <see cref="DateTime"/>.<see cref="DateTime.MaxValue"/>, which is "9999-12-31T23:59:59.9999999". You could set
		/// this to the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if the client program sends the <see
		/// cref="DateTime"/>.<see cref="DateTime.MaxValue"/>.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter sends the specified value to SAP if the client program
		/// sends the <see cref="DateTime"/>.<see cref="DateTime.MaxValue"/> value.
		/// </item>
		/// </list>
		/// Default is 99991231.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.DateTimeMaxToTims"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to send a TIMS value when the adapter client sends the value
		/// <see cref="DateTime"/>.<see cref="DateTime.MaxValue"/>, which is "9999-12-31T23:59:59.9999999". You could set
		/// this to the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if the client program sends the <see
		/// cref="DateTime"/>.<see cref="DateTime.MaxValue"/> value.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter sends the specified value to SAP if the client program
		/// sends the <see cref="DateTime"/>.<see cref="DateTime.MaxValue"/> value.
		/// </item>
		/// </list>
		/// Default is 235959.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.DateTimeMinToDats"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to send a DATS value when the adapter client sends the value
		/// <see cref="DateTime"/>.<see cref="DateTime.MinValue"/>, which is "0001-01-01T00:00:00". You could set this to
		/// the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if the client program sends the <see
		/// cref="DateTime"/>.<see cref="DateTime.MinValue"/> value.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter sends the specified value to SAP if the client program
		/// sends the When set to this, the adapter sends the specified value to SAP if the client program sends the <see
		/// cref="DateTime"/>.<see cref="DateTime.MinValue"/> value.
		/// </item>
		/// </list>
		/// Default is 00010101.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.DateTimeMinToTims"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to send a TIMS value when the adapter client sends the value
		/// <see cref="DateTime"/>.<see cref="DateTime.MinValue"/>, which is "0001-01-01T00:00:00". You could set this to
		/// the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if the client program sends the <see
		/// cref="DateTime"/>.<see cref="DateTime.MinValue"/> value
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter sends the specified value to SAP if the client program
		/// sends the <see cref="DateTime"/>.<see cref="DateTime.MinValue"/> value.
		/// </item>
		/// </list>
		/// Default is 000000.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.DateTimeNullToDats"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to send a DATS value when the adapter client sends a NULL
		/// DateTime value. You could set this to the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if the client program sends a NULL DateTime
		/// value.
		/// </item>
		/// <item>
		/// "SKIP" &#8212; When set to this, the adapter skips the field and does not send any value to SAP if the client
		/// program sends a NULL DateTime value.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter sends the specified value to SAP if the client program
		/// sends a NULL DateTime value.
		/// </item>
		/// </list>
		/// Default is "SKIP".
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.DateTimeNullToTims"/></term>
		/// Specifies the behavior the adapter should follow to send a TIMS value when the adapter client sends a NULL
		/// DateTime value. You could set this to the following values:
		/// <description>
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if the client program sends a NULL DateTime
		/// value.
		/// </item>
		/// <item>
		/// "SKIP" &#8212; When set to this, the adapter skips the field and does not send any value to SAP if the client
		/// program sends a NULL DateTime value.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter sends the specified value to SAP if the client program
		/// sends a NULL DateTime value.
		/// </item>
		/// </list>
		/// Default is "SKIP".
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.DatsMaxToDateTime"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to retrieve a DateTime value when the adapter receives a
		/// DATS.MAX value, which is 99999999, from SAP. You could set this to the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if it receives a DATS.MAX value from SAP.
		/// </item>
		/// <item>
		/// "NULL" &#8212; When set to this, the adapter returns NULL if it receives a DATS.MAX value from SAP.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter parses the specified value in the XSD:DateTime format and
		/// returns it to the client program.
		/// </item>
		/// </list>
		/// Default is "ERROR".
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.DatsMinToDateTime"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to retrieve a DateTime value when the adapter receives a
		/// DATS.MIN value, which is 00000000, from SAP. You could set this to the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if it receives a DATS.MIN value from SAP.
		/// </item>
		/// <item>
		/// "NULL" &#8212; When set to this, the adapter returns NULL if it receives a DATS.MIN value from SAP.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter parses the specified value in the XSD:DateTime format and
		/// returns it to the client program.
		/// </item>
		/// </list>
		/// Default is "ERROR".
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.EmptyDatsToDateTime"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to retrieve a DateTime value when the adapter receives an
		/// empty DATS value from SAP. You could set this to the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if it receives an empty DATS value from SAP.
		/// </item>
		/// <item>
		/// "NULL" &#8212; When set to this, the adapter returns NULL if it receives an empty DATS value from SAP.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter parses the specified value in the XSD:DateTime format and
		/// returns it to the client program.
		/// </item>
		/// </list>
		/// Default is 0001-01-01T00:00:00.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.EmptyNumcToInt"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to retrieve an integer value when the adapter receives an
		/// empty NUMC value (all spaces) from SAP. You could set this to the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if it receives an empty NUMC value from SAP.
		/// </item>
		/// <item>
		/// "NULL" &#8212; When set to this, the adapter returns NULL if it receives an empty NUMC value from SAP.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter assumes that the specified value is a valid Int32 or Int64
		/// value and returns it to the client program.
		/// </item>
		/// </list>
		/// Default is 0.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.EmptyTimsToDateTime"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to retrieve a DateTime value when the adapter receives an
		/// empty TIMS value from SAP. You could set this to the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if it receives an empty TIMS value from SAP.
		/// </item>
		/// <item>
		/// "NULL" &#8212; When set to this, the adapter returns NULL if it receives an empty TIMS value from SAP.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter parses the specified value in the XSD:DateTime format and
		/// returns it to the client program.
		/// </item>
		/// </list>
		/// Default is 0001-01-01T00:00:00.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.InvalidDatsToDateTime"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to retrieve a DateTime value when the adapter receives an
		/// invalid DATS value from SAP. You could set this to the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if it receives an invalid DATS value from SAP.
		/// </item>
		/// <item>
		/// "NULL" &#8212; When set to this, the adapter returns NULL if it receives an invalid DATS value from SAP.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter parses the specified value in the XSD:DateTime format and
		/// returns it to the client program.
		/// </item>
		/// </list>
		/// Default is "ERROR".
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.InvalidNumcToInt"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to retrieve an integer value when the adapter receives an
		/// invalid NUMC value from SAP. You could set this to the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if it receives an invalid NUMC value from SAP.
		/// </item>
		/// <item>
		/// "NULL" &#8212; When set to this, the adapter returns NULL if it receives an invalid NUMC value from SAP.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter assumes that the specified value is a valid Int32 or Int64
		/// value and returns it to the client program.
		/// </item>
		/// </list>
		/// Default is 0.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="SapDataTypesBehavior.TimsMaxToDateTime"/></term>
		/// <description>
		/// Specifies the behavior the adapter should follow to retrieve a DateTime value when the adapter receives a
		/// TIMS.MAX value from SAP. You could set this to the following values:
		/// <list type="bullet">
		/// <item>
		/// "ERROR" &#8212; When set to this, the adapter throws an error if it receives a TIMS.MAX value from SAP.
		/// </item>
		/// <item>
		/// "NULL" &#8212; When set to this, the adapter returns NULL if it receives a TIMS.MAX value from SAP.
		/// </item>
		/// <item>
		/// &lt;VALUE&gt; &#8212; When set to this, the adapter parses the specified value in the XSD:DateTime format and
		/// returns it to the client program.
		/// </item>
		/// </list>
		/// Default is "ERROR".
		/// </description>
		/// </item>
		/// </list>
		/// </remarks>
		public SapDataTypesBehavior DataTypesBehavior
		{
			get { return _bindingConfigurationElement.DataTypesBehavior; }
			set { _bindingConfigurationElement.DataTypesBehavior = value; }
		}

		#endregion

		#region Binding Tab - Metadata  Settings

		/// <summary>
		/// Determines whether DATS, TIMS and NUMC are exposed as <seealso cref="string"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This feature controls how the adapter surfaces certain SAP data types. For more information about safe
		/// typing, <see href="https://msdn.microsoft.com/en-us/library/dd787893.aspx">see Basic SAP Data Types</see>.
		/// </para>
		/// <para>
		/// The default is false; safe typing is disabled.
		/// </para>
		/// </remarks>
		public bool EnableSafeTyping
		{
			get { return _bindingConfigurationElement.EnableSafeTyping; }
			set { _bindingConfigurationElement.EnableSafeTyping = value; }
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;

		#region Binding Tab - Connection Settings

		/// <summary>
		/// Determines whether connections are pooled.
		/// </summary>
		/// <remarks>
		/// It defaults to <c>True</c>.
		/// </remarks>
		public bool EnableConnectionPooling
		{
			get { return _bindingConfigurationElement.EnableConnectionPooling; }
			set { _bindingConfigurationElement.EnableConnectionPooling = value; }
		}

		/// <summary>
		/// The amount of time a connection remains idle in the connection pool before getting closed.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The idle connection timeout only applies to connections in the pool that are not being used. It does not
		/// affect active (open) connections which may be waiting for data.
		/// </para>
		/// <para>
		/// It defaults to 15 minutes.
		/// </para>
		/// </remarks>
		public TimeSpan IdleConnectionTimeout
		{
			get { return _bindingConfigurationElement.IdleConnectionTimeout; }
			set { _bindingConfigurationElement.IdleConnectionTimeout = value; }
		}

		/// <summary>
		/// Specifies the maximum number of connections in the SAP adapter connection pool that are allowed to connect
		/// to a SAP system.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <seealso cref="MaxConnectionsPerSystem"/> is a static property within an application domain. This means
		/// that when you change <seealso cref="MaxConnectionsPerSystem"/> for one binding instance in an application
		/// domain, the new value applies to all objects created from all binding instances within that application
		/// domain. 
		/// </para>
		/// <para>
		/// By default, the SAP client library (librfc32u.dll) supports a maximum of 100 connections to the SAP system.
		/// If you exceed this number of connections, an exception will be thrown by the SAP adapter. For this reason,
		/// you should not set MaxConnectionsPerSystem to a value greater than the number of connections supported by
		/// the SAP client library. You can increase the number of connections that the SAP client library supports by
		/// setting the environment variable, CPIC_MAX_CONV. You must reboot your computer after setting this variable
		/// for the change to take effect.
		/// </para>
		/// <para>
		/// It defaults to 50.
		/// </para>
		/// </remarks>
		public int MaxConnectionsPerSystem
		{
			get { return _bindingConfigurationElement.MaxConnectionsPerSystem; }
			set { _bindingConfigurationElement.MaxConnectionsPerSystem = value; }
		}

		/// <summary>
		/// Specifies the external programs that the RFC client library can start, if required by an RFC partner. For
		/// example, if you are invoking an RFC that internally invokes a program on the computer running the adapter
		/// client, you must specify the name of that program for this binding property.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If you are specifying multiple programs for this binding property, they must be separated by a semi-colon.
		/// For example, if you want to specify the sapftp and saphttp programs, you must specify them as
		/// sapftp;saphttp. Also, make sure the following conditions are met:
		/// <list type="bullet">
		/// <item>
		/// The external program required by the RFC is available on the computer running the adapter client.
		/// </item>
		/// <item>
		/// The location of the external program is present in the PATH variable on the computer running the adapter
		/// client.
		/// </item>
		/// </list>
		/// For example, BAPI_DOCUMENT_CHECKOUTVIEW2 internally executes a program, sapftp. So, while invoking this
		/// RFC, you must set the RfcAllowStartProgram binding property to sapftp. You must also ensure that the sapftp
		/// program is available locally, and the location of the sapftp program is added to the PATH variable on the
		/// computer running the adapter client.
		/// </para>
		/// <para>
		/// It defaults to <seealso cref="string.Empty"/>.
		/// </para>
		/// </remarks>
		public string RfcAllowStartProgram
		{
			get { return _bindingConfigurationElement.RfcAllowStartProgram; }
			set { _bindingConfigurationElement.RfcAllowStartProgram = value; }
		}

		#endregion

		#region Binding Tab - LogOn Ticket  Settings

		/// <summary>
		/// The LogOn Ticket to be passed to SAP. This is not supported for use with BizTalk Server.
		/// </summary>
		public string LogOnTicketPassword
		{
			get { return _bindingConfigurationElement.LogOnTicketPassword; }
			set { _bindingConfigurationElement.LogOnTicketPassword = value; }
		}

		/// <summary>
		/// Determines whether LogOn Ticket should be used for authentication. Only <see
		/// cref="Microsoft.Adapters.SAP.LogOnTicketType.None"/> is supported for use with BizTalk Server.
		/// </summary>
		public LogOnTicketType LogOnTicketType
		{
			get { return _bindingConfigurationElement.LogOnTicketType; }
			set { _bindingConfigurationElement.LogOnTicketType = value; }
		}

		#endregion

		#region Binding Tab - SNC  Settings

		/// <summary>
		/// Gets or Sets the External Identification Data. This can optionally be provided when SNC is used.
		/// </summary>
		public string ExternalIdentificationData
		{
			get { return _bindingConfigurationElement.ExternalIdentificationData; }
			set { _bindingConfigurationElement.ExternalIdentificationData = value; }
		}

		/// <summary>
		/// Gets or Sets the External Identification Type. Required if External Identification Data is provided.
		/// </summary>
		public string ExternalIdentificationType
		{
			get { return _bindingConfigurationElement.ExternalIdentificationType; }
			set { _bindingConfigurationElement.ExternalIdentificationType = value; }
		}

		/// <summary>
		/// Specifies the location of the SNC library on your computer. If the PATH environment variable contains the
		/// directory in which the library resides, you only have to supply the filename of the library; otherwise you
		/// must supply the full path.
		/// </summary>
		/// <remarks>
		/// The SncLibrary binding property surfaces an SAP connection property. For more information see the SAP
		/// documentation. You must set the UseSnc parameter in the connection URI to enable Secure Network
		/// Communications (SNC). For more information about the SAP connection URI, <see
		/// href="https://msdn.microsoft.com/en-us/library/dd788617.aspx">see The SAP System Connection URI</see>.
		/// </remarks>
		public string SncLibrary
		{
			get { return _bindingConfigurationElement.SncLibrary; }
			set { _bindingConfigurationElement.SncLibrary = value; }
		}

		/// <summary>
		/// The SNC Partner Name. Required if SNC is used.
		/// </summary>
		/// <remarks>
		/// The SncPartnerName binding property surfaces an SAP connection property. For more information, see the SAP
		/// documentation. You must set the UseSnc parameter in the connection URI to enable Secure Network
		/// Communications (SNC). For more information about the SAP connection URI, <see
		/// href="https://msdn.microsoft.com/en-us/library/dd788617.aspx">see The SAP System Connection URI</see>.
		/// </remarks>
		public string SncPartnerName
		{
			get { return _bindingConfigurationElement.SncPartnerName; }
			set { _bindingConfigurationElement.SncPartnerName = value; }
		}

		#endregion
	}
}

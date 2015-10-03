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
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace Be.Stateless.BizTalk.EnvironmentSettings
{
	[GeneratedCode("EnvironmentSettings", "1.0.0.0")]
	public static class CommonSettings
	{
		public static string BizTalkApplicationUserGroup 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"BizTalk Application Users", @"BizTalk Application Users", null, null }); }
		}

		public static string BizTalkIsolatedHostUserGroup 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"BizTalk Isolated Host Users", @"BizTalk Isolated Host Users", null, null }); }
		}

		public static string BizTalkServerAdministratorGroup 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"BizTalk Server Administrators", @"BizTalk Server Administrators", null, null }); }
		}

		public static string BizTalkServerOperatorEmail 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"biztalk.factory@stateless.be", @"biztalk.factory@stateless.be", null, null }); }
		}

		public static string BizTalkServerAccountName 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"BTS_USER", @"BTS_USER", null, null }); }
		}

		public static string BizTalkServerAccountPassword 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"p@ssw0rd", @"p@ssw0rd", null, null }); }
		}

		public static string SqlAgentAccountName 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"NT AUTHORITY\NetworkService", @"NT AUTHORITY\NetworkService", null, null }); }
		}

		public static string SsoAppUserGroup 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"BizTalk Application Users", @"BizTalk Application Users", null, null }); }
		}

		public static string SsoAppAdminGroup 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"BizTalk Server Administrators", @"BizTalk Server Administrators", null, null }); }
		}

		public static string ManagementDatabaseServer 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"localhost", @"localhost", null, null }); }
		}

		public static string MonitoringDatabaseServer 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"localhost", @"localhost", null, null }); }
		}

		public static string ProcessingDatabaseServer 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"localhost", @"localhost", null, null }); }
		}

		public static string BizTalkFactoryTransientStateDbUri 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"mssql://localhost//BizTalkFactoryTransientStateDb", @"mssql://localhost//BizTalkFactoryTransientStateDb", null, null }); }
		}

		public static string IsolatedHost 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"BizTalkServerIsolatedHost", @"BizTalkServerIsolatedHost", null, null }); }
		}

		public static string ReceiveHost 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"BizTalkServerApplication", @"BizTalkServerApplication", null, null }); }
		}

		public static string TransmitHost 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"BizTalkServerApplication", @"BizTalkServerApplication", null, null }); }
		}

		public static int RetryCountRealTime 
		{
			get { return ValueForTargetEnvironment(new int?[] { null, 0, 0, 0, 0 }); }
		}

		public static int RetryIntervalRealTime 
		{
			get { return ValueForTargetEnvironment(new int?[] { null, 1, 1, 1, 1 }); }
		}

		public static int RetryCountShortRunning 
		{
			get { return ValueForTargetEnvironment(new int?[] { null, 0, 0, 15, 15 }); }
		}

		public static int RetryIntervalShortRunning 
		{
			get { return ValueForTargetEnvironment(new int?[] { null, 1, 1, 2, 2 }); }
		}

		public static int RetryCountLongRunning 
		{
			get { return ValueForTargetEnvironment(new int?[] { null, 0, 0, 300, 300 }); }
		}

		public static int RetryIntervalLongRunning 
		{
			get { return ValueForTargetEnvironment(new int?[] { null, 1, 1, 15, 15 }); }
		}

		public static string LogDirectory 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"c:\files\logs", null, null, null }); }
		}

		public static string LogLevel 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"DEBUG", @"DEBUG", @"INFO", @"INFO" }); }
		}

		public static string TargetEnvironment
		{
			get { return _targetEnvironments[TargetEnvironmentIndex]; }
			set { TargetEnvironmentIndex = Array.IndexOf(_targetEnvironments, value); }
		}

		private static int TargetEnvironmentIndex { get; set; }

		private static T ValueForTargetEnvironment<T>(T?[] values, [CallerMemberName] string propertyName = null) where T : struct
		{
			var value = values[TargetEnvironmentIndex] ?? values[0];
			if (value == null) throw new InvalidOperationException(string.Format("'{0}' has neither a defined nor a default value.", propertyName));
			return value.Value;
		}

		private static T ValueForTargetEnvironment<T>(T[] values, [CallerMemberName] string propertyName = null) where T : class
		{
			var value = values[TargetEnvironmentIndex] ?? values[0];
			if (value == null) throw new InvalidOperationException(string.Format("'{0}' has neither a defined nor a default value.", propertyName));
			return value;
		}

		private static readonly string[] _targetEnvironments = { null, @"DEV", @"BLD", @"ACC", @"PRD" };
	}
}

#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Install;

namespace Be.Stateless.BizTalk.EnvironmentSettings
{
	[GeneratedCode("EnvironmentSettings", "1.0.0.0")]
	public partial class CommonSettings : Be.Stateless.BizTalk.Dsl.Binding.EnvironmentSettings
	{
		static CommonSettings()
		{
			_instance = new CommonSettings();
		}

		public static string BizTalkApplicationUserGroup 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "BizTalk Application Users", "BizTalk Application Users", null, null }); }
		}

		public static string BizTalkIsolatedHostUserGroup 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "BizTalk Isolated Host Users", "BizTalk Isolated Host Users", null, null }); }
		}

		public static string BizTalkServerAdministratorGroup 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "BizTalk Server Administrators", "BizTalk Server Administrators", null, null }); }
		}

		public static string BizTalkServerOperatorEmail 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "biztalk.factory@stateless.be", "biztalk.factory@stateless.be", null, null }); }
		}

		public static string BizTalkServerAccountName 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "BTS_USER", "BTS_USER", null, null }); }
		}

		public static string BizTalkServerAccountPassword 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "p@ssw0rd", "p@ssw0rd", null, null }); }
		}

		public static string SqlAgentAccountName 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "NT AUTHORITY\\NetworkService", "NT AUTHORITY\\NetworkService", null, null }); }
		}

		public static string QuartzAgentTargetHosts 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "-", "-", "*", "*" }); }
		}

		public static string SsoAppUserGroup 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "BizTalk Application Users", "BizTalk Application Users", null, null }); }
		}

		public static string SsoAppAdminGroup 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "BizTalk Server Administrators", "BizTalk Server Administrators", null, null }); }
		}

		public static string ManagementDatabaseServer 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "localhost", "localhost", null, null }); }
		}

		public static string MonitoringDatabaseServer 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "localhost", "localhost", null, null }); }
		}

		public static string ProcessingDatabaseServer 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "localhost", "localhost", null, null }); }
		}

		public static string IsolatedHost 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "BizTalkServerIsolatedHost", "BizTalkServerIsolatedHost", null, null }); }
		}

		public static string ReceiveHost 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "BizTalkServerApplication", "BizTalkServerApplication", null, null }); }
		}

		public static string TransmitHost 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "BizTalkServerApplication", "BizTalkServerApplication", null, null }); }
		}

		public static string LogDirectory 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "c:\\files\\logs", "c:\\files\\logs", null, null }); }
		}

		public static string LogLevel 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "INFO", "WARN", null, null }); }
		}

		protected override string SettingsFileName
		{
			get { return "Common.SettingsFileGenerator"; }
		}

		protected override string[] TargetEnvironments
		{
			get { return _targetEnvironments; }
		}

		private static readonly CommonSettings _instance;
		private static readonly string[] _targetEnvironments = { null, "DEV", "BLD", "ACC", "PRD" };
	}
}

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
	public partial class BizTalkFactorySettings : Be.Stateless.BizTalk.Dsl.Binding.EnvironmentSettings
	{
		static BizTalkFactorySettings()
		{
			_instance = new BizTalkFactorySettings();
		}

		public static int BamArchiveWindowTimeLength 
		{
			get { return _instance.ValueForTargetEnvironment(new int?[] { null, 30, 30, 30, 30 }); }
		}

		public static int BamOnlineWindowTimeLength 
		{
			get { return _instance.ValueForTargetEnvironment(new int?[] { null, 15, 15, 15, 15 }); }
		}

		public static string ClaimStoreAgentTargetHosts 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "-", "-", "*", "*" }); }
		}

		public static string ClaimStoreCheckInDirectory 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "C:\\Files\\Drops\\BizTalk.Factory\\CheckIn", "C:\\Files\\Drops\\BizTalk.Factory\\CheckIn", null, null }); }
		}

		public static string ClaimStoreCheckOutDirectory 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "C:\\Files\\Drops\\BizTalk.Factory\\CheckOut", "C:\\Files\\Drops\\BizTalk.Factory\\CheckOut", null, null }); }
		}

		public static string QuartzAgentTargetHosts 
		{
			get { return _instance.ValueForTargetEnvironment(new string[] { null, "-", "-", "*", "*" }); }
		}

		protected override string SettingsFileName
		{
			get { return "BizTalk.Factory.SettingsFileGenerator"; }
		}

		protected override string[] TargetEnvironments
		{
			get { return _targetEnvironments; }
		}

		private static readonly BizTalkFactorySettings _instance;
		private static readonly string[] _targetEnvironments = { null, "DEV", "BLD", "ACC", "PRD" };
	}
}

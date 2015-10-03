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
	public static class BizTalkFactorySettings
	{
		public static int BamArchiveWindowTimeLength 
		{
			get { return ValueForTargetEnvironment(new int?[] { null, 30, 30, 30, 30 }); }
		}

		public static int BamOnlineWindowTimeLength 
		{
			get { return ValueForTargetEnvironment(new int?[] { null, 15, 15, 15, 15 }); }
		}

		public static int BatchReleasePollingInterval 
		{
			get { return ValueForTargetEnvironment(new int?[] { null, 5, 5, 900, 900 }); }
		}

		public static int ClaimCheckOutPollingInterval 
		{
			get { return ValueForTargetEnvironment(new int?[] { null, 5, 5, 300, 300 }); }
		}

		public static string ClaimStoreAgentTargetHosts 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"-", @"-", @"*", @"*" }); }
		}

		public static string ClaimStoreCheckInDirectory 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"C:\Files\Drops\BizTalk.Factory\CheckIn", @"C:\Files\Drops\BizTalk.Factory\CheckIn", null, null }); }
		}

		public static string ClaimStoreCheckOutDirectory 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"C:\Files\Drops\BizTalk.Factory\CheckOut", @"C:\Files\Drops\BizTalk.Factory\CheckOut", null, null }); }
		}

		public static string QuartzAgentTargetHosts 
		{
			get { return ValueForTargetEnvironment(new string[] { null, @"-", @"-", @"*", @"*" }); }
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
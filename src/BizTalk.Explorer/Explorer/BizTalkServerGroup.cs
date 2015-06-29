#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using Microsoft.Win32;

namespace Be.Stateless.BizTalk.Explorer
{
	/// <summary>
	/// Access point to a BizTalk Server Group which defaults to the local BizTalk Server Group unless specified
	/// otherwise.
	/// </summary>
	public static class BizTalkServerGroup
	{
		static BizTalkServerGroup()
		{
			var key = Registry.LocalMachine.OpenSubKey(
				string.Format(
					@"SOFTWARE{0}Microsoft\Biztalk Server\3.0\Administration",
					Is64BitOperatingSystem ? @"\Wow6432Node\" : @"\"));
			if (key == null) throw new Exception("The registry key containing information about the BizTalk Server 2009 Management Database is missing.");
			ManagementDatabase = new BizTalkServerManagementDatabase((string) key.GetValue("MgmtDBServer"), (string) key.GetValue("MgmtDBName"));
			Applications = new ApplicationCollection();
		}

		/// <summary>
		/// The collection of applications installed on the local BizTalk Server Group.
		/// </summary>
		public static ApplicationCollection Applications { get; private set; }

		public static BizTalkServerManagementDatabase ManagementDatabase { get; private set; }

		/// <summary>
		/// Determines whether the current operating system is a 64-bit operating system.
		/// </summary>
		private static bool Is64BitOperatingSystem
		{
			get { return Environment.Is64BitOperatingSystem; }
		}
	}
}

#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using System.Security.Permissions;
using Be.Stateless.Logging;
using Be.Stateless.Security.Principal;

namespace Be.Stateless
{
	public static class Delegate
	{
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public static void InvokeAs(string username, string password, Action action)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Impersonating user '{0}'.", username);
			using (new WindowsIdentity(username, password).Impersonate())
			{
				action();
			}
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(System.Delegate));
	}
}
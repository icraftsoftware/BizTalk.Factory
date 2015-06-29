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
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions
{
	internal static class RegistryExtensions
	{
		internal static RegistryKey SafeOpenSubKey(this RegistryKey key, string name, [CallerMemberName] string memberName = "")
		{
			var subKey = key.OpenSubKey(name);
			if (subKey == null)
				throw new InvalidOperationException(
					string.Format(
						"Cannot retrieve {0} settings. BizTalk Server might not be deployed on this computer.",
						memberName));
			return subKey;
		}
	}
}

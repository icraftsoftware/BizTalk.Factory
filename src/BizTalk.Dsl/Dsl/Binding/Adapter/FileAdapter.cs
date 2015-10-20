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
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;
using Microsoft.Win32;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class FileAdapter : AdapterBase
	{
		static FileAdapter()
		{
			// [HKCR\TransportMgmt.FileMgmt\CLSID]
			using (var classes = Registry.ClassesRoot)
			using (var classIdKey = classes.SafeOpenSubKey(@"TransportMgmt.FileMgmt\CLSID"))
			{
				var configurationClassId = new Guid((string) classIdKey.GetValue(string.Empty));
				// [HKCR\Wow6432Node\CLSID\{5E49E3A6-B4FC-4077-B44C-22F34A242FDB}\BizTalk]
				var protocolType = GetProtocolTypeFromConfigurationClassId(configurationClassId);
				protocolType.Name = protocolType.Name.ToUpper();
				_protocolType = protocolType;
			}
		}

		protected FileAdapter() : base(_protocolType)
		{
			NetworkCredentials = new Credentials();
		}

		#region Base Class Member Overrides

		protected override void Save(IPropertyBag propertyBag)
		{
			if (NetworkCredentials.Username.IsNullOrEmpty()) return;
			propertyBag.WriteAdapterCustomProperty("Username", NetworkCredentials.Username);
			propertyBag.WriteAdapterCustomProperty("Password", NetworkCredentials.Password);
		}

		#endregion

		/// <summary>
		/// Credentials to use when host does not have access to network share.
		/// </summary>
		/// <remarks>
		/// Alternate credentials to access the file folder cannot be supplied while accessing local drive or a mapped
		/// network drive.
		/// </remarks>
		public Credentials NetworkCredentials { get; set; }

		private static readonly ProtocolType _protocolType;
	}
}

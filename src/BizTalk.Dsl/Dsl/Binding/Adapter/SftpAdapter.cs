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
using System.Diagnostics.CodeAnalysis;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class SftpAdapter<TConfig> : AdapterBase
		where TConfig : AdapterConfig, IAdapterConfigAddress, new()
	{
		static SftpAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("f75aeff5-ebc7-4e7c-a753-fdd68ab45c95"));
		}

		protected SftpAdapter() : base(_protocolType)
		{
			_adapterConfig = new TConfig();
		}

		#region Base Class Member Overrides

		protected override string GetAddress()
		{
			return _adapterConfig.Address;
		}

		protected override void Save(IPropertyBag propertyBag)
		{
			_adapterConfig.Save(propertyBag as Microsoft.BizTalk.ExplorerOM.IPropertyBag);
		}

		protected override void Validate()
		{
			_adapterConfig.Address = GetAddress();
			_adapterConfig.Validate();
			_adapterConfig.Address = null;
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;

		protected readonly TConfig _adapterConfig;
	}
}

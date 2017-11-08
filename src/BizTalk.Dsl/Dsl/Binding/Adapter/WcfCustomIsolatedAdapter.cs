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
using System.ServiceModel.Configuration;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public abstract class WcfCustomIsolatedAdapter<TBinding, TConfig> : WcfCustomAdapterBase<TBinding, TConfig>
		where TBinding : StandardBindingElement, new()
		where TConfig : RLConfig,
			IAdapterConfigAddress,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
			IAdapterConfigBinding,
			Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigEndpointBehavior,
			IAdapterConfigInboundMessageMarshalling,
			IAdapterConfigOutboundMessageMarshalling,
			new()
	{
		static WcfCustomIsolatedAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("16824334-968f-42db-b33b-6f8d62ed1ebc"));
		}

		protected WcfCustomIsolatedAdapter() : base(_protocolType) { }

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}

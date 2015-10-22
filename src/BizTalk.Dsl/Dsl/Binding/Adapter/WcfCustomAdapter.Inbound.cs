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

using System.Diagnostics.CodeAnalysis;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfCustomAdapter
	{
		#region Nested Type: Inbound

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
		public class Inbound : WcfCustomAdapter<CustomRLConfig>, IInboundAdapter {
			protected override string GetAddress()
			{
				throw new System.NotImplementedException();
			}

			protected override void Save(IPropertyBag propertyBag)
			{
				throw new System.NotImplementedException();
			}

			protected override void Validate()
			{
				throw new System.NotImplementedException();
			}
		}

		#endregion
	}
}

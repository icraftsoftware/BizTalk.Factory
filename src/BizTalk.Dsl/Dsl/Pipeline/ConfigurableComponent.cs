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
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	internal class ConfigurableComponent<T, TScope> : PipelineComponentDescriptor<T>, IConfigurableComponent<T, TScope>
		where T : IBaseComponent, IPersistPropertyBag
		where TScope : IHideObjectMembers
	{
		internal ConfigurableComponent(T pipelineComponent, TScope scope)
			: base(pipelineComponent)
		{
			_scope = scope;
		}

		#region IConfigurableComponent<T,TScope> Members

		public TScope Configure(Action<T> componentConfigurator)
		{
			componentConfigurator(this);
			return _scope;
		}

		#endregion

		private readonly TScope _scope;
	}
}

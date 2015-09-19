﻿#region Copyright & License

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

using System.Collections.Generic;
using System.Linq;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	internal class OrchestrationBindingCollection<TNamingConvention>
		: List<IOrchestrationBinding>,
			IOrchestrationBindingCollection,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class

	{
		public OrchestrationBindingCollection(IApplicationBinding<TNamingConvention> applicationBinding)
		{
			_applicationBinding = applicationBinding;
		}

		#region IOrchestrationBindingCollection Members

		IOrchestrationBindingCollection IOrchestrationBindingCollection.Add(IOrchestrationBinding orchestrationBinding)
		{
			return ((IOrchestrationBindingCollection) this).Add(new[] { orchestrationBinding });
		}

		IOrchestrationBindingCollection IOrchestrationBindingCollection.Add(params IOrchestrationBinding[] orchestrationBindings)
		{
			orchestrationBindings.Each(
				orchestrationBinding => {
					((ISupportValidation) orchestrationBinding).Validate();
					Add(orchestrationBinding);
				});
			return this;
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		void IVisitable<IApplicationBindingVisitor>.Accept(IApplicationBindingVisitor visitor)
		{
			this.Cast<IVisitable<IApplicationBindingVisitor>>().Each(ob => ob.Accept(visitor));
		}

		#endregion

		private readonly IApplicationBinding<TNamingConvention> _applicationBinding;
	}
}
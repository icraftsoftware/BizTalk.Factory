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

using System.Collections.Generic;
using System.Linq;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	internal class ReceivePortCollection<TNamingConvention>
		: List<IReceivePort<TNamingConvention>>,
			IReceivePortCollection<TNamingConvention>,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class
	{
		internal ReceivePortCollection(IApplicationBinding<TNamingConvention> applicationBinding)
		{
			_applicationBinding = applicationBinding;
		}

		#region IReceivePortCollection<TNamingConvention> Members

		IReceivePortCollection<TNamingConvention> IReceivePortCollection<TNamingConvention>.Add(IReceivePort<TNamingConvention> receivePort)
		{
			return ((IReceivePortCollection<TNamingConvention>) this).Add(new[] { receivePort });
		}

		IReceivePortCollection<TNamingConvention> IReceivePortCollection<TNamingConvention>.Add(params IReceivePort<TNamingConvention>[] receivePorts)
		{
			receivePorts.Each(
				rp => {
					((ReceivePortBase<TNamingConvention>) rp).ApplicationBinding = _applicationBinding;
					Add(rp);
				});
			return this;
		}

		public IReceivePort<TNamingConvention> Find<T>() where T : IReceivePort<TNamingConvention>
		{
			return this.Single(rp => rp.GetType() == typeof(T));
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		void IVisitable<IApplicationBindingVisitor>.Accept(IApplicationBindingVisitor visitor)
		{
			this.Cast<IVisitable<IApplicationBindingVisitor>>().Each(rp => rp.Accept(visitor));
		}

		#endregion

		private readonly IApplicationBinding<TNamingConvention> _applicationBinding;
	}
}

﻿#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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
	internal class ReferencedApplicationBindingCollection : List<IApplicationBinding>,
		IReferencedApplicationBindingCollection,
		IVisitable<IApplicationBindingVisitor>
	{
		#region IReferencedApplicationBindingCollection Members

		public IReferencedApplicationBindingCollection Add<T>(T applicationBinding) where T : IApplicationBinding
		{
			return ((IReferencedApplicationBindingCollection) this).Add(new[] { applicationBinding });
		}

		public IReferencedApplicationBindingCollection Add<T>(params T[] applicationBindings) where T : IApplicationBinding
		{
			applicationBindings.Each(applicationBinding => base.Add(applicationBinding));
			return this;
		}

		public T Find<T>() where T : IApplicationBinding
		{
			return this.OfType<T>().Single();
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		void IVisitable<IApplicationBindingVisitor>.Accept(IApplicationBindingVisitor visitor)
		{
			this.Cast<IVisitable<IApplicationBindingVisitor>>().Each(visitor.VisitReferencedApplicationBinding);
		}

		#endregion
	}
}

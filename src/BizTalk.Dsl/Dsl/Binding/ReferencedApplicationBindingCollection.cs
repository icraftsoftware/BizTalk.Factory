﻿#region Copyright & License

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

using System.Collections.Generic;
using System.Linq;
using Be.Stateless.BizTalk.Dsl.Binding.Visitor;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	internal class ReferencedApplicationBindingCollection : List<IVisitable<IApplicationBindingVisitor>>,
		IReferencedApplicationBindingCollection,
		IVisitable<IApplicationBindingVisitor>
	{
		public ReferencedApplicationBindingCollection()
		{
			_visitor = new ApplicationBindingSettlerVisitor();
		}

		#region IReferencedApplicationBindingCollection Members

		IReferencedApplicationBindingCollection IReferencedApplicationBindingCollection.Add<TReferencedApplicationNamingConvention>(
			IApplicationBinding<TReferencedApplicationNamingConvention> applicationBinding)
		{
			return ((IReferencedApplicationBindingCollection) this).Add(new[] { applicationBinding });
		}

		IReferencedApplicationBindingCollection IReferencedApplicationBindingCollection.Add<TReferencedApplicationNamingConvention>(
			params IApplicationBinding<TReferencedApplicationNamingConvention>[] applicationBindings)
		{
			applicationBindings.Each(
				applicationBinding => {
					var visitable = (IVisitable<IApplicationBindingVisitor>) applicationBinding;
					visitable.Accept(_visitor);
					Add(visitable);
				});
			return this;
		}

		public T Find<T>()
		{
			return (T) this.Single(ab => ab.GetType() == typeof(T));
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		void IVisitable<IApplicationBindingVisitor>.Accept(IApplicationBindingVisitor visitor)
		{
			this.Each(applicationBinding => applicationBinding.Accept(visitor));
		}

		#endregion

		private readonly ApplicationBindingSettlerVisitor _visitor;
	}
}

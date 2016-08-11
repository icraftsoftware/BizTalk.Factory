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
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel.Configuration;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration
{
	public class CustomBindingElement : StandardBindingElement, IEnumerable<BindingElementExtensionElement>, IBindingElementDecorator
	{
		public CustomBindingElement()
		{
			_decorated = new System.ServiceModel.Configuration.CustomBindingElement("customDslBinding");
		}

		#region IBindingElementDecorator Members

		public IBindingConfigurationElement DecoratedBindingElement
		{
			get
			{
				// StandardBindingElement properties
				_decorated.CloseTimeout = CloseTimeout;
				_decorated.OpenTimeout = OpenTimeout;
				_decorated.ReceiveTimeout = ReceiveTimeout;
				_decorated.SendTimeout = SendTimeout;
				return _decorated;
			}
		}

		#endregion

		#region IEnumerable<BindingElementExtensionElement> Members

		public IEnumerator<BindingElementExtensionElement> GetEnumerator()
		{
			return _decorated.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Base Class Member Overrides

		protected override Type BindingElementType
		{
			get
			{
				throw new NotSupportedException(
					string.Format(
						"Only the actual {0} binding element supports this operation but not its decorator.",
						_decorated.GetType().FullName));
			}
		}

		protected override void OnApplyConfiguration(System.ServiceModel.Channels.Binding binding)
		{
			throw new NotSupportedException(
				string.Format(
					"Only the actual {0} binding element supports this operation but not its decorator.",
					_decorated.GetType().FullName));
		}

		#endregion

		public void Add(BindingElementExtensionElement element)
		{
			Add(new[] { element });
		}

		public void Add(params BindingElementExtensionElement[] elements)
		{
			elements.Each(e => _decorated.Add(e));
		}

		private readonly System.ServiceModel.Configuration.CustomBindingElement _decorated;
	}
}

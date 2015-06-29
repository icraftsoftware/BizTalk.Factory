#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using Microsoft.BizTalk.Component.Interop;
using Moq;

namespace Be.Stateless.BizTalk.Unit.Component
{
	/// <summary>
	/// Utility class that allows <see cref="Mock{T}"/> expectations to be setup and verified.
	/// </summary>
	/// <remarks>
	/// <see cref="IPropertyBag"/> members delegates to virtual class members with signatures one can mock, that is
	/// without <c>ref</c> or <c>out</c> argument.
	/// </remarks>
	internal class PropertyBag : IPropertyBag
	{
		#region IPropertyBag Members

		void IPropertyBag.Read(string propName, out object ptrVar, int errorLog)
		{
			ptrVar = Read(propName);
		}

		void IPropertyBag.Write(string propName, ref object ptrVar)
		{
			Write(propName, ptrVar);
		}

		#endregion

		public bool Contains(string name)
		{
			return _bag.ContainsKey(name);
		}

		internal void Add(string key, object value)
		{
			_bag.Add(key, value);
		}

		internal virtual object Read(string name)
		{
			// will throw KeyNotFoundException if !Contains(name)
			return _bag[name];
		}

		internal virtual void Write(string name, object value)
		{
			_bag[name] = value;
		}

		private readonly IDictionary<string, object> _bag = new Dictionary<string, object>();
	}
}

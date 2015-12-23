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

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Reflection;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	public abstract class ProcessNames<T> where T : class, new()
	{
		static ProcessNames()
		{
			const BindingFlags bindingFlags = BindingFlags.DeclaredOnly
				| BindingFlags.NonPublic | BindingFlags.Public
				| BindingFlags.Instance | BindingFlags.Static;
			var members = typeof(T)
				.GetMembers(bindingFlags)
				// filter out default ctor
				.Where(m => !(m.MemberType == MemberTypes.Constructor && ((ConstructorInfo) m).GetParameters().Length == 0))
				// filter out compiler generated code
				.Where(m => !Attribute.IsDefined(m, typeof(CompilerGeneratedAttribute)))
				.ToArray();

			var message = string.Format("{0} must only declare non-static public string properties.", typeof(T).FullName);
			if (members.Any(m => m.MemberType != MemberTypes.Property)) throw new ArgumentException(message);

			var properties = members.Cast<PropertyInfo>().ToArray();
			// unsupported properties are either non-string or non-public or static
			var unsupportedProperties = properties.Where(
				p => p.PropertyType != typeof(string)
					|| p.GetGetMethod() == null
					|| p.GetGetMethod().IsStatic);
			if (unsupportedProperties.Any()) throw new ArgumentException(message);

			_processes = new T();
			var prefix = typeof(T).FullName + ".";
			properties.Each(p => Reflector.SetProperty(_processes, p.Name, prefix + p.Name));
		}

		public static T Processes
		{
			get { return _processes; }
		}

		private static readonly T _processes;
	}
}

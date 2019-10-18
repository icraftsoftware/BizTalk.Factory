#region Copyright & License

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

using System.Reflection;

namespace Be.Stateless.BizTalk.Unit.Resources
{
	public static class ResourceManagerFactory
	{
		/// <summary>
		/// Instantiate an <see cref="IResourceManager"/> providing access to the resources embedded in the assembly to which belongs <see cref="T"/>, and scoped by the
		/// namespace of the specified type <see cref="T"/>.
		/// </summary>
		/// <typeparam name="T">
		/// The type whose namespace scope is to be used to access the embedded resources in its containing assembly.
		/// </typeparam>
		/// <returns>
		/// An <see cref="IResourceManager"/> that provides access to the resources scoped by the namespace of the specified type <see cref="T"/> and embedded in the
		/// containing assembly of <see cref="T"/>.
		/// </returns>
		public static IResourceManager Create<T>()
		{
			return new ResourceManagerImpl(typeof(T));
		}

		/// <summary>
		/// Instantiate an <see cref="IResourceManager"/> providing access to the resources embedded in <paramref name="assembly"/>.
		/// </summary>
		/// <param name="assembly">
		/// The assembly whose embedded resources need to be accessed
		/// </param>
		/// <returns>
		/// An <see cref="IResourceManager"/> that provide access to the resources embedded in <paramref name="assembly"/>.
		/// </returns>
		public static IResourceManager Create(Assembly assembly)
		{
			return new ResourceManagerImpl(assembly);
		}
	}
}

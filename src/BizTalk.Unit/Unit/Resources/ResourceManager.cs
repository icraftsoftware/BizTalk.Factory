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

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Xsl;

namespace Be.Stateless.BizTalk.Unit.Resources
{
	// TODO to be deprecated
	public static class ResourceManager
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Stream Load(string name)
		{
			return GetResourceManagerImpl().Load(name);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static T Load<T>(string name, Func<Stream, T> deserializer)
		{
			return GetResourceManagerImpl().Load(name, deserializer);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string LoadString(string name)
		{
			return GetResourceManagerImpl().LoadString(name);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static XslCompiledTransform LoadTransform(string name)
		{
			return GetResourceManagerImpl().LoadTransform(name);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string LoadXmlString(string name)
		{
			return GetResourceManagerImpl().LoadXmlString(name);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static IResourceManager GetResourceManagerImpl()
		{
			var resourceManagerCallerFrame = new StackFrame(2);
			return new ResourceManagerImpl(resourceManagerCallerFrame.GetMethod().DeclaringType);
		}
	}
}

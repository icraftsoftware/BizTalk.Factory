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

using System;
using System.IO;
using System.Reflection;

namespace Be.Stateless.Resources
{
	internal static class ResourceManager
	{
		public static T Load<T>(Assembly assembly, string name, Func<Stream, T> deserializer)
		{
			using (var stream = assembly.GetManifestResourceStream(name))
			{
				if (stream == null) throw new FileNotFoundException(string.Format("Cannot find resource in assembly {0}.", assembly.FullName), name);
				return deserializer(stream);
			}
		}
	}
}
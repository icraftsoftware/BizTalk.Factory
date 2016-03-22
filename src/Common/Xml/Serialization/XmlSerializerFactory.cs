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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Be.Stateless.Xml.Serialization
{
	public static class XmlSerializerFactory
	{
		public static XmlSerializer Create(Type type, XmlRootAttribute root)
		{
			return CachedCreate(type, () => new XmlSerializer(type, root));
		}

		public static XmlSerializer Create(Type type, XmlAttributeOverrides overrides)
		{
			return CachedCreate(type, () => new XmlSerializer(type, overrides));
		}

		[SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
		private static XmlSerializer CachedCreate(Type type, Func<XmlSerializer> factory)
		{
			XmlSerializer serializer;
			if (!_cache.TryGetValue(type, out serializer))
			{
				lock (_cache)
				{
					if (!_cache.TryGetValue(type, out serializer))
					{
						serializer = factory();
						_cache.Add(type, serializer);
					}
				}
			}
			return serializer;
		}

		private static readonly Dictionary<Type, XmlSerializer> _cache = new Dictionary<Type, XmlSerializer>();
	}
}

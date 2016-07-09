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

namespace Be.Stateless.Extensions
{
	public static class TypeExtensions
	{
		public static bool IsSubclassOfOpenGeneric(this Type type, Type baseType)
		{
			// http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
			if (!baseType.IsGenericType || baseType.IsConstructedGenericType) return type.IsSubclassOf(baseType);

			while (type != null && type != typeof(object))
			{
				if (type.IsGenericType) type = type.GetGenericTypeDefinition();
				if (type == baseType) return true;
				type = type.BaseType;
			}
			return false;
		}
	}
}

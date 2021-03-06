﻿#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

namespace Be.Stateless.Extensions
{
	public static class TypeExtensions
	{
		public static bool IsTypeOf<T>(this Type type)
		{
			return type == typeof(T);
		}

		public static bool IsTypeOf(this Type type, Type expectedType)
		{
			return type == expectedType;
		}

		public static bool IsTypeOf<T1, T2>(this Type type)
		{
			return type == typeof(T1) || type == typeof(T2);
		}

		public static bool IsTypeOf(this Type type, Type expectedType1, Type expectedType2)
		{
			return type == expectedType1 || type == expectedType2;
		}

		public static bool IsTypeOf<T1, T2, T3>(this Type type)
		{
			return type == typeof(T1) || type == typeof(T2) || type == typeof(T3);
		}

		public static bool IsTypeOf(this Type type, Type expectedType1, Type expectedType2, Type expectedType3)
		{
			return type == expectedType1 || type == expectedType2 || type == expectedType3;
		}

		public static bool IsSubclassOfOpenGenericType(this Type type, Type baseType)
		{
			// http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
			if (!baseType.IsGenericType || baseType.IsConstructedGenericType) return type.IsSubclassOf(baseType);
			return !type.IsInterface && baseType.IsInterface ? type.IsSubclassOfOpenGenericInterface(baseType) : type.IsSubclassOfOpenGenericClass(baseType);
		}

		private static bool IsSubclassOfOpenGenericInterface(this Type type, Type baseType)
		{
			var interfaces = type.GetInterfaces();
			return interfaces.Any(i => i.IsSubclassOfOpenGenericClass(baseType));
		}

		private static bool IsSubclassOfOpenGenericClass(this Type type, Type baseType)
		{
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

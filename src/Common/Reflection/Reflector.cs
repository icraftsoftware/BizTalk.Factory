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
using System.Reflection;

namespace Be.Stateless.Reflection
{
	public static class Reflector
	{
		#region Get Field

		public static object GetField<T>(string fieldName)
		{
			const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			return GetField(typeof(T), null, fieldName, flags);
		}

		public static object GetField<T>(T instance, string fieldName)
		{
			if (Equals(instance, default(T))) throw new ArgumentNullException("instance");
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			return GetField(typeof(T), instance, fieldName, flags);
		}

		public static object GetField(Type type, string fieldName)
		{
			if (type == null) throw new ArgumentNullException("type");
			const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			return GetField(type, null, fieldName, flags);
		}

		private static object GetField(Type type, object instance, string fieldName, BindingFlags flags)
		{
			var field = type.GetField(fieldName, flags);
			if (field == null && instance != null) field = instance.GetType().GetField(fieldName, flags);
			if (field == null) throw new ArgumentException(string.Format("There is no field '{0}' for type '{1}'.", fieldName, type));
			return field.GetValue(instance ?? type);
		}

		#endregion

		#region Set Field

		public static void SetField<T>(string fieldName, object value)
		{
			const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			SetField(typeof(T), null, fieldName, value, flags);
		}

		public static void SetField<T>(T instance, string fieldName, object value)
		{
			if (Equals(instance, default(T))) throw new ArgumentNullException("instance");
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			SetField(typeof(T), instance, fieldName, value, flags);
		}

		private static void SetField(Type type, object instance, string fieldName, object value, BindingFlags flags)
		{
			var field = type.GetField(fieldName, flags);
			if (field == null && instance != null) field = instance.GetType().GetField(fieldName, flags);
			if (field == null) throw new ArgumentException(string.Format("There is no field '{0}' for type '{1}'.", fieldName, type));
			field.SetValue(instance ?? type, value);
		}

		#endregion

		#region Get Property

		public static object GetProperty<T>(string propertyName)
		{
			const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			return GetProperty(typeof(T), null, propertyName, flags);
		}

		public static object GetProperty<T>(T instance, string propertyName)
		{
			if (Equals(instance, default(T))) throw new ArgumentNullException("instance");
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			return GetProperty(typeof(T), instance, propertyName, flags);
		}

		private static object GetProperty(Type type, object instance, string propertyName, BindingFlags flags)
		{
			var property = type.GetProperty(propertyName, flags);
			if (property == null && instance != null) property = instance.GetType().GetProperty(propertyName, flags);
			if (property == null) throw new ArgumentException(string.Format("There is no property '{0}' for type '{1}'.", propertyName, type));
			return property.GetValue(instance ?? type, null);
		}

		#endregion

		#region Set Property

		public static void SetProperty<T>(string propertyName, object value)
		{
			const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			SetProperty(typeof(T), null, propertyName, value, flags);
		}

		public static void SetProperty<T>(T instance, string propertyName, object value)
		{
			if (Equals(instance, default(T))) throw new ArgumentNullException("instance");
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			SetProperty(typeof(T), instance, propertyName, value, flags);
		}

		private static void SetProperty(Type type, object instance, string propertyName, object value, BindingFlags flags)
		{
			var property = type.GetProperty(propertyName, flags);
			if (property == null && instance != null) property = instance.GetType().GetProperty(propertyName, flags);
			if (property == null) throw new ArgumentException(string.Format("There is no property '{0}' for type '{1}'.", propertyName, type));
			property.SetValue(instance ?? type, value, null);
		}

		#endregion

		#region Invoke Method

		public static object InvokeMethod<T>(string methodName, params object[] @params)
		{
			const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			return InvokeMethod(typeof(T), (object) null, methodName, @params, flags);
		}

		public static object InvokeMethod(Type type, string methodName, params object[] @params)
		{
			const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			return InvokeMethod(type, (object) null, methodName, @params, flags);
		}

		public static object InvokeMethod<T>(T instance, string methodName, params object[] @params)
		{
			if (Equals(instance, default(T))) throw new ArgumentNullException("instance");
			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			return InvokeMethod(typeof(T), instance, methodName, @params, flags);
		}

		private static object InvokeMethod(IReflect type, object instance, string methodName, object[] @params, BindingFlags flags)
		{
			try
			{
				var method = type.GetMethod(methodName, flags);
				if (method == null && instance != null) method = instance.GetType().GetMethod(methodName, flags);
				if (method == null) throw new ArgumentException(string.Format("Type '{0}' does not have a method named '{1}'.", type, methodName));
				return method.Invoke(instance, @params);
			}
			catch (AmbiguousMatchException)
			{
				return type.InvokeMember(methodName, flags | BindingFlags.InvokeMethod, null, instance, @params, null, null, null);
			}
		}

		#endregion
	}
}

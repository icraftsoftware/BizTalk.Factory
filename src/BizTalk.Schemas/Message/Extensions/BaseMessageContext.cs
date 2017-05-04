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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	public static class BaseMessageContext
	{
		#region message's context property deletion

		public static void DeleteProperty<T>(this IBaseMessageContext context, MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			context.Write(property.Name, property.Namespace, null);
		}

		public static void DeleteProperty<T, TR>(this IBaseMessageContext context, MessageContextProperty<T, TR> property)
			where T : MessageContextPropertyBase, new()
			where TR : struct
		{
			context.Write(property.Name, property.Namespace, null);
		}

		#endregion

		#region message's context property promotion

		public static bool IsPromoted<T>(this IBaseMessageContext context, MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			return !context.GetProperty(property).IsNullOrEmpty() && context.IsPromoted(property.Name, property.Namespace);
		}

		public static bool IsPromoted<T, TV>(this IBaseMessageContext context, MessageContextProperty<T, TV> property)
			where T : MessageContextPropertyBase, new()
			where TV : struct
		{
			return context.GetProperty(property).HasValue && context.IsPromoted(property.Name, property.Namespace);
		}

		public static void Promote<T>(this IBaseMessageContext context, MessageContextProperty<T, string> property, string value)
			where T : MessageContextPropertyBase, new()
		{
			if (value != null) context.Promote(property.Name, property.Namespace, value);
		}

		public static void Promote<T, TV>(this IBaseMessageContext context, MessageContextProperty<T, TV> property, TV value)
			where T : MessageContextPropertyBase, new()
			where TV : struct
		{
			context.Promote(property.Name, property.Namespace, value);
		}

		#endregion

		#region message's context property reading

		public static string GetProperty<T>(this IBaseMessageContext context, MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			var value = context.Read(property.Name, property.Namespace);
			return (string) value;
		}

		public static TR? GetProperty<T, TR>(this IBaseMessageContext context, MessageContextProperty<T, TR> property)
			where T : MessageContextPropertyBase, new()
			where TR : struct
		{
			var value = context.Read(property.Name, property.Namespace);
			return (TR?) value;
		}

		#endregion

		#region message's context property writing

		public static void SetProperty<T>(this IBaseMessageContext context, MessageContextProperty<T, string> property, string value)
			where T : MessageContextPropertyBase, new()
		{
			if (value != null) context.Write(property.Name, property.Namespace, value);
		}

		public static void SetProperty<T, TV>(this IBaseMessageContext context, MessageContextProperty<T, TV> property, TV value)
			where T : MessageContextPropertyBase, new()
			where TV : struct
		{
			context.Write(property.Name, property.Namespace, value);
		}

		#endregion
	}
}

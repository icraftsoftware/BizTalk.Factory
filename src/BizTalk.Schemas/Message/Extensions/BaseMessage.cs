#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
	/// <summary>
	/// Various <see cref="IBaseMessage"/> extension methods that allow for shorter and <b>type-safe</b> statements.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Noticeably, offers <b>type-safe</b> and easier to use <see cref="IBaseMessageContext"/> property getters,
	/// setters, and promoters.
	/// </para>
	/// </remarks>
	public static class BaseMessage
	{
		#region message's context property deletion

		public static void DeleteProperty<T>(this IBaseMessage message, MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			message.Context.Write(property.Name, property.Namespace, null);
		}

		public static void DeleteProperty<T, TR>(this IBaseMessage message, MessageContextProperty<T, TR> property)
			where T : MessageContextPropertyBase, new()
			where TR : struct
		{
			message.Context.Write(property.Name, property.Namespace, null);
		}

		public static void DeleteProperty<T>(this XLANGMessage message, MessageContextProperty<T, object> property)
			where T : MessageContextPropertyBase, new()
		{
			message.SetPropertyValue(property.Type, null);
		}

		#endregion

		#region message's context property promotion

		public static bool IsPromoted<T>(this IBaseMessage message, MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			return !message.GetProperty(property).IsNullOrEmpty() && message.Context.IsPromoted(property.Name, property.Namespace);
		}

		public static bool IsPromoted<T, TV>(this IBaseMessage message, MessageContextProperty<T, TV> property)
			where T : MessageContextPropertyBase, new()
			where TV : struct
		{
			return message.GetProperty(property).HasValue && message.Context.IsPromoted(property.Name, property.Namespace);
		}

		public static void Promote<T>(this IBaseMessage message, MessageContextProperty<T, string> property, string value)
			where T : MessageContextPropertyBase, new()
		{
			if (value != null) message.Context.Promote(property.Name, property.Namespace, value);
		}

		public static void Promote<T, TV>(this IBaseMessage message, MessageContextProperty<T, TV> property, TV value)
			where T : MessageContextPropertyBase, new()
			where TV : struct
		{
			message.Context.Promote(property.Name, property.Namespace, value);
		}

		#endregion

		#region message's context property reading

		public static string GetProperty<T>(this IBaseMessage message, MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			var value = message.Context.Read(property.Name, property.Namespace);
			return (string) value;
		}

		public static TR? GetProperty<T, TR>(this IBaseMessage message, MessageContextProperty<T, TR> property)
			where T : MessageContextPropertyBase, new()
			where TR : struct
		{
			var value = message.Context.Read(property.Name, property.Namespace);
			return (TR?) value;
		}

		public static string GetProperty<T>(this XLANGMessage message, MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			var value = message.GetPropertyValue(property.Type);
			return (string) value;
		}

		public static TR? GetProperty<T, TR>(this XLANGMessage message, MessageContextProperty<T, TR> property)
			where T : MessageContextPropertyBase, new()
			where TR : struct
		{
			var value = message.GetPropertyValue(property.Type);
			return (TR?) value;
		}

		#endregion

		#region message's context property writing

		public static void SetProperty<T>(this IBaseMessage message, MessageContextProperty<T, string> property, string value)
			where T : MessageContextPropertyBase, new()
		{
			if (value != null) message.Context.Write(property.Name, property.Namespace, value);
		}

		public static void SetProperty<T, TV>(this IBaseMessage message, MessageContextProperty<T, TV> property, TV value)
			where T : MessageContextPropertyBase, new()
			where TV : struct
		{
			message.Context.Write(property.Name, property.Namespace, value);
		}

		public static void SetProperty<T>(this XLANGMessage message, MessageContextProperty<T, string> property, string value)
			where T : MessageContextPropertyBase, new()
		{
			if (value != null) message.SetPropertyValue(property.Type, value);
		}

		public static void SetProperty<T, TV>(this XLANGMessage message, MessageContextProperty<T, TV> property, TV value)
			where T : MessageContextPropertyBase, new()
			where TV : struct
		{
			message.SetPropertyValue(property.Type, value);
		}

		#endregion
	}
}

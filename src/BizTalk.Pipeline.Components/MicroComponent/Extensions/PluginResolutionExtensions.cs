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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.MicroComponent.Extensions
{
	internal static class PluginResolutionExtensions
	{
		public static Type ResolvePluginType<T>(this IBaseMessage message, MessageContextProperty<T, string> messageContextProperty, Type configuredPluginType)
			where T : MessageContextPropertyBase, new()
		{
			// look after plugin type name in message context
			var pluginTypeName = message.GetProperty(messageContextProperty);
			if (!pluginTypeName.IsNullOrEmpty())
			{
				// remove plugin type name from context to ensure no one else will use it too
				message.DeleteProperty(messageContextProperty);
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Using message context's plugin type '{0}'.", pluginTypeName);
				return Type.GetType(pluginTypeName, true);
			}

			// use plugin type configured at pipeline level if no one found in context
			if (configuredPluginType != null)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Using configured plugin type '{0}'.", configuredPluginType.AssemblyQualifiedName);
				return configuredPluginType;
			}

			return null;
		}

		public static T AsPlugin<T>(this Type type)
		{
			if (type == null) return default(T);

			if (!typeof(T).IsAssignableFrom(type))
				throw new InvalidOperationException(
					string.Format(
						"The plugin type '{0}' does not support the type '{1}'.",
						type.AssemblyQualifiedName,
						typeof(T).AssemblyQualifiedName));
			return (T) Activator.CreateInstance(type);
		}

		public static Type OfPluginType<T>(this Type type)
		{
			if (type != null && !typeof(T).IsAssignableFrom(type))
				throw new InvalidOperationException(
					string.Format(
						"The plugin type '{0}' does not support the type '{1}'.",
						type.AssemblyQualifiedName,
						typeof(T).AssemblyQualifiedName));
			return type;
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(PluginResolutionExtensions));
	}
}

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

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TPlugin">The runtime type that the plugin instance is expected to support.</typeparam>
	public abstract class ExtensiblePipelineComponent<TPlugin> : PipelineComponent
		where TPlugin : class
	{
		protected TPlugin ResolvePlugin<T>(IBaseMessage message, MessageContextProperty<T, string> pluginTypeNameContextProperty, Type configuredPluginType)
			where T : MessageContextPropertyBase, new()
		{
			var pluginType = ResolvePluginType(message, pluginTypeNameContextProperty, configuredPluginType);
			return pluginType != null ? (TPlugin) Activator.CreateInstance(pluginType) : default(TPlugin);
		}

		protected Type ResolvePluginType<T>(IBaseMessage message, MessageContextProperty<T, string> pluginTypeNameContextProperty, Type configuredPluginType)
			where T : MessageContextPropertyBase, new()
		{
			Type pluginType = null;
			// look after plugin type name in message context
			var pluginTypeName = message.GetProperty(pluginTypeNameContextProperty);
			if (!pluginTypeName.IsNullOrEmpty())
			{
				// remove plugin type name from context to ensure no one else will use it too
				message.DeleteProperty(pluginTypeNameContextProperty);
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Using message context's plugin type '{0}'.", pluginTypeName);
				pluginType = Type.GetType(pluginTypeName, true);
			}

			if (pluginType == null && configuredPluginType != null)
			{
				// use default plugin type if no one found in context
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Using configured plugin type '{0}'.", configuredPluginType.AssemblyQualifiedName);
				pluginType = configuredPluginType;
			}

			if (pluginType != null && !typeof(TPlugin).IsAssignableFrom(pluginType))
				throw new InvalidOperationException(
					string.Format(
						"The plugin type '{0}' does not support the type '{1}'.",
						pluginType.AssemblyQualifiedName,
						typeof(TPlugin).AssemblyQualifiedName));
			return pluginType;
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ExtensiblePipelineComponent<TPlugin>));
	}
}

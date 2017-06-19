#region Copyright & License

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

using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.Extensions;
using Be.Stateless.Quartz.Server;
using Be.Stateless.Quartz.Server.Core;

namespace Be.Stateless.Quartz.Configuration
{
	/// <summary>
	/// Configuration for the Quartz server.
	/// </summary>
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Used to declare section in config files.")]
	public class QuartzConfigurationSection : ConfigurationSection
	{
		public static NameValueCollection Current
		{
			get { return (NameValueCollection) ConfigurationManager.GetSection(DEFAULT_SECTION_NAME); }
		}

		/// <summary>
		/// Gets the type name of the <see cref="IQuartzServer"/>-derived Quartz Server implementation.
		/// </summary>
		/// <value>
		/// The type of the server implementation.
		/// </value>
		public static string ServerImplementationType
		{
			get { return GetConfiguredOrDefaultValue(SERVER_IMPLEMENTATION_TYPE_KEY, _defaultServerImplementationType); }
		}

		/// <summary>
		/// Returns configuration value with given key. If configuration for the does not exists, return the default
		/// value.
		/// </summary>
		/// <param name="configurationKey">
		/// Key to read configuration with.
		/// </param>
		/// <param name="defaultValue">
		/// Default value to return if configuration is not found.
		/// </param>
		/// <returns>
		/// The configuration value.
		/// </returns>
		private static string GetConfiguredOrDefaultValue(string configurationKey, string defaultValue)
		{
			var value = Current.IfNotNull(values => values[configurationKey]);
			return value.IsNullOrWhiteSpace() ? defaultValue : value;
		}

		private const string DEFAULT_SECTION_NAME = "quartz";
		private const string SERVER_CONFIGURATION_PREFIX = "quartz.server";
		private const string SERVER_IMPLEMENTATION_TYPE_KEY = SERVER_CONFIGURATION_PREFIX + ".type";
		private static readonly string _defaultServerImplementationType = typeof(QuartzServer).AssemblyQualifiedName;
	}
}

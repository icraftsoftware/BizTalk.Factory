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

using System.Collections.Specialized;
using System.Configuration;

namespace Quartz.Server
{
	/// <summary>
	/// Configuration for the Quartz server.
	/// </summary>
	public static class Configuration
	{
		/// <summary>
		/// Initializes the <see cref="Configuration"/> class.
		/// </summary>
		static Configuration()
		{
			_configuration = (NameValueCollection) ConfigurationManager.GetSection("quartz");
		}

		/// <summary>
		/// Gets the type name of the server implementation.
		/// </summary>
		/// <value>The type of the server implementation.</value>
		public static string ServerImplementationType
		{
			get { return GetConfigurationOrDefault(KEY_SERVER_IMPLEMENTATION_TYPE, _defaultServerImplementationType); }
		}

		/// <summary>
		/// Returns configuration value with given key. If configuration
		/// for the does not exists, return the default value.
		/// </summary>
		/// <param name="configurationKey">Key to read configuration with.</param>
		/// <param name="defaultValue">Default value to return if configuration is not found</param>
		/// <returns>The configuration value.</returns>
		private static string GetConfigurationOrDefault(string configurationKey, string defaultValue)
		{
			string retValue = null;
			if (_configuration != null)
			{
				retValue = _configuration[configurationKey];
			}

			if (retValue == null || retValue.Trim().Length == 0)
			{
				retValue = defaultValue;
			}
			return retValue;
		}

		private const string KEY_SERVER_IMPLEMENTATION_TYPE = PREFIX_SERVER_CONFIGURATION + ".type";
		private const string PREFIX_SERVER_CONFIGURATION = "quartz.server";

		private static readonly NameValueCollection _configuration;
		private static readonly string _defaultServerImplementationType = typeof(QuartzServer).AssemblyQualifiedName;
	}
}

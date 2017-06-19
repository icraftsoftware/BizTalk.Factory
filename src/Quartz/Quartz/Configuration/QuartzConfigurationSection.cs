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
using Be.Stateless.Quartz.Host;
using Be.Stateless.Quartz.Host.Core;

namespace Be.Stateless.Quartz.Configuration
{
	/// <summary>
	/// Configuration for the Quartz Scheduler and its host.
	/// </summary>
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Used to declare section in config files.")]
	public class QuartzConfigurationSection : ConfigurationSection
	{
		/// <summary>
		/// Assumes the quartz configuration section is a <see cref="AppSettingsSection"/>.
		/// </summary>
		/// <returns></returns>
		/// <seealso href="How to get the values of a ConfigurationSection of type NameValueSectionHandler">https://stackoverflow.com/questions/3461418/how-to-get-the-values-of-a-configurationsection-of-type-namevaluesectionhandler</seealso>
		/// <seealso href=".net config file AppSettings: NameValueCollection vs. KeyValueConfigurationCollection">https://stackoverflow.com/questions/4782384/net-config-file-appsettings-namevaluecollection-vs-keyvalueconfigurationcolle</seealso>
		public static QuartzConfigurationSection Current
		{
			get { return new QuartzConfigurationSection((NameValueCollection) ConfigurationManager.GetSection(DEFAULT_SECTION_NAME)); }
		}

		private QuartzConfigurationSection(NameValueCollection settings)
		{
			_settings = settings;
		}

		/// <summary>
		/// Gets the type name of the <see cref="IQuartzSchedulerHost"/>-derived Quartz Scheduler Host implementation.
		/// </summary>
		/// <remarks>
		/// If no <c>quartz.host.type</c> property is defined then the <see cref="QuartzSchedulerHost"/> will be used as
		/// the default host.
		/// </remarks>
		/// <value>
		/// The type of the scheduler implementation.
		/// </value>
		public string SchedulerHostType
		{
			get { return GetConfiguredOrDefaultValue(SERVER_IMPLEMENTATION_TYPE_KEY, _defaultSchedulerHostType); }
		}

		public NameValueCollection Settings
		{
			get { return _settings; }
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
		private string GetConfiguredOrDefaultValue(string configurationKey, string defaultValue)
		{
			var value = Settings.IfNotNull(values => values[configurationKey]);
			return value.IsNullOrWhiteSpace() ? defaultValue : value;
		}

		private const string DEFAULT_SECTION_NAME = "quartz";
		private const string SERVER_CONFIGURATION_PREFIX = "quartz.host";
		private const string SERVER_IMPLEMENTATION_TYPE_KEY = SERVER_CONFIGURATION_PREFIX + ".type";
		private static readonly string _defaultSchedulerHostType = typeof(QuartzSchedulerHost).AssemblyQualifiedName;
		private readonly NameValueCollection _settings;
	}
}

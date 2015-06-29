#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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

using System.Configuration;
using System.Threading;
using System.Web.Configuration;

namespace Be.Stateless.BizTalk.Web.Monitoring.Configuration
{
	public class MonitoringConfigurationSection : ConfigurationSection
	{
		#region Factory Helpers

		public static MonitoringConfigurationSection Current
		{
			get
			{
				var section = (MonitoringConfigurationSection) WebConfigurationManager.GetWebApplicationSection(DEFAULT_SECTION_NAME) ?? Default;
				return section;
			}
		}

		internal static MonitoringConfigurationSection Default
		{
			get
			{
				if (_defaultSection == null)
				{
					Interlocked.CompareExchange(ref _defaultSection, new MonitoringConfigurationSection(), null);
				}
				return _defaultSection;
			}
		}

		#endregion

		static MonitoringConfigurationSection()
		{
			_properties.Add(_bizTalkAdministratorAccountProperty);
			_properties.Add(_largeMessageTrackingDirectory);
		}

		[ConfigurationProperty(BIZTALK_ADMINISTRATOR_ACCOUNT_ELEMENT_PROPERTY_NAME, DefaultValue = null)]
		public AccountConfigurationElement BizTalkAdministratorAccount
		{
			get { return (AccountConfigurationElement) base[_bizTalkAdministratorAccountProperty]; }
		}

		[ConfigurationProperty(LARGE_MESSAGE_TRACKING_DIRECTORY)]
		[StringValidator(MinLength = 1)]
		public string LargeMessageTrackingDirectory
		{
			get { return (string) base[_largeMessageTrackingDirectory]; }
		}

		/// <summary>
		/// Gets the collection of properties.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Configuration.ConfigurationPropertyCollection"/> collection of properties for the element.
		/// </returns>
		protected override ConfigurationPropertyCollection Properties
		{
			get { return _properties; }
		}

		private const string DEFAULT_SECTION_NAME = "be.stateless/biztalk/monitoring";
		private const string BIZTALK_ADMINISTRATOR_ACCOUNT_ELEMENT_PROPERTY_NAME = "bizTalkAdministratorAccount";
		private const string LARGE_MESSAGE_TRACKING_DIRECTORY = "largeMessageTrackingDirectory";

		private static MonitoringConfigurationSection _defaultSection;
		private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _bizTalkAdministratorAccountProperty = new ConfigurationProperty(
			BIZTALK_ADMINISTRATOR_ACCOUNT_ELEMENT_PROPERTY_NAME,
			typeof(AccountConfigurationElement),
			null,
			ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _largeMessageTrackingDirectory = new ConfigurationProperty(
			LARGE_MESSAGE_TRACKING_DIRECTORY,
			typeof(string),
			null,
			null,
			ValidatorsAndConverters.NonEmptyStringValidator,
			ConfigurationPropertyOptions.None);
	}
}
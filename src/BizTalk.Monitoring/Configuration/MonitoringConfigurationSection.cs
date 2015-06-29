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

using System.Configuration;
using System.Threading;
using System.Web.Configuration;

namespace Be.Stateless.BizTalk.Monitoring.Configuration
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
			_properties.Add(_claimStoreDirectory);
		}

		#region Base Class Member Overrides

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

		#endregion

		[ConfigurationProperty(CLAIM_STORE_DIRECTORY_PROPERTY_NAME)]
		[StringValidator(MinLength = 1)]
		public string ClaimStoreDirectory
		{
			get { return (string) base[_claimStoreDirectory]; }
		}

		private const string DEFAULT_SECTION_NAME = "be.stateless/biztalk/monitoring";
		private const string CLAIM_STORE_DIRECTORY_PROPERTY_NAME = "claimStoreDirectory";

		private static MonitoringConfigurationSection _defaultSection;
		private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _claimStoreDirectory = new ConfigurationProperty(
			CLAIM_STORE_DIRECTORY_PROPERTY_NAME,
			typeof(string),
			null,
			null,
			ValidatorsAndConverters.NonEmptyStringValidator,
			ConfigurationPropertyOptions.None);
	}
}

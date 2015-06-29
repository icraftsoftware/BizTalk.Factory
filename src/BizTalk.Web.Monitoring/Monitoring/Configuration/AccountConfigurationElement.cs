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

namespace Be.Stateless.BizTalk.Web.Monitoring.Configuration
{
	public class AccountConfigurationElement : ConfigurationElement
	{
		static AccountConfigurationElement()
		{
			_properties.Add(_userNameProperty);
			_properties.Add(_passwordProperty);
		}

		[ConfigurationProperty(USER_NAME_PROPERTY_NAME, IsRequired = true)]
		[StringValidator(MinLength = 1)]
		public string UserName
		{
			get { return (string) base[_userNameProperty]; }
		}

		[ConfigurationProperty(PASSWORD_PROPERTY_NAME, IsRequired = true)]
		[StringValidator(MinLength = 1)]
		public string Password
		{
			get { return (string) base[_passwordProperty]; }
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

		private const string USER_NAME_PROPERTY_NAME = "userName";
		private const string PASSWORD_PROPERTY_NAME = "password";

		private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _userNameProperty = new ConfigurationProperty(
			USER_NAME_PROPERTY_NAME,
			typeof(string),
			null,
			null,
			ValidatorsAndConverters.NonEmptyStringValidator,
			ConfigurationPropertyOptions.IsRequired);

		private static readonly ConfigurationProperty _passwordProperty = new ConfigurationProperty(
			PASSWORD_PROPERTY_NAME,
			typeof(string),
			null,
			null,
			ValidatorsAndConverters.NonEmptyStringValidator,
			ConfigurationPropertyOptions.IsRequired);
	}
}
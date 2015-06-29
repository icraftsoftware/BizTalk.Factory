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
using Be.Stateless.BizTalk.ClaimStore.Configuration.Validators;

namespace Be.Stateless.BizTalk.ClaimStore.Configuration
{
	public sealed class DirectoryConfigurationElement : ConfigurationElement
	{
		static DirectoryConfigurationElement()
		{
			_properties.Add(_pathProperty);
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Gets the collection of properties.
		/// </summary>
		/// <returns>
		/// The <see cref="ConfigurationPropertyCollection"/> collection of properties for the element.
		/// </returns>
		protected override ConfigurationPropertyCollection Properties
		{
			get { return _properties; }
		}

		#endregion

		[ConfigurationProperty(PATH_PROPERTY_NAME, IsKey = true, IsRequired = true)]
		[DirectoryValidator]
		public string Path
		{
			get { return (string) base[_pathProperty]; }
		}

		private const string PATH_PROPERTY_NAME = "path";

		private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _pathProperty = new ConfigurationProperty(
			PATH_PROPERTY_NAME,
			typeof(string),
			null,
			null,
			new DirectoryValidator(),
			ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
	}
}

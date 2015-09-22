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

using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Be.Stateless.BizTalk.ClaimStore.Configuration
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Used to declare section in config files.")]
	public class ClaimStoreConfigurationSection : ConfigurationSection
	{
		static ClaimStoreConfigurationSection()
		{
			_properties.Add(_agentProperty);
		}

		public static ClaimStoreConfigurationSection Current
		{
			get
			{
				var section = (ClaimStoreConfigurationSection) ConfigurationManager.GetSection(DEFAULT_SECTION_NAME);
				return section;
			}
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

		[ConfigurationProperty(AGENT_PROPERTY_NAME, IsRequired = true)]
		public AgentConfigurationElement Agent
		{
			get { return (AgentConfigurationElement) base[_agentProperty]; }
		}

		private const string AGENT_PROPERTY_NAME = "agent";

		private const string DEFAULT_SECTION_NAME = "be.stateless/biztalk/claimStore";

		private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _agentProperty = new ConfigurationProperty(
			AGENT_PROPERTY_NAME,
			typeof(AgentConfigurationElement),
			null,
			ConfigurationPropertyOptions.IsRequired);
	}
}

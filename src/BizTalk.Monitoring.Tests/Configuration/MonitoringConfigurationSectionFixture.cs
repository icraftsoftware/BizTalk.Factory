#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Monitoring.Configuration
{
	[TestFixture]
	public class MonitoringConfigurationSectionFixture
	{
		[Test]
		public void ConfigurationSectionIsDeclaredButEmpty()
		{
			var monitoringConfiguration = (MonitoringConfigurationSection) ConfigurationManager.GetSection("be.stateless.tests/biztalk/monitoring");
			Assert.That(monitoringConfiguration, Is.Not.Null);
			Assert.That(monitoringConfiguration, Is.TypeOf<MonitoringConfigurationSection>());
			Assert.That(monitoringConfiguration, Is.Not.SameAs(MonitoringConfigurationSection.Default));
			// ReSharper disable once PossibleNullReferenceException
			Assert.That(monitoringConfiguration.ClaimStoreDirectory, Is.Null);
		}

		[Test]
		public void ConfigurationSectionIsDeclaredWithDefaultElementNames()
		{
			var monitoringConfiguration = MonitoringConfigurationSection.Current;
			Assert.That(monitoringConfiguration, Is.Not.Null);
			Assert.That(monitoringConfiguration, Is.TypeOf<MonitoringConfigurationSection>());
			Assert.That(monitoringConfiguration, Is.Not.SameAs(MonitoringConfigurationSection.Default));
			Assert.That(monitoringConfiguration.ClaimStoreDirectory, Is.Not.Null);
		}

		[Test]
		public void ConfigurationSectionIsNotDeclared()
		{
			var monitoringConfiguration = (MonitoringConfigurationSection) ConfigurationManager.GetSection("be.stateless.tests/biztalk/undeclaredMonitoringSection");
			Assert.That(monitoringConfiguration, Is.Null);
		}
	}
}

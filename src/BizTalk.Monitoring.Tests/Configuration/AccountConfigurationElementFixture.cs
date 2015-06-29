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
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Be.Stateless.BizTalk.Web.Monitoring.Configuration
{
	[TestFixture]
	public class AccountConfigurationElementFixture
	{
		[Test]
		public void ConfiguredSettings()
		{
			var monitoringConfiguration = MonitoringConfigurationSection.Current;
			Assert.That(monitoringConfiguration.BizTalkAdministratorAccount, Is.Not.Null);
			Assert.That(monitoringConfiguration.BizTalkAdministratorAccount.UserName, Is.EqualTo(@"machine\username"));
			Assert.That(monitoringConfiguration.BizTalkAdministratorAccount.Password, Is.EqualTo(@"password"));
		}

		[Test]
		public void IncompleteSettings()
		{
			ActualValueDelegate monitoringConfiguration = () => ((MonitoringConfigurationSection) ConfigurationManager.GetSection(
				"be.stateless.tests/biztalk/incompleteMonitoringSection")).BizTalkAdministratorAccount;
			Assert.That(
				monitoringConfiguration,
				Throws.TypeOf<ConfigurationErrorsException>().With.Message.StartsWith("Required attribute 'password' not found."));
		}

		[Test]
		public void InvalidSettings()
		{
			ActualValueDelegate monitoringConfiguration = () => ((MonitoringConfigurationSection) ConfigurationManager.GetSection(
				"be.stateless.tests/biztalk/invalidMonitoringSection")).BizTalkAdministratorAccount;
			Assert.That(
				monitoringConfiguration,
				Throws.TypeOf<ConfigurationErrorsException>().With.Message.StartsWith(
					"The value for the property 'userName' is not valid. The error is: The string must be at least 1 characters long."));
		}

		[Test]
		public void NoSettings()
		{
			var monitoringConfiguration = ((MonitoringConfigurationSection) ConfigurationManager.GetSection(
				"be.stateless.tests/biztalk/monitoring"));
			Assert.That(monitoringConfiguration.BizTalkAdministratorAccount, Is.Not.Null);
			Assert.That(monitoringConfiguration.BizTalkAdministratorAccount.UserName, Is.Null);
			Assert.That(monitoringConfiguration.BizTalkAdministratorAccount.Password, Is.Null);
		}
	}
}
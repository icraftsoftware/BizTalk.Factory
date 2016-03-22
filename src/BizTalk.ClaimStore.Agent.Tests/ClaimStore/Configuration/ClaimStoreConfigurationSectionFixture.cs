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

using System;
using System.Configuration;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.ClaimStore.Configuration
{
	[TestFixture]
	public class ClaimStoreConfigurationSectionFixture
	{
		[Test]
		public void DeclaredSection()
		{
			var claimStoreConfigurationSection = (ClaimStoreConfigurationSection) ConfigurationManager.GetSection("be.stateless.test/biztalk/claimStore");
			Assert.That(claimStoreConfigurationSection, Is.Not.Null);
			// ReSharper disable once PossibleNullReferenceException
			Assert.That(claimStoreConfigurationSection.Agent, Is.Not.Null);
			Assert.That(claimStoreConfigurationSection.Agent.PollingInterval, Is.EqualTo(TimeSpan.Parse("00:03:33")));
			Assert.That(claimStoreConfigurationSection.Agent.CheckInDirectories, Is.Not.Null);
			Assert.That(claimStoreConfigurationSection.Agent.CheckInDirectories, Is.EquivalentTo(new[] { @"c:\windows", @"c:\windows\temp" }));
		}

		[Test]
		public void DefaultFileLockTimeout()
		{
			var claimStoreConfigurationSection = (ClaimStoreConfigurationSection) ConfigurationManager.GetSection("be.stateless.test/biztalk/claimStoreWithoutPollingInterval");
			// ReSharper disable once PossibleNullReferenceException
			Assert.That(claimStoreConfigurationSection.Agent.FileLockTimeout, Is.EqualTo(TimeSpan.Parse(AgentConfigurationElement.FILE_LOCK_TIMEOUT_DEFAULT_VALUE)));
		}

		[Test]
		public void DefaultPollingInterval()
		{
			var claimStoreConfigurationSection = (ClaimStoreConfigurationSection) ConfigurationManager.GetSection("be.stateless.test/biztalk/claimStoreWithoutPollingInterval");
			// ReSharper disable once PossibleNullReferenceException
			Assert.That(claimStoreConfigurationSection.Agent.PollingInterval, Is.EqualTo(TimeSpan.Parse(AgentConfigurationElement.POLLING_INTERVAL_DEFAULT_VALUE)));
		}

		[Test]
		public void InexistentCheckInDirectories()
		{
			Assert.That(
				() => ConfigurationManager.GetSection(
					"be.stateless.test/biztalk/claimStoreWithInexistentCheckInDirectories"),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.Message.StartsWith(@"The value for the property 'path' is not valid. The error is: Could not find a part of the path 'c:\some-inexistent-folder'."));
		}

		[Test]
		public void InexistentCheckOutDirectory()
		{
			Assert.That(
				() => ConfigurationManager.GetSection(
					"be.stateless.test/biztalk/claimStoreWithInexistentCheckOutDirectory"),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.Message.StartsWith(
						@"The value for the property 'checkOutDirectory' is not valid. The error is: Could not find a part of the path 'c:\some-inexistent-folder'."));
		}

		[Test]
		public void InvalidCheckInDirectories()
		{
			Assert.That(
				() => ConfigurationManager.GetSection(
					"be.stateless.test/biztalk/claimStoreWithInvalidCheckInDirectories"),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.Message.StartsWith("Required attribute 'path' not found."));
		}

		[Test]
		public void NegativeFileLockTimeout()
		{
			Assert.That(
				() => ConfigurationManager.GetSection(
					"be.stateless.test/biztalk/claimStoreWithNegativeFileLockTimeout"),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.Message.StartsWith("The value for the property 'fileLockTimeout' is not valid. The error is: The time span value must be positive."));
		}

		[Test]
		public void NegativePollingInterval()
		{
			Assert.That(
				() => ConfigurationManager.GetSection(
					"be.stateless.test/biztalk/claimStoreWithNegativePollingInterval"),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.Message.StartsWith("The value for the property 'pollingInterval' is not valid. The error is: The time span value must be positive."));
		}

		[Test]
		public void NoCheckInDirectories()
		{
			Assert.That(
				() => ConfigurationManager.GetSection(
					"be.stateless.test/biztalk/claimStoreWithoutCheckInDirectories"),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.Message.StartsWith("DirectoryConfigurationElementCollection collection contains no items."));
		}

		[Test]
		public void UnconfiguredSection()
		{
			Assert.That(
				() => ConfigurationManager.GetSection(
					"be.stateless.test/biztalk/unconfiguredClaimStore"),
				Throws.TypeOf<ConfigurationErrorsException>()
					.With.Message.StartsWith("Required attribute 'agent' not found."));
		}

		[Test]
		public void UndeclaredSection()
		{
			var claimStoreConfigurationSection = ClaimStoreConfigurationSection.Current;
			Assert.That(claimStoreConfigurationSection, Is.Null);
		}
	}
}

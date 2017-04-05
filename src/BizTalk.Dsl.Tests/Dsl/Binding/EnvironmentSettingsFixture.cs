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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Be.Stateless.BizTalk.Install;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
	public class EnvironmentSettingsFixture : EnvironmentSettings
	{
		#region Setup/Teardown

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			_rootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Dsl\Binding\Data");
		}

		#endregion

		[Test]
		public void ReferenceTypeValueForTargetEnvironmentThrowsWhenValueIsNull()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = _rootPath;

			var sut = new EnvironmentSettingsFixture();
			Assert.That(
				() => sut.ValueForTargetEnvironment(new string[] { null, null, null, null, null }, "ClaimStoreCheckInDirectory2"),
				Throws.InvalidOperationException.With.Message.EqualTo("'ClaimStoreCheckInDirectory2' does not have a defined value neither for 'DEV' or default target environment."));
		}

		[Test]
		public void ReferenceTypeValueForTargetEnvironmentWithNotFoundOverridePropertyFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = _rootPath;

			var sut = new EnvironmentSettingsFixture();
			var value = sut.ValueForTargetEnvironment(new[] { null, "C:\\Files\\CheckIn", null, null, null }, "ClaimStoreCheckInDirectory2");
			Assert.That(value, Is.EqualTo("C:\\Files\\CheckIn"));
		}

		[Test]
		public void ReferenceTypeValueForTargetEnvironmentWithNotFoundSettingOverrideFileFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = Path.Combine(_rootPath, "dummy");

			var sut = new EnvironmentSettingsFixture();
			var value = sut.ValueForTargetEnvironment(new[] { null, "C:\\Files\\Drops\\BizTalk.Factory\\CheckIn", null, null, null }, "ClaimStoreCheckInDirectory");
			Assert.That(value, Is.EqualTo("C:\\Files\\Drops\\BizTalk.Factory\\CheckIn"));
		}

		[Test]
		public void ReferenceTypeValueForTargetEnvironmentWithoutOverride()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = null;

			var sut = new EnvironmentSettingsFixture();
			var value = sut.ValueForTargetEnvironment(new[] { null, "C:\\Files\\CheckIn", null, null, null }, "ClaimStoreCheckInDirectory");
			Assert.That(value, Is.EqualTo("C:\\Files\\CheckIn"));
		}

		[Test]
		public void ReferenceTypeValueForTargetEnvironmentWithOverride()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = _rootPath;

			var sut = new EnvironmentSettingsFixture();
			var value = sut.ValueForTargetEnvironment(new[] { null, "C:\\Files\\CheckIn", null, null, null }, "ClaimStoreCheckInDirectory");
			Assert.That(value, Is.EqualTo("C:\\Files\\Drops\\BizTalk.Factory\\CheckIn"));
		}

		[Test]
		public void ValueTypeValueForTargetEnvironmentThrowsWhenValueIsNull()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = _rootPath;

			var sut = new EnvironmentSettingsFixture();
			Assert.That(
				() => sut.ValueForTargetEnvironment(new int?[] { null, null, null, null, null }, "BamArchiveWindowTimeLength2"),
				Throws.InvalidOperationException.With.Message.EqualTo("'BamArchiveWindowTimeLength2' does not have a defined value neither for 'DEV' or default target environment."));
		}

		[Test]
		public void ValueTypeValueForTargetEnvironmentWithNotFoundOverridePropertyFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = _rootPath;

			var sut = new EnvironmentSettingsFixture();
			var value = sut.ValueForTargetEnvironment(new int?[] { null, 1, 2, 3, 4 }, "BamArchiveWindowTimeLength2");
			Assert.That(value, Is.EqualTo(1));
		}

		[Test]
		public void ValueTypeValueForTargetEnvironmentWithNotFoundSettingOverrideFileFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = Path.Combine(_rootPath, "dummy");

			var sut = new EnvironmentSettingsFixture();
			var value = sut.ValueForTargetEnvironment(new int?[] { null, 30, 30, 30, 30 }, "BamArchiveWindowTimeLength");
			Assert.That(value, Is.EqualTo(30));
		}

		[Test]
		public void ValueTypeValueForTargetEnvironmentWithoutOverride()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = null;

			var sut = new EnvironmentSettingsFixture();
			var value = sut.ValueForTargetEnvironment(new int?[] { null, 30, 30, 30, 30 }, "BamArchiveWindowTimeLength");
			Assert.That(value, Is.EqualTo(30));
		}

		[Test]
		public void ValueTypeValueForTargetEnvironmentWithOverride()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = _rootPath;

			var sut = new EnvironmentSettingsFixture();
			var value = sut.ValueForTargetEnvironment(new int?[] { null, 1, 2, 3, 4 }, "BamArchiveWindowTimeLength");
			Assert.That(value, Is.EqualTo(30));
		}

		protected override string SettingsFileName
		{
			get { return "BizTalk.Factory.SettingsFileGenerator"; }
		}

		protected override string[] TargetEnvironments
		{
			get { return _targetEnvironments; }
		}

		private string _rootPath;

		private static readonly string[] _targetEnvironments = { null, "DEV", "BLD", "ACC", "PRD" };
	}
}

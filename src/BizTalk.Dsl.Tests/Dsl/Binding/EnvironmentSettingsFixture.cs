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

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			_rootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Dsl\Binding\Data");
		}

		#endregion

		[Test]
		public void ReferenceTypeForTargetEnvironmentWithNotFoundOverride()
		{
			BindingGenerationContext.Instance.TargetEnvironment = "DEV";
			BindingGenerationContext.Instance.EnvironmentSettingRootPath = _rootPath;

			var sut = new EnvironmentSettingsFixture();
			Assert.That(
				() => sut.ValueForTargetEnvironment(new[] { null, "C:\\Files\\CheckIn", null, null, null }, "ClaimStoreCheckInDirectory2"),
				Throws.InvalidOperationException.With.Message.EqualTo(
					string.Format(
						"Environment setting file '{0}' does not define the setting '{1}'.",
						Path.Combine(_rootPath, SettingsFileName + ".xml"),
						"ClaimStoreCheckInDirectory2")));
		}

		[Test]
		public void ReferenceTypeForTargetEnvironmentWithoutOverride()
		{
			BindingGenerationContext.Instance.TargetEnvironment = "DEV";
			BindingGenerationContext.Instance.EnvironmentSettingRootPath = null;

			var sut = new EnvironmentSettingsFixture();
			var value = sut.ValueForTargetEnvironment(new[] { null, "C:\\Files\\Drops\\BizTalk.Factory\\CheckIn", null, null, null }, "ClaimStoreCheckInDirectory");
			Assert.That(value, Is.EqualTo("C:\\Files\\Drops\\BizTalk.Factory\\CheckIn"));
		}

		[Test]
		public void ReferenceTypeForTargetEnvironmentWithOverride()
		{
			BindingGenerationContext.Instance.TargetEnvironment = "DEV";
			BindingGenerationContext.Instance.EnvironmentSettingRootPath = _rootPath;

			var sut = new EnvironmentSettingsFixture();
			var value = sut.ValueForTargetEnvironment(new[] { null, "C:\\Files\\CheckIn", null, null, null }, "ClaimStoreCheckInDirectory");
			Assert.That(value, Is.EqualTo("C:\\Files\\Drops\\BizTalk.Factory\\CheckIn"));
		}

		[Test]
		public void ValueTypeForTargetEnvironmentWithNotFoundOverride()
		{
			BindingGenerationContext.Instance.TargetEnvironment = "DEV";
			BindingGenerationContext.Instance.EnvironmentSettingRootPath = _rootPath;

			var sut = new EnvironmentSettingsFixture();
			Assert.That(
				() => sut.ValueForTargetEnvironment(new int?[] { null, 1, 2, 3, 4 }, "BamArchiveWindowTimeLength2"),
				Throws.InvalidOperationException.With.Message.EqualTo(
					string.Format(
						"Environment setting file '{0}' does not define the setting '{1}'.",
						Path.Combine(_rootPath, SettingsFileName + ".xml"),
						"BamArchiveWindowTimeLength2")));
		}

		[Test]
		public void ValueTypeForTargetEnvironmentWithoutOverride()
		{
			BindingGenerationContext.Instance.TargetEnvironment = "DEV";
			BindingGenerationContext.Instance.EnvironmentSettingRootPath = null;

			var sut = new EnvironmentSettingsFixture();
			var value = sut.ValueForTargetEnvironment(new int?[] { null, 30, 30, 30, 30 }, "BamArchiveWindowTimeLength");
			Assert.That(value, Is.EqualTo(30));
		}

		[Test]
		public void ValueTypeForTargetEnvironmentWithOverride()
		{
			BindingGenerationContext.Instance.TargetEnvironment = "DEV";
			BindingGenerationContext.Instance.EnvironmentSettingRootPath = _rootPath;

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

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

using System;
using System.IO;
using System.Runtime.CompilerServices;
using Be.Stateless.BizTalk.Install;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public abstract class EnvironmentSettings : IEnvironmentSettingOverrides
	{
		#region IEnvironmentSettingOverrides Members

		public T[] ValuesForProperty<T>(string propertyName, T[] defaultValues) where T : class
		{
			return defaultValues;
		}

		public T?[] ValuesForProperty<T>(string propertyName, T?[] defaultValues) where T : struct
		{
			return defaultValues;
		}

		#endregion

		protected abstract string SettingsFileName { get; }

		protected abstract string[] TargetEnvironments { get; }

		private IEnvironmentSettingOverrides SettingsOverrides
		{
			get
			{
				if (BindingGenerationContext.EnvironmentSettingRootPath != null && _environmentSettingOverrides == null)
				{
					var filePath = Path.Combine(BindingGenerationContext.EnvironmentSettingRootPath, SettingsFileName + ".xml");
					if (File.Exists(filePath)) _environmentSettingOverrides = new EnvironmentSettingOverrides(filePath);
				}
				return _environmentSettingOverrides ?? (_environmentSettingOverrides = this);
			}
		}

		private int TargetEnvironmentIndex
		{
			get
			{
				_targetEnvironmentsIndex = Array.IndexOf(TargetEnvironments, BindingGenerationContext.TargetEnvironment);
				if (_targetEnvironmentsIndex < 0)
					throw new InvalidOperationException(
						string.Format(
							"'{0}' is not a target environment declared in the '{1}' file.",
							BindingGenerationContext.TargetEnvironment,
							SettingsFileName));
				return _targetEnvironmentsIndex;
			}
		}

		protected T ValueForTargetEnvironment<T>(T?[] values, [CallerMemberName] string propertyName = null) where T : struct
		{
			values = SettingsOverrides.ValuesForProperty(propertyName, values);
			var value = values[TargetEnvironmentIndex] ?? values[0];
			if (value == null)
				throw new InvalidOperationException(
					string.Format(
						"'{0}' does not have a defined value neither for '{1}' or default target environment.",
						propertyName,
						BindingGenerationContext.TargetEnvironment));
			return value.Value;
		}

		protected T ValueForTargetEnvironment<T>(T[] values, [CallerMemberName] string propertyName = null) where T : class
		{
			values = SettingsOverrides.ValuesForProperty(propertyName, values);
			var value = values[TargetEnvironmentIndex] ?? values[0];
			if (value == null)
				throw new InvalidOperationException(
					string.Format(
						"'{0}' does not have a defined value neither for '{1}' or default target environment.",
						propertyName,
						BindingGenerationContext.TargetEnvironment));
			return value;
		}

		private IEnvironmentSettingOverrides _environmentSettingOverrides;

		private int _targetEnvironmentsIndex = -1;
	}
}

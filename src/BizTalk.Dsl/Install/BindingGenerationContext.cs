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

namespace Be.Stateless.BizTalk.Install
{
	public static class BindingGenerationContext
	{
		#region Nested Type: BindingGenerationContextMemento

		[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
		private class BindingGenerationContextMemento : IBindingGenerationContext
		{
			#region IBindingGenerationContext Members

			public string EnvironmentSettingRootPath { get; internal set; }

			public string TargetEnvironment { get; internal set; }

			#endregion
		}

		#endregion

		public static string EnvironmentSettingRootPath
		{
			get { return _instance.EnvironmentSettingRootPath; }
			internal set { _instance.EnvironmentSettingRootPath = value; }
		}

		public static IBindingGenerationContext Instance
		{
			get { return _instance; }
		}

		public static string TargetEnvironment
		{
			get { return _instance.TargetEnvironment; }
			internal set { _instance.TargetEnvironment = value; }
		}

		private static readonly BindingGenerationContextMemento _instance = new BindingGenerationContextMemento();
	}
}

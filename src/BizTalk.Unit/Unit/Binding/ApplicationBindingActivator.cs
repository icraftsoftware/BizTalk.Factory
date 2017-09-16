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
using System.Collections.Generic;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Visitor;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Unit.Binding
{
	public static class ApplicationBindingActivator<TNamingConvention> where TNamingConvention : class
	{
		public static T Create<T>(string targetEnvironment)
			where T : ApplicationBindingBase<TNamingConvention>, new()
		{
			if (targetEnvironment.IsNullOrEmpty()) throw new ArgumentNullException("targetEnvironment");
			BindingGenerationContext.TargetEnvironment = targetEnvironment;
			if (!_cache.ContainsKey(targetEnvironment))
			{
				var applicationBinding = new T();
				SettleApplicationBindingForBindingGenerationContext(applicationBinding);
				_cache[targetEnvironment] = applicationBinding;
			}
			return (T) _cache[targetEnvironment];
		}

		public static T Create<T>(string targetEnvironment, string environmentSettingRootPath)
			where T : ApplicationBindingBase<TNamingConvention>, new()
		{
			if (environmentSettingRootPath.IsNullOrEmpty()) throw new ArgumentNullException("environmentSettingRootPath");
			BindingGenerationContext.EnvironmentSettingRootPath = environmentSettingRootPath;
			return Create<T>(targetEnvironment);
		}

		private static void SettleApplicationBindingForBindingGenerationContext<T>(T applicationBinding)
			where T : IVisitable<IApplicationBindingVisitor>
		{
			var applicationBindingEnvironmentSettlerVisitor = new ApplicationBindingEnvironmentSettlerVisitor();
			applicationBinding.Accept(applicationBindingEnvironmentSettlerVisitor);
		}

		private static readonly Dictionary<string, ApplicationBindingBase<TNamingConvention>> _cache = new Dictionary<string, ApplicationBindingBase<TNamingConvention>>();
	}
}

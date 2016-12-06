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
using System.Collections;
using System.Configuration.Install;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Visitor;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Install
{
	public abstract class ApplicationInstaller<T> : Installer
		where T : class, IBindingSerializerFactory, IVisitable<IApplicationBindingVisitor>, new()
	{
		#region Base Class Member Overrides

		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);

			try
			{
				var targetEnvironment = Context.Parameters["TargetEnvironment"];
				if (targetEnvironment.IsNullOrEmpty()) throw new InvalidOperationException("TargetEnvironment has no defined value.");
				BindingGenerationContext.Instance.TargetEnvironment = targetEnvironment;

				var environmentSettingRootPath = Context.Parameters["EnvironmentSettingOverridesRootPath"];
				if (!environmentSettingRootPath.IsNullOrEmpty()) BindingGenerationContext.Instance.EnvironmentSettingRootPath = environmentSettingRootPath;

				BizTalkAssemblyResolver.Register(msg => Context.LogMessage(msg));
				BizTalkAssemblyResolver.RegisterProbingPaths(Context.Parameters["AssemblyPath"]);
				if (Context.Parameters.ContainsKey("BindingFilePath"))
				{
					var bindingFilePath = Context.Parameters["BindingFilePath"];
					GenerateBindingFile(bindingFilePath);
				}
				if (Context.Parameters.ContainsKey("SetupFileAdapterPaths"))
				{
					var users = Context.Parameters["Users"].IfNotNullOrEmpty(u => u.Split(';', ','));
					SetupFileAdapterPaths(users);
				}
				if (Context.Parameters.ContainsKey("InitializeServices"))
				{
					InitializeServices();
				}
			}
			finally
			{
				BizTalkAssemblyResolver.Unregister();
			}
		}

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);

			try
			{
				var targetEnvironment = Context.Parameters["TargetEnvironment"];
				if (targetEnvironment.IsNullOrEmpty()) throw new InvalidOperationException("TargetEnvironment has no defined value.");
				BindingGenerationContext.Instance.TargetEnvironment = targetEnvironment;

				BizTalkAssemblyResolver.Register(msg => Context.LogMessage(msg));
				BizTalkAssemblyResolver.RegisterProbingPaths(Context.Parameters["AssemblyPath"]);
				if (Context.Parameters.ContainsKey("TeardownFileAdapterPaths"))
				{
					var recurse = Context.Parameters.ContainsKey("Recurse");
					TeardownFileAdapterPaths(recurse);
				}
			}
			finally
			{
				BizTalkAssemblyResolver.Unregister();
			}
		}

		#endregion

		private T ApplicationBinding
		{
			get { return _applicationBinding ?? (_applicationBinding = new T()); }
		}

		private void GenerateBindingFile(string bindingFilePath)
		{
			if (bindingFilePath.IsNullOrEmpty()) throw new ArgumentNullException("bindingFilePath");

			var applicationBindingSerializer = ApplicationBinding.GetBindingSerializer();
			applicationBindingSerializer.Save(bindingFilePath);
		}

		private void SetupFileAdapterPaths(string[] users)
		{
			if (users == null) throw new ArgumentNullException("users");

			var visitor = FileAdapterFolderConfiguratorVisitor.CreateInstaller(users);
			ApplicationBinding.Accept(visitor);
		}

		private void TeardownFileAdapterPaths(bool recurse)
		{
			var visitor = FileAdapterFolderConfiguratorVisitor.CreateUninstaller(recurse);
			ApplicationBinding.Accept(visitor);
		}

		private void InitializeServices()
		{
			var visitor = BizTalkServiceConfiguratorVisitor.Create();
			ApplicationBinding.Accept(visitor);
			visitor.Commit();
		}

		private T _applicationBinding;
	}
}

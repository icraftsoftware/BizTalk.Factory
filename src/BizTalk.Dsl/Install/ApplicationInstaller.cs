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

using System;
using System.Collections;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.BizTalk.Dsl.Binding.Visitor;
using Be.Stateless.Extensions;
using Microsoft.Win32;

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

				AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
				if (Context.Parameters.ContainsKey("BindingFilePath"))
				{
					var bindingFilePath = Context.Parameters["BindingFilePath"];
					GenerateBindingFile(targetEnvironment, bindingFilePath);
				}
				if (Context.Parameters.ContainsKey("SetupFileAdapterPaths"))
				{
					var users = Context.Parameters["Users"].IfNotNullOrEmpty(u => u.Split(';', ','));
					SetupFileAdapterPaths(targetEnvironment, users);
				}
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
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

				AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
				if (Context.Parameters.ContainsKey("TeardownFileAdapterPaths"))
				{
					var recurse = Context.Parameters.ContainsKey("Recurse");
					TeardownFileAdapterPaths(targetEnvironment, recurse);
				}
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
			}
		}

		#endregion

		private T ApplicationBinding
		{
			get { return _applicationBinding ?? (_applicationBinding = new T()); }
		}

		private String BizTalkInstallPath
		{
			get
			{
				if (_installPath == null)
				{
					// [HKLM\SOFTWARE\Microsoft\BizTalk Server\3.0]
					using (var classes32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
					using (var btsKey = classes32.SafeOpenSubKey(@"SOFTWARE\Microsoft\BizTalk Server\3.0"))
					{
						_installPath = (string) btsKey.GetValue("InstallPath");
					}
				}
				return _installPath;
			}
		}

		private void GenerateBindingFile(string targetEnvironment, string bindingFilePath)
		{
			if (bindingFilePath.IsNullOrEmpty()) throw new ArgumentNullException("bindingFilePath");

			var applicationBindingSerializer = ApplicationBinding.GetBindingSerializer(targetEnvironment);
			applicationBindingSerializer.Save(bindingFilePath);
		}

		private void SetupFileAdapterPaths(string targetEnvironment, string[] users)
		{
			if (users == null) throw new ArgumentNullException("users");

			var visitor = FileAdapterFolderConfiguratorVisitor.CreateInstaller(targetEnvironment, users);
			ApplicationBinding.Accept(visitor);
		}

		private void TeardownFileAdapterPaths(string targetEnvironment, bool recurse)
		{
			var visitor = FileAdapterFolderConfiguratorVisitor.CreateUninstaller(targetEnvironment, recurse);
			ApplicationBinding.Accept(visitor);
		}

		private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			// unexisting resource assemblies
			if (args.Name.StartsWith("Microsoft.BizTalk.Pipeline.Components.resources, Version=3.0.")) return null;
			if (args.Name.StartsWith("Microsoft.ServiceModel.Channels.resources, Version=3.0.")) return null;

			var name = new AssemblyName(args.Name);
			var locationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			// ReSharper disable once AssignNullToNotNullAttribute
			var fullPath = Path.Combine(locationPath, name.Name + ".dll");
			if (File.Exists(fullPath))
			{
				Context.LogMessage(string.Format("   Resolved assembly '{0}'.", fullPath));
				return Assembly.LoadFile(fullPath);
			}

			fullPath = Path.Combine(BizTalkInstallPath, @"SDK\Utilities\PipelineTools", name.Name + ".dll");
			if (File.Exists(fullPath))
			{
				Context.LogMessage(string.Format("   Resolved assembly '{0}'.", fullPath));
				return Assembly.LoadFile(fullPath);
			}

			Context.LogMessage(string.Format("   Could not resolve assembly '{0}'.", args.Name));
			return null;
		}

		private T _applicationBinding;
		private string _installPath;
	}
}

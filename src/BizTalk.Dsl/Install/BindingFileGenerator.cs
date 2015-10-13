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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.Extensions;
using Microsoft.Win32;

namespace Be.Stateless.BizTalk.Install
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Pseudo installer class.")]
	public abstract class BindingFileGenerator<T> : Installer
		where T : IBindingSerializerFactory, new()
	{
		#region Base Class Member Overrides

		public override void Install(IDictionary stateSaver)
		{
			var targetEnvironment = Context.Parameters["TargetEnvironment"];
			var bindingFilePath = Context.Parameters["BindingFilePath"];
			GenerateBindingFile(targetEnvironment, bindingFilePath);
			base.Install(stateSaver);
		}

		#endregion

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
			if (targetEnvironment.IsNullOrEmpty()) throw new ArgumentNullException("targetEnvironment");
			if (bindingFilePath.IsNullOrEmpty()) throw new ArgumentNullException("bindingFilePath");

			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
				BindingGenerationContext.Instance.TargetEnvironment = targetEnvironment;
				var applicationBindingSerializerFactory = new T();
				var applicationBindingSerializer = applicationBindingSerializerFactory.GetBindingSerializer(targetEnvironment);
				applicationBindingSerializer.Save(bindingFilePath);
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
			}
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

		private string _installPath;
	}
}

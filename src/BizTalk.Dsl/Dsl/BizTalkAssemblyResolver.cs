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
using System.IO;
using System.Reflection;
using Be.Stateless.Extensions;
using Microsoft.Win32;

namespace Be.Stateless.BizTalk.Dsl
{
	internal class BizTalkAssemblyResolver
	{
		public static void Register(Action<string> log)
		{
			// TODO use log4net instead, but should work with both InstallUtil and MSBuild
			_instance._log = log;
			AppDomain.CurrentDomain.AssemblyResolve += _instance.OnAssemblyResolve;
		}

		public static void Unregister()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= _instance.OnAssemblyResolve;
		}

		private BizTalkAssemblyResolver() { }

		private string BizTalkInstallPath
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
				_log(string.Format("   Resolved assembly '{0}'.", fullPath));
				return Assembly.LoadFile(fullPath);
			}

			fullPath = Path.Combine(BizTalkInstallPath, @"SDK\Utilities\PipelineTools", name.Name + ".dll");
			if (File.Exists(fullPath))
			{
				_log(string.Format("   Resolved assembly '{0}'.", fullPath));
				return Assembly.LoadFile(fullPath);
			}

			fullPath = Path.Combine(BizTalkInstallPath, @"Developer Tools", name.Name + ".dll");
			if (File.Exists(fullPath))
			{
				_log(string.Format("   Resolved assembly '{0}'.", fullPath));
				return Assembly.LoadFile(fullPath);
			}

			_log(string.Format("   Could not resolve assembly '{0}'.", args.Name));
			return null;
		}

		private static readonly BizTalkAssemblyResolver _instance = new BizTalkAssemblyResolver();
		private string _installPath;
		private Action<string> _log;
	}
}

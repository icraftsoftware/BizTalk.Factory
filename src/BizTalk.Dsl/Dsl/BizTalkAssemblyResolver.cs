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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Be.Stateless.Extensions;
using Microsoft.Win32;

namespace Be.Stateless.BizTalk.Dsl
{
	internal class BizTalkAssemblyResolver
	{
		static BizTalkAssemblyResolver()
		{
			// [HKLM\SOFTWARE\Microsoft\BizTalk Server\3.0]
			using (var classes32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
			using (var btsKey = classes32.SafeOpenSubKey(@"SOFTWARE\Microsoft\BizTalk Server\3.0"))
			{
				_installPath = (string) btsKey.GetValue("InstallPath");
			}

			_defaultProbingPaths = new[] {
				_installPath,
				Path.Combine(_installPath, @"Developer Tools"),
				Path.Combine(_installPath, @"SDK\Utilities\PipelineTools")
			};

			_instance = new BizTalkAssemblyResolver();
		}

		private static string BizTalkInstallPath
		{
			get { return _installPath; }
		}

		public static void Register(Action<string> log)
		{
			// TODO use log4net instead, but should work with both InstallUtil and MSBuild
			_instance._log = log;
			_instance._privateProbingPaths = Enumerable.Empty<string>();
			AppDomain.CurrentDomain.AssemblyResolve += _instance.OnAssemblyResolve;
		}

		public static void RegisterProbingPaths(params string[] assemblyPaths)
		{
			_instance._privateProbingPaths = assemblyPaths.Select(Path.GetDirectoryName);
		}

		public static void Unregister()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= _instance.OnAssemblyResolve;
		}

		private BizTalkAssemblyResolver() { }

		private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			// nonexistent resource assemblies
			if (args.Name.StartsWith("Microsoft.BizTalk.ExplorerOM.resources, Version=3.0.")) return null;
			if (args.Name.StartsWith("Microsoft.BizTalk.Pipeline.Components.resources, Version=3.0.")) return null;
			if (args.Name.StartsWith("Microsoft.ServiceModel.Channels.resources, Version=3.0.")) return null;

			// nonexistent xml serializers
			if (Regex.IsMatch(args.Name, @"(Microsoft|Be\.Stateless)\..+\.XmlSerializers, Version=")) return null;

			var assemblyName = new AssemblyName(args.Name);

			var resolutionPath = _defaultProbingPaths.Concat(_privateProbingPaths)
				.Select(path => Path.Combine(path, assemblyName.Name + ".dll"))
				.FirstOrDefault(File.Exists);
			if (resolutionPath != null)
			{
				_log(string.Format("   Resolved assembly '{0}'.", resolutionPath));
				return Assembly.LoadFile(resolutionPath);
			}

			_log(string.Format("   Could not resolve assembly '{0}'.", args.Name));
			return null;
		}

		private static readonly string[] _defaultProbingPaths;
		private static readonly string _installPath;
		private static readonly BizTalkAssemblyResolver _instance;
		private Action<string> _log;
		private IEnumerable<string> _privateProbingPaths;
	}
}

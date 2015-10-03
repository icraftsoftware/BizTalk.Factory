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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.Extensions;
using Be.Stateless.Resources;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Be.Stateless.BizTalk.Dsl.MSBuild.Tasks
{
	[Serializable]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Implements Msbuild Task API.")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Implements Msbuild Task API.")]
	public class GenerateEnvironmentSettingsClasses : Task
	{
		#region Nested Type: Typifier

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "XSLT Extension Object.")]
		private class Typifier
		{
			public bool IsInteger(string value)
			{
				int i;
				return int.TryParse(value, out i);
			}
		}

		#endregion

		static GenerateEnvironmentSettingsClasses()
		{
			var type = typeof(GenerateEnvironmentSettingsClasses);
			_xslt = ResourceManager.Load(
				type.Assembly,
				type.FullName + ".xslt",
				stream => {
					using (var xmlReader = XmlReader.Create(stream))
					{
						var xslt = new XslCompiledTransform(true);
						xslt.Load(xmlReader, XsltSettings.TrustedXslt, new XmlUrlResolver());
						return xslt;
					}
				});
		}

		#region Base Class Member Overrides

		public override bool Execute()
		{
			try
			{
				var outputs = new List<ITaskItem>();
				foreach (var settingsFile in SettingsFiles)
				{
					Log.LogMessage(MessageImportance.High, "Generating environment settings class '{0}'.", settingsFile.GetMetadata("Identity"));
					var outputPath = ComputeOutputPath(settingsFile);
					var taskItem = new TaskItem(outputPath);
					taskItem.SetMetadata("DependentUpon", settingsFile.GetMetadata("Filename") + settingsFile.GetMetadata("Extension"));
					outputs.Add(taskItem);
					using (var reader = XmlReader.Create(settingsFile.GetMetadata("FullPath")))
					using (var writer = File.CreateText(taskItem.GetMetadata("FullPath")))
					{
						var arguments = new XsltArgumentList();
						arguments.AddExtensionObject("urn:extensions.stateless.be:biztalk:environment-settings-class-generation:typifier:2015:10", new Typifier());
						arguments.AddParam("clr-namespace-name", string.Empty, ComputeNamespace(taskItem));
						arguments.AddParam("clr-class-name", string.Empty, ComputeClassName(taskItem));
						_xslt.Transform(reader, arguments, writer);
					}
				}
				// TODO ensure all generated classes have the same target environment value list
				SettingsClass = outputs.ToArray();
				return true;
			}
			catch (Exception exception)
			{
				Log.LogErrorFromException(exception, true, true, null);
				return false;
			}
		}

		#endregion

		[Required]
		public string RootNamespace { get; set; }

		[Output]
		public ITaskItem[] SettingsClass { get; private set; }

		[Required]
		public ITaskItem[] SettingsFiles { get; set; }

		private string ComputeClassName(ITaskItem taskItem)
		{
			var name = Regex.Match(taskItem.GetMetadata("Filename"), @"(?<ClassName>.+)(?i:\.SettingsFileGenerator\.*)")
				.Groups["ClassName"].Value;
			var className = Regex.Replace(name, @"\W", string.Empty) + "Settings";
			return className;
		}

		private string ComputeNamespace(ITaskItem taskItem)
		{
			var @namespace = RootNamespace + "." + taskItem.GetMetadata("RelativeDir").Replace('\\', '.').Trim('.');
			return @namespace;
		}

		private string ComputeOutputPath(ITaskItem settingsFile)
		{
			var inputPath = settingsFile.GetMetadata("Link").IfNotNullOrEmpty(p => p) ?? settingsFile.GetMetadata("Identity");
			// ReSharper disable once AssignNullToNotNullAttribute
			return Path.Combine(Path.GetDirectoryName(inputPath), Path.GetFileNameWithoutExtension(inputPath) + ".Designer.cs");
		}

		private static readonly XslCompiledTransform _xslt;
	}
}

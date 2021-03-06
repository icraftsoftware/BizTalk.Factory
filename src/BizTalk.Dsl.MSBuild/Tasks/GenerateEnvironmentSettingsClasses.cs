﻿#region Copyright & License

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
using System.CodeDom;
using System.CodeDom.Compiler;
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
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Implements Msbuild Task API.")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Implements Msbuild Task API.")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Msbuild Task.")]
	public class GenerateEnvironmentSettingsClasses : Task
	{
		#region Nested Type: Stringifier

		private class Stringifier
		{
			public string Escape(string value)
			{
				// http://stackoverflow.com/questions/323640/can-i-convert-a-c-sharp-string-value-to-an-escaped-string-literal
				using (var writer = new StringWriter())
				using (var provider = CodeDomProvider.CreateProvider("CSharp"))
				{
					provider.GenerateCodeFromExpression(new CodePrimitiveExpression(value), writer, null);
					return writer.ToString();
				}
			}
		}

		#endregion

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
				foreach (var settingsFileTaskItem in SettingsFiles)
				{
					Log.LogMessage(MessageImportance.High, "Generating environment settings class '{0}'.", settingsFileTaskItem.GetMetadata("Identity"));
					var outputPath = ComputeOutputPath(settingsFileTaskItem);
					var settingsClassTaskItem = new TaskItem(outputPath);
					settingsClassTaskItem.SetMetadata("DependentUpon", settingsFileTaskItem.GetMetadata("Filename") + settingsFileTaskItem.GetMetadata("Extension"));
					outputs.Add(settingsClassTaskItem);
					using (var reader = XmlReader.Create(settingsFileTaskItem.GetMetadata("FullPath")))
					using (var writer = File.CreateText(settingsClassTaskItem.GetMetadata("FullPath")))
					{
						var arguments = new XsltArgumentList();
						arguments.AddExtensionObject("urn:extensions.stateless.be:biztalk:environment-settings-class-generation:string:2015:10", new Stringifier());
						arguments.AddExtensionObject("urn:extensions.stateless.be:biztalk:environment-settings-class-generation:typifier:2015:10", new Typifier());
						arguments.AddParam("clr-namespace-name", string.Empty, ComputeNamespace(settingsClassTaskItem));
						arguments.AddParam("clr-class-name", string.Empty, ComputeClassName(settingsClassTaskItem));
						arguments.AddParam("settings-file-name", string.Empty, settingsFileTaskItem.GetMetadata("Filename"));
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

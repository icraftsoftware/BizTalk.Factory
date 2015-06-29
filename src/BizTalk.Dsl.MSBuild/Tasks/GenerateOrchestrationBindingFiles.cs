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
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using Be.Stateless.BizTalk.Dsl.Binding.CodeDom;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CSharp;

namespace Be.Stateless.BizTalk.Dsl.MSBuild.Tasks
{
	[Serializable]
	public class GenerateOrchestrationBindingFiles : AppDomainIsolatedTask
	{
		#region Base Class Member Overrides

		public override bool Execute()
		{
			using (var provider = new CSharpCodeProvider())
			{
				// TODO refactor this in task ResolveReferencedBizTalkOrchestrationAssemblies
				var orchestrationTypes = OrchestrationAssemblies
					.Select(ra => ra.GetMetadata("Identity"))
					.Select(Assembly.LoadFrom)
					// make sure all assemblies are loaded before proceeding with reflection
					.ToArray()
					.Where(a => a.IsBizTalkAssembly())
					.SelectMany(a => a.GetOrchestrations());

				// TODO delete all previously generated files

				foreach (var orchestrationType in orchestrationTypes)
				{
					Log.LogMessage(MessageImportance.High, "Generating binding class for orchestration '{0}'.", orchestrationType.FullName);
					var path = ComputeOutputPath(orchestrationType);
					// ReSharper disable once AssignNullToNotNullAttribute
					Directory.CreateDirectory(Path.GetDirectoryName(path));
					using (var writer = new StreamWriter(path))
					{
						provider.GenerateCodeFromCompileUnit(
							orchestrationType.ConvertToOrchestrationBindingCodeCompileUnit(),
							writer,
							new CodeGeneratorOptions { BracingStyle = "C", IndentString = "\t", VerbatimOrder = true });
					}
				}
			}
			return true;
		}

		#endregion

		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		[Required]
		public ITaskItem[] OrchestrationAssemblies { get; set; }

		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		[Required]
		public string RootNamespace { get; set; }

		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		[Required]
		public string RootPath { get; set; }

		private string ComputeOutputPath(Type orchestrationType)
		{
			var projectDirectory = RootPath ?? Directory.GetCurrentDirectory();
			var projectRootNamespace = RootNamespace ?? new Project(BuildEngine.ProjectFileOfTaskNode).AllEvaluatedProperties.Single(p => p.Name == "RootNamespace").EvaluatedValue;

			// ReSharper disable once PossibleNullReferenceException
			var relativePath = orchestrationType.Namespace.StartsWith(projectRootNamespace + ".")
				? orchestrationType.Namespace.Substring(projectRootNamespace.Length + 1)
				: "Orchestrations";
			var folder = Path.Combine(projectDirectory, relativePath);
			return Path.Combine(folder, orchestrationType.Name + "OrchestrationBinding.Designer.cs");
		}
	}
}

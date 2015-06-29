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
using System.IO;
using System.Linq;
using System.Reflection;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Be.Stateless.BizTalk.Dsl.MSBuild.Tasks
{
	[Serializable]
	public class GeneratePipelineFiles : AppDomainIsolatedTask
	{
		#region Base Class Member Overrides

		public override bool Execute()
		{
			// TODO refactor this in task ResolveReferencedBizTalkPipelineAssemblies
			var pipelineTypes = PipelineDefinitionAssemblies
				.Select(ra => ra.GetMetadata("Identity"))
				.Select(Assembly.LoadFrom)
				// make sure all assemblies are loaded before proceeding with reflection
				.ToArray()
				.SelectMany(a => a.GetTypes())
				.Where(t => typeof(SendPipeline).IsAssignableFrom(t) || typeof(ReceivePipeline).IsAssignableFrom(t));

			// TODO delete all previously generated files

			var outputs = new List<ITaskItem>();
			foreach (var pipelineType in pipelineTypes)
			{
				var path = ComputeOutputPath(pipelineType);
				Log.LogMessage(MessageImportance.High, "Generating pipeline '{0}'.", pipelineType.FullName);
				// ReSharper disable once AssignNullToNotNullAttribute
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				var pipelineSerializerFactory = (IPipelineSerializerFactory) Activator.CreateInstance(pipelineType);
				pipelineSerializerFactory.GetPipelineDesignerDocumentSerializer().Save(path);
				Log.LogMessage(MessageImportance.High, "Adding pipeline to item group {0}", path);
				var taskItem = new TaskItem(path);
				taskItem.SetMetadata("TypeName", pipelineType.Name);
				taskItem.SetMetadata("Namespace", pipelineType.Namespace);
				outputs.Add(taskItem);
			}
			PipelineFiles = outputs.ToArray();
			return true;
		}

		#endregion

		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		[Required]
		public ITaskItem[] PipelineDefinitionAssemblies { get; set; }

		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		[Output]
		public ITaskItem[] PipelineFiles { get; private set; }

		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		[Required]
		public string RootNamespace { get; set; }

		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		[Required]
		public string RootPath { get; set; }

		private string ComputeOutputPath(Type pipelineType)
		{
			var projectDirectory = RootPath ?? Directory.GetCurrentDirectory();
			var projectRootNamespace = RootNamespace ?? new Project(BuildEngine.ProjectFileOfTaskNode).AllEvaluatedProperties.Single(p => p.Name == "RootNamespace").EvaluatedValue;

			// ReSharper disable once PossibleNullReferenceException
			var relativePath = pipelineType.Namespace.StartsWith(projectRootNamespace + ".")
				? pipelineType.Namespace.Substring(projectRootNamespace.Length + 1)
				: "Pipelines";
			var folder = Path.Combine(projectDirectory, relativePath);
			return Path.Combine(folder, pipelineType.Name + ".btp");
		}
	}
}

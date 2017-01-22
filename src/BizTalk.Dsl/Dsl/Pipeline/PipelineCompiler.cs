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
using System.IO;
using Be.Stateless.Reflection;
using Microsoft.BizTalk.PipelineEditor.PipelineFile;
using BizTalkPipelineCompiler = Microsoft.BizTalk.PipelineEditor.PipelineCompiler;
using PolicyFileDocument = Microsoft.BizTalk.PipelineEditor.PolicyFile.Document;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	public class PipelineCompiler : IDslSerializer
	{
		internal PipelineCompiler(IVisitable<IPipelineVisitor> pipeline)
		{
			_pipeline = pipeline;
		}

		#region IDslSerializer Members

		/// <summary>
		/// </summary>
		/// <remarks>
		/// See <see cref="Document"/>'s <c>Load(string)</c> method.
		/// </remarks>
		/// <see cref="Document" />
		/// <see cref="BizTalkPipelineCompiler.Compile(object, object, object, string, out object)" />
		public string Serialize()
		{
			var pipelineDocument = GetCompilationDocument();
			var policyFilePath = Path.IsPathRooted(pipelineDocument.PolicyFilePath)
				? pipelineDocument.PolicyFilePath
				: Path.Combine((string) Reflector.GetProperty<Document>("PolicyFileDirectory"), pipelineDocument.PolicyFilePath);
			var policyDocument = (PolicyFileDocument) Reflector.InvokeMethod<PolicyFileDocument>("Load", policyFilePath);
			Reflector.InvokeMethod<Document>("MergePipelineFileWithPolicyFile", pipelineDocument, policyDocument);

			var compiler = Activator.CreateInstance<BizTalkPipelineCompiler>();
			Reflector.InvokeMethod(compiler, "FixupManagedComponents", pipelineDocument);
			var compiledOutput = (string) Reflector.InvokeMethod<BizTalkPipelineCompiler>("GetCompiledOutput", pipelineDocument);
			return compiledOutput;
		}

		public void Save(string filePath)
		{
			throw new NotSupportedException();
		}

		public void Write(Stream stream)
		{
			throw new NotSupportedException();
		}

		#endregion

		private Document GetCompilationDocument()
		{
			var visitor = new PipelineCompilerVisitor();
			_pipeline.Accept(visitor);
			return visitor.Document;
		}

		private readonly IVisitable<IPipelineVisitor> _pipeline;
	}
}

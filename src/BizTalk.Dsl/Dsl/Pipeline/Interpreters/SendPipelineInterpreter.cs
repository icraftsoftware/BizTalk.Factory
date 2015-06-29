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

namespace Be.Stateless.BizTalk.Dsl.Pipeline.Interpreters
{
	public class SendPipelineInterpreter<T> : Microsoft.BizTalk.PipelineOM.SendPipeline
		where T : SendPipeline, new()
	{
		static SendPipelineInterpreter()
		{
			_pipelineDefinition = new T();
		}

		#region Base Class Member Overrides

		public override Guid VersionDependentGuid
		{
			get
			{
				var versionDependentGuid = _pipelineDefinition.VersionDependentGuid;
				if (versionDependentGuid == null) throw new InvalidOperationException("VersionDependentGuid cannot be null when used by a pipeline interpreter.");
				return versionDependentGuid;
			}
		}

		public override string XmlContent
		{
			get { return ((IPipelineSerializerFactory) _pipelineDefinition).GetPipelineCompiler().Serialize(); }
		}

		#endregion

		// ReSharper disable StaticFieldInGenericType
		private static readonly T _pipelineDefinition;
		// ReSharper restore StaticFieldInGenericType
	}
}

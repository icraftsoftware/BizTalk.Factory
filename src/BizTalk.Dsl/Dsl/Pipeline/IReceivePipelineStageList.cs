#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	public interface IReceivePipelineStageList : IPipelineStageList
	{
		/// <summary>
		/// 1st stage of a <see cref="ReceivePipeline"/>
		/// </summary>
		IStage Decode { get; }

		/// <summary>
		/// 2nd stage of a <see cref="ReceivePipeline"/>
		/// </summary>
		IStage Disassemble { get; }

		/// <summary>
		/// 3rd stage of a <see cref="ReceivePipeline"/>
		/// </summary>
		IStage Validate { get; }

		/// <summary>
		/// 4th stage of a <see cref="ReceivePipeline"/>
		/// </summary>
		IStage ResolveParty { get; }
	}
}

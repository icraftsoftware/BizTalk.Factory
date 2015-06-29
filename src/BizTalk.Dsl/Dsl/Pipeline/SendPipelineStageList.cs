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
	public class SendPipelineStageList : StageList, ISendPipelineStageList
	{
		internal SendPipelineStageList()
		{
			PreAssemble = Add(new Stage(StageCategory.Any.Id));
			Assemble = Add(new Stage(StageCategory.AssemblingSerializer.Id));
			Encode = Add(new Stage(StageCategory.Encoder.Id));
		}

		#region ISendPipelineStageList Members

		public IStage PreAssemble { get; private set; }

		public IStage Assemble { get; private set; }

		public IStage Encode { get; private set; }

		#endregion
	}
}

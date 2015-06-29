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

using System.Collections.Generic;
using System.Linq;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	public class StageList : List<Stage>, IVisitable<IPipelineVisitor>
	{
		#region IVisitable<IPipelineVisitor> Members

		void IVisitable<IPipelineVisitor>.Accept(IPipelineVisitor visitor)
		{
			this.Cast<IVisitable<IPipelineVisitor>>().Each(stage => stage.Accept(visitor));
		}

		#endregion

		protected new Stage Add(Stage stage)
		{
			base.Add(stage);
			return stage;
		}
	}
}

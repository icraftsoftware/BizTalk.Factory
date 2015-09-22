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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Dsl.Pipeline;

namespace Be.Stateless.BizTalk.Pipelines
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "DSL-based pipeline definition.")]
	public class BatchReceive : ReceivePipeline
	{
		public BatchReceive()
		{
			Description = "BizTalk.Factory.RL1.Batch.Release.WCF-SQL.XML port's dedicated receive pipeline.";
			Version = new Version(1, 0);
			VersionDependentGuid = new Guid("c3686f9c-5320-4cce-9a71-c46f8cb6e4f3");
			Stages.Decode
				.AddComponent(new FailedMessageRoutingEnablerComponent())
				.AddComponent(new BatchTrackerComponent())
				.AddComponent(new EnvelopeBuilderComponent())
				.AddComponent(new ContextPropertyExtractorComponent());
		}
	}
}

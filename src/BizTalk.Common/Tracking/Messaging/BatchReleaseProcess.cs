#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using Be.Stateless.Extensions;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	/// <summary>
	/// Allows to track activity of a batch release process at the messaging level.
	/// </summary>
	internal class BatchReleaseProcess : Process
	{
		// internal because cannot prevent NullReferenceException should pipelineContext be null when calling .GetEventStream()
		internal BatchReleaseProcess(IPipelineContext pipelineContext, IBaseMessage message, string name)
			: base(pipelineContext, message)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentException("Process name is null or empty.", "name");
			ProcessName = name;
		}

		protected BatchReleaseProcess(string activityId, EventStream eventStream) : base(activityId, eventStream) { }

		#region Base Class Member Overrides

		public override void AddStep(MessagingStep messagingStep)
		{
			throw new NotSupportedException();
		}

		public override void TrackActivity()
		{
			BeginProcessActivity();
			base.TrackActivity();
		}

		#endregion

		public virtual void AddSteps(IEnumerable<string> messagingStepActivityIdCollection)
		{
			messagingStepActivityIdCollection.Each(
				messagingStepActivityId => {
					var step = new ProcessMessagingStep(Tracking.ActivityId.NewActivityId(), EventStream);
					step.BeginProcessMessagingStepActivity();
					step.MessagingStepActivityID = messagingStepActivityId;
					step.ProcessActivityID = ActivityId;
					step.CommitProcessMessagingStepActivity();
					step.EndProcessMessagingStepActivity();
				});
		}
	}
}

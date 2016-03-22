#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using Microsoft.BizTalk.Bam.EventObservation;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	/// <summary>
	/// Denotes an ongoing <see cref="Process"/> activity which only needs to be affiliated with new <see
	/// cref="MessagingStep"/> activities.
	/// </summary>
	/// <remarks>
	/// Notice that <see cref="ProcessReference"/> will never begin, update, nor end an <see cref="EventStream"/>
	/// activity.
	/// </remarks>
	internal sealed class ProcessReference : Process
	{
		internal ProcessReference(string activityId, EventStream eventStream) : base(activityId, eventStream) { }

		#region Base Class Member Overrides

		public override void AddStep(MessagingStep messagingStep)
		{
			// A ProcessReference denotes an ongoing *non* messaging-only process; whereas a MessagingStepReference denotes
			// the initiating message of a messaging-only flow...
			if (messagingStep is MessagingStepReference) throw new NotSupportedException();
			base.AddStep(messagingStep);
		}

		public override void TrackActivity()
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}

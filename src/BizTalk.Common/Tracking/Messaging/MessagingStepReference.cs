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
using Be.Stateless.BizTalk.Streaming;
using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	/// <summary>
	/// Denotes an already tracked <see cref="MessagingStep"/> activity that only needs to be affiliated to a <see
	/// cref="Process"/>.
	/// </summary>
	/// <remarks>
	/// Notice that <see cref="MessagingStepReference"/> will never begin, upodate, nor end an <see cref="EventStream"/>
	/// activity.
	/// </remarks>
	internal sealed class MessagingStepReference : MessagingStep
	{
		internal MessagingStepReference(string activityId, EventStream eventStream) : base(activityId, eventStream) { }

		#region Base Class Member Overrides

		public override IBaseMessage Message
		{
			get { throw new NotSupportedException(); }
		}

		internal override void TrackActivity(ActivityTrackingModes trackingModes, TrackingStream trackingStream)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}

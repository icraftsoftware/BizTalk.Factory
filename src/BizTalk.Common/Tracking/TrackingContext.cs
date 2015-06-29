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
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Simple structure that holds the BAM activity identifiers of the activities used for tracking at the process,
	/// processing step, and messaging step levels.
	/// </summary>
	[Serializable]
	public struct TrackingContext
	{
		/// <summary>
		/// The identifier of the BAM process activity.
		/// </summary>
		public string ProcessActivityId { get; internal set; }

		/// <summary>
		/// The identifier of the BAM processing step activity.
		/// </summary>
		public string ProcessingStepActivityId { get; internal set; }

		/// <summary>
		/// The identifier of the BAM messaging step activity.
		/// </summary>
		public string MessagingStepActivityId { get; internal set; }

		/// <summary>
		/// Applies this <see cref="TrackingContext"/> instance to the <paramref name="targetMessage"/>.
		/// </summary>
		/// <param name="targetMessage">
		/// Message to contain the tracking information.
		/// </param>
		public void Apply(XLANGMessage targetMessage)
		{
			if (targetMessage == null) throw new ArgumentNullException("targetMessage");
			targetMessage.SetTrackingContext(this);
		}

		/// <summary>
		/// Whether the current <see cref="TrackingContext"/> is already affiliated to a process or not.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the current <see cref="TrackingContext"/> is affiliated to a process. <c>false</c> otherwise.
		/// </returns>
		public bool HasProcessAffiliation()
		{
			return !ProcessActivityId.IsNullOrEmpty();
		}

		/// <summary>
		/// Whether none of the discrete acitivity Ids of the <see cref="TrackingContext"/> are set or not.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if none of the discrete acitivity Ids of the <see cref="TrackingContext"/> are set;
		/// <c>false</c> otherwise.
		/// </returns>
		public bool IsEmpty()
		{
			return (ProcessActivityId.IsNullOrEmpty() && ProcessingStepActivityId.IsNullOrEmpty() && MessagingStepActivityId.IsNullOrEmpty());
		}
	}
}

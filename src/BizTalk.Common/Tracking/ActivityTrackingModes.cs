#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Policy denoting to what extent a messaging activity will be tracked.
	/// </summary>
	[Flags]
	public enum ActivityTrackingModes
	{
		/// <summary>
		/// Do not track the messaging step at all.
		/// </summary>
		None = 0,

		/// <summary>
		/// Track the messaging step without any context or payload data.
		/// </summary>
		Step = 1 << 0,

		/// <summary>
		/// Track the messaging step together with its context data, but without payload data.
		/// </summary>
		Context = Step | 1 << 1,

		/// <summary>
		/// Track the messaging step together with its context and payload data.
		/// </summary>
		Body = Context | 1 << 2,

		/// <summary>
		/// Track the messaging step together with its context and payload data. Moreover the payload data will be claimed
		/// to disk and replaced by a <see cref="Schemas.Xml.Claim.CheckIn"/> payload token.
		/// </summary>
		Claim = Body | 1 << 3
	}

	public static class ActivityTrackingModesExtensions
	{
		public static bool RequiresBodyClaimChecking(this ActivityTrackingModes modes)
		{
			return (modes & ActivityTrackingModes.Claim) == ActivityTrackingModes.Claim;
		}

		public static bool RequiresBodyTracking(this ActivityTrackingModes modes)
		{
			return (modes & ActivityTrackingModes.Body) == ActivityTrackingModes.Body;
		}

		public static bool RequiresContextTracking(this ActivityTrackingModes modes)
		{
			return (modes & ActivityTrackingModes.Context) == ActivityTrackingModes.Context;
		}

		public static bool RequiresStepTracking(this ActivityTrackingModes modes)
		{
			return (modes & ActivityTrackingModes.Step) == ActivityTrackingModes.Step;
		}
	}
}

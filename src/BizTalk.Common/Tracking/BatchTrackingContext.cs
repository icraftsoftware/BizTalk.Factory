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

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Simple structure that holds the BAM activity identifiers of the various steps that were involved to build a batch
	/// envelope message, ranging from the aggregating steps to its release.
	/// </summary>
	internal class BatchTrackingContext
	{
		/// <summary>
		/// The identifiers of the BAM messaging step activities that aggregated the various parts of the batch envelope.
		/// </summary>
		internal string[] MessagingStepActivityIdList { get; set; }

		/// <summary>
		/// The identifier of the BAM process activity that triggered the release of the batch.
		/// </summary>
		internal string ProcessActivityId { get; set; }
	}
}

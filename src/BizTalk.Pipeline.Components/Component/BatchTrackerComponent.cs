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

using System.Runtime.InteropServices;
using Be.Stateless.BizTalk.MicroComponent;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Component
{
	/// <summary>
	/// <see cref="ActivityTrackerComponent"/> that specifically tracks the messaging activities involved in the release
	/// process of a batch message.
	/// </summary>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_Decoder)]
	[Guid(CLASS_ID)]
	public class BatchTrackerComponent : ActivityTrackerComponent
	{
		public BatchTrackerComponent() : base(new BatchTracker()) { }

		private const string CLASS_ID = "24ed2249-599f-4794-9777-8c25a3e8e439";
	}
}

#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	/// <summary>
	/// Send port priority.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See also:
	/// <list type="bullet">
	/// <item>
	/// How to Configure Transport Advanced Options for a Send Port,
	/// https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-transport-advanced-options-for-a-send-port.
	/// </item>
	/// </list>
	/// </para>
	/// </remarks>
	public enum Priority
	{
		Highest = 1,
		Higher = 2,
		High = 3,
		AboveNormal = 4,
		Normal = 5,
		//CorrelatingSubscription = 5,
		BelowNormal = 6,
		//ActivatingSubscription = 7,
		Low = 8,
		Lower = 9,
		Lowest = 10
	}
}

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

namespace Be.Stateless.BizTalk.Xml
{
	/// <summary>
	/// Represents the callback method that will handle XML valuedness validation events and the <see cref="T:System.Xml.Schema.ValuednessValidationCallbackArgs" />.
	/// </summary>
	/// <param name="sender">
	/// The source of the event.
	/// </param>
	/// <param name="e">
	/// The event data.
	/// </param>
	/// <remarks>
	/// Determine the type of a sender before using it in your code. You cannot assume that the sender is an instance of a particular type. The sender is also not
	/// guaranteed to not be null. Always surround your casts with failure handling logic.
	/// </remarks>
	public delegate void ValuednessValidationCallback(object sender, ValuednessValidationCallbackArgs e);
}

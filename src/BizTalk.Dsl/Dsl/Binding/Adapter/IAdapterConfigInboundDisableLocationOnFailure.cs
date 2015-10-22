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

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public interface IAdapterConfigInboundDisableLocationOnFailure
	{
		/// <summary>
		/// Specify whether to disable the receive location that fails inbound processing due to a receive pipeline
		/// failure or a routing failure.
		/// </summary>
		/// <remarks>
		/// It defaults to <c>False</c>
		/// </remarks>
		bool DisableLocationOnFailure { get; set; }
	}
}

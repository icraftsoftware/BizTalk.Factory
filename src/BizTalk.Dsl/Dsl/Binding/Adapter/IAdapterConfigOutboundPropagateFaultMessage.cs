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

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public interface IAdapterConfigOutboundPropagateFaultMessage
	{
		#region Messages Tab - Error Handling Settings

		/// <summary>
		/// Specify whether to route or suspend messages failed in outbound processing.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <list type="bullet">
		/// <item>
		/// <c>True</c> &#8212; Route the message that fails outbound processing to a subscribing application (such as
		/// another receive port or orchestration schedule).
		/// </item>
		/// <item>
		/// <c>False</c> &#8212; Suspend failed messages and generate a negative acknowledgment (NACK).
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// This property is valid only for solicit-response ports.
		/// </para>
		/// <para>
		/// It defaults to <c>True</c>.
		/// </para>
		/// </remarks>
		bool PropagateFaultMessage { get; set; }

		#endregion
	}
}

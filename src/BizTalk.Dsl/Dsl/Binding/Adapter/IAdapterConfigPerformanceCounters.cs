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
	public interface IAdapterConfigPerformanceCounters
	{
		/// <summary>
		/// Specifies whether to enable the WCF LOB Adapter SDK performance counters and the LOB Latency performance
		/// counter. The LOB Latency performance counter measures the total time spent by the adapter in making calls to
		/// the system.
		/// </summary>
		/// <remarks>
		/// <para>
		/// EnablePerformanceCounters is a static property within an application domain for the WCF LOB Adapter SDK
		/// performance counters, but it is an instance property for the adapter's LOB Latency performance counter. This
		/// means that changing EnablePerformanceCounters for a binding instance in an application domain will:
		/// <list type="bullet">
		/// <item>
		/// enable or disable the WCF LOB Adapter SDK performance counters for all objects created from all binding
		/// instances within the same app domain.
		/// </item>
		/// <item>
		/// enable or disable the adapter's LOB Latency performance counter only for objects created from that binding
		/// instance after the change is made.
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// It defaults to <c>False</c>.
		/// </para>
		/// </remarks>
		bool EnablePerformanceCounters { get; set; }
	}
}

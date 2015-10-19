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

using Microsoft.BizTalk.ExplorerOM;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	public class ServiceStateFixture
	{
		[Test]
		public void CastToOrchestrationStatus()
		{
			Assert.That((OrchestrationStatus) ServiceState.Unenlisted, Is.EqualTo(OrchestrationStatus.Unenlisted));
			Assert.That((OrchestrationStatus) ServiceState.Enlisted, Is.EqualTo(OrchestrationStatus.Enlisted));
			Assert.That((OrchestrationStatus) ServiceState.Stopped, Is.EqualTo(OrchestrationStatus.Enlisted));
			Assert.That((OrchestrationStatus) ServiceState.Started, Is.EqualTo(OrchestrationStatus.Started));
		}

		[Test]
		public void CastToPortStatus()
		{
			Assert.That((PortStatus) ServiceState.Unenlisted, Is.EqualTo(PortStatus.Bound));
			Assert.That((PortStatus) ServiceState.Enlisted, Is.EqualTo(PortStatus.Stopped));
			Assert.That((PortStatus) ServiceState.Stopped, Is.EqualTo(PortStatus.Stopped));
			Assert.That((PortStatus) ServiceState.Started, Is.EqualTo(PortStatus.Started));
		}

		[Test]
		public void EnlistedIsStopped()
		{
			Assert.That(ServiceState.Enlisted, Is.EqualTo(ServiceState.Stopped));
		}
	}
}

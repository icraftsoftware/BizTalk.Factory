#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Runtime.InteropServices;
using Microsoft.BizTalk.ExplorerOM;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Management
{
	[TestFixture]
	public class OrchestrationFixture
	{
		#region Setup/Teardown

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_orchestrationType = typeof(Microsoft.BizTalk.Edi.BatchingOrchestration._MODULE_PROXY_)
				.Assembly
				.GetType("Microsoft.BizTalk.Edi.BatchSuspendOrchestration.BatchElementSuspendService", true);
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
		}

		#endregion

		[Test]
		public void EnlistEnlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Assert.That(() => orchestration.Enlist(), Throws.TypeOf<COMException>());
		}

		[Test]
		public void EnlistStartedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Assert.That(() => orchestration.Enlist(), Throws.TypeOf<COMException>());
		}

		[Test]
		public void EnlistUnenlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Assert.That(() => orchestration.Enlist(), Throws.Nothing);
			Assert.That(orchestration.Status, Is.EqualTo(OrchestrationStatus.Enlisted));
		}

		[Test]
		public void StartEnlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Assert.That(() => orchestration.Start(), Throws.Nothing);
			Assert.That(orchestration.Status, Is.EqualTo(OrchestrationStatus.Started));
		}

		[Test]
		public void StartStartedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Assert.That(() => orchestration.Start(), Throws.TypeOf<COMException>());
		}

		[Test]
		public void StartUnenlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Assert.That(() => orchestration.Start(), Throws.TypeOf<COMException>());
		}

		[Test]
		public void StopEnlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Assert.That(() => orchestration.Stop(), Throws.TypeOf<COMException>());
		}

		[Test]
		public void StopStartedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Assert.That(() => orchestration.Stop(), Throws.Nothing);
			Assert.That(orchestration.Status, Is.EqualTo(OrchestrationStatus.Enlisted));
		}

		[Test]
		public void StopUnenlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Assert.That(() => orchestration.Stop(), Throws.TypeOf<COMException>());
		}

		[Test]
		public void ThrowsIfNotBtxServiceDerivedType()
		{
			Assert.That(
				() => new Orchestration(typeof(string)),
				Throws.ArgumentException
					.With.Message.StartsWith("Type 'System.String' is not an BTXService-derived orchestration type"));
		}

		[Test]
		public void UnenlistEnlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureNotUnenlisted();
			Assert.That(() => orchestration.Unenlist(), Throws.Nothing);
			Assert.That(orchestration.Status, Is.EqualTo(OrchestrationStatus.Unenlisted));
		}

		[Test]
		public void UnenlistStartedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureStarted();
			Assert.That(() => orchestration.Unenlist(), Throws.Nothing);
			Assert.That(orchestration.Status, Is.EqualTo(OrchestrationStatus.Unenlisted));
		}

		[Test]
		public void UnenlistUnenlistedOrchestration()
		{
			var orchestration = new Orchestration(_orchestrationType);
			orchestration.EnsureUnenlisted();
			Assert.That(() => orchestration.Unenlist(), Throws.TypeOf<COMException>());
		}

		private Type _orchestrationType;
	}
}

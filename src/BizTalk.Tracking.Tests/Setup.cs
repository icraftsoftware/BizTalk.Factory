#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using System.Data.Entity;
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.BizTalk.Tracking;
using NUnit.Framework;

namespace Be.Stateless.BizTalk
{
	[SetUpFixture]
	public class Setup
	{
		[SetUp]
		public void BeforeAnyTests()
		{
			// Microsoft.BizTalk.ExplorerOM requires x86 and R# Test Runner follows project settings' platform target
			Assert.That(Environment.Is64BitProcess, Is.False, "If using ReSharper's Unit Test Runner, ensure project settings' platform target is x86.");
			// database is deployed along with the BAM activity model and never needs to be initialized by EF
			Database.SetInitializer<ActivityContext>(null);

			// TODO Assert.That(Process, Is.Elevated()); or BizTalk Artifact will not be deployed
		}

		[TearDown]
		public void AfterAnyTests()
		{
			TrackingDatabase.ActivityContext.Dispose();
		}
	}
}

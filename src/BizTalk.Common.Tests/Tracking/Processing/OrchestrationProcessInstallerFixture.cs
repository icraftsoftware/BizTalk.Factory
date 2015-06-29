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

using System;
using System.Collections.Generic;
using Microsoft.BizTalk.XLANGs.BTXEngine;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking.Processing
{
	[TestFixture]
	public class OrchestrationProcessInstallerFixture : OrchestrationProcessInstaller
	{
		[Test]
		public void DiscoverProcessNames()
		{
			var expected = new[] {
				typeof(Orchestration1).Namespace, typeof(Orchestration2).Namespace
			};

			var processNames = ProcessNames;

			Assert.That(processNames, Is.EquivalentTo(expected));
		}

		[Test]
		public void DiscoverProcessNamesWhenSomeOrchestrationIsExcluded()
		{
			var expected = new[] {
				typeof(Orchestration2).Namespace
			};

			ExcludedTypes = new List<Type> { typeof(Orchestration1) };

			var processNames = ProcessNames;

			Assert.That(processNames, Is.EquivalentTo(expected));
		}
	}

	public abstract class Orchestration1 : BTXService
	{
		protected Orchestration1() : base(0, Guid.NewGuid(), null, "stuff") { }
	}

	public abstract class Orchestration2 : BTXService
	{
		protected Orchestration2() : base(0, Guid.NewGuid(), null, "stuff") { }
	}
}

#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.Linq;
using Microsoft.BizTalk.XLANGs.BTXEngine;
using Microsoft.XLANGs.BaseTypes;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking.Processing
{
	[TestFixture]
	public class OrchestrationProcessInstallerFixture : OrchestrationProcessInstaller
	{
		[Test]
		public void DiscoverProcessNamesForAllOrchestrations()
		{
			var expected = new[] {
				typeof(Orchestration1).Namespace,
				typeof(Orchestration2).Namespace
			};

			_excludedTypes = () => base.ExcludedTypes;

			Assert.That(GetProcessNames(), Is.EquivalentTo(expected));
		}

		[Test]
		public void DiscoverProcessNamesForSomeOrchestrations()
		{
			var expected = new[] {
				typeof(Orchestration2).Namespace
			};

			_excludedTypes = () => base.ExcludedTypes.Concat(new List<Type> { typeof(Orchestration1) });

			Assert.That(GetProcessNames(), Is.EquivalentTo(expected));
		}

		[Test]
		public void ExcludedTypesByDefault()
		{
			var expected = new[] {
				typeof(Step),
				typeof(SubProcess)
			};

			_excludedTypes = () => base.ExcludedTypes;

			Assert.That(ExcludedTypes, Is.EquivalentTo(expected));
		}

		protected override IEnumerable<Type> ExcludedTypes
		{
			get { return _excludedTypes(); }
		}

		private Func<IEnumerable<Type>> _excludedTypes;
	}

	[StaticSubscription(0, "CompensationCommandReceivePort", "Receive", -1, -1, true)]
	public abstract class Orchestration1 : BTXService
	{
		protected Orchestration1() : base(0, Guid.NewGuid(), null, "stuff") { }
	}

	[StaticSubscription(0, "CompensationCommandReceivePort", "Receive", -1, -1, true)]
	public abstract class Orchestration2 : BTXService
	{
		protected Orchestration2() : base(0, Guid.NewGuid(), null, "stuff") { }
	}

	public abstract class Step : BTXService
	{
		protected Step() : base(0, Guid.NewGuid(), null, "stuff") { }
	}

	public abstract class SubProcess : BTXService
	{
		protected SubProcess() : base(0, Guid.NewGuid(), null, "stuff") { }
	}
}

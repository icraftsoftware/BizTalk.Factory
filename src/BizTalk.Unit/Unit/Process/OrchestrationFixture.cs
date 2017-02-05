﻿#region Copyright & License

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
using System.Collections.Generic;
using System.Linq;
using Be.Stateless.BizTalk.Management;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.XLANGs.BTXEngine;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Process
{
	public abstract class OrchestrationFixture<T> : CompletedProcessFixture
		where T : BTXService
	{
		protected virtual IEnumerable<Type> DependantOrchestrationTypes
		{
			get { return Enumerable.Empty<Type>(); }
		}

		[OneTimeSetUp]
		public void BizTalkFactoryOrchestrationFixtureOneTimeSetUp()
		{
			DependantOrchestrationTypes.Each(
				ot => { new Orchestration(ot).EnsureStarted(); }
			);
			new Orchestration<T>().EnsureStarted();
		}

		[OneTimeTearDown]
		public void BizTalkFactoryOrchestrationFixtureOneTimeTearDown()
		{
			new Orchestration<T>().EnsureUnenlisted();
			DependantOrchestrationTypes.Each(
				ot => { new Orchestration(ot).EnsureUnenlisted(); }
			);
		}
	}
}
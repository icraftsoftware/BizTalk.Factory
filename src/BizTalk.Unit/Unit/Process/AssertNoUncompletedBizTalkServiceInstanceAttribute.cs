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

using System;
using Be.Stateless.BizTalk.Operations.Extensions;
using Be.Stateless.BizTalk.Unit.Constraints;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Be.Stateless.BizTalk.Unit.Process
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AssertNoUncompletedBizTalkServiceInstanceAttribute : TestActionAttribute
	{
		#region Base Class Member Overrides

		public override void AfterTest(ITest test)
		{
			Assert.That(BizTalkOperationsExtensions.GetRunningOrSuspendedServiceInstances(), Has.No.UncompletedInstances());
		}

		public override ActionTargets Targets
		{
			get { return ActionTargets.Test; }
		}

		#endregion
	}
}

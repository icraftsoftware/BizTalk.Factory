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

using System.Collections.Generic;
using Be.Stateless.BizTalk.Operations.Extensions;
using Microsoft.BizTalk.Operations;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Constraints
{
	[TestFixture]
	public class UncompletedInstanceConstraintFixture
	{
		[Test]
		public void HasNoUncompletedInstances()
		{
			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void NullHasNoUncompletedInstances()
		{
			Assert.That((MessageBoxServiceInstance[]) null, Has.No.UncompletedInstances());
		}

		private IEnumerable<MessageBoxServiceInstance> BizTalkServiceInstances
		{
			get
			{
				using (var bizTalkOperations = new BizTalkOperations())
				{
					return bizTalkOperations.GetRunningOrSuspendedServiceInstances();
				}
			}
		}
	}
}

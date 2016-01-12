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

using Be.Stateless.BizTalk.Factory.Areas;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Monitoring.Extensions
{
	[TestFixture]
	public class StringExtensionsFixture
	{
		// todo: test source
		[Test]
		[TestCaseSource("Cases")]
		public void ToFriendlyProcessName(string processName, string expected)
		{
			Assert.That(processName.ToFriendlyProcessName(), Is.EqualTo(expected));
		}

		static readonly object[] Cases = {
			new object[] {Default.Processes.Failed, "Factory/Failed"},
			new object[] {Default.Processes.Unidentified, "Factory/Unidentified"},
			new object[] {Batch.Processes.Aggregate, "Factory/Batch/Aggregate"},
			new object[] {Batch.Processes.Release, "Factory/Batch/Release"},
			new object[] {Claim.Processes.Check, "Factory/Claim/Check"},
			new object[] {"Be.Stateless.Accounting.Orchestrations.Invoicing.UpdateMasterData", "Accounting/Invoicing/UpdateMasterData"},
			new object[] {"Be.Stateless.Accounting.Orchestrations.UpdateMasterData", "Accounting/UpdateMasterData"}
		};
	}
}

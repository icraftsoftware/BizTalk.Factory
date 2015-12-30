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

using NUnit.Framework;

namespace Be.Stateless.BizTalk.Monitoring.Extensions
{
	[TestFixture]
	public class StringExtensionsFixture
	{
		[Test]
		public void ToFriendlyProcessName()
		{
			Assert.That(Factory.GlobalArea.Processes.Failed.ToFriendlyProcessName(), Is.EqualTo("Factory/Failed"));
			Assert.That(Factory.GlobalArea.Processes.Unidentified.ToFriendlyProcessName(), Is.EqualTo("Factory/Unidentified"));
			Assert.That(Factory.ServiceArea.Batch.Processes.Aggregate.ToFriendlyProcessName(), Is.EqualTo("Factory/Batch/Aggregate"));
			Assert.That(Factory.ServiceArea.Batch.Processes.Release.ToFriendlyProcessName(), Is.EqualTo("Factory/Batch/Release"));
			Assert.That(Factory.ServiceArea.Claim.Processes.Check.ToFriendlyProcessName(), Is.EqualTo("Factory/Claim/Check"));
		}
	}
}

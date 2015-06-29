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

using NUnit.Framework;

namespace Be.Stateless.BizTalk.Monitoring.Extensions
{
	[TestFixture]
	public class StringExtensionsFixture
	{
		[Test]
		public void ToFriendlyProcessName()
		{
			Assert.That("Be.Stateless.BizTalk.Factory.DefaultProcesses.Failed".ToFriendlyProcessName(), Is.EqualTo("Factory/Failed"));
			Assert.That("Be.Stateless.BizTalk.Factory.DefaultProcesses.Unidentified".ToFriendlyProcessName(), Is.EqualTo("Factory/Unidentified"));
			Assert.That("Be.Stateless.BizTalk.Factory.Processes.Batching.Aggregator".ToFriendlyProcessName(), Is.EqualTo("Factory/Batching/Aggregator"));
			Assert.That("Be.Stateless.BizTalk.Factory.Processes.Batching.Releaser".ToFriendlyProcessName(), Is.EqualTo("Factory/Batching/Releaser"));
		}
	}
}

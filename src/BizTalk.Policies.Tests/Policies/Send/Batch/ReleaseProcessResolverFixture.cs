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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Unit.RuleEngine;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Policies.Send.Batch
{
	[TestFixture]
	public class ReleaseProcessResolverFixture : PolicyFixture<ReleaseProcessResolver>
	{
		[Test]
		public void DoNotWriteProcessNameInContext()
		{
			Facts.Assert(Context.Property(TrackingProperties.ProcessName).WithValue("some-process-name"));
			ExecutePolicy();
			Facts.Verify(Context.Property(TrackingProperties.ProcessName).WithAnyValue().HasNotBeenWritten());
		}

		[Test]
		public void WriteProcessNameInContext()
		{
			ExecutePolicy();
			Facts.Verify(Context.Property(TrackingProperties.ProcessName).WithValue(Factory.Services.Batch.Processes.Release).HasBeenWritten());
		}
	}
}

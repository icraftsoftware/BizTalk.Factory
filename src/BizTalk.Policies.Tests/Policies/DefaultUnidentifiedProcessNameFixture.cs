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

using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.RuleEngine;
using Be.Stateless.BizTalk.Factory;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Unit.RuleEngine;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Policies
{
	[TestFixture]
	public class DefaultUnidentifiedProcessNameFixture : PolicyFixture<DefaultUnidentifiedProcessNameFixture.DefaultUnidentifiedProcess>
	{
		#region Nested Type: DefaultUnidentifiedProcess

		public class DefaultUnidentifiedProcess : RuleSet
		{
			public DefaultUnidentifiedProcess()
			{
				Rules.Add(
					Rule("DefaultUnidentifiedProcessName")
						.If(() => true)
						.Then(() => Context.Write(TrackingProperties.ProcessName, DefaultProcesses.Unidentified))
					);
			}
		}

		#endregion

#pragma warning disable 1584,1711,1572,1581,1580
		/// <summary>
		/// Sanity check to ensure that both <see cref="ActivityTrackerComponent.UNIDENTIFIED_PROCESS_NAME"/> and <see
		/// cref="Be.Stateless.BizTalk.Monitoring.Model.Process.UNIDENTIFIED_PROCESS_NAME"/> will not drift away from <see 
		/// cref="DefaultProcesses.Unidentified"/>'s computed value. There will never be any UnidentifiedProcessResolver
		/// policy.
		/// </summary>
#pragma warning restore 1584,1711,1572,1581,1580
		[Test]
		public void DefaultProcessNames()
		{
			// necessary to have PolicyFixture base class initialize DefaultProcesses.Unidentified
			ExecutePolicy();
			Assert.That(DefaultProcesses.Unidentified, Is.EqualTo(TrackingResolver.UNIDENTIFIED_PROCESS_NAME));
			Assert.That(DefaultProcesses.Unidentified, Is.EqualTo(Monitoring.Model.Process.UNIDENTIFIED_PROCESS_NAME));
		}
	}
}

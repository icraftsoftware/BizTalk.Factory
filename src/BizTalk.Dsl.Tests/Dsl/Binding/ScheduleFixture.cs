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

using System;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	public class ScheduleFixture
	{
		[Test]
		public void NoneEqualsBindingDefaultServiceWindow()
		{
			var rl = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocation();

			Assert.That(Schedule.None.StartDate, Is.EqualTo(rl.StartDate));
			Assert.That(Schedule.None.StartDateEnabled, Is.False);
			Assert.That(Schedule.None.StopDate, Is.EqualTo(rl.EndDate));
			Assert.That(Schedule.None.StopDateEnabled, Is.False);

			Assert.That(Schedule.None.ServiceWindow.Enabled, Is.EqualTo(ServiceWindow.None.Enabled));
			Assert.That(Schedule.None.ServiceWindow.StartTime, Is.EqualTo(ServiceWindow.None.StartTime));
			Assert.That(Schedule.None.ServiceWindow.StopTime, Is.EqualTo(ServiceWindow.None.StopTime));
		}

		[Test]
		public void StartAndStopDateEnabled()
		{
			var s = new Schedule { StartDate = new DateTime(2015, 2, 13), StopDate = new DateTime(2015, 2, 20) };

			Assert.That(s.StartDateEnabled, Is.True);
			Assert.That(s.StopDateEnabled, Is.True);

			Assert.That(s.ServiceWindow.Enabled, Is.EqualTo(ServiceWindow.None.Enabled));
			Assert.That(s.ServiceWindow.StartTime, Is.EqualTo(ServiceWindow.None.StartTime));
			Assert.That(s.ServiceWindow.StopTime, Is.EqualTo(ServiceWindow.None.StopTime));
		}

		[Test]
		public void StartDateEnabled()
		{
			var s = new Schedule { StartDate = new DateTime(2015, 2, 13) };

			Assert.That(s.StartDateEnabled, Is.True);
			Assert.That(s.StopDate, Is.EqualTo(Schedule.None.StopDate));
			Assert.That(s.StopDateEnabled, Is.False);

			Assert.That(s.ServiceWindow.Enabled, Is.EqualTo(ServiceWindow.None.Enabled));
			Assert.That(s.ServiceWindow.StartTime, Is.EqualTo(ServiceWindow.None.StartTime));
			Assert.That(s.ServiceWindow.StopTime, Is.EqualTo(ServiceWindow.None.StopTime));
		}

		[Test]
		public void StopDateEnabled()
		{
			var s = new Schedule { StopDate = new DateTime(2015, 2, 20) };

			Assert.That(s.StartDate, Is.EqualTo(Schedule.None.StartDate));
			Assert.That(s.StartDateEnabled, Is.False);
			Assert.That(s.StopDateEnabled, Is.True);

			Assert.That(s.ServiceWindow.Enabled, Is.EqualTo(ServiceWindow.None.Enabled));
			Assert.That(s.ServiceWindow.StartTime, Is.EqualTo(ServiceWindow.None.StartTime));
			Assert.That(s.ServiceWindow.StopTime, Is.EqualTo(ServiceWindow.None.StopTime));
		}
	}
}

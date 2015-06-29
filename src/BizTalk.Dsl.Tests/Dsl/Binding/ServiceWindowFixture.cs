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
	public class ServiceWindowFixture
	{
		[Test]
		public void DateOfCustomStartAndSopTimeIsEqualsToDateOfDefaultStartAndStopTime()
		{
			var sw = new ServiceWindow { StartTime = new Time(8, 0), StopTime = new Time(20, 0) };

			Assert.That(((DateTime) sw.StartTime).Date, Is.EqualTo(((DateTime) ServiceWindow.None.StartTime).Date));
			Assert.That(((DateTime) sw.StartTime).TimeOfDay, Is.Not.EqualTo(((DateTime) ServiceWindow.None.StartTime).TimeOfDay));
			Assert.That(((DateTime) sw.StopTime).Date, Is.EqualTo(((DateTime) ServiceWindow.None.StopTime).Date));
			Assert.That(((DateTime) sw.StopTime).TimeOfDay, Is.Not.EqualTo(((DateTime) ServiceWindow.None.StopTime).TimeOfDay));
		}

		[Test]
		public void EnabledOnStartAndStopTime()
		{
			var sw = new ServiceWindow { StartTime = new Time(8, 0), StopTime = new Time(20, 0) };

			Assert.That(sw.Enabled, Is.True);
		}

		[Test]
		public void EnabledOnStartTime()
		{
			var sw = new ServiceWindow { StartTime = new Time(8, 0) };

			Assert.That(sw.Enabled, Is.True);
		}

		[Test]
		public void EnabledOnStopTime()
		{
			var sw = new ServiceWindow { StopTime = new Time(20, 0) };

			Assert.That(sw.Enabled, Is.True);
		}

		[Test]
		public void NoneEqualsBindingDefaultServiceWindow()
		{
			var rl = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocation();

			Assert.That(((DateTime) ServiceWindow.None.StartTime), Is.EqualTo(rl.FromTime));
			Assert.That(((DateTime) ServiceWindow.None.StopTime), Is.EqualTo(rl.ToTime));
			Assert.That(ServiceWindow.None.Enabled, Is.False);
		}
	}
}

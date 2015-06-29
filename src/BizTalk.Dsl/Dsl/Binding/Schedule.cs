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

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class Schedule
	{
		static Schedule()
		{
			var rl = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocation();
			_none = new Schedule(rl.StartDate, rl.EndDate, ServiceWindow.None);
		}

		public static Schedule None
		{
			get { return _none; }
		}

		public Schedule()
		{
			StartDate = None.StartDate;
			StopDate = None.StopDate;
			ServiceWindow = ServiceWindow.None;
		}

		private Schedule(DateTime startDate, DateTime stopDate, ServiceWindow serviceWindow)
		{
			StartDate = startDate;
			StopDate = stopDate;
			ServiceWindow = serviceWindow;
		}

		/// <summary>
		/// <see cref="ServiceWindow"/> restricts the <see cref="ReceiveLocationBase{TNamingConvention}"/> to work during
		/// certain hours of the day.
		/// </summary>
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public ServiceWindow ServiceWindow { get; set; }

		public DateTime StartDate { get; set; }

		public bool StartDateEnabled
		{
			get { return StartDate != None.StartDate; }
		}

		public DateTime StopDate { get; set; }

		public bool StopDateEnabled
		{
			get { return StopDate != None.StopDate; }
		}

		private static readonly Schedule _none;
	}
}

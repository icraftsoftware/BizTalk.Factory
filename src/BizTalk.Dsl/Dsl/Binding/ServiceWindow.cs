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
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class ServiceWindow
	{
		static ServiceWindow()
		{
			var ti = new TransportInfo();
			_none = new ServiceWindow(ti.FromTime, ti.ToTime);
		}

		public static ServiceWindow None
		{
			get { return _none; }
		}

		public ServiceWindow()
		{
			StartTime = None.StartTime;
			StopTime = None.StopTime;
		}

		private ServiceWindow(DateTime startTime, DateTime stopTime)
		{
			_startTime = startTime;
			_stopTime = stopTime;
		}

		public bool Enabled
		{
			get { return _startTime != None._startTime || _stopTime != None._stopTime; }
		}

		public Time StartTime
		{
			get { return _startTime; }
			set { _startTime = BuildDateTime(value); }
		}

		public Time StopTime
		{
			get { return _stopTime; }
			set { _stopTime = BuildDateTime(value); }
		}

		private DateTime BuildDateTime(Time time)
		{
			var date = _none._startTime.Date;
			var timeOfDay = ((DateTime) time).TimeOfDay;
			return new DateTime(date.Year, date.Month, date.Day, timeOfDay.Hours, timeOfDay.Minutes, timeOfDay.Seconds);
		}

		private static readonly ServiceWindow _none;
		private DateTime _startTime;
		private DateTime _stopTime;
	}
}

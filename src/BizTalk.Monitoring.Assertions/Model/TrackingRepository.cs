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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace Be.Stateless.BizTalk.Monitoring.Model
{
	public static class TrackingRepository
	{
		static TrackingRepository()
		{
			_context = new TrackingActivityContext(ConfigurationManager.ConnectionStrings["TrackingActivityModel"].ConnectionString) {
				ObjectTrackingEnabled = true,
				DeferredLoadingEnabled = true
			};
			DefaultTimeout = TimeSpan.FromSeconds(30);
		}

		public static TimeSpan DefaultTimeout { get; set; }

		internal static TrackingActivityContext TrackingActivityContext
		{
			get { return _context; }
		}

		public static IEnumerable<Process> Processes
		{
			get { return TrackingActivityContext.Processes; }
		}

		// TODO [Obsolete]
		public static IEnumerable<Process> Where(this IEnumerable<Process> processes, Func<Process, bool> predicate)
		{
			// ReSharper disable PossibleMultipleEnumeration
			var previousCount = 0;
			var absoluteExpirationTimeout = DateTime.Now.Add(DefaultTimeout);
			var enumerable = Enumerable.Where(processes, predicate);
			while (DateTime.Now < absoluteExpirationTimeout)
			{
				var count = enumerable.Count();
				// a positive steady count indicates TDDS have moved all steps into BAM
				if (count > 0 && count == previousCount) return enumerable.ToArray();
				previousCount = count;
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
			return enumerable.ToArray();
			// ReSharper restore PossibleMultipleEnumeration
		}

		// TODO [Obsolete]
		public static Process SingleProcess(Func<Process, bool> predicate)
		{
			return SingleProcess(predicate, DefaultTimeout);
		}

		// TODO [Obsolete]
		public static Process SingleProcess(Func<Process, bool> predicate, TimeSpan timeout)
		{
			var absoluteExpirationTimeout = DateTime.Now.Add(timeout);
			while (DateTime.Now < absoluteExpirationTimeout)
			{
				var process = TrackingActivityContext.Processes.SingleOrDefault(predicate);
				if (process != null) return process;
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
			return TrackingActivityContext.Processes.Single(predicate);
		}

		private static readonly TrackingActivityContext _context;
	}
}

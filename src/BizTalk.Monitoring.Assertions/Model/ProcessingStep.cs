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
using System.Linq;
using System.Threading;

namespace Be.Stateless.BizTalk.Monitoring.Model
{
	public partial class ProcessingStep
	{
		public Process Process
		{
			get
			{
				var absoluteExpirationTimeout = DateTime.Now.Add(TrackingRepository.DefaultTimeout);
				var processes = TrackingRepository.TrackingActivityContext.Processes
					.Where(p => p.ActivityID == ProcessActivityID);
				// ReSharper disable PossibleMultipleEnumeration
				while (DateTime.Now < absoluteExpirationTimeout)
				{
					var process = processes.SingleOrDefault();
					if (process != null) return process;
					Thread.Sleep(TimeSpan.FromSeconds(1));
				}
				return processes.Single();
				// ReSharper restore PossibleMultipleEnumeration
			}
		}
	}
}

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
using System.Linq;
using System.Threading;

namespace Be.Stateless.BizTalk.Monitoring.Model
{
	public partial class Process
	{
		public IEnumerable<MessagingStep> MessagingSteps
		{
			get { return ProcessMessagingSteps.Select(pms => pms.MessagingStep); }
		}

		// TODO [Obsolete]
		public MessagingStep SingleMessagingStep(Func<MessagingStep, bool> predicate)
		{
			return SingleMessagingStep(predicate, TrackingRepository.DefaultTimeout);
		}

		// TODO [Obsolete]
		public MessagingStep SingleMessagingStep(Func<MessagingStep, bool> predicate, TimeSpan timeout)
		{
			var absoluteExpirationTimeout = DateTime.Now.Add(timeout);
			var steps = TrackingRepository.TrackingActivityContext.ProcessMessagingSteps
				.Where(pms => pms.ProcessActivityID == ActivityID)
				.Select(pms => pms.MessagingStep);
			while (DateTime.Now < absoluteExpirationTimeout)
			{
				var step = steps.SingleOrDefault(predicate);
				if (step != null) return step;
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
			return steps.Single(predicate);
		}

		// TODO [Obsolete]
		public ProcessingStep SingleProcessingStep(Func<ProcessingStep, bool> predicate)
		{
			return SingleProcessingStep(predicate, TrackingRepository.DefaultTimeout);
		}

		// TODO [Obsolete]
		public ProcessingStep SingleProcessingStep(Func<ProcessingStep, bool> predicate, TimeSpan timeout)
		{
			var absoluteExpirationTimeout = DateTime.Now.Add(timeout);
			var steps = TrackingRepository.TrackingActivityContext.ProcessingSteps
				.Where(pms => pms.ProcessActivityID == ActivityID);
			while (DateTime.Now < absoluteExpirationTimeout)
			{
				var step = steps.SingleOrDefault(predicate);
				if (step != null) return step;
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
			return steps.Single(predicate);
		}
	}
}

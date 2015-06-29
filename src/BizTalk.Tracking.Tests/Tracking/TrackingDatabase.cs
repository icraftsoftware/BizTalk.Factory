#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using System.Data.Entity;
using System.Linq;
using System.Threading;
using Be.Stateless.BizTalk.Monitoring.Model;

namespace Be.Stateless.BizTalk.Tracking
{
	internal static class TrackingDatabase
	{
		public static ActivityContext ActivityContext
		{
			get { return _activityContext; }
		}

		public static Process SingleProcess(Func<Process, bool> predicate, bool loadStepsLazily = false)
		{
			Process process = null;
			var attempts = 0;
			IQueryable<Process> processes = ActivityContext.Processes;
			if (!loadStepsLazily)
			{
				processes
					.Include(p => p.ProcessingSteps)
					.Include(p => p.MessagingSteps)
					.Include(p => p.MessagingSteps.Select(ms => ms.Context))
					.Include(p => p.MessagingSteps.Select(ms => ms.Message));
			}
			while (attempts++ < MAX_READ_ATTEMPTS)
			{
				process = processes.SingleOrDefault(predicate);
				if (process != null) return process;
				WaitForTdds();
			}
			return processes.Single(predicate);
		}

		public static MessagingStep SingleMessagingStep(Func<MessagingStep, bool> predicate, bool loadProcessLazily = false)
		{
			MessagingStep messagingStep = null;
			var attempts = 0;
			IQueryable<MessagingStep> messagingSteps = ActivityContext.MessagingSteps
				.Include(ms => ms.Message)
				.Include(ms => ms.Context);
			if (!loadProcessLazily)
			{
				messagingSteps
					.Include(ms => ms.Processes)
					.Include(ms => ms.Processes.Select(p => p.ProcessingSteps))
					.Include(ms => ms.Processes.Select(p => p.MessagingSteps))
					.Include(ms => ms.Processes.Select(p => p.MessagingSteps.Select(sms => sms.Context)))
					.Include(ms => ms.Processes.Select(p => p.MessagingSteps.Select(sms => sms.Message)));
			}
			while (attempts++ < MAX_READ_ATTEMPTS)
			{
				messagingStep = messagingSteps.SingleOrDefault(predicate);
				if (messagingStep != null) return messagingStep;
				WaitForTdds();
			}
			return messagingSteps.Single(predicate);
		}

		/// <summary>
		/// Give TDDS some time to move BAM data from BizTalkMsgBoxDb to BAMPrimaryImport database.
		/// </summary>
		private static void WaitForTdds()
		{
			Thread.Sleep(_tddsWaitTime);
		}

		private const int MAX_READ_ATTEMPTS = 7;

		private static readonly ActivityContext _activityContext = new ActivityContext();
		private static readonly TimeSpan _tddsWaitTime = TimeSpan.FromSeconds(2);
	}
}

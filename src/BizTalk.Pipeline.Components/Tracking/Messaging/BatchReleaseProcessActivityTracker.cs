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
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
	internal class BatchReleaseProcessActivityTracker
	{
		internal static BatchReleaseProcessActivityTracker Create(IPipelineContext pipelineContext, IBaseMessage message)
		{
			return Factory(pipelineContext, message);
		}

		#region Mock's Factory Hook Point

		internal static Func<IPipelineContext, IBaseMessage, BatchReleaseProcessActivityTracker> Factory
		{
			get { return _factory; }
			set { _factory = value; }
		}

		#endregion

		// ReSharper disable once MemberCanBePrivate.Global
		// ctor is protected for mocking purposes
		protected BatchReleaseProcessActivityTracker(IPipelineContext pipelineContext, IBaseMessage message)
		{
			_pipelineContext = pipelineContext;
			_message = message;
		}

		internal virtual void TrackActivity(BatchTrackingContext batchTrackingContext)
		{
			var shouldProceed = batchTrackingContext != null
				&& batchTrackingContext.MessagingStepActivityIdList != null
				&& batchTrackingContext.MessagingStepActivityIdList.Length > 0;

			if (shouldProceed)
			{
				if (_logger.IsInfoEnabled) _logger.Debug("Associating the batch being released with its parts.");
				var activityFactory = (IBatchProcessActivityFactory) _pipelineContext.ActivityFactory();
				var process = batchTrackingContext.ProcessActivityId.IsNullOrEmpty()
					? activityFactory.CreateProcess(_message, PROCESS_NAME)
					: activityFactory.FindProcess(batchTrackingContext.ProcessActivityId);

				process.TrackActivity();
				process.AddSteps(
					batchTrackingContext.MessagingStepActivityIdList
						.Concat(new[] { _message.GetProperty(TrackingProperties.MessagingStepActivityId) }));
			}
			else
			{
				if (_logger.IsInfoEnabled) _logger.Debug("The batch being released cannot be associated with its parts because their ActivityIDs have not been captured.");
			}
		}

		internal const string PROCESS_NAME = "Be.Stateless.BizTalk.Factory.Processes.Batch.Release";

		private static Func<IPipelineContext, IBaseMessage, BatchReleaseProcessActivityTracker> _factory =
			(pipelineContext, message) => new BatchReleaseProcessActivityTracker(pipelineContext, message);

		private static readonly ILog _logger = LogManager.GetLogger(typeof(BatchReleaseProcessActivityTracker));

		private readonly IBaseMessage _message;
		private readonly IPipelineContext _pipelineContext;
	}
}

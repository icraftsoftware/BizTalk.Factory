#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.ContextProperties.Extensions;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	/// <summary>
	/// Allows to track activity of a process at the messaging level.
	/// </summary>
	[SuppressMessage("ReSharper", "LocalizableElement")]
	public partial class Process
	{
		protected Process(IPipelineContext pipelineContext, IBaseMessage message)
			: this(Tracking.ActivityId.NewActivityId(), pipelineContext.GetEventStream())
		{
			if (message == null) throw new ArgumentNullException("message");
			_message = message;
		}

		// internal because cannot prevent NullReferenceException should pipelineContext be null when calling .GetEventStream()
		internal Process(IPipelineContext pipelineContext, IBaseMessage message, string name)
			: this(Tracking.ActivityId.NewActivityId(), pipelineContext.GetEventStream())
		{
			if (message == null) throw new ArgumentNullException("message");
			if (name.IsNullOrEmpty()) throw new ArgumentException("Process name is null or empty.", "name");
			_message = message;
			BeginProcessActivity();
			message.SetProperty(TrackingProperties.ProcessActivityId, _activityId);
			ProcessName = name;
		}

		protected EventStream EventStream
		{
			get { return _eventStream; }
		}

		public virtual void AddStep(MessagingStep messagingStep)
		{
			if (messagingStep == null) throw new ArgumentNullException("messagingStep");

			var processMessagingStep = new ProcessMessagingStep(Tracking.ActivityId.NewActivityId(), _eventStream);
			processMessagingStep.BeginProcessMessagingStepActivity();
			processMessagingStep.MessagingStepActivityID = messagingStep.ActivityId;
			// A MessagingStepReference denotes the initiating message of a messaging-only flow. Adding this step to a
			// process will only occur when we are tracking the outbound message of this messaging-only flow; it's
			// therefore too late to capture the initiating message status since the only message context at hand is the
			// one of the outgoing message.
			if (!(messagingStep is MessagingStepReference))
			{
				// don't bother to duplicate status other than failure
				processMessagingStep.MessagingStepStatus = messagingStep.Message.GetProperty(ErrorReportProperties.ErrorType);
			}
			processMessagingStep.ProcessActivityID = ActivityId;
			processMessagingStep.CommitProcessMessagingStepActivity();
			processMessagingStep.EndProcessMessagingStepActivity();
		}

		public virtual void TrackActivity()
		{
			BeginTime = DateTime.UtcNow;
			_message.GetProperty(BtsProperties.InterchangeID).IfNotNull(id => InterchangeID = id.AsNormalizedActivityId());
			Value1 = _message.GetProperty(TrackingProperties.Value1);
			Value2 = _message.GetProperty(TrackingProperties.Value2);
			Value3 = _message.GetProperty(TrackingProperties.Value3);
			// _message has not been reassigned since ctor but HasFailed() nonetheless reflects the right end result status
			// as, in a publish-subscribe/messaging-only flow, the context of the completing message has inherited most of
			// the context properties of the initiating message
			Status = _message.HasFailed() ? TrackingStatus.Failed : TrackingStatus.Completed;
			EndTime = DateTime.UtcNow;
			CommitProcessActivity();
			EndProcessActivity();
		}

		private readonly IBaseMessage _message;
	}
}

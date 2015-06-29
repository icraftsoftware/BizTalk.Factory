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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;

namespace Be.Stateless.BizTalk.Tracking.Processing
{
	public partial class Process
	{
		/// <summary>
		/// Method intended to be called at the start of an orchestration immediately after each one of its activating
		/// parallel receives.
		/// </summary>
		/// <param name="activatingMessage">
		/// The message that activates the orchestration instance.
		/// </param>
		/// <remarks>
		/// Notice that no BAM process tracking activity will be created. Instead, the activating message will be
		/// associated ahead of time to its forthcoming BAM process activity, see <see cref="ParallelInitiate()" /> and
		/// <see cref="ParallelInitiate(Microsoft.XLANGs.BaseTypes.XLANGMessage)"/>.
		/// </remarks>
		public static void ParallelPreInitiate(XLANGMessage activatingMessage)
		{
			if (activatingMessage == null) throw new ArgumentNullException("activatingMessage");
			new Process(Service.RootService.InstanceId.AsNormalizedActivityId())
				.AddStep(new MessagingStep(activatingMessage));
		}

		/// <summary>
		/// Method intended to be called near the start of an orchestration after all of its activating parallel receives
		/// (<see cref="ParallelPreInitiate"/>) have completed, or failed, to create a BAM process tracking activity.
		/// </summary>
		/// <returns>
		/// The <see cref="TrackingContext"/> that contains the BAM tracking activities identifiers for the current
		/// process. Notice that, cntrary to <see cref="Initiate(XLANGMessage)"/>, <see
		/// cref="TrackingContext.MessagingStepActivityId"/> will not be filled.
		/// </returns>
		/// <remarks>
		/// The BAM process tracking activity, whose parallel activating messages have already all been received (<see
		/// cref="ParallelPreInitiate"/>), will be created thereby providing the missing half of the process-to-activating
		/// messages associations. The name used for the process instance is the containing namespace of the calling
		/// orchestration.
		/// </remarks>
		public static TrackingContext ParallelInitiate()
		{
			var process = Initiate(Service.RootService.InstanceId, Service.RootService.GetType().Namespace, null, null, null);

			// set up orchestration's tracking context
			return new TrackingContext {
				ProcessActivityId = process.ActivityId
			};
		}

		/// <summary>
		/// Method intended to be called near the start of an orchestration after all of its activating parallel receives
		/// (<see cref="ParallelPreInitiate"/>) have completed, or failed, to create a BAM process tracking activity.
		/// </summary>
		/// <param name="keyMessage">
		/// The message that contains the key business values to be captured for tracking purposes.
		/// </param>
		/// <returns>
		/// The <see cref="TrackingContext"/> that contains the BAM tracking activities identifiers for the current
		/// process. Notice that, cntrary to <see cref="Initiate(XLANGMessage)"/>, <see
		/// cref="TrackingContext.MessagingStepActivityId"/> will not be filled.
		/// </returns>
		/// <remarks>
		/// The BAM process tracking activity, whose parallel activating messages have already all been received (<see
		/// cref="ParallelPreInitiate"/>), will be created thereby providing the missing half of the process-to-activating
		/// messages associations. The name used for the process instance is the containing namespace of the calling
		/// orchestration.
		/// </remarks>
		public static TrackingContext ParallelInitiate(XLANGMessage keyMessage)
		{
			var process = Initiate(
				Service.RootService.InstanceId,
				Service.RootService.GetType().Namespace,
				keyMessage.GetProperty(TrackingProperties.Value1),
				keyMessage.GetProperty(TrackingProperties.Value2),
				keyMessage.GetProperty(TrackingProperties.Value3));

			// set up orchestration's tracking context
			return new TrackingContext {
				ProcessActivityId = process.ActivityId
			};
		}

		/// <summary>
		/// Method intended to be called at the start of an orchestration to create a BAM process tracking activity.
		/// </summary>
		/// <param name="activatingMessage">
		/// The message that activates the orchestration instance.
		/// </param>
		/// <remarks>The name used for the process instance is the containing namespace of the calling orchestration.</remarks>
		/// <returns>
		/// The <see cref="TrackingContext"/> that contains the BAM tracking activities identifiers for the current
		/// process. Notice that <see cref="TrackingContext.MessagingStepActivityId"/> will be filled with the tracking activity
		/// identifier of <paramref name="activatingMessage"/> if it exists.
		/// </returns>
		public static TrackingContext Initiate(XLANGMessage activatingMessage)
		{
			if (activatingMessage == null) throw new ArgumentNullException("activatingMessage");
			return Initiate(activatingMessage, Service.RootService.GetType().Namespace);
		}

		/// <summary>
		/// Method intended to be called at the start of an orchestration to create a BAM process tracking activity.
		/// </summary>
		/// <param name="activatingMessage">
		/// The message that activates the orchestration instance.
		/// </param>
		/// <param name="processName">The name that will be used to qualify the new tracked process instance.</param>
		/// <returns>
		/// The <see cref="TrackingContext"/> that contains the BAM tracking activities identifiers for the current
		/// process. Notice that <see cref="TrackingContext.MessagingStepActivityId"/> will be filled with the tracking
		/// activity identifier of <paramref name="activatingMessage"/> if it exists.
		/// </returns>
		public static TrackingContext Initiate(XLANGMessage activatingMessage, string processName)
		{
			if (activatingMessage == null) throw new ArgumentNullException("activatingMessage");
			if (processName.IsNullOrEmpty()) throw new ArgumentException("String is null or empty", "processName");

			var process = Initiate(
				Service.RootService.InstanceId,
				processName,
				activatingMessage.GetProperty(TrackingProperties.Value1),
				activatingMessage.GetProperty(TrackingProperties.Value2),
				activatingMessage.GetProperty(TrackingProperties.Value3));

			// link inbound activating messaging step to its process
			// only an orchestation can link an activating messaging step to its process
			// as ProcessActivityId is initialized *by* the orchestration's first step
			var messagingStep = new MessagingStep(activatingMessage);
			process.AddStep(messagingStep);

			// set up orchestration's tracking context
			return new TrackingContext {
				ProcessActivityId = process.ActivityId,
				MessagingStepActivityId = messagingStep.ActivityId
			};
		}

		private static Process Initiate(Guid activityId, string processName, string value1, string value2, string value3)
		{
			var process = new Process(activityId.AsNormalizedActivityId());
			process.BeginProcessActivity();
			process.BeginTime = DateTime.UtcNow;
			process.ProcessName = processName;
			process.Status = TrackingStatus.Pending;
			process.Value1 = value1;
			process.Value2 = value2;
			process.Value3 = value3;
			// set up continuation for later process completion or failure
			process.EnableContinuation();
			process.CommitProcessActivity();
			process.EndProcessActivity();
			return process;
		}

		/// <summary>
		/// Method intended to be called at the end of a successful orchestration to end its corresponding BAM process
		/// tracking activity.
		/// </summary>
		/// <param name="trackingContext"><see cref="TrackingContext"/>
		/// Structure containing the appropriate activity identifiers.
		/// </param>
		/// <remarks>
		/// Method to be called at the end of a process, it will end the process activity and set its <see
		/// cref="Process.Status"/> to <see cref="TrackingStatus.Completed"/>.
		/// </remarks>
		public static void Complete(TrackingContext trackingContext)
		{
			Terminate(trackingContext, TrackingStatus.Completed);
		}

		/// <summary>
		/// Method intended to be called at the end of a failed orchestration to end its corresponding BAM process
		/// tracking activity.
		/// </summary>
		/// <param name="trackingContext"><see cref="TrackingContext"/>
		/// Structure containing the appropriate activity identifiers.
		/// </param>
		/// <remarks>
		/// Method to be called at the end of a process, it will end the process activity and set its <see
		/// cref="Process.Status"/> to <see cref="TrackingStatus.Failed"/>.
		/// </remarks>
		public static void Fail(TrackingContext trackingContext)
		{
			Terminate(trackingContext, TrackingStatus.Failed);
		}

		/// <summary>
		/// Adds a messaging step to a process.
		/// </summary>
		/// <param name="trackingContext">
		/// The <see cref="TrackingContext"/> that contains the <see cref="TrackingContext.ProcessActivityId"/> of the
		/// process tracking activty.
		/// </param>
		/// <param name="message">
		/// The message whose BAM tracking activity must be attached to the process tracking activity.
		/// </param>
		/// <remarks>
		/// It is typically used when an orchestration receives a message in the middle of its execution, or when convoys
		/// are used to start it. In these cases, the message(s) cannot be associated with the process in the receive
		/// pipeline, and a call to this method is needed. Notice that to attach a single activating message to a process,
		/// the <see cref="Initiate(XLANGMessage)"/> method must be used instead.
		/// </remarks>
		public static void AddStep(TrackingContext trackingContext, XLANGMessage message)
		{
			if (message == null) throw new ArgumentNullException("message");
			if (trackingContext.ProcessActivityId.IsNullOrEmpty()) throw new ArgumentException("trackingContext.ProcessActivityId is null or empty: process tracking has not been initiated.", "trackingContext");

			new Process(trackingContext.ProcessActivityId).AddStep(new MessagingStep(message));
		}

		internal static void Terminate(TrackingContext trackingContext, string status)
		{
			if (trackingContext.ProcessActivityId.IsNullOrEmpty()) throw new ArgumentException("trackingContext.ProcessActivityId is null or empty: process tracking has not been initiated.", "trackingContext");
			var process = new Process(ContinuationPrefix + trackingContext.ProcessActivityId) {
				EndTime = DateTime.UtcNow,
				Status = status
			};
			process.CommitProcessActivity();
			process.EndProcessActivity();
		}

		internal void AddStep(MessagingStep messagingStep)
		{
			var tracking = new ProcessMessagingStep(Tracking.ActivityId.NewActivityId());
			tracking.BeginProcessMessagingStepActivity();
			tracking.MessagingStepActivityID = messagingStep.ActivityId;
			// don't bother to duplicate status other than failure
			tracking.MessagingStepStatus = messagingStep.Message.GetProperty(ErrorReportProperties.ErrorType);
			tracking.ProcessActivityID = ActivityId;
			tracking.CommitProcessMessagingStepActivity();
			tracking.EndProcessMessagingStepActivity();
		}
	}
}

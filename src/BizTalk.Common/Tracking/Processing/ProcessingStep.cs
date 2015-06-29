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
using System.Xml;
using Be.Stateless.Extensions;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Tracking.Processing
{
	public partial class ProcessingStep
	{
		/// <summary>
		/// Method intended to be called when starting a processing step in the middle of an orchestration. It is expected 
		/// that a tracking context has been created previously, at the start of the orchestration by a call to <see
		/// cref="Process.Initiate(XLANGMessage)"/> or by a call to this method. A new processing step tracking activity
		/// is always started, and it is always linked to the process tracking activity identified in <paramref
		/// name="trackingContext"/>.
		/// </summary>
		/// <param name="trackingContext">
		///   The tracking context that is applicable just before entering the new processing step.
		/// </param>
		/// <param name="processingStepName">
		///   The name of the processing step entered.
		/// </param>
		/// <returns>
		/// The new <see cref="TrackingContext"/> for the processing step scope.
		/// </returns>
		public static TrackingContext Initiate(TrackingContext trackingContext, string processingStepName)
		{
			if (string.IsNullOrEmpty(processingStepName)) throw new ArgumentException("Processing step name is null or empty.", "processingStepName");
			if (trackingContext.ProcessActivityId.IsNullOrEmpty()) throw new ArgumentException("trackingContext.ProcessActivityId is null or empty: process tracking has not been initiated.", "trackingContext");

			var processingStep = new ProcessingStep();
			processingStep.BeginProcessingStepActivity();
			processingStep.BeginTime = DateTime.UtcNow;
			processingStep.MachineName = Environment.MachineName;
			processingStep.ProcessActivityID = trackingContext.ProcessActivityId;
			processingStep.Status = TrackingStatus.Pending;
			processingStep.StepName = processingStepName;
			// set up continuation for later processing step completion or failure
			processingStep.EnableContinuation();
			processingStep.CommitProcessingStepActivity();
			processingStep.EndProcessingStepActivity();

			// propagate the tracking context with this processing step's activity id
			var newTrackingContext = trackingContext;
			newTrackingContext.ProcessingStepActivityId = processingStep.ActivityId;
			return newTrackingContext;
		}

		/// <summary>
		/// Method to be called at the end of a successful orchestration processing step.
		/// </summary>
		/// <param name="trackingContext"><see cref="TrackingContext"/>
		/// Structure containing the appropriate activity identifiers.
		/// </param>
		/// <remarks>
		/// It will end the processing step activity, setting its <see cref="Status"/> to <see
		/// cref="TrackingStatus.Completed"/>, but will not end the process activity.
		/// </remarks>
		public static void Complete(TrackingContext trackingContext)
		{
			Terminate(trackingContext, TrackingStatus.Completed, null);
		}

		/// <summary>
		/// Method to be called at the end of a failed orchestration processing step.
		/// </summary>
		/// <param name="trackingContext"><see cref="TrackingContext"/>
		/// Structure containing the appropriate activity identifiers.
		/// </param>
		/// <param name="exception">
		/// The exception that causes the processing step to fail.
		/// </param>
		/// <remarks>
		/// It will end the processing step activity, setting its <see cref="Status"/> to <see
		/// cref="TrackingStatus.Failed"/>.
		/// </remarks>
		public static void Fail(TrackingContext trackingContext, Exception exception)
		{
			Terminate(trackingContext, TrackingStatus.Failed, exception.IfNotNull(e => e.ToString()));
		}

		/// <summary>
		/// Method to be called at the end of a failed orchestration processing step.
		/// </summary>
		/// <param name="trackingContext"><see cref="TrackingContext"/>
		/// Structure containing the appropriate activity identifiers.
		/// </param>
		/// <param name="faultMessage">
		/// The fault message that causes the processing step to fail.
		/// </param>
		/// <remarks>
		/// It will end the processing step activity, setting its <see cref="Status"/> to <see
		/// cref="TrackingStatus.Failed"/>.
		/// </remarks>
		public static void Fail(TrackingContext trackingContext, XLANGMessage faultMessage)
		{
			string errorDescription = null;
			if (faultMessage != null)
			{
				var faultDocument = (XmlDocument) faultMessage[0].RetrieveAs(typeof(XmlDocument));
				// TODO: extract error info from fault message
				errorDescription = faultDocument.OuterXml;
			}

			Terminate(trackingContext, TrackingStatus.Failed, errorDescription);
		}

		private static void Terminate(TrackingContext trackingContext, string status, string errorDescription)
		{
			if (trackingContext.ProcessingStepActivityId.IsNullOrEmpty()) throw new ArgumentException("trackingContext.ProcessingStepActivityId is null or empty: processing step tracking has not been initiated.", "trackingContext");

			var processingStep = new ProcessingStep(ContinuationPrefix + trackingContext.ProcessingStepActivityId) {
				EndTime = DateTime.UtcNow,
				ErrorDescription = errorDescription,
				MachineName = Environment.MachineName,
				Status = status
			};
			processingStep.CommitProcessingStepActivity();
			processingStep.EndProcessingStepActivity();
		}

		internal ProcessingStep() : this(Tracking.ActivityId.NewActivityId()) { }
	}
}

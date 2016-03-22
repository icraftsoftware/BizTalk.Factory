#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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

using Be.Stateless.BizTalk.Schemas.Xml;
using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	/// <summary>
	/// Tracking-activity factory for messaging-only activities, i.e. <see cref="EventStream"/>-based activities.
	/// </summary>
	/// <remarks>
	/// Notice that, as this is a messaging-only activity factory API, there is no way to create a <see
	/// cref="ProcessingStep"/> activity.
	/// </remarks>
	public interface IActivityFactory
	{
		/// <summary>
		/// Creates a <see cref="Process"/> tracking activity denoting a process yet to be tracked.
		/// </summary>
		/// <param name="message">
		/// The <see cref="IBaseMessage"/> to which will be attached the ambient <see cref="TrackingContext"/> referencing
		/// the activity id of the current <see cref="Process"/>.
		/// </param>
		/// <param name="name">
		/// The name of the process to create.
		/// </param>
		/// <returns>
		/// A <see cref="Process"/> instance that will be used to feed the BAM Process activity.
		/// </returns>
		Process CreateProcess(IBaseMessage message, string name);

		/// <summary>
		/// Creates a <see cref="ProcessReference"/> tracking activity denoting an already tracked, though possibly
		/// ongoing, process.
		/// </summary>
		/// <param name="trackingContext">
		/// The <see cref="TrackingContext"/> whose <see cref="TrackingContext.ProcessActivityId"/> identifies the ongoing
		/// process.
		/// </param>
		/// <returns>
		/// A <see cref="ProcessReference"/> instance that will be used to feed the BAM Process activity: specifically,
		/// affiliating new <see cref="MessagingStep"/> tracking activities.
		/// </returns>
		Process FindProcess(TrackingContext trackingContext);

		/// <summary>
		/// Creates a <see cref="MessagingStep"/> tracking activity denoting a message yet to be tracked.
		/// </summary>
		/// <param name="message">
		/// The <see cref="IBaseMessage"/> whose <see cref="IBaseMessage.BodyPart"/> and <see cref="IBaseMessageContext"/>
		/// data will be fed to the BAM MessagingStep activity.
		/// </param>
		/// <returns>
		/// A <see cref="MessagingStep"/> instance that will be used to feed the BAM MessagingStep activity.
		/// </returns>
		MessagingStep CreateMessagingStep(IBaseMessage message);

		/// <summary>
		/// Creates a <see cref="MessagingStepReference"/> tracking activity denoting an already tracked message.
		/// </summary>
		/// <param name="trackingContext">
		/// The <see cref="TrackingContext"/> whose <see cref="TrackingContext.MessagingStepActivityId"/> identifies the
		/// already tracked <see cref="MessagingStep"/>.
		/// </param>
		/// <returns>
		/// A <see cref="MessagingStepReference"/> instance that will be used to feed the BAM Process activity:
		/// specifically, affiliating it to a <see cref="Process"/> tracking activity.
		/// </returns>
		MessagingStep FindMessagingStep(TrackingContext trackingContext);
	}

	/// <summary>
	/// Batch-release process tracking-activity factory.
	/// </summary>
	/// <remarks>
	/// Notice that this is by design a messaging-only activity, i.e. <see cref="EventStream"/>-based activities.
	/// </remarks>
	internal interface IBatchProcessActivityFactory
	{
		/// <summary>
		/// Creates a batch release <see cref="BatchReleaseProcess"/> tracking activity.
		/// </summary>
		/// <param name="message">
		/// The <see cref="IBaseMessage"/> denoting the <see cref="Batch.Content"/> being released.
		/// </param>
		/// <param name="name">
		/// The name of the process to create.
		/// </param>
		/// <returns>
		/// A <see cref="Process"/> instance that will be used to feed the BAM Process activity.
		/// </returns>
		BatchReleaseProcess CreateProcess(IBaseMessage message, string name);

		/// <summary>
		/// Creates a reference to the batch release <see cref="BatchReleaseProcessReference"/> tracking activity that was
		/// initiated by a <see cref="Batch.Release"/> control message.
		/// </summary>
		/// <param name="processActivityId">
		/// The <see cref="Process.ActivityId"/> of the batch release process that was initiated by a <see
		/// cref="Batch.Release"/> control message.
		/// </param>
		/// <returns>
		/// A <see cref="ProcessReference"/> instance that will be used to feed the BAM Process activity.
		/// </returns>
		BatchReleaseProcess FindProcess(string processActivityId);
	}
}

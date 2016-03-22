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

using System;
using System.Globalization;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	/// <summary>
	/// Allows to track activity of a messaging step at the messaging level.
	/// </summary>
	public partial class MessagingStep
	{
		// internal because cannot prevent NullReferenceException should pipelineContext be null when calling .GetEventStream()
		internal MessagingStep(IPipelineContext pipelineContext, IBaseMessage message)
			: this(Tracking.ActivityId.NewActivityId(), pipelineContext.GetEventStream())
		{
			if (message == null) throw new ArgumentNullException("message");
			_message = message;
			BeginMessagingStepActivity();
			_message.SetProperty(TrackingProperties.MessagingStepActivityId, _activityId);
		}

		/// <summary>
		/// The actual <see cref="IBaseMessage"/> message that this messaging step is all about.
		/// </summary>
		public virtual IBaseMessage Message
		{
			get { return _message; }
		}

		/// <summary>
		/// Creates a BAM <c>MessagingStep</c> activity that captures descriptive information about the <see
		/// cref="IBaseMessage"/> being processed.
		/// </summary>
		/// <param name="trackingModes">
		/// The extent to which information about a messaging step activity will be tracked and recorded.
		/// </param>
		/// <param name="trackingStream">
		/// The <see cref="TrackingStream"/> that wraps the actual message body's stream that will captured.
		/// </param>
		internal virtual void TrackActivity(ActivityTrackingModes trackingModes, TrackingStream trackingStream)
		{
			if (trackingModes.RequiresStepTracking()) TrackStep(trackingStream.Length);
			if (trackingModes.RequiresContextTracking()) TrackMessageContext();
			if (trackingModes.RequiresBodyTracking()) TrackMessageBody(trackingStream.CaptureDescriptor);
		}

		/// <summary>
		/// Associates the <see cref="MessageBodyCaptureDescriptor"/> describing how and where the <see
		/// cref="IBaseMessage.BodyPart"/>'s <see cref="IBaseMessagePart.Data"/> of the <see cref="IBaseMessage"/> being
		/// processed is captured with its related BAM <c>MessagingStep</c> activity.
		/// </summary>
		/// <param name="captureDescriptor">
		/// Descriptive information on how and where the <see cref="IBaseMessage.BodyPart"/>'s <see
		/// cref="IBaseMessagePart.Data"/> of the <see cref="IBaseMessage"/> being processed is captured.
		/// </param>
		private void TrackMessageBody(MessageBodyCaptureDescriptor captureDescriptor)
		{
			if (captureDescriptor == null) throw new ArgumentNullException("captureDescriptor");

			AddCustomReference(
				captureDescriptor.CaptureMode.ToString(),
				MESSAGE_BODY_REFERENCE_NAME,
				// timestamp because it is mandatory to use a non-null reference data
				DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
				captureDescriptor.Data);
		}

		/// <summary>
		/// Captures the <see cref="IBaseMessage.Context"/> of the <see cref="IBaseMessage"/> being processed and
		/// associate it with its related BAM <c>MessagingStep</c> activity.
		/// </summary>
		private void TrackMessageContext()
		{
			AddCustomReference(
				MESSAGE_CONTEXT_REFERENCE_TYPE,
				MESSAGE_CONTEXT_REFERENCE_NAME,
				// timestamp because it is mandatory to use a non-null reference data
				DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
				_message.Context.ToXml());
		}

		/// <summary>
		/// Creates a BAM <c>MessagingStep</c> activity that captures descriptive information about the <see
		/// cref="IBaseMessage"/> being processed.
		/// </summary>
		/// <param name="messageSize">
		/// Size in bytes of the <see cref="IBaseMessage"/> body payload stream.
		/// </param>
		private void TrackStep(long messageSize)
		{
			TrackProperties(messageSize);
			CommitMessagingStepActivity();
			EndMessagingStepActivity();
		}

		private void TrackProperties(long messageSize)
		{
			_message.GetProperty(BtsProperties.InterchangeID).IfNotNull(id => InterchangeID = id.AsNormalizedActivityId());
			RetryCount = _message.GetProperty(BtsProperties.ActualRetryCount);
			MessageSize = (int) messageSize;
			if (_message.Direction().IsInbound())
			{
				TransportType = _message.GetProperty(BtsProperties.InboundTransportType);
			}
			else // if (_message.Direction().IsOutbound())
			{
				TransportType = _message.GetProperty(BtsProperties.OutboundTransportType);
			}
			Value1 = _message.GetProperty(TrackingProperties.Value1);
			Value2 = _message.GetProperty(TrackingProperties.Value2);
			Value3 = _message.GetProperty(TrackingProperties.Value3);

			if (_message.HasFailed()) TrackFailureProperties();
			else TrackSuccessfulProperties();
		}

		private void TrackFailureProperties()
		{
			_message.GetProperty(ErrorReportProperties.FailureMessageID).IfNotNull(id => MessageID = id.AsNormalizedActivityId());
			MessageType = _message.GetProperty(ErrorReportProperties.MessageType);
			Status = _message.GetProperty(ErrorReportProperties.ErrorType);
			if (_message.FailedDirection().IsInbound())
			{
				PortName = _message.GetProperty(ErrorReportProperties.ReceivePortName);
				TransportLocation = _message.GetProperty(ErrorReportProperties.InboundTransportLocation);
			}
			else // if (_message.FailedDirection().IsOutbound())
			{
				PortName = _message.GetProperty(ErrorReportProperties.SendPortName);
				TransportLocation = _message.GetProperty(ErrorReportProperties.OutboundTransportLocation);
			}
			ErrorCode = _message.GetProperty(ErrorReportProperties.FailureCode);
			ErrorDescription = _message.GetProperty(ErrorReportProperties.Description);
			MachineName = _message.GetProperty(ErrorReportProperties.ProcessingServer);
			Time = _message.GetProperty(ErrorReportProperties.FailureTime);
		}

		private void TrackSuccessfulProperties()
		{
			MessageID = _message.MessageID.AsNormalizedActivityId();
			MessageType = _message.GetProperty(BtsProperties.MessageType);
			if (_message.Direction().IsInbound())
			{
				// e.g. the response-side of a solicit-response port has no ReceiveLocationName but only a ReceivePortName
				PortName = _message.GetProperty(BtsProperties.ReceiveLocationName) ?? _message.GetProperty(BtsProperties.ReceivePortName);
				Status = TrackingStatus.Received;
				TransportLocation = _message.GetProperty(BtsProperties.InboundTransportLocation);
			}
			else // if (_message.Direction().IsOutbound())
			{
				PortName = _message.GetProperty(BtsProperties.SendPortName);
				Status = TrackingStatus.Sent;
				TransportLocation = _message.GetProperty(BtsProperties.OutboundTransportLocation);
			}
			MachineName = Environment.MachineName;
			Time = DateTime.UtcNow;
		}

		internal const string MESSAGE_BODY_REFERENCE_NAME = "MessageBody";
		internal const string MESSAGE_CONTEXT_REFERENCE_NAME = "MessageContext";
		internal const string MESSAGE_CONTEXT_REFERENCE_TYPE = "Ctxt";

		private readonly IBaseMessage _message;
	}
}

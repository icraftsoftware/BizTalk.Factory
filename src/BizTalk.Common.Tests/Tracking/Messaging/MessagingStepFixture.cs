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
using System.IO;
using System.Linq;
using System.Text;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Linq;
using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;
using MessageMock = Be.Stateless.BizTalk.Unit.Message.Mock<Microsoft.BizTalk.Message.Interop.IBaseMessage>;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	[TestFixture]
	public class MessagingStepFixture
	{
		[Test]
		public void ActivityIsBegunAndActivityIdWrittenInMessageContext()
		{
			var eventStream = new Mock<EventStream>();
			var pipelineContext = new Mock<IPipelineContext>();
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var message = new MessageMock();
			message.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			var sut = new MessagingStep(pipelineContext.Object, message.Object);
			var activityId = sut.ActivityId;

			message.Verify(m => m.SetProperty(TrackingProperties.MessagingStepActivityId, activityId), Times.Once());
			eventStream.Verify(s => s.BeginActivity(MessagingStep.ActivityName, activityId), Times.Once());
		}

		[Test]
		public void ActivityIsCommittedAndEnded()
		{
			var eventStream = new Mock<EventStream>();
			var pipelineContext = new Mock<IPipelineContext>();
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var message = new MessageMock();
			message.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			var sut = new MessagingStep(pipelineContext.Object, message.Object);
			using (var trackingStream = new TrackingStream(new MemoryStream(_content)))
			{
				// TrackActivity is supposed to occur at stream's end
				trackingStream.Drain();
				sut.TrackActivity(ActivityTrackingModes.Step, trackingStream);
			}

			eventStream.Verify(s => s.UpdateActivity(MessagingStep.ActivityName, sut.ActivityId, It.IsAny<object[]>()), Times.Once());
			eventStream.Verify(s => s.Flush(), Times.Once());
			eventStream.Verify(s => s.EndActivity(MessagingStep.ActivityName, sut.ActivityId), Times.Once());
		}

		[Test]
		public void InboundFailedPropertiesAreTracked()
		{
			var message = new MessageMock();
			SetupCommonProperties(message);
			SetupOutboundSuccessfulProperties(message);
			SetupCommonFailedProperties(message);
			message.Setup(m => m.GetProperty(ErrorReportProperties.ReceivePortName)).Returns("failed-receive-port-name");
			message.Setup(m => m.GetProperty(ErrorReportProperties.InboundTransportLocation)).Returns("failed-inbound-transport-location");

			var activityId = string.Empty;
			Dictionary<string, object> data = null;
			var eventStream = new Mock<EventStream>();
			eventStream
				// ReSharper disable AccessToModifiedClosure
				.Setup(es => es.UpdateActivity(MessagingStep.ActivityName, It.Is<string>(id => id == activityId), It.IsAny<object[]>()))
				// ReSharper restore AccessToModifiedClosure
				.Callback<string, string, object[]>((n, id, d) => data = ToDictionary(d))
				.Verifiable();

			var pipelineContext = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var sut = new MessagingStep(pipelineContext.Object, message.Object);
			activityId = sut.ActivityId;
			using (var trackingStream = new TrackingStream(new MemoryStream(_content)))
			{
				// TrackActivity is supposed to occur at stream's end
				trackingStream.Drain();
				sut.TrackActivity(ActivityTrackingModes.Step, trackingStream);
			}

			eventStream.Verify();

			var expectedData = new Dictionary<string, object> {
				{ MessagingStep.TransportTypeFieldName, "outbound-transport-type" },
				{ MessagingStep.PortNameFieldName, "failed-receive-port-name" },
				{ MessagingStep.TransportLocationFieldName, "failed-inbound-transport-location" }
			}
				.Union(ExpectedCommonFailedData, new LambdaComparer<KeyValuePair<string, object>>((kvp1, kvp2) => kvp1.Key == kvp2.Key))
				.Union(ExpectedCommonData, new LambdaComparer<KeyValuePair<string, object>>((kvp1, kvp2) => kvp1.Key == kvp2.Key))
				.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			Assert.That(data, Is.EquivalentTo(expectedData));
		}

		[Test]
		public void InboundSuccessfulPropertiesAreTracked()
		{
			var message = new MessageMock();
			SetupCommonProperties(message);
			SetupInboundSuccessfulProperties(message);

			var activityId = string.Empty;
			Dictionary<string, object> data = null;
			var eventStream = new Mock<EventStream>();
			eventStream
				// ReSharper disable AccessToModifiedClosure
				.Setup(es => es.UpdateActivity(MessagingStep.ActivityName, It.Is<string>(id => id == activityId), It.IsAny<object[]>()))
				// ReSharper restore AccessToModifiedClosure
				.Callback<string, string, object[]>((n, id, d) => data = ToDictionary(d))
				.Verifiable();

			var pipelineContext = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var sut = new MessagingStep(pipelineContext.Object, message.Object);
			activityId = sut.ActivityId;
			using (var trackingStream = new TrackingStream(new MemoryStream(_content)))
			{
				// TrackActivity is supposed to occur at stream's end
				trackingStream.Drain();
				sut.TrackActivity(ActivityTrackingModes.Step, trackingStream);
			}

			eventStream.Verify();

			var expectedData = new Dictionary<string, object> {
				{ MessagingStep.MessageIDFieldName, _messageId.AsNormalizedActivityId() },
				{ MessagingStep.MessageTypeFieldName, "message-type" },
				{ MessagingStep.PortNameFieldName, "receive-location-name" },
				{ MessagingStep.TransportLocationFieldName, "inbound-transport-location" },
				{ MessagingStep.TransportTypeFieldName, "inbound-transport-type" },
				{ MessagingStep.StatusFieldName, TrackingStatus.Received },
				{ MessagingStep.MachineNameFieldName, Environment.MachineName },
				{ MessagingStep.TimeFieldName, sut.Time }
			}
				.Union(ExpectedCommonData, new LambdaComparer<KeyValuePair<string, object>>((kvp1, kvp2) => kvp1.Key == kvp2.Key))
				.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			Assert.That(data, Is.EquivalentTo(expectedData));
		}

		[Test]
		public void InboundSuccessfulPropertiesForSolicitResponsePortAreTracked()
		{
			var message = new MessageMock();
			SetupCommonProperties(message);
			// no ReceiveLocationName on the inbound of a solicit-response port but a ReceivePortName
			message.Setup(m => m.GetProperty(BtsProperties.ReceivePortName)).Returns("receive-port-name");
			message.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			message.Setup(m => m.GetProperty(BtsProperties.InboundTransportType)).Returns("inbound-transport-type");

			var activityId = string.Empty;
			Dictionary<string, object> data = null;
			var eventStream = new Mock<EventStream>();
			eventStream
				// ReSharper disable AccessToModifiedClosure
				.Setup(es => es.UpdateActivity(MessagingStep.ActivityName, It.Is<string>(id => id == activityId), It.IsAny<object[]>()))
				// ReSharper restore AccessToModifiedClosure
				.Callback<string, string, object[]>((n, id, d) => data = ToDictionary(d))
				.Verifiable();

			var pipelineContext = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var sut = new MessagingStep(pipelineContext.Object, message.Object);
			activityId = sut.ActivityId;
			using (var trackingStream = new TrackingStream(new MemoryStream(_content)))
			{
				// TrackActivity is supposed to occur at stream's end
				trackingStream.Drain();
				sut.TrackActivity(ActivityTrackingModes.Step, trackingStream);
			}

			eventStream.Verify();

			var expectedData = new Dictionary<string, object> {
				{ MessagingStep.MessageIDFieldName, _messageId.AsNormalizedActivityId() },
				{ MessagingStep.MessageTypeFieldName, "message-type" },
				{ MessagingStep.PortNameFieldName, "receive-port-name" },
				{ MessagingStep.TransportLocationFieldName, "inbound-transport-location" },
				{ MessagingStep.TransportTypeFieldName, "inbound-transport-type" },
				{ MessagingStep.StatusFieldName, TrackingStatus.Received },
				{ MessagingStep.MachineNameFieldName, Environment.MachineName },
				{ MessagingStep.TimeFieldName, sut.Time }
			}
				.Union(ExpectedCommonData, new LambdaComparer<KeyValuePair<string, object>>((kvp1, kvp2) => kvp1.Key == kvp2.Key))
				.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			Assert.That(data, Is.EquivalentTo(expectedData));
		}

		[Test]
		public void MessageBodyIsTracked()
		{
			var message = new MessageMock();
			message.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			var eventStream = new Mock<EventStream>();

			var pipelineContext = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			pipelineContext
				.Setup(pc => pc.GetEventStream())
				.Returns(eventStream.Object);

			var sut = new MessagingStep(pipelineContext.Object, message.Object);
			using (var trackingStream = new TrackingStream(new MemoryStream(_content), new MessageBodyCaptureDescriptor("data", MessageBodyCaptureMode.Claimed)))
			{
				// TrackActivity is supposed to occur at stream's end
				trackingStream.Drain();
				sut.TrackActivity(ActivityTrackingModes.Body, trackingStream);
			}

			eventStream.Verify(
				es => es.AddReference(
					MessagingStep.ActivityName,
					sut.ActivityId,
					MessageBodyCaptureMode.Claimed.ToString(),
					MessagingStep.MESSAGE_BODY_REFERENCE_NAME,
					It.IsAny<string>(),
					It.IsAny<string>()),
				Times.Once());
		}

		[Test]
		public void MessageContextIsTracked()
		{
			var ns = BtsProperties.MessageType.Namespace;
			var name = BtsProperties.MessageType.Name;

			var message = new MessageMock();
			message.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			message.Setup(m => m.Context.CountProperties).Returns(1);
			message.Setup(m => m.Context.ReadAt(0, out name, out ns)).Returns("message-type");
			message.Setup(m => m.Context.IsPromoted(name, ns)).Returns(false);

			var eventStream = new Mock<EventStream>();

			var pipelineContext = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var sut = new MessagingStep(pipelineContext.Object, message.Object);
			using (var trackingStream = new TrackingStream(new MemoryStream(_content)))
			{
				// TrackActivity is supposed to occur at stream's end
				trackingStream.Drain();
				sut.TrackActivity(ActivityTrackingModes.Context, trackingStream);
			}

			eventStream.Verify(
				es => es.AddReference(
					MessagingStep.ActivityName,
					sut.ActivityId,
					MessagingStep.MESSAGE_CONTEXT_REFERENCE_TYPE,
					MessagingStep.MESSAGE_CONTEXT_REFERENCE_NAME,
					It.IsAny<string>(),
					message.Object.Context.ToXml()),
				Times.Once());
		}

		[Test]
		public void OutboundFailedPropertiesAreTracked()
		{
			var message = new MessageMock();
			SetupCommonProperties(message);
			SetupInboundSuccessfulProperties(message);
			SetupOutboundSuccessfulProperties(message);
			SetupCommonFailedProperties(message);
			message.Setup(m => m.GetProperty(ErrorReportProperties.SendPortName)).Returns("failed-send-port-name");
			message.Setup(m => m.GetProperty(ErrorReportProperties.OutboundTransportLocation)).Returns("failed-outbound-transport-location");

			var activityId = string.Empty;
			Dictionary<string, object> data = null;
			var eventStream = new Mock<EventStream>();
			eventStream
				// ReSharper disable AccessToModifiedClosure
				.Setup(es => es.UpdateActivity(MessagingStep.ActivityName, It.Is<string>(id => id == activityId), It.IsAny<object[]>()))
				// ReSharper restore AccessToModifiedClosure
				.Callback<string, string, object[]>((n, id, d) => data = ToDictionary(d))
				.Verifiable();

			var pipelineContext = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var sut = new MessagingStep(pipelineContext.Object, message.Object);
			activityId = sut.ActivityId;
			using (var trackingStream = new TrackingStream(new MemoryStream(_content)))
			{
				// TrackActivity is supposed to occur at stream's end
				trackingStream.Drain();
				sut.TrackActivity(ActivityTrackingModes.Step, trackingStream);
			}

			eventStream.Verify();

			var expectedData = new Dictionary<string, object> {
				{ MessagingStep.TransportTypeFieldName, "outbound-transport-type" },
				{ MessagingStep.PortNameFieldName, "failed-send-port-name" },
				{ MessagingStep.TransportLocationFieldName, "failed-outbound-transport-location" }
			}
				.Union(ExpectedCommonFailedData, new LambdaComparer<KeyValuePair<string, object>>((kvp1, kvp2) => kvp1.Key == kvp2.Key))
				.Union(ExpectedCommonData, new LambdaComparer<KeyValuePair<string, object>>((kvp1, kvp2) => kvp1.Key == kvp2.Key))
				.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			Assert.That(data, Is.EquivalentTo(expectedData));
			Assert.That(sut.MessageType, Is.EqualTo("failed-message-type"));
		}

		[Test]
		public void OutboundSuccessfulPropertiesAreTrackedAtStreamExhaustion()
		{
			var message = new MessageMock();
			SetupCommonProperties(message);
			SetupInboundSuccessfulProperties(message);
			SetupOutboundSuccessfulProperties(message);

			var activityId = string.Empty;
			Dictionary<string, object> data = null;
			var eventStream = new Mock<EventStream>();
			eventStream
				// ReSharper disable AccessToModifiedClosure
				.Setup(es => es.UpdateActivity(MessagingStep.ActivityName, It.Is<string>(id => id == activityId), It.IsAny<object[]>()))
				// ReSharper restore AccessToModifiedClosure
				.Callback<string, string, object[]>((n, id, d) => data = ToDictionary(d))
				.Verifiable();

			var pipelineContext = new Mock<IPipelineContext> { DefaultValue = DefaultValue.Mock };
			pipelineContext.Setup(pc => pc.GetEventStream()).Returns(eventStream.Object);

			var sut = new MessagingStep(pipelineContext.Object, message.Object);
			activityId = sut.ActivityId;
			using (var trackingStream = new TrackingStream(new MemoryStream(_content)))
			{
				// TrackActivity is supposed to occur at stream's end
				trackingStream.Drain();
				sut.TrackActivity(ActivityTrackingModes.Step, trackingStream);
			}

			eventStream.Verify();

			var expectedData = new Dictionary<string, object> {
				{ MessagingStep.MessageIDFieldName, _messageId.AsNormalizedActivityId() },
				{ MessagingStep.PortNameFieldName, "send-port-name" },
				{ MessagingStep.TransportLocationFieldName, "outbound-transport-location" },
				{ MessagingStep.TransportTypeFieldName, "outbound-transport-type" },
				{ MessagingStep.StatusFieldName, TrackingStatus.Sent },
				{ MessagingStep.MachineNameFieldName, Environment.MachineName },
				{ MessagingStep.TimeFieldName, sut.Time }
			}
				.Union(ExpectedCommonData)
				.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			Assert.That(data, Is.EquivalentTo(expectedData));
			Assert.That(sut.MessageType, Is.EqualTo("message-type"));
		}

		private Dictionary<string, object> ExpectedCommonData
		{
			get
			{
				return new Dictionary<string, object> {
					{ MessagingStep.InterchangeIDFieldName, _interchangeId.AsNormalizedActivityId() },
					{ MessagingStep.MessageIDFieldName, _messageId.AsNormalizedActivityId() },
					{ MessagingStep.MessageSizeFieldName, _content.Length },
					{ MessagingStep.MessageTypeFieldName, "message-type" },
					{ MessagingStep.RetryCountFieldName, 3 },
					{ MessagingStep.Value1FieldName, "value-1" },
					{ MessagingStep.Value2FieldName, "value-2" },
					{ MessagingStep.Value3FieldName, "value-3" }
				};
			}
		}

		private Dictionary<string, object> ExpectedCommonFailedData
		{
			get
			{
				return new Dictionary<string, object> {
					{ MessagingStep.MessageIDFieldName, _failedMessageId.AsNormalizedActivityId() },
					{ MessagingStep.MessageTypeFieldName, "failed-message-type" },
					{ MessagingStep.StatusFieldName, TrackingStatus.FailedMessage },
					{ MessagingStep.TransportTypeFieldName, "inbound-transport-type" },
					{ MessagingStep.ErrorCodeFieldName, "failure-code" },
					{ MessagingStep.ErrorDescriptionFieldName, "failure-description" },
					{ MessagingStep.MachineNameFieldName, "failing-machine" },
					{ MessagingStep.TimeFieldName, _failureTime }
				};
			}
		}

		private void SetupCommonProperties(Unit.Message.Mock<IBaseMessage> message)
		{
			_interchangeId = Guid.NewGuid();
			message.Setup(m => m.GetProperty(BtsProperties.InterchangeID)).Returns(_interchangeId.ToString());

			_messageId = Guid.NewGuid();
			message.Setup(m => m.MessageID).Returns(_messageId);
			message.Setup(m => m.GetProperty(BtsProperties.MessageType)).Returns("message-type");
			message.Setup(m => m.GetProperty(BtsProperties.ActualRetryCount)).Returns(3);
			message.Setup(m => m.GetProperty(TrackingProperties.Value1)).Returns("value-1");
			message.Setup(m => m.GetProperty(TrackingProperties.Value2)).Returns("value-2");
			message.Setup(m => m.GetProperty(TrackingProperties.Value3)).Returns("value-3");
		}

		private void SetupCommonFailedProperties(Unit.Message.Mock<IBaseMessage> message)
		{
			_failedMessageId = Guid.NewGuid();
			_failureTime = DateTime.UtcNow;
			message.Setup(m => m.GetProperty(ErrorReportProperties.FailureMessageID)).Returns(_failedMessageId.ToString());
			message.Setup(m => m.GetProperty(ErrorReportProperties.MessageType)).Returns("failed-message-type");
			message.Setup(m => m.GetProperty(ErrorReportProperties.ErrorType)).Returns(TrackingStatus.FailedMessage);
			message.Setup(m => m.GetProperty(ErrorReportProperties.FailureCode)).Returns("failure-code");
			message.Setup(m => m.GetProperty(ErrorReportProperties.Description)).Returns("failure-description");
			message.Setup(m => m.GetProperty(ErrorReportProperties.ProcessingServer)).Returns("failing-machine");
			message.Setup(m => m.GetProperty(ErrorReportProperties.FailureTime)).Returns(_failureTime);
		}

		private void SetupInboundSuccessfulProperties(Unit.Message.Mock<IBaseMessage> message)
		{
			message.Setup(m => m.GetProperty(BtsProperties.ReceiveLocationName)).Returns("receive-location-name");
			message.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
			message.Setup(m => m.GetProperty(BtsProperties.InboundTransportType)).Returns("inbound-transport-type");
		}

		private static void SetupOutboundSuccessfulProperties(Unit.Message.Mock<IBaseMessage> message)
		{
			message.Setup(m => m.GetProperty(BtsProperties.SendPortName)).Returns("send-port-name");
			message.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");
			message.Setup(m => m.GetProperty(BtsProperties.OutboundTransportType)).Returns("outbound-transport-type");
		}

		private static Dictionary<string, object> ToDictionary(object[] data)
		{
			return Enumerable.Range(0, data.Length / 2)
				.ToDictionary(i => (string) data[i * 2], i => data[i * 2 + 1])
				.OrderBy(kvp => kvp.Key)
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		private readonly byte[] _content = Encoding.Unicode.GetBytes(new string('A', 512));
		private Guid _interchangeId;
		private Guid _messageId;
		private Guid _failedMessageId;
		private DateTime _failureTime;
	}
}

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
using System.IO;
using System.Transactions;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Streaming;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	[TestFixture]
	public class MessageBodyTrackerFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_claimStoreInstance = ClaimStore.Instance;
			ClaimStoreMock = new Mock<ClaimStore>();
			ClaimStore.Instance = ClaimStoreMock.Object;

			MessageMock = new Unit.Message.Mock<IBaseMessage>();
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");

			PipelineContextMock = new Mock<IPipelineContext>();
			PipelineContextMock.Setup(pc => pc.ResourceTracker).Returns(new Mock<IResourceTracker>().Object);

			_trackingResolverFactory = TrackingResolver.Factory;
			TrackingResolverMock = new Mock<TrackingResolver>(null, MessageMock.Object);
			TrackingResolver.Factory = (policyName, message) => TrackingResolverMock.Object;
		}

		[TearDown]
		public void TearDown()
		{
			ClaimStore.Instance = _claimStoreInstance;
			TrackingResolver.Factory = _trackingResolverFactory;
		}

		#endregion

		[Test]
		public void ArchivingAndCaptureAreNotSetupWhenNotRequiredButStreamIsWrappedInTrackingStream()
		{
			using (var stream = new MemoryStream())
			{
				MessageMock.Object.BodyPart.Data = stream;

				var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Context, null));
				sut.SetupTracking();
			}

			Assert.That(MessageMock.Object.BodyPart.Data, Is.TypeOf<TrackingStream>());

			ClaimStoreMock.Verify(
				cs => cs.SetupMessageBodyCapture(It.IsAny<TrackingStream>(), It.IsAny<ActivityTrackingModes>(), It.IsAny<Func<IKernelTransaction>>()),
				Times.Never());

			ClaimStoreMock.Verify(
				cs => cs.SetupMessageBodyArchiving(It.IsAny<TrackingStream>(), It.IsAny<string>(), It.IsAny<Func<IKernelTransaction>>()),
				Times.Never());

			TrackingResolverMock.Verify(
				tr => tr.ResolveArchiveTargetLocation(),
				Times.Never());
		}

		[Test]
		public void ArchivingIsNotSetupIfTrackingStreamAlreadyHasArchiveDescriptor()
		{
			using (var trackingStream = new TrackingStream(new MemoryStream()))
			{
				trackingStream.ArchiveDescriptor = new ArchiveDescriptor("from", "to");
				MessageMock.Object.BodyPart.Data = trackingStream;

				var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Archive, null));
				sut.SetupTracking();
			}

			ClaimStoreMock.Verify(
				cs => cs.SetupMessageBodyArchiving(It.IsAny<TrackingStream>(), It.IsAny<string>(), It.IsAny<Func<IKernelTransaction>>()),
				Times.Never());

			TrackingResolverMock.Verify(
				tr => tr.ResolveArchiveTargetLocation(),
				Times.Never());
		}

		[Test]
		public void ArchivingIsSetup()
		{
			using (var trackingStream = new MemoryStream())
			{
				MessageMock.Object.BodyPart.Data = trackingStream;

				var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Archive, null));
				sut.SetupTracking();
			}

			ClaimStoreMock.Verify(
				cs => cs.SetupMessageBodyArchiving(It.IsAny<TrackingStream>(), It.IsAny<string>(), It.IsAny<Func<IKernelTransaction>>()),
				Times.Once());

			TrackingResolverMock.Verify(
				tr => tr.ResolveArchiveTargetLocation(),
				Times.Once());
		}

		[Test]
		public void AscertainedTrackingModeIsPushedBackInActivityTrackerComponentContext()
		{
			var context = new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Archive | ActivityTrackingModes.Claim, null);
			ClaimStoreMock
				.Setup(
					cs => cs.SetupMessageBodyCapture(It.IsAny<TrackingStream>(), ActivityTrackingModes.Archive | ActivityTrackingModes.Claim, It.IsAny<Func<IKernelTransaction>>()))
				.Returns(ActivityTrackingModes.Step);

			using (var stream = new MemoryStream())
			{
				MessageMock.Object.BodyPart.Data = stream;

				var sut = MessageBodyTracker.Create(context);
				sut.SetupTracking();
			}

			Assert.That(context.TrackingModes, Is.EqualTo(ActivityTrackingModes.Step));
		}

		[Test]
		public void CaptureIsNotSetupIfTrackingStreamAlreadyHasCaptureDescriptor()
		{
			using (var trackingStream = new TrackingStream(new MemoryStream(), new MessageBodyCaptureDescriptor("url", MessageBodyCaptureMode.Claimed)))
			{
				MessageMock.Object.BodyPart.Data = trackingStream;

				var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Body, null));
				sut.SetupTracking();
			}

			ClaimStoreMock.Verify(
				cs => cs.SetupMessageBodyCapture(It.IsAny<TrackingStream>(), It.IsAny<ActivityTrackingModes>(), It.IsAny<Func<IKernelTransaction>>()),
				Times.Never());
		}

		[Test]
		public void CaptureOfInboundMessageIsSetup()
		{
			using (var stream = new MemoryStream())
			{
				MessageMock.Object.BodyPart.Data = stream;

				var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Body, null));
				sut.SetupTracking();
			}

			ClaimStoreMock.Verify(
				cs => cs.SetupMessageBodyCapture(It.IsAny<TrackingStream>(), It.IsAny<ActivityTrackingModes>(), It.Is<Func<IKernelTransaction>>(ktf => ktf != null)),
				Times.Once());

			Assert.That(MessageMock.Object.BodyPart.Data, Is.TypeOf<TrackingStream>());
		}

		[Test]
		public void CaptureOfInboundMessagePiggiesBackKernelTransaction()
		{
			using (new TransactionScope())
			using (var stream = new MemoryStream())
			{
				MessageMock.Object.BodyPart.Data = stream;

				var transaction = TransactionInterop.GetDtcTransaction(Transaction.Current);
				PipelineContextMock.As<IPipelineContextEx>()
					.Setup(pc => pc.GetTransaction())
					.Returns((IKernelTransaction) transaction);

				var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Body, null));
				sut.SetupTracking();

				ClaimStoreMock.Verify(
					cs => cs.SetupMessageBodyCapture(
						It.IsAny<TrackingStream>(),
						It.IsAny<ActivityTrackingModes>(),
						It.Is<Func<IKernelTransaction>>(ktf => ReferenceEquals(ktf(), transaction))),
					Times.Once());
			}
		}

		[Test]
		public void CaptureOfOutboundMessageDoesNotPiggyBackKernelTransaction()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");

			using (new TransactionScope())
			using (var stream = new MemoryStream())
			{
				MessageMock.Object.BodyPart.Data = stream;

				var transaction = TransactionInterop.GetDtcTransaction(Transaction.Current);
				PipelineContextMock.As<IPipelineContextEx>()
					.Setup(pc => pc.GetTransaction())
					.Returns((IKernelTransaction) transaction);

				var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Body, null));
				sut.SetupTracking();

				ClaimStoreMock.Verify(
					cs => cs.SetupMessageBodyCapture(
						It.IsAny<TrackingStream>(),
						It.IsAny<ActivityTrackingModes>(),
						It.Is<Func<IKernelTransaction>>(ktf => ktf == null)),
					Times.Once());
			}
		}

		[Test]
		public void CaptureOfOutboundMessageIsSetup()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");

			using (var stream = new MemoryStream())
			{
				MessageMock.Object.BodyPart.Data = stream;

				var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Body, null));
				sut.SetupTracking();
			}

			ClaimStoreMock.Verify(
				cs => cs.SetupMessageBodyCapture(It.IsAny<TrackingStream>(), It.IsAny<ActivityTrackingModes>(), It.Is<Func<IKernelTransaction>>(ktf => ktf == null)),
				Times.Once());

			Assert.That(MessageMock.Object.BodyPart.Data, Is.TypeOf<TrackingStream>());
		}

		[Test]
		public void InboundMessageBodyIsCheckedIn()
		{
			var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Claim, null));
			sut.TryCheckInMessageBody();

			ClaimStoreMock.Verify(cs => cs.Claim(MessageMock.Object, It.IsAny<IResourceTracker>()), Times.Once());
		}

		[Test]
		public void InboundMessageBodyIsNotCheckedIn()
		{
			var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Body, null));
			sut.TryCheckInMessageBody();

			ClaimStoreMock.Verify(cs => cs.Claim(MessageMock.Object, It.IsAny<IResourceTracker>()), Times.Never());
		}

		[Test]
		public void OutboundMessageBodyIsCheckedOut()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");

			var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Claim, null));
			sut.TryCheckOutMessageBody();

			ClaimStoreMock.Verify(cs => cs.Redeem(MessageMock.Object, It.IsAny<IResourceTracker>()), Times.Once());
		}

		[Test]
		public void OutboundMessageBodyIsNotCheckedOut()
		{
			MessageMock.Setup(m => m.GetProperty(BtsProperties.OutboundTransportLocation)).Returns("outbound-transport-location");

			var sut = MessageBodyTracker.Create(new ActivityTrackerComponent.Context(PipelineContextMock.Object, MessageMock.Object, ActivityTrackingModes.Body, null));
			sut.TryCheckOutMessageBody();

			ClaimStoreMock.Verify(cs => cs.Redeem(MessageMock.Object, It.IsAny<IResourceTracker>()), Times.Never());
		}

		private Mock<ClaimStore> ClaimStoreMock { get; set; }

		private Unit.Message.Mock<IBaseMessage> MessageMock { get; set; }

		private Mock<IPipelineContext> PipelineContextMock { get; set; }

		private Mock<TrackingResolver> TrackingResolverMock { get; set; }

		private ClaimStore _claimStoreInstance;
		private Func<PolicyName, IBaseMessage, TrackingResolver> _trackingResolverFactory;
	}
}

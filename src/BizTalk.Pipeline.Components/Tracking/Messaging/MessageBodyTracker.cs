#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.Logging;

namespace Be.Stateless.BizTalk.Tracking.Messaging
{
	// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
	internal class MessageBodyTracker
	{
		internal static MessageBodyTracker Create(MicroComponent.ActivityTracker.Context context)
		{
			return Factory(context);
		}

		#region Mock's Factory Hook Point

		internal static Func<MicroComponent.ActivityTracker.Context, MessageBodyTracker> Factory
		{
			get { return _factory; }
			set { _factory = value; }
		}

		#endregion

		// ReSharper disable once MemberCanBePrivate.Global
		// ctor is protected for mocking purposes
		protected MessageBodyTracker(MicroComponent.ActivityTracker.Context context)
		{
			_context = context;
		}

		private Func<IKernelTransaction> KernelTransactionFactory
		{
			get
			{
				// kernelTransactionFactory exists only for inbound messages: only receive pipelines support transaction piggybacking
				var kernelTransactionFactory = _context.Message.Direction().IsInbound()
					? () => _context.PipelineContext.GetKernelTransaction()
					: (Func<IKernelTransaction>) null;
				return kernelTransactionFactory;
			}
		}

		/// <summary>
		/// Setup a <see cref="TrackingStream"/> that will enable all archiving, capture and tracking requirements.
		/// </summary>
		/// <remarks>
		/// Setup a TrackingStream, if not already done, to ensure that:
		/// <list type="bullet">
		/// <item><description>Stream can be probed;</description></item>
		/// <item><description>Payload can be captured and possibly archived;</description></item>
		/// <item><description>Activity tracking occurs at end of stream.</description></item>
		/// </list>
		/// </remarks>
		internal virtual void SetupTracking()
		{
			// TODO track other parts than BodyPart as well

			// reuse the TrackingStream setup by an earlier CheckOut/Redeem operation if there is one or install a new one
			_trackingStream = _context.Message.BodyPart.GetOriginalDataStream() as TrackingStream
				?? _context.Message.BodyPart.WrapOriginalDataStream(stream => new TrackingStream(stream), _context.PipelineContext.ResourceTracker);

			if (_context.TrackingModes.RequiresBodyTracking() && _trackingStream.CaptureDescriptor == null)
			{
				_context.TrackingModes = ClaimStore.Instance.SetupMessageBodyCapture(_trackingStream, _context.TrackingModes, KernelTransactionFactory);
			}

			if (_context.TrackingModes.RequiresBodyArchiving() && _trackingStream.ArchiveDescriptor == null)
			{
				ClaimStore.Instance.SetupMessageBodyArchiving(_trackingStream, _context.TrackingResolver.ResolveArchiveTargetLocation(), KernelTransactionFactory);
			}
		}

		/// <summary>
		/// Replace the business message body's payload stream with either a <see cref="Claim.Check"/> or a <see
		/// cref="Claim.CheckIn"/> token message if its content has been assessed to be captured to disk while being
		/// tracked. Leave the message body's payload stream unaltered otherwise.
		/// </summary>
		internal virtual void TryCheckInMessageBody()
		{
			if (_context.Message.Direction().IsInbound() && _context.TrackingModes.RequiresBodyClaimChecking())
			{
				if (_logger.IsDebugEnabled) _logger.Debug("Claiming token and checking in message payload");
				ClaimStore.Instance.Claim(_context.Message, _context.PipelineContext.ResourceTracker);
			}
		}

		/// <summary>
		/// Restore the original business message body's payload stream if it has been saved to disk while being tracked,
		/// that is if current payload is either of the <see cref="Claim.Check"/> or <see cref="Claim.CheckOut"/> token
		/// message. Leave the message body's payload stream unaltered otherwise.
		/// </summary>
		internal virtual void TryCheckOutMessageBody()
		{
			if (_context.Message.Direction().IsOutbound() && _context.TrackingModes.RequiresBodyClaimChecking())
			{
				if (_logger.IsDebugEnabled) _logger.Debug("Redeeming token and checking out message payload");
				ClaimStore.Instance.Redeem(_context.Message, _context.PipelineContext.ResourceTracker);
			}
		}

		private static Func<MicroComponent.ActivityTracker.Context, MessageBodyTracker> _factory = context => new MessageBodyTracker(context);

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ActivityTracker));
		private readonly MicroComponent.ActivityTracker.Context _context;
		private TrackingStream _trackingStream;
	}
}

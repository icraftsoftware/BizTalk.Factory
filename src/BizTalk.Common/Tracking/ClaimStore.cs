#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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
using System.IO;
using System.Net;
using System.Threading;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Tracking.Messaging;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Logging;
using Be.Stateless.Xml.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Path = System.IO.Path;

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Central file store where message body's payloads are claimed, or stored; that is to say where message body's
	/// payloads are written to disk instead of flowing through the database, like the BAM monitoring database or the
	/// <c>BizTalkMsgBoxDb</c>.
	/// </summary>
	[SuppressMessage("ReSharper", "LocalizableElement")]
	internal class ClaimStore
	{
		public static ClaimStore Instance
		{
			get { return _instance; }
			internal set { _instance = value; }
		}

		internal static string CheckInDirectory
		{
			get
			{
				// TODO ?? sure to want SSO auto cache refresh to be discarded as value is cached as static ??
				if (_checkInDirectory != null) return _checkInDirectory;
				var directory = BizTalkFactorySettings.ClaimStoreCheckInDirectory.TrimEnd('\\');
				Interlocked.CompareExchange(ref _checkInDirectory, directory, null);
				return directory;
			}
			set { _checkInDirectory = value; }
		}

		/// <summary>
		/// Folder in the claim store where the payload of all the tracked message bodies will be moved to.
		/// </summary>
		/// <remarks>
		/// To locate the payload of a business message body, one has to combine this <see cref="CheckOutDirectory"/> with
		/// the claim token's url (see <see cref="MessageBodyCaptureDescriptor.Data"/> when <see
		/// cref="MessageBodyCaptureDescriptor.CaptureMode"/> is <see cref="MessageBodyCaptureMode.Claimed"/>) to get the
		/// full path to the claim.
		/// </remarks>
		internal static string CheckOutDirectory
		{
			get
			{
				// TODO ?? sure to want SSO auto cache refresh to be discarded as value is cached as static ??
				if (_checkOutDirectory != null) return _checkOutDirectory;
				var directory = BizTalkFactorySettings.ClaimStoreCheckOutDirectory.TrimEnd('\\');
				Interlocked.CompareExchange(ref _checkOutDirectory, directory, null);
				return directory;
			}
			set { _checkOutDirectory = value; }
		}

		#region Message Body Capture

		/// <summary>
		/// Ascertain, i.e. assess and setup, whether the payload of a message body <paramref name="trackingStream"/>
		/// needs to be captured outside of the BAM monitoring database, that is in the <see cref="ClaimStore"/>, or not.
		/// </summary>
		/// <param name="trackingStream">
		/// The stream to assess.
		/// </param>
		/// <param name="trackingModes">
		/// The extent to which a messaging activity will be recorded.
		/// </param>
		/// <param name="transactionFactory">
		/// The <see cref="IKernelTransaction"/> factory method whose offspring transaction has to be piggybacked if the
		/// message body's payload needs to be claimed to disk. It can be <c>null</c> if piggybacking is not desired nor
		/// possible, like for instance when calling this method from a send pipeline, as BizTalk does not provide for
		/// transaction piggybacking in send pipelines.
		/// </param>
		/// <returns>
		/// The ascertained <see cref="ActivityTrackingModes"/>, that is the one that will actually be applied.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If the size of the <see cref="TrackingStream"/> does not exceed a 512KB threshold, the <see
		/// cref="TrackingStream"/>'s capture will be setup according to a <see cref="MessageBodyCaptureDescriptor"/>
		/// whose <see cref="MessageBodyCaptureDescriptor.Data"/> will contain a base64-encoding of the compressed stream
		/// content.
		/// </para>
		/// <para>
		/// If the size of the <see cref="TrackingStream"/> exceeds the 512KB threshold, the <see
		/// cref="TrackingStream"/>'s capture will be setup according to a <see cref="MessageBodyCaptureDescriptor"/>
		/// whose <see cref="MessageBodyCaptureDescriptor.Data"/> will contain the relative URL that can be used to get
		/// back the payload from the <see cref="ClaimStore"/>.
		/// </para>
		/// <para>
		/// The 512KB threshold has been chosen according to the BizTalk documentation which proscribes BAM
		/// <c>LongReferenceData</c> item of more than 512 KB.
		/// </para>
		/// </remarks>
		public virtual void SetupMessageBodyCapture(TrackingStream trackingStream, ActivityTrackingModes trackingModes, Func<IKernelTransaction> transactionFactory)
		{
			if (trackingStream == null) throw new ArgumentNullException("trackingStream");

			if (_logger.IsDebugEnabled)
				_logger.DebugFormat(
					"Message body capture is being set up for '{0}' tracking requirements.",
					Convert.ToString(trackingModes));

			// never want to claim (save to disk) a payload if it is small, hence always probe for its size
			string encodedCompression;
			var isCompressible = trackingStream.AsMarkable().TryCompressToBase64String(PAYLOAD_SIZE_THRESHOLD, out encodedCompression);
			if (isCompressible)
			{
				var captureDescriptor = new MessageBodyCaptureDescriptor(encodedCompression, MessageBodyCaptureMode.Unclaimed);
				trackingStream.SetupCapture(captureDescriptor);
			}
			else
			{
				var captureDescriptor = new MessageBodyCaptureDescriptor(GenerateClaimStoreEntry(), MessageBodyCaptureMode.Claimed);
				var capturingStream = CreateCapturingStream(captureDescriptor.Data, trackingModes, transactionFactory);
				trackingStream.SetupCapture(captureDescriptor, capturingStream);
			}
		}

		/// <summary>
		/// Create a <see cref="Stream"/> to an entry in the claim store that piggies back a kernel transaction if one can
		/// be factored.
		/// </summary>
		/// <param name="url">
		/// The claim store entry.
		/// </param>
		/// <param name="trackingModes">
		/// </param>
		/// <param name="transactionFactory">
		/// The <see cref="IKernelTransaction"/> factory.
		/// </param>
		/// <returns>
		/// The <see cref="Stream"/> to the claim store entry.
		/// </returns>
		private Stream CreateCapturingStream(string url, ActivityTrackingModes trackingModes, Func<IKernelTransaction> transactionFactory)
		{
			// RequiresCheckInAndOut entails payloads are first saved locally and moved by ClaimStore.Agent into claim
			// store where there will be a subfolder for each date that some payload has been saved/tracked to disk. To
			// ease the job of the ClaimStore.Agent the subfolder is not created locally (but the current date is however
			// kept in the name). When not RequiresCheckInAndOut, the payloads are tracked/saved to disk at the exact same
			// place that it will be redeemed from afterwards, that is in a subfolder corresponding to the current date (of
			// course, one needs to ensure the folders get created).

			string filePath;
			if (RequiresCheckInAndOut)
			{
				// .trk extension is used to denote simple message body's payload tracking scenarios, i.e. where the
				// message's payload is large enough that it would not fit in the BAM tracking database. Message body's
				// payload are therefore tracked to the local disk are then brought asynchronously to the central claim
				// store by the ClaimStore.Agent.

				// .chk extension is used to denote full-fledged claim check scenarios, i.e. where the actual message's
				// payload is claimed to disk (as with regular tracking) but also replaced by a token that needs to be
				// checked in and out. Message body's payload are claimed to the local disk and then brought asynchronously
				// to the central claim store by the ClaimStore.Agent. Because claims are brought asynchronously to the
				// central claim store, one has therefore to ensure a claim is available in the central claim store before
				// it could be redeemed.

				// The .chk and .trk extensions are there to allow the ClaimStore.Agent to distinguish these scenarios so
				// that it can, (1) bring claimed or tracked payloads to the central claim store and (2) make claims
				// available for redeem as soon as they have been brought to the central store.

				var extension = trackingModes.RequiresBodyClaimChecking() ? ".chk" : ".trk";
				filePath = Path.Combine(CheckInDirectory, url.Replace("\\", "") + extension);
			}
			else
			{
				filePath = Path.Combine(CheckInDirectory, url);
				// ReSharper disable once AssignNullToNotNullAttribute
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			}
			return FileTransacted.Create(filePath, 8192, transactionFactory.IfNotNull(ktf => ktf()));
		}

		/// <summary>
		/// Generates a unique file name according to the pattern expected by the tracking agent that moves locally
		/// claimed streams to the central claim store, that is from the <see cref="CheckInDirectory"/> to the <see
		/// cref="CheckOutDirectory"/>.
		/// </summary>
		/// <returns>
		/// A unique file name.
		/// </returns>
		/// <remarks>
		/// The file name is generated according to the following pattern: <c>&lt;yyyyMMdd&gt;\&lt;guid&gt;</c>, where
		/// <list type="bullet">
		/// <item><description>
		/// <c>&lt;yyyyMMdd&gt;</c> is the current date, and denotes a subfolder in the claim store where all the message
		/// tracked at the same date will be store;
		/// </description></item>
		/// <item><description>
		/// <c>&lt;guid&gt;</c> is a <see cref="Guid"/> formatted as an hex string without any dashes or braces.
		/// </description></item>
		/// </list>
		/// </remarks>
		private string GenerateClaimStoreEntry()
		{
			return Path.Combine(DateTime.Today.ToString("yyyyMMdd"), Guid.NewGuid().ToString("N"));
		}

		#endregion

		#region Claim Token Message Processing

		/// <summary>
		/// Whether a <see cref="FileStream"/> that has to be claimed into the <see cref="ClaimStore"/> requires to be
		/// checked in and out, or simply just saved into it.
		/// </summary>
		/// <remarks>
		/// <para>
		/// When a business message body is required to be checked in and out of the <see cref="ClaimStore"/>, its payload
		/// will first be claimed on the server's local file system (see <see cref="CheckInDirectory"/>), and then moved
		/// asynchronously to a network-shared file system (see <see cref="CheckOutDirectory"/>). For this reason, the
		/// business message body's payload will be replaced by a <see cref="Schemas.Xml.Claim.CheckIn"/> token message
		/// that will mutate into a <see cref="Schemas.Xml.Claim.CheckOut"/> token message as soon as it can be redeemed
		/// from the <see cref="ClaimStore"/>; that is when the the original business message body's payload has been made
		/// available in the <see cref="ClaimStore"/>.
		/// </para>
		/// <para>
		/// When a business message body is not required to be checked in and out of the <see cref="ClaimStore"/>, its
		/// payload will be directly saved in a shared location and will not need to be moved asynchronously to another
		/// location. For this reason, the business message body's payload will be replaced by an immutable <see
		/// cref="Schemas.Xml.Claim.Check"/> token message that can be readily redeemed from the <see cref="ClaimStore"/>.
		/// </para>
		/// </remarks>
		internal static bool RequiresCheckInAndOut
		{
			get { return !string.Equals(Path.GetFullPath(CheckInDirectory), Path.GetFullPath(CheckOutDirectory), StringComparison.OrdinalIgnoreCase); }
		}

		/// <summary>
		/// Replace the message body's payload stream with either a <see cref="Schemas.Xml.Claim.Check"/> or a <see
		/// cref="Schemas.Xml.Claim.CheckIn"/> token message if its content has been assessed to be saved to disk while
		/// being tracked (see <see cref="SetupMessageBodyCapture"/>). Leave the message body's payload stream unaltered
		/// otherwise.
		/// </summary>
		/// <param name="message">
		/// The <see cref="IBaseMessage"/> whose message body's payload stream is going to be claimed and replace by a
		/// token message.
		/// </param>
		/// <param name="resourceTracker">
		/// Pipeline's <see cref="IResourceTracker"/> to which to report the newly created message token stream.
		/// </param>
		/// <remarks>
		/// The <see cref="IBaseMessage"/>'s <see cref="IBaseMessageContext"/> is also updated with the message type of
		/// the token message that is put in place of the actual message body's payload.
		/// </remarks>
		public virtual void Claim(IBaseMessage message, IResourceTracker resourceTracker)
		{
			var trackingStream = message.BodyPart.GetOriginalDataStream() as TrackingStream;

			if (trackingStream == null)
			{
				if (_logger.IsDebugEnabled) _logger.Debug("Skipping claim of message body stream's payload; BodyPart's OriginalDataStream is not a TrackingStream.");
			}
			else if (trackingStream.CaptureDescriptor.CaptureMode != MessageBodyCaptureMode.Claimed)
			{
				if (_logger.IsDebugEnabled) _logger.Debug("Skipping claim of message body stream's payload; CaptureDescriptor.CaptureMode is not MessageBodyCaptureMode.Claimed.");
			}
			else
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Performing claim of message body stream's payload '{0}'.", trackingStream.CaptureDescriptor.Data);
				var claimedMessageType = message.GetProperty(BtsProperties.MessageType);
				DocumentSpec documentSpec;
				Stream tokenMessageStream;
				if (RequiresCheckInAndOut)
				{
					documentSpec = typeof(Claim.CheckIn).GetMetadata().DocumentSpec;
					tokenMessageStream = MessageFactory.CreateClaimCheckIn(claimedMessageType, trackingStream.CaptureDescriptor.Data).AsStream();
				}
				else
				{
					documentSpec = typeof(Claim.Check).GetMetadata().DocumentSpec;
					tokenMessageStream = MessageFactory.CreateClaimCheck(claimedMessageType, trackingStream.CaptureDescriptor.Data).AsStream();
				}

				// fix the message type before capturing the body stream so that it get captured while tracking MessagingStep
				message.Promote(BtsProperties.MessageType, documentSpec.DocType);
				message.Promote(BtsProperties.SchemaStrongName, documentSpec.DocSpecStrongName);

				// drain and capture body stream
				trackingStream.Capture();

				// replace message body's payload with the token message stream
				message.BodyPart.SetDataStream(tokenMessageStream, resourceTracker);
			}
		}

		/// <summary>
		/// Restore the previously claimed message body's payload via a <see cref="TrackingStream"/> stream if it has been
		/// claimed to disk while being processed, that is if current payload is either one of the <see
		/// cref="Schemas.Xml.Claim.Check"/> or <see cref="Schemas.Xml.Claim.CheckOut"/> token messages. Leave the message
		/// body's payload stream unaltered otherwise.
		/// </summary>
		/// <param name="message">
		/// The <see cref="IBaseMessage"/> token message whose message body's payload stream is going to be restored to
		/// the payload that has been previously claimed.
		/// </param>
		/// <param name="resourceTracker">
		/// Pipeline's <see cref="IResourceTracker"/> to which to register the restored message body stream for later
		/// cleanup.
		/// </param>
		/// <remarks>
		/// The <see cref="TrackingStream"/> that will replace the <paramref name="message"/> stream with its previously
		/// captured original payload, will have its <see cref="TrackingStream.CaptureDescriptor"/> point to the previously
		/// captured-to-disk body payload, thereby providing an opportunity for the <see cref="MessagingStep"/> to share
		/// that already captured payload without saving it to disk again.
		/// </remarks>
		public virtual void Redeem(IBaseMessage message, IResourceTracker resourceTracker)
		{
			var messageType = message.GetOrProbeMessageType(resourceTracker);
			if (messageType == typeof(Claim.Check).GetMetadata().MessageType || messageType == typeof(Claim.CheckOut).GetMetadata().MessageType)
			{
				var messageBodyCaptureDescriptor = message.BodyPart.AsMessageBodyCaptureDescriptor();
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Performing redeem of claim check token '{0}'.", messageBodyCaptureDescriptor.Data);
				// replace claim token with actual content
				message.BodyPart.SetDataStream(
					new TrackingStream(OpenClaim(messageBodyCaptureDescriptor.Data), messageBodyCaptureDescriptor),
					resourceTracker);
			}
			else if (messageType == typeof(Claim.CheckIn).GetMetadata().MessageType)
			{
				throw new InvalidOperationException(
					string.Format(
						"Invalid token message, {0} token is not expected to be redeemed.",
						typeof(Claim.CheckIn).GetMetadata().RootElementName));
			}
			else
			{
				if (_logger.IsDebugEnabled) _logger.Debug("Skipping redeem of claim check token; message body stream's payload is not a token.");
			}
		}

		private Stream OpenClaim(string url)
		{
			// if can't create Uri because url is not absolute, combine it with CheckOutDirectory, otherwise assume it is
			// correct. This allows to redeem payloads from e.g. http resources without having to check them in beforehand.
			Uri uri;
			if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
			{
				uri = new Uri(Path.Combine(CheckOutDirectory, url), UriKind.Absolute);
			}
			// TODO SECURITY: don't accept/allow any Uri scheme and Urls to resources from anywhere
			return new WebClient().OpenRead(uri);
		}

		#endregion

		// 512 KB of data (512 * 1024), base64 encoding: 3 bytes encoded in 4 chars (3/4), each char takes 2 bytes in UTF-16 (/2)
		private const int PAYLOAD_SIZE_THRESHOLD = 512 * 1024 * 3 / 4 / 2;
		private static string _checkInDirectory;
		private static string _checkOutDirectory;
		private static ClaimStore _instance = new ClaimStore();
		private static readonly ILog _logger = LogManager.GetLogger(typeof(ClaimStore));
	}
}

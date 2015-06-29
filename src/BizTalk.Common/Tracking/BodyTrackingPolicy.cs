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
using Be.Stateless.BizTalk.Runtime;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.Extensions;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Logging;

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Claim the payload of a tracked message body, given as a <see cref="TrackingStream"/>, to disk if its size exceeds a
	/// 512KB threshold or leave it unclaimed otherwise.
	/// </summary>
	public class BodyTrackingPolicy : IBodyAssessmentPolicy
	{
		public static BodyTrackingPolicy Instance
		{
			get { return _instance; }
		}

		private BodyTrackingPolicy() { }

		#region IBodyAssessmentPolicy Members

		/// <summary>
		/// Assess whether the payload stream of a message body, <paramref name="stream"/>, needs to be checked in
		/// (i.e. <see cref="BodyTrackingMode.Claimed"/>) or not. That is to say whether the payload stream will make its
		/// way to the database, e.g. the <c>BizTalkMsgBoxDb</c> or the BAM monitoring database, or be routed to a file in
		/// a central claim store.
		/// </summary>
		/// <param name="stream">
		/// The payload stream of the message body to assess for checkin.
		/// </param>
		/// <param name="kernelTransactionFactory">
		/// The <see cref="IKernelTransaction"/> factory method whose offspring transaction has to be piggybacked if the
		/// message body's payload needs to be claimed. It can be <c>null</c> if piggybacking is not desired nor possible,
		/// like for instance when calling this method from a send pipeline, as BizTalk does not provide for transaction
		/// piggybacking in send pipelines.
		/// </param>
		/// <returns>
		/// A <see cref="BodyTrackingDescriptor"/> describing whether and how the stream payload of a message body will
		/// actually be claimed to disk while being read.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If the size of the <see cref="TrackingStream"/> does not exceed a 512KB threshold, returns a <see
		/// cref="BodyTrackingDescriptor"/> whose <see cref="BodyTrackingDescriptor.Data"/> will contain a base64-encoding
		/// of the compressed stream content.
		/// </para>
		/// <para>
		/// If the size of the <see cref="TrackingStream"/> exceeds the 512KB threshold, returns a <see
		/// cref="BodyTrackingDescriptor"/> whose <see cref="BodyTrackingDescriptor.Data"/> will contain the relative (to
		/// a central claim-store directory) path of the file where the stream content has been claimed to.
		/// </para>
		/// <para>
		/// The 512KB threshold has been chosen according to the BizTalk documentation which prohibits BAM
		/// LongReferenceData item of more than 512 KB.
		/// </para>
		/// </remarks>
		/// <seealso cref="BodyTrackingPolicy"/>
		public BodyTrackingDescriptor Assess<T>(T stream, Func<IKernelTransaction> kernelTransactionFactory) where T : Stream, IAssessableStream
		{
			if (stream == null) throw new ArgumentNullException("stream");

			// 512 KB of data (512 * 1024), base64 encoding: 3 bytes encoded in 4 chars (3/4), each char takes 2 bytes in UTF-16 (/2)
			const int claimedMessageThreshold = 512 * 1024 * 3 / 4 / 2;

			if (_logger.IsDebugEnabled) _logger.Debug("Assessing claim type...");
			BodyTrackingDescriptor descriptor;
			Stream replicatingStream = null;
			stream.InitiateAssessment();
			var encodedCompression = stream.CompressToBase64String(claimedMessageThreshold);
			if (encodedCompression != null)
			{
				// return a base64-encoding of the compressed stream content if threshold has not been reached
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Unclaimed message: stream content length is smaller than threshold ({0}).", claimedMessageThreshold);
				descriptor = new BodyTrackingDescriptor(encodedCompression, BodyTrackingMode.Unclaimed);
			}
			else
			{
				// return a claim to the stream content if threshold has been reached
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Claimed message: stream content length is larger than threshold ({0}).", claimedMessageThreshold);
				// piggyback kernel transaction if one can be factored
				var handle = ClaimStore.CreateClaim(kernelTransactionFactory.IfNotNull(ktf => ktf()));
				descriptor = new BodyTrackingDescriptor(handle.Url, BodyTrackingMode.Claimed);
				replicatingStream = handle.FileStream;
			}
			stream.CompleteAssessment(descriptor, replicatingStream);
			return descriptor;
		}

		#endregion

		private static readonly ILog _logger = LogManager.GetLogger(typeof(BodyTrackingPolicy));
		private static readonly BodyTrackingPolicy _instance = new BodyTrackingPolicy();
	}
}

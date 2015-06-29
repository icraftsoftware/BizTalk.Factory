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
using Be.Stateless.BizTalk.Streaming;

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Contract interface for body assessment policies.
	/// </summary>
	public interface IBodyAssessmentPolicy
	{
		/// <summary>
		/// Assess whether the payload stream of a message body, <paramref name="stream"/>, needs to be checked in (i.e.
		/// <see cref="BodyTrackingMode.Claimed"/>) or not. That is to say whether the payload stream will make its way to
		/// the database, e.g. the <c>BizTalkMsgBoxDb</c> or the BAM monitoring database, or be routed to a file in a
		/// central claim store.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="Stream"/>-derived stream to assess, which also implements the <see cref="IAssessableStream"/>
		/// interface.
		/// </typeparam>
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
		/// <seealso cref="BodyTrackingPolicy"/>
		BodyTrackingDescriptor Assess<T>(T stream, Func<IKernelTransaction> kernelTransactionFactory) where T : Stream, IAssessableStream;
	}
}

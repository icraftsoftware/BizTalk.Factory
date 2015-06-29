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

using System.IO;
using Be.Stateless.BizTalk.Tracking;

namespace Be.Stateless.BizTalk.Streaming
{
	/// <summary>
	/// Provide a stream with assessment, which is typically performed when one needs to assess whether the stream
	/// content actually needs to be tracked outside of the BAM monitoring database because its content exceeds the
	/// amount of data allocated in the database to this end. See for instance <see cref="BodyTrackingPolicy"/>.
	/// </summary>
	public interface IAssessableStream
	{
		/// <summary>
		/// Initiate the assessment of a stream.
		/// </summary>
		void InitiateAssessment();

		/// <summary>
		/// Conclude the assessment of a stream and provide information about its outcome.
		/// </summary>
		/// <param name="descriptor">
		/// General information about the outcome of the assessment. For instance, in the case of a <see
		/// cref="TrackingStream"/>, the <paramref name="descriptor"/> will indicates whether the <see
		/// cref="TrackingStream"/> stream content will actually flow through to the database (e.g. the BAM monitoring
		/// database) or be claimed to disk instead. See <see cref="BodyTrackingDescriptor.TrackingMode"/> which can
		/// either be <see cref="BodyTrackingMode.Claimed"/> or <see cref="BodyTrackingMode.Unclaimed"/>.
		/// </param>
		/// <param name="targetStream">
		/// The <see cref="Stream"/> to which to claim, or replicate this stream's content to as the same time it is being
		/// read, if the outcome of the assessment is <see cref="BodyTrackingMode.Claimed"/>. <paramref
		/// name="targetStream"/> is disregarded otherwise.
		/// </param>
		void CompleteAssessment(BodyTrackingDescriptor descriptor, Stream targetStream);
	}
}

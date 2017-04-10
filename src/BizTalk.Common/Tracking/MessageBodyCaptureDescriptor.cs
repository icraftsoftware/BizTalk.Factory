#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Describes how and where a message payload stream will be, or has been, captured.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A <see cref="MessageBodyCaptureDescriptor"/> describes how a message body is being captured so that it could be
	/// both tracked in the BAM monitoring database or participate in a claim-check MEP.
	/// </para>
	/// <para>
	/// A <see cref="MessageBodyCaptureDescriptor"/> whose <see cref="CaptureMode"/> is equal to <see
	/// cref="MessageBodyCaptureMode.Unclaimed"/> denotes a message body whose payload is being captured directly in the
	/// BAM monitoring database. In this case, <see cref="Data"/> denotes the actual data that are going to be stored in
	/// the monitoring database; specifically, the actual message body payload compressed and encoded as a Base64 string.
	/// </para>
	/// <para>
	/// A <see cref="MessageBodyCaptureDescriptor"/> whose <see cref="CaptureMode"/> is equal to <see
	/// cref="MessageBodyCaptureMode.Claimed"/> denotes a message body whose payload is being captured outside of the
	/// BAM monitoring database. In this case, <see cref="Data"/> denotes the relative path to a file in the claim store
	/// where the message body stream will be, or has been, committed to.
	/// </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "LocalizableElement")]
	public class MessageBodyCaptureDescriptor
	{
		internal MessageBodyCaptureDescriptor(string data, MessageBodyCaptureMode captureMode)
		{
			if (data == null) throw new ArgumentNullException("data");
			// If we have a claimed message body, the data parameter must always contain the path to the claimed file,
			// and thus may never be empty. Otherwise, an empty message body is possible and legal.
			if (captureMode == MessageBodyCaptureMode.Claimed && data.Length == 0) throw new ArgumentException("Value cannot be empty, it is expected to be a relative path for claimed message bodies.", "data");
			Data = data;
			CaptureMode = captureMode;
		}

		/// <summary>
		/// Whether the payload of a message body is being captured directly into the BAM monitoring database or not, see
		/// <see cref="MessageBodyCaptureMode.Unclaimed"/> and <see cref="MessageBodyCaptureMode.Claimed"/>
		/// respectively.
		/// </summary>
		/// <remarks>
		/// If it equals to <see cref="MessageBodyCaptureMode.Unclaimed"/>, <see cref="Data"/> denotes the actual payload
		/// of the message body compressed and encoded as a Base64 string. If it is equals to <see
		/// cref="MessageBodyCaptureMode.Claimed"/>, <see cref="Data"/> denotes the relative path to a file in the claim
		/// store where the message body stream will be, or has been, committed to.
		/// </remarks>
		public MessageBodyCaptureMode CaptureMode { get; private set; }

		/// <summary>
		/// Either the message body's actual payload or the relative path to a file in the claim store where the message
		/// body stream will be, or has been, committed to.
		/// </summary>
		/// <remarks>
		/// If <see cref="CaptureMode"/> is equals to <see cref="MessageBodyCaptureMode.Unclaimed"/>, it denotes the
		/// actual payload of the message body compressed and encoded as a Base64 string. If <see cref="CaptureMode"/> is
		/// equals to <see cref="MessageBodyCaptureMode.Claimed"/>, it denotes the relative path to a file in the claim
		/// store where the message body stream will be, or has been, committed to.
		/// </remarks>
		public string Data { get; private set; }
	}
}

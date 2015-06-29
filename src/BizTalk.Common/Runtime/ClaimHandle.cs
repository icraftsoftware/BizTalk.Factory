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
using Be.Stateless.IO;

namespace Be.Stateless.BizTalk.Runtime
{
	/// <summary>
	/// Handle to a file-based claim, that is where the payload of a message body will be written to disk instead of the
	/// database, like for instance the BAM monitoring database or the <c>BizTalkMsgBoxDb</c>.
	/// </summary>
	public class ClaimHandle
	{
		/// <summary>
		/// Creates and initializes a new <see cref="ClaimHandle"/>.
		/// </summary>
		/// <param name="stream">
		/// The <see cref="FileStream"/> where to write, or claim, the message body's payload to.
		/// </param>
		/// <param name="url">
		/// The location of the claim within the central <see cref="ClaimStore"/>.
		/// </param>
		internal ClaimHandle(FileStream stream, string url)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (url == null) throw new ArgumentNullException("url");
			_stream = stream;
			_url = url;
		}

		/// <summary>
		/// <see cref="FileStream"/> where to write, or claim, the message body's payload to.
		/// </summary>
		/// <remarks>
		/// Either a <see cref="FileStream"/> or a <see cref="FileStreamTransacted"/> depending on whether the transaction
		/// outcome has to be explicitly managed, and provided the local file system supports transactions; see <see
		/// cref="ClaimStore.CreateClaim"/>.
		/// </remarks>
		public FileStream FileStream
		{
			get { return _stream; }
		}

		/// <summary>
		/// Location of the claim within the central <see cref="ClaimStore"/>.
		/// </summary>
		/// <remarks>
		/// Notice the <see cref="Url"/> is mangled as prescribed by the tracking agent that moves locally claimed streams
		/// to the central claim store.
		/// </remarks>
		public string Url
		{
			get { return _url; }
		}

		private readonly FileStream _stream;
		private readonly string _url;
	}
}

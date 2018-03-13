#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.Net.Http;

namespace Be.Stateless.BizTalk.Streaming
{
	public class MultipartFormDataContentStream : Stream
	{
		public MultipartFormDataContentStream(Stream stream)
		{
			_multipartContent = new MultipartFormDataContent { new StreamContent(stream) };
			_stream = _multipartContent.ReadAsStreamAsync().Result;
		}

		#region Base Class Member Overrides

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _multipartContent != null)
			{
				_stream.Dispose();
				_stream = null;
				_multipartContent.Dispose();
				_multipartContent = null;
			}
			base.Dispose(disposing);
		}

		public override void Flush()
		{
			_stream.Flush();
		}

		public override long Length
		{
			get { throw new NotSupportedException(); }
		}

		public override long Position { get; set; }

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _stream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		#endregion

		public string ContentType
		{
			get { return _multipartContent.Headers.ContentType.ToString(); }
		}

		private MultipartFormDataContent _multipartContent;
		private Stream _stream;
	}
}

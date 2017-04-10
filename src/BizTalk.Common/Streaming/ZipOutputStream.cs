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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Be.Stateless.BizTalk.Streaming
{
	/// <summary>
	/// Wraps a zip-compressing stream around a data stream.
	/// </summary>
	/// <remarks>
	/// <see cref="ZipOutputStream"/> relies on <see cref="ICSharpCode.SharpZipLib.Zip.ZipOutputStream"/> for the
	/// compression and will have exactly one <see cref="ZipEntry"/>.
	/// </remarks>
	/// <seealso href="http://github.com/icsharpcode/SharpZipLib/wiki/Zip-Samples">SharpZipLib Samples</seealso>
	/// <seealso href="http://my.safaribooksonline.com/book/operating-systems-and-server-administration/microsoft-biztalk/9780470046425/pipelines/121"></seealso>
	[SuppressMessage("ReSharper", "LocalizableElement")]
	public class ZipOutputStream : Stream
	{
		#region Nested Type: BufferStream

		private class BufferStream : Stream
		{
			public BufferStream(ZipOutputStream zipOutputStream)
			{
				_zipOutputStream = zipOutputStream;
			}

			#region Base Class Member Overrides

			public override bool CanRead
			{
				get { return false; }
			}

			public override bool CanSeek
			{
				get { return false; }
			}

			public override bool CanWrite
			{
				get { return true; }
			}

			public override void Close() { }

			public override void Flush() { }

			public override long Length
			{
				get { throw new NotSupportedException(); }
			}

			public override long Position
			{
				get { throw new NotSupportedException(); }
				set { throw new NotSupportedException(); }
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
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
				if (buffer == null) throw new ArgumentNullException("buffer");
				if (offset < 0) throw new ArgumentOutOfRangeException("offset", "Cannot be negative.");
				if (count < 0) throw new ArgumentOutOfRangeException("count", "Cannot be negative.");
				if (offset + count > buffer.Length) throw new ArgumentException("The sum of offset and count is greater than the byte array length.");

				_zipOutputStream.AppendCompressedBytes(buffer, offset, count);
			}

			#endregion

			private readonly ZipOutputStream _zipOutputStream;
		}

		#endregion

		public ZipOutputStream(Stream streamToCompress, string zipEntryName) : this(streamToCompress, zipEntryName, 4096) { }

		public ZipOutputStream(Stream streamToCompress, string zipEntryName, int bufferSize)
		{
			if (streamToCompress == null) throw new ArgumentNullException("streamToCompress");
			if (zipEntryName.IsNullOrEmpty()) throw new ArgumentNullException("zipEntryName");
			if (bufferSize <= 0) throw new ArgumentOutOfRangeException("bufferSize", "Buffer size must be strictly positive.");

			_streamToCompress = streamToCompress;
			_zipEntryName = zipEntryName;
			_buffer = new byte[bufferSize];
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

		public override void Close()
		{
			if (_streamToCompress != null)
			{
				_streamToCompress.Close();
				_streamToCompress = null;
			}
			if (_compressedStream != null)
			{
				_compressedStream.Close();
				_compressedStream = null;
			}
			base.Close();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override long Length
		{
			get { throw new NotSupportedException(); }
		}

		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_streamToCompress == null) throw new ObjectDisposedException(typeof(ZipOutputStream).Name);
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0) throw new ArgumentOutOfRangeException("offset", "Cannot be negative.");
			if (count < 0) throw new ArgumentOutOfRangeException("count", "Cannot be negative.");
			if (offset + count > buffer.Length) throw new ArgumentException("The sum of offset and count is greater than the byte array length.");

			_bufferController = new BufferController(buffer, offset, count);
			_backlogs = _bufferController.Append(_backlogs).ToList();
			while (_bufferController.Availability > 0 && !_eof)
			{
				var bytesRead = _streamToCompress.Read(_buffer, 0, _buffer.Length);
				if (bytesRead == 0)
				{
					CompressedStream.CloseEntry();
					CompressedStream.Close();
					_eof = true;
				}
				else
				{
					CompressedStream.Write(_buffer, 0, bytesRead);
					CompressedStream.Flush();
				}
			}
			return _bufferController.Count;
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

		private ICSharpCode.SharpZipLib.Zip.ZipOutputStream CompressedStream
		{
			get
			{
				if (_compressedStream == null)
				{
					// compression level, which ranges from 0 to 9 ---9 being the highest level of compression,--- defaults to 6
					_compressedStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(new BufferStream(this));
					_compressedStream.PutNextEntry(new ZipEntry(ZipEntry.CleanName(_zipEntryName)) { DateTime = DateTime.UtcNow });
				}
				return _compressedStream;
			}
		}

		private void AppendCompressedBytes(byte[] buffer, int offset, int count)
		{
			var backlog = _bufferController.Append(buffer, offset, count);
			if (backlog != null && backlog.Length > 0) _backlogs.Add(backlog);
		}

		private readonly byte[] _buffer;
		private readonly string _zipEntryName;
		private IList<byte[]> _backlogs = new List<byte[]>();
		private BufferController _bufferController;
		// TODO instead of SharpZipLib's ZipOutputStream use BCL's System.IO.Compression.ZipArchive or System.IO.Compression.GZipStream or System.IO.Compression.DeflateStream as only one zip entry is supported
		private ICSharpCode.SharpZipLib.Zip.ZipOutputStream _compressedStream;
		private bool _eof;
		private Stream _streamToCompress;
	}
}

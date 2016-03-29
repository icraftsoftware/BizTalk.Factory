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
using Be.Stateless.Logging;
using Microsoft.BizTalk.Streaming;

namespace Be.Stateless.BizTalk.Streaming
{
	/// <summary>
	/// An eventing read-only stream replacement of <see cref="Microsoft.BizTalk.Streaming.EventingReadStream" /> that
	/// deterministically fires the <see cref="IProvideReadStreamEvents.AfterLastReadEvent"/> event.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In short terms, this stream ensures that its contents is always exhausted and end-of-stream detection happens
	/// deterministically.
	/// </para>
	/// <para>
	/// In detailed terms, regardless of whether this stream's client will assume the end-of-stream has been reached as
	/// soon as a <see cref="Stream.Read"/> operation returns less bytes than the number of bytes requested, or whether
	/// it will only assume the end-of-stream has been reached once the <see cref="Stream.Read"/> operation returns zero
	/// (0) &#8212; which is, by the way, the officially documented way of being notified the end-of-stream has been
	/// reached &#8212; an <see cref="EventingReadStream"/> instance will always fire the <see
	/// cref="IProvideReadStreamEvents.AfterLastReadEvent"/> event as soon as the inner stream's end has been reached.
	/// </para>
	/// </remarks>
	public class EventingReadStream : Microsoft.BizTalk.Streaming.EventingReadStream
	{
		/// <summary>
		/// Construct an <see cref="EventingReadStream"/> instance wrapper around <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">
		/// The inner <see cref="Stream"/> to wrap.
		/// </param>
		public EventingReadStream(Stream stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			InnerStream = stream;
		}

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
		/// </summary>
		/// <returns>
		/// true if the stream supports reading; otherwise, false.
		/// </returns>
		public override bool CanRead
		{
			get
			{
				try
				{
					ThrowIfDisposed();
					return InnerStream.CanRead;
				}
				catch (Exception exception)
				{
					if (_logger.IsWarnEnabled) _logger.Warn(string.Format("{0} stream has encountered an error.", GetType()), exception);
					throw;
				}
			}
		}

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
		/// </summary>
		/// <returns>
		/// true if the stream supports seeking; otherwise, false.
		/// </returns>
		public override bool CanSeek
		{
			get
			{
				try
				{
					ThrowIfDisposed();
					// prevent messing around with current position and corrupt fired events while stream is still being read for the 1st time
					return ReadCompleted && InnerStream.CanSeek;
				}
				catch (Exception exception)
				{
					if (_logger.IsWarnEnabled) _logger.Warn(string.Format("{0} stream has encountered an error.", GetType()), exception);
					throw;
				}
			}
		}

		/// <summary>
		/// This property returns the inner stream length in bytes.
		/// </summary>
		/// <returns>
		/// A long value representing the length of the stream in bytes.
		/// </returns>
		/// <remarks>
		/// The <see cref="Length"/> will be computed while the stream is being read and will consequently only be
		/// available and known accurately once the whole stream has been exhausted. While being read, <see
		/// cref="Length"/> will therefore throw a <see cref="T:System.NotSupportedException"/> exception.
		/// </remarks>
		/// <exception cref="T:System.NotSupportedException">
		/// A class derived from Stream does not support seeking.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// Methods were called after the stream was closed.
		/// </exception>
		/// <seealso cref="InnerStream"/>
		/// <seealso cref="CanSeek"/>
		public override long Length
		{
			get
			{
				try
				{
					ThrowIfDisposed();
					if (!CanSeek) throw new NotSupportedException();
					return _length;
				}
				catch (NotSupportedException)
				{
					// avoid logging in this case as Microsoft.BizTalk.Internal.MessagePart.GetSize() systematically calls
					// this getter without first calling CanSeek's getter as is expected against any Stream-derived class.
					// see Microsoft.BizTalk.Messaging, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
					throw;
				}
				catch (Exception exception)
				{
					if (_logger.IsWarnEnabled) _logger.Warn(string.Format("{0} stream has encountered an error.", GetType()), exception);
					throw;
				}
			}
		}

		/// <summary>
		/// When overridden in a derived class, gets or sets the position within the current stream.
		/// </summary>
		/// <returns>
		/// The current position within the stream.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">The stream does not support seeking.</exception>
		public override long Position
		{
			get
			{
				try
				{
					ThrowIfDisposed();
					return InnerStream.Position;
				}
				catch (Exception exception)
				{
					if (_logger.IsWarnEnabled) _logger.Warn(string.Format("{0} stream has encountered an error.", GetType()), exception);
					throw;
				}
			}
			set
			{
				try
				{
					ThrowIfDisposed();
					// prevent messing around with current position and corrupt fired events (stream is still being read for the 1st time)
					if (!ReadCompleted)
						throw new InvalidOperationException(
							string.Format(
								"{0} is not seekable while its contents has not been read thoroughly.",
								typeof(EventingReadStream).Name));
					InnerStream.Position = value;
				}
				catch (Exception exception)
				{
					if (_logger.IsWarnEnabled) _logger.Warn(string.Format("{0} stream has encountered an error.", GetType()), exception);
					throw;
				}
			}
		}

		/// <summary>
		/// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be
		/// written to the underlying device.
		/// </summary>
		public override void Flush()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the
		/// position within the stream by the number of bytes read.
		/// </summary>
		/// <returns>
		/// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
		/// many bytes are not currently available, or zero (0) if the end of the stream has been reached.
		/// </returns>
		/// <param name="buffer">
		/// An array of bytes. When this method returns, the buffer contains the specified byte array with the values
		/// between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by
		/// the bytes read from the current source.
		/// </param>
		/// <param name="offset">
		/// The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the
		/// current stream.
		/// </param>
		/// <param name="count">
		/// The maximum number of bytes to be read from the current stream.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="buffer"/> is null.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="offset"/> or <paramref name="count"/> is negative.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.
		/// </exception>
		/// <remarks>
		/// <see cref="Read"/> will always try to read as many bytes as requested to circumvent idiosyncratic clients that
		/// would fail to issue the required last extra call to <see cref="Read"/>, which would return zero (0), because
		/// that they assume that the end of the stream has been reached as soon as <see cref="Read"/> returns a total
		/// number of bytes read that is less than <paramref name="count"/>.
		/// </remarks>
		public override int Read(byte[] buffer, int offset, int count)
		{
			try
			{
				ThrowIfDisposed();
				if (offset + count > buffer.Length) throw new ArgumentException("The sum of offset and count is larger than the buffer length");

				// ensures there always is a call to base Microsoft.BizTalk.Streaming.EventingReadStream.Read() that returns 0
				// so that base EventingReadStream class behaves as expected and always fires its AfterLastReadEvent event
				int totalReadCount = 0, lastReadCount;
				do
				{
					lastReadCount = base.Read(buffer, offset + totalReadCount, count - totalReadCount);
					if (_logger.IsFineEnabled) _logger.FineFormat("Read {0} bytes from inner stream.", lastReadCount);
					totalReadCount += lastReadCount;
				}
				while (totalReadCount < count && 0 < lastReadCount);
				return totalReadCount;
			}
			catch (Exception exception)
			{
				if (_logger.IsWarnEnabled) _logger.Warn(string.Format("{0} stream has encountered an error.", GetType()), exception);
				throw;
			}
		}

		/// <summary>
		/// Delegates to <see cref="InnerStream"/>.
		/// </summary>
		/// <param name="buffer">
		/// An array of bytes. When this method returns, the buffer contains the specified byte array with the values
		/// between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by
		/// the bytes read from the current source.
		/// </param>
		/// <param name="offset">
		/// The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the
		/// current stream.
		/// </param>
		/// <param name="count">
		/// The maximum number of bytes to be read from the current stream.
		/// </param>
		/// <returns>
		/// The total number of bytes read into the buffer.
		/// </returns>
		protected override int ReadInternal(byte[] buffer, int offset, int count)
		{
			ThrowIfDisposed();
			var bytesReadCount = InnerStream.Read(buffer, offset, count);
			if (!ReadCompleted) _length += bytesReadCount;
			return bytesReadCount;
		}

		/// <summary>
		/// When overridden in a derived class, sets the position within the current stream.
		/// </summary>
		/// <returns>
		/// The new position within the current stream.
		/// </returns>
		/// <param name="offset">
		/// A byte offset relative to the <paramref name="origin"/> parameter.
		/// </param>
		/// <param name="origin">
		/// A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new
		/// position.
		/// </param>
		public override long Seek(long offset, SeekOrigin origin)
		{
			try
			{
				ThrowIfDisposed();
				// prevent messing around with current position and corrupt fired events (stream is still being read for the 1st time)
				if (!ReadCompleted)
					throw new InvalidOperationException(
						string.Format(
							"{0} cannot be sought while its contents has not been read thoroughly.",
							typeof(EventingReadStream).Name));
				return InnerStream.Seek(offset, origin);
			}
			catch (Exception exception)
			{
				if (_logger.IsWarnEnabled) _logger.Warn(string.Format("{0} stream has encountered an error.", GetType()), exception);
				throw;
			}
		}

		/// <summary>
		/// When overridden in a derived class, sets the length of the current stream.
		/// </summary>
		/// <param name="value">
		/// The desired length of the current stream in bytes.
		/// </param>
		/// <exception cref="T:System.NotSupportedException">
		/// The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or
		/// console output.
		/// </exception>
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// The wrapped inner <see cref="Stream"/>.
		/// </summary>
		protected Stream InnerStream { get; set; }

		protected override void Dispose(bool disposing)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("{0} is {1}.", typeof(EventingReadStream).Name, disposing ? "disposing" : "finalizing");
			if (disposing && InnerStream != null)
			{
				InnerStream.Dispose();
				InnerStream = null;
			}
			base.Dispose(disposing);
		}

		protected void ThrowIfDisposed()
		{
			if (InnerStream == null) throw new ObjectDisposedException(GetType().Name);
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(EventingReadStream));
		private long _length;
	}
}

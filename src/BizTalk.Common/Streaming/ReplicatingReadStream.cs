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
using Be.Stateless.Logging;

namespace Be.Stateless.BizTalk.Streaming
{
	/// <summary>
	/// A read-only <see cref="Stream"/> that replicates itself to another <see cref="Stream"/> while being read.
	/// </summary>
	public class ReplicatingReadStream : Stream
	{
		/// <summary>
		/// Construct a <see cref="ReplicatingReadStream"/> instance.
		/// </summary>
		/// <param name="source">
		/// The <see cref="Stream"/> to replicate to <paramref name="target"/> while being read.
		/// </param>
		/// <param name="target">
		/// The <see cref="Stream"/> to replicate <paramref name="source"/> to while being read.
		/// </param>
		public ReplicatingReadStream(Stream source, Stream target)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (target == null) throw new ArgumentNullException("target");
			_source = source;
			_target = target;
		}

		#region Base Class Member Overrides

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
		/// </summary>
		/// <returns>
		/// true if the stream supports reading; otherwise, false.
		/// </returns>
		public override bool CanRead
		{
			get { return _source.CanRead; }
		}

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
		/// </summary>
		/// <returns>
		/// true if the stream supports seeking; otherwise, false.
		/// </returns>
		public override bool CanSeek
		{
			// must wait for the _source to have been thoroughly read, _target would be unmanageable otherwise
			get { return _readCompleted && _source.CanSeek; }
		}

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the stream supports writing; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanWrite
		{
			get { return false; }
		}

		/// <summary>
		/// This property exposes the inner stream length in bytes.
		/// </summary>
		/// <returns>
		/// A long value representing the length of the stream in bytes.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// The inner stream has not been read to its end yet.
		/// </exception>
		/// <remarks>
		/// <see cref="Length"/> property getter will throw unless the end of the inner stream has been reached.
		/// </remarks>
		public override long Length
		{
			get
			{
				try
				{
					ThrowIfDisposed();
					// Length is unknown while the inner stream has not been read to its end.
					if (!CanSeek) throw new NotSupportedException();
					return _length;
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
					return _source.Position;
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
					if (_source.Position == value) return;
					// must wait for the _source to have been thoroughly read, _target would be unmanageable otherwise
					if (!CanSeek)
						throw new InvalidOperationException(
							string.Format(
								"{0} is not seekable while the inner stream has not been thoroughly read and replicated.",
								typeof(ReplicatingReadStream).Name));
					_source.Position = value;
				}
				catch (Exception exception)
				{
					if (_logger.IsWarnEnabled) _logger.Warn(string.Format("{0} stream has encountered an error.", GetType()), exception);
					throw;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("{0} is {1}.", typeof(ReplicatingReadStream).Name, disposing ? "disposing" : "finalizing");
			if (disposing)
			{
				if (_source != null)
				{
					_source.Dispose();
					_source = null;
				}
				if (_target != null)
				{
					_target.Dispose();
					_target = null;
				}
			}
			base.Dispose(disposing);
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
		/// Reads and replicates a sequence of bytes from the current stream and advances the position within the stream
		/// by the number of bytes read.
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
		public override int Read(byte[] buffer, int offset, int count)
		{
			try
			{
				ThrowIfDisposed();
				var bytesReadCount = _source.Read(buffer, offset, count);

				// if read has already been completed, than what follows has already been performed and must not be performed again
				if (!_readCompleted)
				{
					_length += bytesReadCount;

					if (bytesReadCount > 0)
					{
						if (_logger.IsDebugEnabled) _logger.DebugFormat("Replicating {0} bytes to target stream.", bytesReadCount);
						_target.Write(buffer, offset, bytesReadCount);
					}
					else if (!_readCompleted) // extra _readCompleted check to ensure _target stream is closed only once
					{
						_readCompleted = true;
						var streamTransacted = _target as IStreamTransacted;
						if (streamTransacted != null)
						{
							if (_logger.IsDebugEnabled) _logger.Debug("Replication completed, committing the transacted stream.");
							streamTransacted.Commit();
						}
						else
						{
							if (_logger.IsDebugEnabled) _logger.Debug("Replication completed, closing the transacted stream.");
							_target.Close();
						}
						_target = null;
					}
				}

				return bytesReadCount;
			}
			catch (Exception exception)
			{
				if (_logger.IsWarnEnabled) _logger.Warn(string.Format("{0} stream has encountered an error.", GetType()), exception);
				throw;
			}
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
		/// <exception cref="T:System.InvalidOperationException">
		/// The stream cannot be sought while the inner stream has not been thoroughly read and replicated.
		/// </exception>
		public override long Seek(long offset, SeekOrigin origin)
		{
			try
			{
				ThrowIfDisposed();
				// must wait for the _source to have been thoroughly read, _target would be unmanageable otherwise
				if (!CanSeek)
					throw new InvalidOperationException(
						string.Format(
							"{0} cannot be sought while the inner stream has not been thoroughly read and replicated.",
							typeof(ReplicatingReadStream).Name));
				return _source.Seek(offset, origin);
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
		/// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current
		/// position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">
		/// An array of bytes. This method copies count bytes from <paramref name="buffer"/> to the current stream.
		/// </param>
		/// <param name="offset">
		/// The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.
		/// </param>
		/// <param name="count">
		/// The number of bytes to be written to the current stream.
		/// </param>
		/// <exception cref="NotSupportedException">
		/// The stream does not support writing.
		/// </exception>
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		#endregion

		private void ThrowIfDisposed()
		{
			if (_source == null) throw new ObjectDisposedException(GetType().Name);
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(ReplicatingReadStream));

		private long _length;
		private bool _readCompleted;
		private Stream _source;
		private Stream _target;
	}
}

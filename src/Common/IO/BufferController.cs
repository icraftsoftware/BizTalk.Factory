#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using System.IO;
using System.Linq;
using System.Text;
using Be.Stateless.Extensions;

namespace Be.Stateless.IO
{
	/// <summary>
	/// Control operations on a temporary buffer.
	/// </summary>
	/// <remarks>
	/// A temporary buffer is typically used in a <see cref=" Stream"/>-based component while reading from one <see
	/// cref=" Stream"/> and writing to an other, or when one needs to transcode the bytes that have been read before
	/// being written to the buffer allocated by the <see cref=" Stream"/>'s client.
	/// </remarks>
	public class BufferController
	{
		/// <summary>
		/// Wraps the <c>byte[]</c> array that need be to operated upon.
		/// </summary>
		/// <param name="buffer">
		/// The underlying controlled buffer.
		/// </param>
		/// <param name="offset">
		/// The offset at which bytes can be appended to the underlying controlled buffer.
		/// </param>
		/// <param name="availability">
		/// Number of available bytes that can be appended to the underlying controlled buffer.
		/// </param>
		public BufferController(byte[] buffer, int offset, int availability)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			_buffer = buffer;
			_availability = availability;
			_count = 0;
			_offset = offset;
		}

		/// <summary>
		/// The number of available bytes than can yet be appended to the underlying controlled buffer.
		/// </summary>
		public int Availability
		{
			get { return _availability; }
		}

		/// <summary>
		/// The total number of bytes that have been appended to the underlying controlled buffer.
		/// </summary>
		public int Count
		{
			get { return _count; }
		}

		/// <summary>
		/// Append an array of bytes to the underlying controlled buffer.
		/// </summary>
		/// <param name="bytes">
		/// The array of bytes to append.
		/// </param>
		/// <returns>
		/// The <paramref name="bytes"/> sub-array, that is the array of bytes, that could not be appended to the
		/// underlying controlled buffer because of availability shortage.
		/// </returns>
		public byte[] Append(byte[] bytes)
		{
			if (bytes == null || bytes.Length == 0 || _availability == 0) return bytes;
			var count = Math.Min(_availability, bytes.Length);
			Buffer.BlockCopy(bytes, 0, _buffer, _offset, count);
			_availability -= count;
			_count += count;
			_offset += count;
			// return the trailing bytes that could not be appended to the buffer
			return bytes.Subarray(count);
		}

		/// <summary>
		/// Append a sequence of bytes from the given <paramref name="bytes"/> array to the underlying controlled buffer.
		/// </summary>
		/// <param name="bytes">
		/// An array of bytes. This method appends count bytes from <paramref name="bytes"/> to the underlying controlled
		/// buffer. 
		/// </param>
		/// <param name="offset">
		/// The zero-based byte offset in <paramref name="bytes"/> at which to begin copying bytes to the underlying
		/// controlled buffer.
		/// </param>
		/// <param name="count">
		/// The number of bytes to be written to the underlying controlled buffer.
		/// </param>
		/// <returns>
		/// The <paramref name="bytes"/> sub-array, that is the array of bytes, that could not be appended to the
		/// underlying controlled buffer because of availability shortage.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="bytes"/> is null.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="offset"/> or <paramref name="count"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the <paramref
		/// name="bytes"/>' array length.
		/// </exception>
		public byte[] Append(byte[] bytes, int offset, int count)
		{
			if (bytes == null) throw new ArgumentNullException("bytes");
			if (offset < 0) throw new ArgumentOutOfRangeException("offset", "Cannot be negative.");
			if (count < 0) throw new ArgumentOutOfRangeException("count", "Cannot be negative.");
			if (offset + count > bytes.Length) throw new ArgumentException("The sum of offset and count is greater than the byte array length.");

			if (bytes.Length == 0 || count == 0) return null;

			var actualCount = Math.Min(_availability, count);

			Buffer.BlockCopy(bytes, offset, _buffer, _offset, actualCount);
			_availability -= actualCount;
			_count += actualCount;
			_offset += actualCount;

			// return the trailing bytes that could not be appended to the buffer
			var data = new byte[count - actualCount];
			Buffer.BlockCopy(bytes, offset + actualCount, data, 0, data.Length);
			return data;
		}

		/// <summary>
		/// Append an <see cref="IEnumerable{T}"/> of byte-array <paramref name="buffers"/> to the underlying controlled
		/// buffer.
		/// </summary>
		/// <param name="buffers">
		/// The <see cref="IEnumerable{T}"/> of byte-array <paramref name="buffers"/> to append to the underlying
		/// controlled buffer.
		/// </param>
		/// <returns>
		/// The <see cref="IEnumerable{T}"/> of byte-array buffers that could not be appended to the underlying controlled
		/// buffer because of availability shortage. Notice that no byte will be lost, and that the first buffer in the
		/// <see cref="IEnumerable{T}"/> of byte-array buffers might be a subset of what it was initially.
		/// </returns>
		public IEnumerable<byte[]> Append(IEnumerable<byte[]> buffers)
		{
			if (buffers == null) throw new ArgumentNullException("buffers");

			// ReSharper disable PossibleMultipleEnumeration
			while (_availability > 0 && buffers.Any())
			{
				var backlog = Append(buffers.First());
				buffers = buffers.Skip(1);
				if (backlog != null && backlog.Length > 0)
				{
					return Enumerable.Repeat(backlog, 1).Concat(buffers).ToArray();
				}
			}
			return buffers;
			// ReSharper restore PossibleMultipleEnumeration
		}

		/// <summary>
		/// Append the bytes making up a given string according to a specific encoding.
		/// </summary>
		/// <param name="string">
		/// The string whose bytes will be appended to the buffer.
		/// </param>
		/// <param name="encoding">
		/// The encoding according to which the string must be converted to bytes.
		/// </param>
		/// <returns>
		/// The array of bytes that could not be appended to the underlying controlled buffer because of availability
		/// shortage.
		/// </returns>
		public byte[] Append(string @string, Encoding encoding)
		{
			if (encoding == null) throw new ArgumentNullException("encoding");
			return @string.IsNullOrEmpty()
				? null
				: Append(encoding.GetBytes(@string));
		}

		/// <summary>
		/// Append to the underlying buffer by delegation.
		/// </summary>
		/// <param name="delegate">
		/// A <see cref="Func{T}"/> delegate with both a signature and semantics identical to the one of the <see
		/// cref="Stream.Read"/> method.
		/// </param>
		/// <returns>
		/// The total number of bytes appended to the underlying controlled buffer.
		/// </returns>
		public int Append(Func<byte[], int, int, int> @delegate)
		{
			if (_availability <= 0)
				throw new InvalidOperationException(
					string.Format(
						"{0} has no more availability to append further bytes to buffer.",
						typeof(BufferController).Name));
			var count = @delegate(_buffer, _offset, _availability);
			_availability -= count;
			_count += count;
			_offset += count;
			return count;
		}

		private readonly byte[] _buffer;
		private int _availability;
		private int _count;
		private int _offset;
	}
}

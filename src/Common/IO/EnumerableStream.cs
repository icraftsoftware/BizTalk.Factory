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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Be.Stateless.IO
{
	/// <summary>
	/// Transform an <see cref="IEnumerable{T}"/> of either <see cref="string"/>s or arrays of <see cref="byte"/>s into a
	/// readonly, non-seekable <see cref="Stream"/> of <see cref="byte"/>s.
	/// </summary>
	/// <remarks>
	/// When <see cref="string"/>s are used as the underlying type of the <see cref="IEnumerable{T}"/>, the <see
	/// cref="Encoding.Unicode"/> encoding is assumed to convert them to <see cref="byte"/>s.
	/// </remarks>
	public class EnumerableStream : Stream
	{
		/// <summary>
		/// Transform an <see cref="IEnumerable{T}"/> of arrays of <see cref="byte"/>s into a <see cref="Stream"/>.
		/// </summary>
		/// <param name="enumerable">
		/// The <see cref="IEnumerable{T}"/> over the arrays of <see cref="byte"/>s.
		/// </param>
		public EnumerableStream(IEnumerable<byte[]> enumerable)
		{
			if (enumerable == null) throw new ArgumentNullException("enumerable");
			_enumerator = enumerable.GetEnumerator();
		}

		/// <summary>
		/// Transform an <see cref="IEnumerable{T}"/> of <see cref="string"/>s into a <see cref="Stream"/>.
		/// </summary>
		/// <param name="enumerable">
		/// The <see cref="IEnumerable{T}"/> over the <see cref="string"/>s.
		/// </param>
		/// <remarks>
		/// The <see cref="Encoding.Unicode"/> encoding will be assumed to transform the <see cref="string"/>s into their
		/// <see cref="byte"/> equivalent.
		/// </remarks>
		public EnumerableStream(IEnumerable<string> enumerable) : this(enumerable.Select(s => Encoding.Unicode.GetBytes(s))) { }

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

		public override long Length
		{
			get { throw new NotSupportedException(); }
		}

		public override long Position
		{
			get { return _position; }
			set { throw new NotSupportedException(); }
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			var bufferController = new BufferController(buffer, offset, count);
			// try to exhaust backlog if any while keeping any extra of it
			_backlog = bufferController.Append(_backlog);
			while (bufferController.Availability > 0 && _enumerator.MoveNext())
			{
				_backlog = bufferController.Append(_enumerator.Current);
			}
			_position += bufferController.Count;
			return bufferController.Count;
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

		private readonly IEnumerator<byte[]> _enumerator;
		private byte[] _backlog;
		private int _position;
	}
}

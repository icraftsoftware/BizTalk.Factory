#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Be.Stateless.IO.Extensions;

namespace Be.Stateless.BizTalk.Streaming
{
	/// <summary>
	/// Wraps a zip-decompressing stream around a data stream and ensures that the data stream is exhausted once the
	/// decompression is complete.
	/// </summary>
	/// <remarks>
	/// <see cref="ZipInputStream"/> relies on <see cref="ICSharpCode.SharpZipLib.Zip.ZipInputStream"/> for the
	/// decompression of the zip-stream that must have exactly one <see cref="ICSharpCode.SharpZipLib.Zip.ZipEntry"/>.
	/// </remarks>
	/// <seealso href="http://github.com/icsharpcode/SharpZipLib/wiki/Zip-Samples">SharpZipLib Samples</seealso>
	public class ZipInputStream : ICSharpCode.SharpZipLib.Zip.ZipInputStream
	{
		public ZipInputStream(Stream baseInputStream) : base(baseInputStream)
		{
			_baseInputStream = baseInputStream;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Replicate base class API.")]
		public ZipInputStream(Stream baseInputStream, int bufferSize) : base(baseInputStream, bufferSize)
		{
			_baseInputStream = baseInputStream;
		}

		#region Base Class Member Overrides

		public override int Read(byte[] buffer, int offset, int count)
		{
			var byteCount = base.Read(buffer, offset, count);
			if (byteCount == 0) _baseInputStream.Drain();
			return byteCount;
		}

		#endregion

		private readonly Stream _baseInputStream;
	}
}

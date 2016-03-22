#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using System.IO.Compression;
using System.Runtime.InteropServices;
using Be.Stateless.Extensions;

namespace Be.Stateless.IO.Extensions
{
	public static class StreamExtensions
	{
		/// <summary>
		/// Compress a stream and returns its compressed content as a base64 encoding.
		/// </summary>
		/// <param name="stream">The stream to compress and encode.</param>
		/// <returns>The base64 encoding of the compressed stream.</returns>
		public static string CompressToBase64String(this Stream stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (!stream.CanRead) throw new InvalidOperationException("Cannot compress a non-readable stream.");
			var destinationStream = new MemoryStream();
			using (var compressionStream = new DeflateStream(destinationStream, CompressionMode.Compress, true))
			{
				stream.CopyTo(compressionStream);
			}
			return Convert.ToBase64String(destinationStream.GetBuffer(), 0, (int) destinationStream.Position);
		}

		/// <summary>
		/// Returns a stream content as a base64 encoding.
		/// </summary>
		/// <param name="stream">The stream to encode in base64.</param>
		/// <returns>The base64 encoding of the stream.</returns>
		public static string ToBase64String(this Stream stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (!stream.CanRead) throw new InvalidOperationException("Cannot compress a non-readable stream.");

			var destinationStream = new MemoryStream();
			stream.CopyTo(destinationStream);
			return Convert.ToBase64String(destinationStream.GetBuffer(), 0, (int) destinationStream.Position);
		}

		/// <summary>
		/// Decompress a base64 encoding of a compressed stream.
		/// </summary>
		/// <param name="base64StringEncodedCompressedStream">The base64 encoding of a compressed stream.</param>
		/// <returns>The decoded and uncompressed stream.</returns>
		public static Stream DecompressFromBase64String(this string base64StringEncodedCompressedStream)
		{
			if (base64StringEncodedCompressedStream.IsNullOrEmpty()) return new MemoryStream();
			var decodedStream = new MemoryStream(Convert.FromBase64String(base64StringEncodedCompressedStream), false);
			return new DeflateStream(decodedStream, CompressionMode.Decompress);
		}

		/// <summary>
		/// Consume the content of the <see cref="Stream"/> <paramref name="stream"/>.
		/// </summary>
		/// <remarks>
		/// The position of the <see cref="Stream"/> <paramref name="stream"/> will be moved to the end of stream.
		/// </remarks>
		/// <param name = "stream">The stream to drain.</param>
		public static void Drain(this Stream stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (!stream.CanRead) throw new InvalidOperationException("Cannot drain a non-readable stream.");
			// Read and write in chunks of 1K
			var tempBuffer = new byte[1024];
			while (stream.Read(tempBuffer, 0, tempBuffer.Length) != 0) { }
		}

		/// <summary>
		/// Save the content of a stream to disk by going through a temporary file.
		/// </summary>
		/// <param name="stream">The stream whose content needs to be saved.</param>
		/// <param name="folder">The folder where to save the stream.</param>
		/// <param name="name">The file name to use to save the strem.</param>
		/// <remarks>
		/// <para>
		/// The <paramref name="stream"/> is first saved to a <c>&lt;name.tmp&gt;</c> file and then moved to a
		/// <c>&lt;name&gt;</c> file, but only after its content has been completely flushed to disk.
		/// </para>
		/// <para>
		/// The target <paramref name="folder"/> is created if it does not exist.
		/// </para>
		/// </remarks>
		public static void DropToFolder(this Stream stream, string folder, string name)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (folder.IsNullOrEmpty()) throw new ArgumentNullException("folder");
			if (name.IsNullOrEmpty()) throw new ArgumentNullException("name");
			Directory.CreateDirectory(folder);
			var path = System.IO.Path.Combine(folder, name);
			File.Delete(path);
			// save to a temporary, extensionless file with a GUID as name
			var tempFileName = Guid.NewGuid().ToString("N");
			var tempPath = System.IO.Path.Combine(folder, tempFileName);
			stream.Save(tempPath);
			stream.Close();
			File.Move(tempPath, path);
		}

		/// <summary>
		/// Determines the MIME type from the data stram provided.
		/// </summary>
		/// <param name="stream">
		/// The stream that contains the data to be sniffed.
		/// </param>
		/// <returns>
		/// </returns>
		/// <remarks>
		/// MIME type detection, or "data sniffing," refers to the process of determining an appropriate MIME type from
		/// binary data. The final result depends on a combination of server-supplied MIME type headers, file name
		/// extension, and/or the data itself. Usually, only the first 256 bytes of data are significant. For more
		/// information and a complete list of recognized MIME types, see <a
		/// href="http://msdn.microsoft.com/en-us/library/ms775147(v=vs.85).aspx">MIME Type Detection in Internet
		/// Explorer.</a>
		/// </remarks>
		/// <seealso href="http://msdn.microsoft.com/en-us/library/ms775107(v=vs.85).aspx"/>
		/// <seealso href="Microsoft.Practices.ESB.ExceptionHandling.PipelineComponents.SafeNativeMethods.GetMimeType(Stream)"/>
		/// <seealso href="http://forums.asp.net/t/1041821.aspx/1?Determine+Mime+Type+for+server+side+file"/>
		public static string GetMimeType(this Stream stream)
		{
			string mimeType;
			var count = 256;
			var buffer = new byte[count];
			count = stream.Read(buffer, 0, count);
			var hresult = FindMimeFromData(IntPtr.Zero, null, buffer, count, null, 0, out mimeType, 0);
			if (hresult != 0) throw Marshal.GetExceptionForHR(hresult);
			if (stream.CanSeek) stream.Position = 0L;
			return mimeType;
		}

		/// <summary>
		/// Save the content of a stream to disk.
		/// </summary>
		/// <param name="stream">The stream whose content needs to be saved.</param>
		/// <param name="path">The file path to where to save the stream.</param>
		/// <remarks>Usefull for debugging.</remarks>
		public static void Save(this Stream stream, string path)
		{
			using (Stream file = File.OpenWrite(path))
			{
				stream.CopyTo(file);
			}
		}

		/// <summary>
		/// Try to compress a stream and outputs its compressed content as a base64 encoded string, as long as its
		/// compressed content does not exceed a given <paramref name="threshold"/>.
		/// </summary>
		/// <param name="stream">
		/// The stream to compress and encode.
		/// </param>
		/// <param name="threshold">
		/// The threshold in bytes that the compressed stream cannot exceed.
		/// </param>
		/// <param name="encodedCompression">
		/// The base64 encoding of the compressed stream if the latter's length does not exceed the compression threshold.
		/// </param>
		/// <returns>
		/// <c>true</c> if the stream can be compressed without exceeding the <paramref name="threshold"/>; <c>false</c>
		/// otherwise.
		/// </returns>
		public static bool TryCompressToBase64String(this Stream stream, int threshold, out string encodedCompression)
		{
			// 16KB compressed, should be OK for most streams having a threshold limit on compressed size
			const int bufferSize = 16 * 1024;
			var compressedStream = new MemoryStream(bufferSize);
			using (var compressionStream = new DeflateStream(compressedStream, CompressionMode.Compress, true))
			{
				var buffer = new byte[bufferSize];
				var length = 0;
				int read;
				while (compressedStream.Length < threshold && 0 < (read = stream.Read(buffer, 0, bufferSize)))
				{
					compressionStream.Write(buffer, 0, read);
					length += read;
					// force compression stream to flush once more than the threshold has been read to ensure an accurate
					// reading of the compressed stream length; pointless if no more than the threshold of uncompressed
					// data has been read.
					if (length > threshold) compressionStream.Flush();
				}
			}
			if (compressedStream.Length < threshold)
			{
				encodedCompression = Convert.ToBase64String(compressedStream.GetBuffer(), 0, (int) compressedStream.Length);
				return true;
			}
			encodedCompression = null;
			return false;
		}

		#region External Imports

		// ReSharper disable InconsistentNaming

		[DllImport("urlmon.dll", CharSet = CharSet.Auto)]
		private static extern int FindMimeFromData(
			IntPtr pBC,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
			[MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
			int cbSize,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
			int dwMimeFlags,
			[MarshalAs(UnmanagedType.LPWStr)] out string ppwzMimeOut,
			int dwReserved);

		// ReSharper restore InconsistentNaming

		#endregion
	}
}

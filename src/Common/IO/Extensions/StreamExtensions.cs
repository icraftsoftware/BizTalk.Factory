﻿#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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
		/// The <paramref name="stream"/> is first saved to a temporary file before being moved to file with the given <paramref name="name"/>, but only after its content
		/// has been completely flushed to disk.
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
			// save to a temporary file with a GUID and no extension as name
			var tempFileName = Guid.NewGuid().ToString("N");
			var tempPath = System.IO.Path.Combine(folder, tempFileName);
			stream.Save(tempPath);
			stream.Close();
			File.Move(tempPath, path);
		}

		/// <summary>
		/// Determines the MIME type from the data stream provided.
		/// </summary>
		/// <param name="stream">
		/// The stream that contains the data to be sniffed.
		/// </param>
		/// <returns>
		/// </returns>
		/// <remarks>
		/// MIME type detection, or "data sniffing," refers to the process of determining an appropriate MIME type from binary data. The final result depends on a
		/// combination of server-supplied MIME type headers, file name extension, and/or the data itself. Usually, only the first 256 bytes of data are significant. For
		/// more information and a complete list of recognized MIME types, see <a href="http://msdn.microsoft.com/en-us/library/ms775147(v=vs.85).aspx">MIME Type Detection
		/// in Internet Explorer.</a>
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
		/// Returns the content of the stream as a <see cref="string"/>.
		/// </summary>
		/// <param name="stream">The stream whose content will be returned as a <see cref="string"/>.</param>
		/// <returns>The <paramref name="stream"/>'s content as a <see cref="string"/>.</returns>
		/// <remarks>
		/// The <paramref name="stream"/> will be rewinded before being read.
		/// </remarks>
		public static string ReadToEnd(this Stream stream)
		{
			return new StreamReader(stream.Rewind()).ReadToEnd();
		}

		/// <summary>
		/// Rewind the stream to its <see cref="Stream.Position"/> 0.
		/// </summary>
		/// <param name="stream">The <see cref="Stream"/> to rewind.</param>
		/// <returns>The <paramref name="stream"/> which has been rewinded.</returns>
		/// <exception cref="InvalidOperationException">If the <paramref name="stream"/> is not seekable, i.e. <see cref="Stream.CanSeek"/> is <c>false</c>.</exception>
		public static Stream Rewind(this Stream stream)
		{
			if (!stream.CanSeek) throw new InvalidOperationException("Stream cannot be rewinded.");
			stream.Position = 0;
			return stream;
		}

		/// <summary>
		/// Save the content of a stream to disk.
		/// </summary>
		/// <param name="stream">The stream whose content needs to be saved.</param>
		/// <param name="path">The file path to where to save the stream.</param>
		/// <remarks>Useful for debugging.</remarks>
		public static void Save(this Stream stream, string path)
		{
			using (Stream file = File.OpenWrite(path))
			{
				stream.CopyTo(file);
			}
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
		/// Try to compress a stream and outputs its compressed content as a base64 encoded string, as long as its compressed content does not exceed a given <paramref
		/// name="threshold"/>. Note that <paramref name="threshold"/> is an approximate limit, and can be slightly overrun. This is because a compressing stream's length
		/// cannot be accurately determined while writing data to it.
		/// </summary>
		/// <param name="stream">
		/// The stream to compress and encode.
		/// </param>
		/// <param name="threshold">
		/// The threshold in bytes that the compressed stream cannot exceed. Notice that this threshold is approximate, and can in certain cases be exceeded.
		/// </param>
		/// <param name="compressedBase64String">
		/// The base64 encoding of the compressed stream if the latter's length does not exceed the compression threshold.
		/// </param>
		/// <returns>
		/// <c>true</c> if the stream can be compressed without exceeding the <paramref name="threshold"/>; <c>false</c>
		/// otherwise.
		/// </returns>
		public static bool TryCompressToBase64String(this Stream stream, int threshold, out string compressedBase64String)
		{
			// 16KB compressed, should be OK for most streams having a threshold limit on compressed size
			const int bufferSize = 16 * 1024;
			var compressedStream = new MemoryStream(bufferSize);
			var bytesRead = 0;
			var buffer = new byte[bufferSize];
			using (var compressionStream = new DeflateStream(compressedStream, CompressionMode.Compress, true))
			{
				while (compressedStream.Length < threshold && 0 < (bytesRead = stream.Read(buffer, 0, bufferSize)))
				{
					compressionStream.Write(buffer, 0, bytesRead);
				}
			}
			// HACK to avoid (most of the time) firing end of stream event on input stream when we do not actually allow compression
			var endOfStreamPossiblyReached = bytesRead < bufferSize && bytesRead > 0;
			// try to read one more byte to check for end of stream if the last read returned less than what we requested if we are unlucky, that last read will return the
			// last byte and fire the end of stream event :-( but this should not be the case most of the time
			if (endOfStreamPossiblyReached)
			{
				bytesRead = stream.Read(buffer, 0, 1);
			}
			var endOfInputStreamReached = bytesRead == 0;
			if (endOfInputStreamReached)
			{
				compressedBase64String = Convert.ToBase64String(compressedStream.GetBuffer(), 0, (int) compressedStream.Length);
				return true;
			}
			compressedBase64String = null;
			return false;
		}

		#region External Imports

		[DllImport("urlmon.dll", CharSet = CharSet.Auto)]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		[SuppressMessage("ReSharper", "StringLiteralTypo")]
		private static extern int FindMimeFromData(
			IntPtr pBC,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
			[MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
			int cbSize,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
			int dwMimeFlags,
			[MarshalAs(UnmanagedType.LPWStr)] out string ppwzMimeOut,
			int dwReserved);

		#endregion
	}
}

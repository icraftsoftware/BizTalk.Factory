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

using System;
using System.IO;
using System.Linq;
using System.Text;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.IO;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.BizTalk.Streaming;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Streaming
{
	[TestFixture]
	public class ZipOutputStreamFixture
	{
		[Test]
		public void ValidateCompressedData()
		{
			using (var clearStream = FakeTextStream.Create(1024 * 64))
			using (var compressedStream = new ZipOutputStream(clearStream, "entry-name"))
			using (var validator = new ZipFile(new ReadOnlySeekableStream(compressedStream)))
			{
				validator.TestArchive(true, TestStrategy.FindFirstError, null);
			}
		}

		[Test]
		public void ZipUnzipLargePayload()
		{
			using (var memoryStream = new MemoryStream())
			using (var clearStream = new ReplicatingReadStream(FakeTextStream.Create(1024 * 64), memoryStream))
			using (var compressedStream = new ZipOutputStream(clearStream, "entry-name"))
			using (var uncompressedStream = new ZipInputStream(compressedStream))
			{
				uncompressedStream.GetNextEntry();
				using (var reader = new StreamReader(uncompressedStream))
				{
					Assert.That(reader.ReadToEnd(), Is.EqualTo(Encoding.UTF8.GetString(memoryStream.ToArray())));
				}
			}
		}

		[Test]
		public void ZipUnzipLargePayloadUsingSmallBuffer()
		{
			using (var memoryStream = new MemoryStream())
			using (var clearStream = new ReplicatingReadStream(FakeTextStream.Create(1024 * 64), memoryStream))
			using (var compressedStream = new ZipOutputStream(clearStream, "entry-name", 256))
			using (var uncompressedStream = new ZipInputStream(compressedStream))
			{
				uncompressedStream.GetNextEntry();
				using (var reader = new StreamReader(uncompressedStream))
				{
					Assert.That(reader.ReadToEnd(), Is.EqualTo(Encoding.UTF8.GetString(memoryStream.ToArray())));
				}
			}
		}

		[Test]
		public void ZipUnzipLargePayloadUsingTinyBuffer()
		{
			// computing content beforehand is much more faster that using a ReplicatingReadStream
			var content = Enumerable.Range(0, 1024)
				.Select(i => Guid.NewGuid().ToString())
				.Aggregate(string.Empty, (k, v) => k + v);
			using (var clearStream = new StringStream(content))
			using (var compressedStream = new ZipOutputStream(clearStream, "entry-name", 16))
			using (var uncompressedStream = new ZipInputStream(compressedStream))
			{
				uncompressedStream.GetNextEntry();
				using (var reader = new StreamReader(uncompressedStream))
				{
					Assert.That(reader.ReadToEnd(), Is.EqualTo(content));
				}
			}
		}

		[Test]
		public void ZipUnzipSmallPayload()
		{
			const string content = "text";
			using (var clearStream = new StringStream(content))
			using (var compressedStream = new ZipOutputStream(clearStream, "entry-name"))
			using (var uncompressedStream = new ZipInputStream(compressedStream))
			{
				uncompressedStream.GetNextEntry();
				using (var reader = new StreamReader(uncompressedStream))
				{
					Assert.That(reader.ReadToEnd(), Is.EqualTo(content));
				}
			}
		}
	}
}

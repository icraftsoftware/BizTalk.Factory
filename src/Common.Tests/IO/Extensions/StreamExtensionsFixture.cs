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
using System.IO;
using System.Reflection;
using System.Text;
using Be.Stateless.BizTalk.Unit.Resources;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.IO.Extensions
{
	[TestFixture]
	public class StreamExtensionsFixture
	{
		[Test]
		public void CanCompressEmptyStream()
		{
			Assert.That(new MemoryStream().CompressToBase64String(), Is.Not.Null);
		}

		[Test]
		public void CanDecompressEmptyString()
		{
			Assert.That(string.Empty.DecompressFromBase64String(), Is.Not.Null);
		}

		[Test]
		public void CanDecompressNullString()
		{
			Assert.That(((string) null).DecompressFromBase64String(), Is.Not.Null);
		}

		[Test]
		public void CompressAboveThreshold()
		{
			using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(new string('A', 4096))))
			{
				string encoded;
				Assert.That(stream.TryCompressToBase64String(16, out encoded), Is.False);
			}
		}

		[Test]
		public void CompressBelowThreshold()
		{
			using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(new string('A', 4096))))
			{
				string encoded;
				Assert.That(stream.TryCompressToBase64String(1024, out encoded));
			}
		}

		[Test]
		public void CompressDecompressRoundtrip()
		{
			using (var inputStream = new MemoryStream())
			using (var writer = new StreamWriter(inputStream))
			{
				const string initial = "This is a string that I would like to find back after roundtrip.";
				writer.Write(initial);
				writer.Flush();

				inputStream.Position = 0;
				var encoded = inputStream.CompressToBase64String();

				string roundtripped;
				using (var decoded = encoded.DecompressFromBase64String())
				using (var reader = new StreamReader(decoded))
				{
					roundtripped = reader.ReadToEnd();
				}

				Assert.That(roundtripped, Is.EqualTo(initial));
			}
		}

		[Test]
		public void CompressedOutputIsBase64()
		{
			var input = new MemoryStream(128);
			var buffer = Encoding.UTF8.GetBytes("This is a test string.");
			input.Write(buffer, 0, buffer.Length);

			input.Position = 0;
			var output = input.CompressToBase64String();

			Assert.That(() => Convert.FromBase64String(output), Throws.Nothing);
		}

		[Test]
		public void CompressionStartsAtCurrentPosition()
		{
			using (var inputStream = new MemoryStream())
			using (var writer = new StreamWriter(inputStream))
			{
				const string initial = "This is a string that I would like to find back after roundtrip.";
				writer.Write(initial);
				writer.Flush();

				var position = "This is ".Length;
				inputStream.Position = position;
				var encoded = inputStream.CompressToBase64String();

				string roundtripped;
				using (var decoded = encoded.DecompressFromBase64String())
				using (var reader = new StreamReader(decoded))
				{
					roundtripped = reader.ReadToEnd();
				}

				Assert.That(roundtripped, Is.EqualTo(initial.Substring(position)));
			}
		}

		[Test]
		public void DrainReadToEnd()
		{
			var source = new MemoryStream();
			source.Write(new byte[100], 0, 100);
			source.Position = 0;

			source.Drain();

			Assert.That(source.Position, Is.EqualTo(source.Length));
		}

		[Test]
		public void GetApplicationMimeType()
		{
			var stream = File.OpenRead(Assembly.GetExecutingAssembly().Location);
			Assert.That(stream.GetMimeType(), Is.EqualTo("application/x-msdownload"));
		}

		[Test]
		public void GetMimeTypeOfDeflatedStream()
		{
			var stream = ResourceManager.Load("Schema.xsd")
				.CompressToBase64String()
				.DecompressFromBase64String();
			Assert.That(stream.GetMimeType(), Is.EqualTo("text/xml"));
		}

		[Test]
		public void GetXmlMimeType()
		{
			var stream = ResourceManager.Load("Schema.xsd");
			Assert.That(stream.GetMimeType(), Is.EqualTo("text/xml"));
		}

		[Test]
		public void OutputEqualsInputAfterCopy()
		{
			var sourceBuffer = new byte[100];
			sourceBuffer[56] = 123;
			var source = new MemoryStream(sourceBuffer);
			var target = new MemoryStream();

			source.CopyTo(target);
			Assert.That(target.Length, Is.EqualTo(source.Length));

			var targetBuffer = new byte[100];
			target.Position = 0;
			target.Read(targetBuffer, 0, 100);
			Assert.That(targetBuffer, Is.EquivalentTo(sourceBuffer));
		}

		[Test]
		public void PositionIsAtEndOfStreamForSourceAndTargetAfterCopy()
		{
			var source = new MemoryStream();
			source.Write(new byte[100], 0, 100);
			source.Position = 0;

			var target = new MemoryStream();
			target.Write(new byte[20], 0, 20);
			target.Position = 0;

			source.CopyTo(target);

			Assert.That(source.Position, Is.EqualTo(source.Length));
			Assert.That(target.Position, Is.EqualTo(target.Length));
		}

		[Test]
		public void ThrowsWhenCompressingFromNonReadableInput()
		{
			var input = new Mock<Stream>();
			input.Setup(s => s.CanRead).Returns(false);

			Assert.That(() => input.Object.CompressToBase64String(), Throws.InvalidOperationException);
		}

		[Test]
		public void ThrowsWhenCompressingFromNullInput()
		{
			Assert.That(() => ((Stream) null).CompressToBase64String(), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void ThrowsWhenCopyingFromNonReadableStream()
		{
			var source = new Mock<Stream>();
			source.Setup(s => s.CanRead).Returns(false);
			var target = new MemoryStream();

			Assert.That(
				() => source.Object.CopyTo(target),
				Throws.TypeOf<ObjectDisposedException>().With.Message.EqualTo("Cannot access a closed Stream."));
		}

		[Test]
		public void ThrowsWhenCopyingToNonWritableTarget()
		{
			var source = new MemoryStream();
			var target = new Mock<Stream>();
			target.Setup(s => s.CanWrite).Returns(false);

			Assert.That(
				() => source.CopyTo(target.Object),
				Throws.TypeOf<ObjectDisposedException>().With.Message.StartsWith("Cannot access a closed Stream."));
		}

		[Test]
		public void ThrowsWhenCopyingToNullTarget()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Assert.That(() => new MemoryStream().CopyTo(null), Throws.TypeOf<ArgumentNullException>());
		}
	}
}

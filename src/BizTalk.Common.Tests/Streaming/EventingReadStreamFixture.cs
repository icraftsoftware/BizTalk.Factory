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
using System.Text;
using Be.Stateless.IO.Extensions;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Streaming
{
	[TestFixture]
	public class EventingReadStreamFixture
	{
		[Test]
		public void InnerStreamIsClosed()
		{
			var innerStream = new Mock<Stream> { CallBase = true };
			innerStream.Setup(s => s.Close()).Verifiable("innerStream");
			using (new EventingReadStream(innerStream.Object)) { }
			innerStream.Verify();
		}

		[Test]
		public void InnerStreamIsDisposed()
		{
			// notice that because sourceStream.As<IDisposable>().Verify(s => s.Dispose()) is, oddly enough, never
			// satisfied, fallback on testing that the protected override Dispose(bool disposing) is being called,
			// which is just an indirect way to test that dispose is being called.
			// notice also that Dispose just call Close... why are we bother at all... :/

			var innerStream = new Mock<Stream> { CallBase = true };
			innerStream.Protected().Setup("Dispose", true).Verifiable("sourceStream");
			using (new EventingReadStream(innerStream.Object)) { }
			innerStream.Verify();
		}

		[Test]
		public void LengthCanOnlyBeReadAfterStreamExhaustion()
		{
			var streamMock = new Mock<MemoryStream>(_content) { CallBase = true };
			using (var stream = new EventingReadStream(streamMock.Object))
			{
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => stream.Length, Throws.TypeOf<NotSupportedException>());
				// ReSharper restore AccessToDisposedClosure
				stream.Drain();
				Assert.That(stream.Length, Is.EqualTo(_content.Length));
			}
		}

		[Test]
		public void LengthCanOnlyBeReadAfterStreamExhaustionIfStreamIsNotSeekable()
		{
			var streamMock = new Mock<MemoryStream>(_content) { CallBase = true };
			streamMock.SetupGet(s => s.CanSeek).Returns(false);
			using (var stream = new EventingReadStream(streamMock.Object))
			{
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => stream.Length, Throws.TypeOf<NotSupportedException>());
				// ReSharper restore AccessToDisposedClosure
				stream.Drain();
				Assert.That(stream.Length, Is.EqualTo(_content.Length));
			}
		}

		[Test]
		public void LengthWontBeAdjustedAnyMoreAfterStreamExhaustion()
		{
			using (var stream = new EventingReadStream(new MemoryStream(_content)))
			{
				stream.Drain();
				Assert.That(stream.Length, Is.EqualTo(_content.Length));
				stream.Position = 0;
				stream.Drain();
				Assert.That(stream.Length, Is.EqualTo(_content.Length));
			}
		}

		[Test]
		public void ReadDoesNotCorruptStream()
		{
			using (var target = new MemoryStream())
			using (var stream = new EventingReadStream(new MemoryStream(_content)))
			{
				// ensure working buffer's size is less than content's total length
				stream.CopyTo(target, _content.Length / 8);
				Assert.That(target.ToArray(), Is.EqualTo(_content));
			}
		}

		[Test]
		public void ReadEnsuresEndOfStreamIsReachable()
		{
			// BizTalk Factory's EventingReadStream will eventually set ReadCompleted to true, AfterLastReadEvent will be fired as well
			var tempBuffer = new byte[1024];
			using (var stream = new EventingReadStream(new MemoryStream(_content)))
			{
				var edgeEventsCount = 0;
				stream.AfterLastReadEvent += (sender, args) => ++edgeEventsCount;
				while (stream.Read(tempBuffer, 0, tempBuffer.Length) == tempBuffer.Length) { }
				Assert.That(stream.ReadCompleted);
				Assert.That(edgeEventsCount, Is.EqualTo(1));
			}

			// while MicrosoftEventingReadStream will not always set ReadCompleted to true, AfterLastReadEvent won't be fired either
			using (var stream = new MicrosoftEventingReadStream())
			{
				var edgeEventsCount = 0;
				stream.AfterLastReadEvent += (sender, args) => ++edgeEventsCount;
				while (stream.Read(tempBuffer, 0, tempBuffer.Length) == tempBuffer.Length) { }
				Assert.That(stream.ReadCompleted, Is.False);
				Assert.That(edgeEventsCount, Is.EqualTo(0));
			}
		}

		static EventingReadStreamFixture()
		{
			var content = string.Empty;
			for (var i = 0; i < 70; i++)
			{
				content += Guid.NewGuid().ToString("N");
			}
			_content = Encoding.Default.GetBytes(content);
		}

		private class MicrosoftEventingReadStream : Microsoft.BizTalk.Streaming.EventingReadStream
		{
			#region Base Class Member Overrides

			public override bool CanRead
			{
				get { return true; }
			}

			public override bool CanSeek
			{
				get { throw new NotSupportedException(); }
			}

			public override void Flush()
			{
				throw new NotSupportedException();
			}

			public override long Length
			{
				get { throw new NotSupportedException(); }
			}

			public override long Position { get; set; }

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region Base Class Member Overrides

			protected override int ReadInternal(byte[] buffer, int offset, int count)
			{
				// always read less bytes than requested
				return _innerStream.Read(buffer, 0, count - 7);
			}

			#endregion

			private readonly Stream _innerStream = new MemoryStream(_content);
		}

		private static readonly byte[] _content;
	}
}

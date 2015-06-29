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
using System.Text;
using Be.Stateless.IO;
using Be.Stateless.IO.Extensions;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Streaming
{
	[TestFixture]
	public class ReplicatingReadStreamFixture
	{
		[Test]
		public void LengthCanBeReadAfterStreamExhaustion()
		{
			using (var stream = new ReplicatingReadStream(new MemoryStream(_content), new MemoryStream()))
			{
				stream.Drain();
				Assert.That(stream.Length, Is.EqualTo(_content.Length));
			}
		}

		[Test]
		public void LengthIsUnknownBeforeStreamExhaustion()
		{
			using (var stream = new ReplicatingReadStream(new MemoryStream(_content), new MemoryStream()))
			{
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => stream.Length, Throws.TypeOf<NotSupportedException>());
				// ReSharper restore AccessToDisposedClosure
			}
		}

		[Test]
		public void ReplicatingReadStreamRequiresTargetStreamToReplicate()
		{
			Assert.That(
				() => new ReplicatingReadStream(new MemoryStream(_content), null),
				Throws.InstanceOf<ArgumentNullException>().With.Message.EqualTo("Value cannot be null.\r\nParameter name: target"));
		}

		[Test]
		public void SourceAndTargetStreamsAreClosed()
		{
			var sourceStream = new Mock<Stream> { CallBase = true };
			sourceStream.Setup(s => s.Close()).Verifiable("sourceStream");
			var targetStream = new Mock<Stream> { CallBase = true };
			targetStream.Setup(s => s.Close()).Verifiable("targetStream");
			using (new ReplicatingReadStream(sourceStream.Object, targetStream.Object)) { }
			sourceStream.Verify();
			targetStream.Verify();
		}

		[Test]
		public void SourceAndTargetStreamsAreDisposed()
		{
			// notice that because sourceStream.As<IDisposable>().Verify(s => s.Dispose()) is, oddly enough, never
			// satisfied, fallback on testing that the protected override Dispose(bool disposing) is being called,
			// which is just an indirect way to test that dispose is being called.
			// notice also that Dispose just call Close... why are we bother at all... :/

			var sourceStream = new Mock<Stream> { CallBase = true };
			sourceStream.Protected().Setup("Dispose", true).Verifiable("sourceStream");
			var targetStream = new Mock<Stream> { CallBase = true };
			targetStream.Protected().Setup("Dispose", true).Verifiable("targetStream");
			using (new ReplicatingReadStream(sourceStream.Object, targetStream.Object)) { }
			sourceStream.Verify();
			targetStream.Verify();
		}

		[Test]
		public void SourceStreamCannotBeSoughtBeforeExhaustion()
		{
			using (var stream = new ReplicatingReadStream(new MemoryStream(_content), new MemoryStream()))
			{
				Assert.That(stream.CanSeek, Is.False);
				// don't drain the whole stream
				stream.Read(new byte[1024], 0, 1024);
				Assert.That(
					// ReSharper disable AccessToDisposedClosure
					() => stream.Position = 0,
					// ReSharper restore AccessToDisposedClosure
					Throws.InstanceOf<InvalidOperationException>()
						.With.Message.EqualTo(
							string.Format("{0} is not seekable while the inner stream has not been thoroughly read and replicated.", typeof(ReplicatingReadStream).Name)));
				Assert.That(
					// ReSharper disable AccessToDisposedClosure
					() => stream.Seek(0, SeekOrigin.Begin),
					// ReSharper restore AccessToDisposedClosure
					Throws.InstanceOf<InvalidOperationException>()
						.With.Message.EqualTo(
							string.Format("{0} cannot be sought while the inner stream has not been thoroughly read and replicated.", typeof(ReplicatingReadStream).Name)));
			}
		}

		[Test]
		public void SourceStreamIsReplicatedToTargetStreamWhileBeingRead()
		{
			using (var targetStream = new MemoryStream())
			using (var stream = new ReplicatingReadStream(new MemoryStream(_content), targetStream))
			{
				stream.Drain();
				Assert.That(_content, Is.EqualTo(targetStream.ToArray()));
			}
		}

		[Test]
		public void SourceStreamIsSeekableAfterExhaustion()
		{
			using (var stream = new ReplicatingReadStream(new MemoryStream(_content), new MemoryStream()))
			{
				Assert.That(stream.CanSeek, Is.False);
				stream.Drain();
				Assert.That(stream.CanSeek);
				// ReSharper disable AccessToDisposedClosure
				Assert.That(() => stream.Position = 0, Throws.Nothing);
				// ReSharper restore AccessToDisposedClosure
			}
		}

		[Test]
		public void TargetStreamIsCommitedOnlyOnceEvenIfStreamIsRewinded()
		{
			var targetStream = new Mock<Stream> { CallBase = true };
			var streamTransacted = targetStream.As<IStreamTransacted>();
			streamTransacted.Setup(s => s.Commit());
			using (var stream = new ReplicatingReadStream(new MemoryStream(_content), targetStream.Object))
			{
				stream.Drain();
				stream.Position = 0;
				stream.Drain();
			}
			streamTransacted.Verify(s => s.Commit(), Times.Once());
		}

		[Test]
		public void TargetStreamIsCommittedUponSourceStreamExhaustion()
		{
			var targetStream = new Mock<Stream> { CallBase = true };
			targetStream.As<IStreamTransacted>().Setup(st => st.Commit()).Verifiable("targetStream");
			using (var stream = new ReplicatingReadStream(new MemoryStream(_content), targetStream.Object))
			{
				stream.Drain();
			}
			targetStream.VerifyAll();
		}

		[Test]
		public void TargetStreamIsNotCommittedIfSourceStreamNotExhausted()
		{
			var targetStream = new Mock<Stream> { CallBase = true };
			targetStream.As<IStreamTransacted>();
			using (var stream = new ReplicatingReadStream(new MemoryStream(_content), targetStream.Object))
			{
				// don't drain the whole stream
				stream.Read(new byte[1024], 0, 1024);
			}
			// Rollback() is never called explicitly when targetStream is disposed, but neither is Commit()
			targetStream.As<IStreamTransacted>().Verify(s => s.Commit(), Times.Never());
		}

		private readonly byte[] _content = Encoding.Unicode.GetBytes(new string('A', 3999));
	}
}

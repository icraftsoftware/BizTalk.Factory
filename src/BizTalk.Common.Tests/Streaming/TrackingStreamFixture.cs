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
using System.Text;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Reflection;
using Microsoft.BizTalk.Streaming;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Streaming
{
	[TestFixture]
	public class TrackingStreamFixture
	{
		[Test]
		public void CaptureDrainsInnerStream()
		{
			using (var innerStream = new MemoryStream(_content))
			using (var stream = new TrackingStream(innerStream))
			{
				stream.SetupCapture(new MessageBodyCaptureDescriptor("some-data", MessageBodyCaptureMode.Claimed), new MemoryStream());
				Assert.That(innerStream.Position, Is.EqualTo(0));
				Assert.That(
					// ReSharper disable AccessToDisposedClosure
					() => stream.Capture(),
					// ReSharper restore AccessToDisposedClosure
					Throws.Nothing);
				Assert.That(innerStream.Position, Is.EqualTo(innerStream.Length));
			}
		}

		[Test]
		public void CaptureSetupResetMarkablePosition()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content)))
			{
				var ms = stream.AsMarkable();
				ms.Drain();
				Assert.That(stream.Position, Is.EqualTo(_content.Length));

				stream.SetupCapture(new MessageBodyCaptureDescriptor("some-data", MessageBodyCaptureMode.Unclaimed));
				Assert.That(stream.Position, Is.EqualTo(0));
			}
		}

		[Test]
		public void CaptureThrowsWhenCaptureModeIsNotClaimed()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content)))
			{
				stream.SetupCapture(new MessageBodyCaptureDescriptor("some-data", MessageBodyCaptureMode.Unclaimed));
				Assert.That(
					// ReSharper disable AccessToDisposedClosure
					() => stream.Capture(),
					// ReSharper restore AccessToDisposedClosure
					Throws.InstanceOf<InvalidOperationException>()
						.With.Message.EqualTo("TrackingStream cannot be captured because its Descriptor's CaptureMode has not been set to Claimed but to Unclaimed."));
			}
		}

		[Test]
		public void CaptureThrowsWhenNoCaptureDescriptor()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content)))
			{
				Assert.That(
					// ReSharper disable AccessToDisposedClosure
					() => stream.Capture(),
					// ReSharper restore AccessToDisposedClosure
					Throws.InstanceOf<InvalidOperationException>()
						.With.Message.EqualTo("TrackingStream cannot be captured because its Descriptor is null and has not been initialized for tracking."));
			}
		}

		[Test]
		public void CaptureThrowsWhenNoCapturingStream()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content), new MessageBodyCaptureDescriptor("url", MessageBodyCaptureMode.Claimed)))
			{
				Assert.That(
					// ReSharper disable AccessToDisposedClosure
					() => stream.Capture(),
					// ReSharper restore AccessToDisposedClosure
					Throws.InstanceOf<InvalidOperationException>()
						.With.Message.EqualTo("TrackingStream cannot be captured unless it has been setup with another capturing stream to replicate its payload to."));
			}
		}

		[Test]
		public void EventsAreNotFiredThroughMarkableInnerStream()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content)))
			{
				var edgeEventsCount = 0;
				stream.AfterLastReadEvent += (sender, args) => ++edgeEventsCount;
				stream.BeforeFirstReadEvent += (sender, args) => ++edgeEventsCount;
				var eventsCount = 0;
				stream.ReadEvent += (sender, args) => ++eventsCount;

				var markableForwardOnlyEventingReadStream = stream.AsMarkable();
				Assert.That(edgeEventsCount, Is.EqualTo(0));
				Assert.That(eventsCount, Is.EqualTo(0));

				markableForwardOnlyEventingReadStream.Drain();
				Assert.That(edgeEventsCount, Is.EqualTo(0));
				Assert.That(eventsCount, Is.EqualTo(0));
			}
		}

		[Test]
		public void InnerStreamIsWrappedByMarkableStream()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content)))
			{
				stream.AsMarkable();
				Assert.That(Reflector.GetProperty(stream, "InnerStream"), Is.InstanceOf<MarkableForwardOnlyEventingReadStream>());
			}
		}

		[Test]
		public void InnerStreamIsWrappedByReplicatingStreamIfTracked()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content)))
			{
				stream.SetupCapture(new MessageBodyCaptureDescriptor("some-data", MessageBodyCaptureMode.Claimed), new MemoryStream());
				Assert.That(Reflector.GetProperty(stream, "InnerStream"), Is.InstanceOf<ReplicatingReadStream>());
			}
		}

		[Test]
		public void PayloadIsBeingRedeemed()
		{
			Assert.That(
				() => new TrackingStream(new MemoryStream(), new MessageBodyCaptureDescriptor("url", MessageBodyCaptureMode.Unclaimed)),
				Throws.ArgumentException.With.Message.StartsWith(
					"A TrackingStream, whose payload is being redeemed, cannot be instantiated with a CaptureDescriptor having a CaptureMode of Unclaimed; "
						+ "the only compliant CaptureMode is Claimed."));

			using (var stream = new TrackingStream(new MemoryStream(_content), new MessageBodyCaptureDescriptor("url", MessageBodyCaptureMode.Claimed)))
			{
				Assert.That(stream.IsRedeemed, Is.True);
			}
		}

		[Test]
		public void PayloadIsNotBeingRedeemed()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content)))
			{
				Assert.That(stream.IsRedeemed, Is.False);
				stream.SetupCapture(new MessageBodyCaptureDescriptor("some-data", MessageBodyCaptureMode.Unclaimed));
				Assert.That(stream.IsRedeemed, Is.False);
			}
			using (var stream = new TrackingStream(new MemoryStream(_content)))
			{
				Assert.That(stream.IsRedeemed, Is.False);
				stream.SetupCapture(new MessageBodyCaptureDescriptor("url", MessageBodyCaptureMode.Claimed), new MemoryStream());
				Assert.That(stream.IsRedeemed, Is.False);
			}
		}

		[Test]
		public void SetupCaptureAsClaimedThrowsWhenCaptureModeIsOther()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content)))
			{
				Assert.That(
					// ReSharper disable AccessToDisposedClosure
					() => stream.SetupCapture(new MessageBodyCaptureDescriptor("some-data", MessageBodyCaptureMode.Unclaimed), new MemoryStream()),
					// ReSharper restore AccessToDisposedClosure
					Throws.InstanceOf<InvalidOperationException>()
						.With.Message.EqualTo("TrackingStream's capture cannot be setup with a CaptureMode of Unclaimed; other CaptureMode than Claimed cannot use a capturing stream."));
			}
		}

		[Test]
		public void SetupCaptureAsClaimedThrowsWithoutCapturingStream()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content)))
			{
				Assert.That(
					// ReSharper disable AccessToDisposedClosure
					() => stream.SetupCapture(new MessageBodyCaptureDescriptor("some-data", MessageBodyCaptureMode.Claimed), null),
					// ReSharper restore AccessToDisposedClosure
					Throws.InstanceOf<ArgumentNullException>()
						.With.Property("ParamName").EqualTo("capturingStream"));
			}
		}

		[Test]
		public void SetupCaptureAsUnclaimedThrowsWhenCaptureModeIsOther()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content)))
			{
				Assert.That(
					// ReSharper disable AccessToDisposedClosure
					() => stream.SetupCapture(new MessageBodyCaptureDescriptor("some-data", MessageBodyCaptureMode.Claimed)),
					// ReSharper restore AccessToDisposedClosure
					Throws.InstanceOf<InvalidOperationException>()
						.With.Message.EqualTo("TrackingStream's capture cannot be setup with a CaptureMode of Claimed; other CaptureMode than Unclaimed requires a capturing stream."));
			}
		}

		[Test]
		public void SetupCaptureThrowsIfCaptureDescriptorHasAlreadyBeenSetup()
		{
			using (var stream = new TrackingStream(new MemoryStream(_content), new MessageBodyCaptureDescriptor("some-data", MessageBodyCaptureMode.Claimed)))
			{
				Assert.That(
					// ReSharper disable AccessToDisposedClosure
					() => stream.SetupCapture(new MessageBodyCaptureDescriptor("other-data", MessageBodyCaptureMode.Unclaimed)),
					// ReSharper restore AccessToDisposedClosure
					Throws.InstanceOf<InvalidOperationException>()
						.With.Message.EqualTo("TrackingStream's capture has already been setup and cannot be overwritten."));

				Assert.That(
					// ReSharper disable AccessToDisposedClosure
					() => stream.SetupCapture(new MessageBodyCaptureDescriptor("other-data", MessageBodyCaptureMode.Claimed), new MemoryStream()),
					// ReSharper restore AccessToDisposedClosure
					Throws.InstanceOf<InvalidOperationException>()
						.With.Message.EqualTo("TrackingStream's capture has already been setup and cannot be overwritten."));
			}
		}

		private readonly byte[] _content = Encoding.Unicode.GetBytes(new string('A', 3999));
	}
}

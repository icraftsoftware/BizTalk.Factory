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
using System.Xml.Schema;
using Be.Stateless.IO;
using Be.Stateless.Xml.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	[TestFixture]
	public class BaseMessagePartFixture
	{
		[Test]
		public void AsMessageBodyCaptureDescriptor()
		{
			const string expected = "claim";
			var originalStream = MessageFactory.CreateClaimCheckOut(expected).AsStream();
			var part = new Mock<IBaseMessagePart>();
			part.Setup(p => p.GetOriginalDataStream())
				.Returns(originalStream);

			var descriptor = part.Object.AsMessageBodyCaptureDescriptor();

			Assert.That(descriptor.Data, Is.EqualTo(expected));
		}

		[Test]
		public void AsMessageBodyCaptureDescriptorThrowsWhenInvalidClaimTokenMessage()
		{
			//var originalStream = new StringStream("<root>text</root>");
			var originalStream = new StringStream("<ns0:CheckOut xmlns:ns0='urn:schemas.stateless.be:biztalk:claim:2013:04'></ns0:CheckOut>");
			var part = new Mock<IBaseMessagePart>();
			part.Setup(p => p.GetOriginalDataStream())
				.Returns(originalStream);

			Assert.That(
				() => part.Object.AsMessageBodyCaptureDescriptor(),
				Throws.TypeOf<XmlSchemaValidationException>());
		}

		[Test]
		public void SetDataStream()
		{
			var stream = new MemoryStream();
			var part = new Mock<IBaseMessagePart>().SetupAllProperties();
			var tracker = new Mock<IResourceTracker>();

			part.Object.SetDataStream(stream, tracker.Object);

			Assert.That(part.Object.Data, Is.SameAs(stream));
			tracker.Verify(t => t.AddResource(stream));
		}

		[Test]
		public void SetDataStreamThrowsIfNullArguments()
		{
			Assert.That(
				() => ((IBaseMessagePart) null).SetDataStream(null, null),
				Throws.InstanceOf<ArgumentNullException>().With.Property("ParamName").EqualTo("messagePart"));

			var part = new Mock<IBaseMessagePart>().SetupAllProperties();
			Assert.That(
				() => part.Object.SetDataStream(null, null),
				Throws.InstanceOf<ArgumentNullException>().With.Property("ParamName").EqualTo("stream"));

			Assert.That(
				() => part.Object.SetDataStream(new MemoryStream(), null),
				Throws.InstanceOf<ArgumentNullException>().With.Property("ParamName").EqualTo("tracker"));
		}

		[Test]
		public void WrapOriginalDataStream()
		{
			var originalStream = new MemoryStream();
			var part = new Mock<IBaseMessagePart>()
				.SetupAllProperties();
			part.Setup(p => p.GetOriginalDataStream())
				.Returns(originalStream);
			var tracker = new Mock<IResourceTracker>();

			Stream wrapper = null;
			part.Object.WrapOriginalDataStream(s => wrapper = new MarkableForwardOnlyEventingReadStream(s), tracker.Object);

			Assert.That(part.Object.Data, Is.SameAs(wrapper));
			tracker.Verify(t => t.AddResource(wrapper));
		}

		[Test]
		public void WrapOriginalDataStreamDoesNothingIfWrappingStreamIsSameAsOriginalStream()
		{
			var originalStream = new MemoryStream();
			var part = new Mock<IBaseMessagePart>()
				.SetupAllProperties();
			part.Setup(p => p.GetOriginalDataStream())
				.Returns(originalStream);
			var tracker = new Mock<IResourceTracker>();

			part.Object.WrapOriginalDataStream(s => s, tracker.Object);

			// is null because mock did not setup property... which allows us to indirectly test that no wrapping has taken place
			Assert.That(part.Object.Data, Is.Null);
			tracker.Verify(t => t.AddResource(It.IsAny<object>()), Times.Never());
		}

		[Test]
		public void WrapOriginalDataStreamThrowsIfNullArguments()
		{
			Assert.That(
				() => ((IBaseMessagePart) null).WrapOriginalDataStream<Stream>(null, null),
				Throws.InstanceOf<ArgumentNullException>().With.Property("ParamName").EqualTo("messagePart"));

			var part = new Mock<IBaseMessagePart>().SetupAllProperties();
			Assert.That(
				() => part.Object.WrapOriginalDataStream<Stream>(null, null),
				Throws.InstanceOf<ArgumentNullException>().With.Property("ParamName").EqualTo("wrapper"));

			Assert.That(
				() => part.Object.WrapOriginalDataStream(s => new MarkableForwardOnlyEventingReadStream(s), null),
				Throws.InstanceOf<ArgumentNullException>().With.Property("ParamName").EqualTo("tracker"));
		}
	}
}

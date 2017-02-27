#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Runtime.Caching;
using Be.Stateless.BizTalk.Transform;
using Be.Stateless.BizTalk.Transforms.ToSql.Procedures.Batch;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.BizTalk.Unit.Transform;
using Be.Stateless.BizTalk.Xml;
using Be.Stateless.IO;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Streaming.Extensions
{
	[TestFixture]
	public class TransformerFixture
	{
		[Test]
		public void ApplyTransformsWithImportedAndIncludedStylesheets()
		{
			using (var stream = ResourceManager.Load("Be.Stateless.BizTalk.Streaming.Data.BatchContent.xml").Transform().Apply(typeof(CompositeEmbeddedTransform)))
			using (var reader = XmlReader.Create(stream))
			{
				reader.MoveToContent();
				Assert.That(
					reader.ReadOuterXml(),
					Contains.Substring("\n  <ns:Parts>\n    dropped by Included.xsl\n    dropped by Imported.xsl\n    dropped by EmbeddedTransform.xsl\n  </ns:Parts>"));
			}
		}

		[Test]
		public void BuildArgumentListYieldsFreshCopyWhenRequired()
		{
			var arguments = new XsltArgumentList();
			var contextMock = new Mock<IBaseMessageContext>();

			var sut = new Transformer(new Stream[] { new MemoryStream() });
			sut.ExtendWith(contextMock.Object);

			var descriptor = XsltCache.Instance[typeof(IdentityTransform)];

			// no specific arguments, no message ctxt requirement, same XsltArgumentList instance can be shared
			Assert.That(sut.BuildArgumentList(descriptor, null), Is.SameAs(descriptor.Arguments));

			// specific arguments, no message ctxt requirement, new XsltArgumentList instance is required
			Assert.That(sut.BuildArgumentList(descriptor, arguments), Is.Not.SameAs(descriptor.Arguments).And.Not.SameAs(arguments));

			descriptor = XsltCache.Instance[typeof(AnyToAddPart)];

			// no specific arguments but message ctxt requirement, new XsltArgumentList instance is required
			Assert.That(sut.BuildArgumentList(descriptor, null), Is.Not.SameAs(descriptor.Arguments).And.Not.SameAs(arguments));

			// specific arguments and message ctxt requirement, new XsltArgumentList instance is required
			Assert.That(sut.BuildArgumentList(descriptor, arguments), Is.Not.SameAs(descriptor.Arguments).And.Not.SameAs(arguments));
		}

		[Test]
		public void HonoursMessageContextExtensionRequirements()
		{
			var stream = new StringStream("<?xml version=\"1.0\" encoding=\"utf-16\" ?><root></root>");
			var contextMock = new Mock<IBaseMessageContext>();
			var transform = typeof(AnyToAddPart);
			var arguments = XsltCache.Instance[transform].Arguments;

			Assert.That(arguments.GetExtensionObject(BaseMessageContextFunctions.TARGET_NAMESPACE), Is.Null);

			var sut = new Transformer(new Stream[] { stream });
			sut.ExtendWith(contextMock.Object).Apply(transform);

			Assert.That(arguments.GetExtensionObject(BaseMessageContextFunctions.TARGET_NAMESPACE), Is.Null);

			contextMock.Verify(c => c.Read(BizTalkFactoryProperties.EnvelopeSpecName.Name, BizTalkFactoryProperties.EnvelopeSpecName.Namespace), Times.Once());
			contextMock.Verify(c => c.Read(TrackingProperties.MessagingStepActivityId.Name, TrackingProperties.MessagingStepActivityId.Namespace), Times.Once());
		}

		[Test]
		public void NoMessageContextExtensionRequirements()
		{
			var stream = new StringStream("<?xml version=\"1.0\" encoding=\"utf-16\" ?><root></root>");
			var transform = typeof(IdentityTransform);
			var arguments = XsltCache.Instance[transform].Arguments;
			Assert.That(arguments.GetExtensionObject(BaseMessageContextFunctions.TARGET_NAMESPACE), Is.Null);

			var sut = new Transformer(new Stream[] { stream });
			sut.Apply(transform);

			Assert.That(arguments.GetExtensionObject(BaseMessageContextFunctions.TARGET_NAMESPACE), Is.Null);
		}

		[Test]
		public void ThrowIfCannotHonourMessageContextExtensionRequirements()
		{
			var stream = new StringStream("<?xml version=\"1.0\" encoding=\"utf-16\" ?><root></root>");
			var transform = typeof(AnyToAddPart);

			var sut = new Transformer(new Stream[] { stream });
			Assert.That(
				() => sut.Apply(transform),
				Throws.TypeOf<ArgumentNullException>().With.Message.EqualTo("Value cannot be null.\r\nParameter name: context"));
		}
	}
}

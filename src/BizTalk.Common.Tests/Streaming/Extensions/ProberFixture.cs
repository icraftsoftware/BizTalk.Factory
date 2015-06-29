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

using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.IO;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Streaming.Extensions
{
	[TestFixture]
	public class ProberFixture
	{
		[Test]
		public void BatchDescriptor()
		{
			using (var stream = ResourceManager.Load("BatchContent.xml").AsMarkable())
			{
				var batchDescriptor = ((IProbeBatchContentStream) stream.Probe()).BatchDescriptor;
				Assert.That(batchDescriptor.EnvelopeSpecName, Is.EqualTo(new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName));
				Assert.That(batchDescriptor.Partition, Is.Null);
			}
		}

		[Test]
		public void BatchDescriptorHasPartition()
		{
			const string batchContent = @"<ns:BatchContent xmlns:ns=""urn:schemas.stateless.be:biztalk:batch:2012:12"">
  <ns:EnvelopeSpecName>envelope-spec-name</ns:EnvelopeSpecName>
  <ns:Partition>p-one</ns:Partition>
  <ns:MessagingStepActivityIds>013684EE620E4A0BB6D6F7355B26D21B</ns:MessagingStepActivityIds>
  <ns:Parts />
</BatchContent>";
			using (var stream = new StringStream(batchContent).AsMarkable())
			{
				var batchDescriptor = ((IProbeBatchContentStream) stream.Probe()).BatchDescriptor;
				Assert.That(batchDescriptor.EnvelopeSpecName, Is.EqualTo("envelope-spec-name"));
				Assert.That(batchDescriptor.Partition, Is.EqualTo("p-one"));
			}
		}

		[Test]
		public void BatchDescriptorIsNullIfIncompleteBatchContent()
		{
			const string batchContent = @"<ns:BatchContent xmlns:ns=""urn:schemas.stateless.be:biztalk:batch:2012:12"" />";
			using (var stream = new StringStream(batchContent).AsMarkable())
			{
				var batchDescriptor = ((IProbeBatchContentStream) stream.Probe()).BatchDescriptor;
				Assert.That(batchDescriptor, Is.Null);
			}
		}

		[Test]
		public void BatchDescriptorIsNullIfInvalidBatchContent()
		{
			const string batchContent = @"<ns:BatchContent xmlns:ns=""urn:schemas.stateless.be:biztalk:batch:2012:12"">
  <ns:MessagingStepActivityIds />
  <ns:EnvelopeSpecName>Be.Stateless.BizTalk.Schemas.Xml.BatchControl+ReleaseBatches</ns:EnvelopeSpecName>
  <ns:Parts />
</BatchContent>";
			using (var stream = new StringStream(batchContent).AsMarkable())
			{
				var batchDescriptor = ((IProbeBatchContentStream) stream.Probe()).BatchDescriptor;
				Assert.That(batchDescriptor, Is.Null);
			}
		}

		[Test]
		public void BatchDescriptorIsNullIfNotXml()
		{
			using (var stream = new StringStream("invalid xml content").AsMarkable())
			{
				var batchDescriptor = ((IProbeBatchContentStream) stream.Probe()).BatchDescriptor;
				Assert.That(batchDescriptor, Is.Null);
			}
		}

		[Test]
		public void BatchTrackingContext()
		{
			using (var stream = ResourceManager.Load("BatchContent.xml").AsMarkable())
			{
				var batchTrackingContext = ((IProbeBatchContentStream) stream.Probe()).BatchTrackingContext;
				Assert.That(
					batchTrackingContext.MessagingStepActivityIdList,
					Is.EqualTo(new[] { "013684EE620E4A0BB6D6F7355B26D21B", "08FCB363E00F4BD78D15D8EB2E80B411", "0B12CC6AE51740F6ABF672E3B32B496D" }));
				Assert.That(
					batchTrackingContext.ProcessActivityId,
					Is.EqualTo("A800441B209E46A087A16833661590C2"));
			}
		}

		[Test]
		public void BatchTrackingContextIsEmptyIfIncompleteBatchContent()
		{
			const string batchContent = @"<ns:BatchContent xmlns:ns=""urn:schemas.stateless.be:biztalk:batch:2012:12"" />";
			using (var stream = new StringStream(batchContent).AsMarkable())
			{
				var batchTrackingContext = ((IProbeBatchContentStream) stream.Probe()).BatchTrackingContext;
				Assert.That(batchTrackingContext.ProcessActivityId, Is.Null);
				Assert.That(batchTrackingContext.MessagingStepActivityIdList, Is.Null);
			}
		}

		[Test]
		public void BatchTrackingContextIsEmptyIfInvalidBatchContent()
		{
			const string batchContent = @"<ns:BatchContent xmlns:ns=""urn:schemas.stateless.be:biztalk:batch:2012:12"">
  <ns:MessagingStepActivityIds />
  <ns:EnvelopeSpecName>Be.Stateless.BizTalk.Schemas.Xml.BatchControl+ReleaseBatches</ns:EnvelopeSpecName>
  <ns:Parts />
</BatchContent>";
			using (var stream = new StringStream(batchContent).AsMarkable())
			{
				var batchTrackingContext = ((IProbeBatchContentStream) stream.Probe()).BatchTrackingContext;
				Assert.That(batchTrackingContext.ProcessActivityId, Is.Null);
				Assert.That(batchTrackingContext.MessagingStepActivityIdList, Is.Null);
			}
		}

		[Test]
		public void BatchTrackingContextIsNullIfNotXml()
		{
			using (var stream = new StringStream("invalid xml content").AsMarkable())
			{
				var batchTrackingContext = ((IProbeBatchContentStream) stream.Probe()).BatchTrackingContext;
				Assert.That(batchTrackingContext, Is.Null);
			}
		}

		[Test]
		public void BatchTrackingContextOnlyHasMessagingStepActivityIds()
		{
			const string batchContent = @"<ns:BatchContent xmlns:ns=""urn:schemas.stateless.be:biztalk:batch:2012:12"">
  <ns:EnvelopeSpecName>Be.Stateless.BizTalk.Schemas.Xml.BatchControl+ReleaseBatches</ns:EnvelopeSpecName>
  <ns:MessagingStepActivityIds>013684EE620E4A0BB6D6F7355B26D21B,08FCB363E00F4BD78D15D8EB2E80B411,0B12CC6AE51740F6ABF672E3B32B496D</ns:MessagingStepActivityIds>
  <ns:Parts />
</BatchContent>";
			using (var stream = new StringStream(batchContent).AsMarkable())
			{
				var batchTrackingContext = ((IProbeBatchContentStream) stream.Probe()).BatchTrackingContext;
				Assert.That(
					batchTrackingContext.ProcessActivityId,
					Is.Null);
				Assert.That(
					batchTrackingContext.MessagingStepActivityIdList,
					Is.EqualTo(new[] { "013684EE620E4A0BB6D6F7355B26D21B", "08FCB363E00F4BD78D15D8EB2E80B411", "0B12CC6AE51740F6ABF672E3B32B496D" }));
			}
		}

		[Test]
		public void BatchTrackingContextOnlyHasProcessActivityId()
		{
			const string batchContent = @"<ns:BatchContent xmlns:ns=""urn:schemas.stateless.be:biztalk:batch:2012:12"">
  <ns:EnvelopeSpecName>Be.Stateless.BizTalk.Schemas.Xml.BatchControl+ReleaseBatches</ns:EnvelopeSpecName>
  <ns:Partition>partition</ns:Partition>
  <ns:ProcessActivityId>A800441B209E46A087A16833661590C2</ns:ProcessActivityId>
  <ns:Parts />
</BatchContent>";
			using (var stream = new StringStream(batchContent).AsMarkable())
			{
				var batchTrackingContext = ((IProbeBatchContentStream) stream.Probe()).BatchTrackingContext;
				Assert.That(
					batchTrackingContext.ProcessActivityId,
					Is.EqualTo("A800441B209E46A087A16833661590C2"));
				Assert.That(
					batchTrackingContext.MessagingStepActivityIdList,
					Is.Null);
			}
		}

		[Test]
		public void MessageTypeIsNullIfNotXml()
		{
			using (var stream = new StringStream("invalid xml content").AsMarkable())
			{
				Assert.That(stream.Probe().MessageType, Is.Null);
			}
		}
	}
}

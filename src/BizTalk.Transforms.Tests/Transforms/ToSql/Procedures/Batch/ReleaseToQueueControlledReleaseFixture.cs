#region Copyright & License

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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Sql.Procedures.Batch;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.BizTalk.Unit.Transform;
using Be.Stateless.IO;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Transforms.ToSql.Procedures.Batch
{
	[TestFixture]
	public class ReleaseToQueueControlledReleaseFixture : ClosedTransformFixture<ReleaseToQueueControlledRelease>
	{
		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ValidateTransform()
		{
			var instance = MessageFactory.CreateMessage<Schemas.Xml.Batch.Release>(ResourceManager.LoadString("Data.ReleaseBatch.xml"));
			using (var stream = new StringStream(instance.OuterXml))
			{
				var setup = Given(input => input.Message(stream).Context(new Mock<IBaseMessageContext>().Object))
					.Transform
					.OutputsXml(output => output.ConformingTo<QueueControlledRelease>().WithStrictConformanceLevel());
				var result = setup.Validate();
				Assert.That(result.SelectSingleNode("//usp:envelopeSpecName/text()").Value, Is.EqualTo(new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName));
				Assert.That(result.Select("//usp:partition").Count, Is.EqualTo(0));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ValidateTransformWithEnvironmentTag()
		{
			var instance = MessageFactory.CreateMessage<Schemas.Xml.Batch.Release>(ResourceManager.LoadString("Data.ReleaseBatchEnvironmentTag.xml"));
			using (var stream = new StringStream(instance.OuterXml))
			{
				var setup = Given(input => input.Message(stream).Context(new Mock<IBaseMessageContext>().Object))
					.Transform
					.OutputsXml(output => output.ConformingTo<QueueControlledRelease>().WithStrictConformanceLevel());
				var result = setup.Validate();
				Assert.That(result.SelectSingleNode("//usp:envelopeSpecName/text()").Value, Is.EqualTo(new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName));
				Assert.That(result.SelectSingleNode("//usp:environmentTag/text()").Value, Is.EqualTo("Tag"));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ValidateTransformWithPartition()
		{
			var instance = MessageFactory.CreateMessage<Schemas.Xml.Batch.Release>(ResourceManager.LoadString("Data.ReleaseBatchPartition.xml"));
			using (var stream = new StringStream(instance.OuterXml))
			{
				var setup = Given(input => input.Message(stream).Context(new Mock<IBaseMessageContext>().Object))
					.Transform
					.OutputsXml(output => output.ConformingTo<QueueControlledRelease>().WithStrictConformanceLevel());
				var result = setup.Validate();
				Assert.That(result.SelectSingleNode("//usp:envelopeSpecName/text()").Value, Is.EqualTo(new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName));
				Assert.That(result.SelectSingleNode("//usp:partition/text()").Value, Is.EqualTo("A"));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ValidateTransformWithProcessActivityId()
		{
			var contextMock = new Mock<IBaseMessageContext>();
			contextMock
				.Setup(c => c.Read(TrackingProperties.ProcessActivityId.Name, TrackingProperties.ProcessActivityId.Namespace))
				.Returns("D4D3A8E583024BAC9D35EC98C5422E82");

			var instance = MessageFactory.CreateMessage<Schemas.Xml.Batch.Release>(ResourceManager.LoadString("Data.ReleaseBatch.xml"));
			using (var stream = new StringStream(instance.OuterXml))
			{
				var setup = Given(input => input.Message(stream).Context(contextMock.Object))
					.Transform
					.OutputsXml(output => output.ConformingTo<QueueControlledRelease>().WithStrictConformanceLevel());
				var result = setup.Validate();
				Assert.That(result.SelectSingleNode("//usp:envelopeSpecName/text()").Value, Is.EqualTo(new SchemaMetadata<Envelope>().DocumentSpec.DocSpecStrongName));
				Assert.That(result.Select("//usp:partition").Count, Is.EqualTo(0));
				Assert.That(result.SelectSingleNode("//usp:processActivityId/text()").Value, Is.EqualTo("D4D3A8E583024BAC9D35EC98C5422E82"));
			}
		}
	}
}

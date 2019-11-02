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
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schemas.Sql.Procedures.Batch;
using Be.Stateless.BizTalk.Unit;
using Be.Stateless.BizTalk.Unit.Transform;
using Be.Stateless.IO;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Transforms.ToSql.Procedures.Batch
{
	[TestFixture]
	public class AnyToAddPartFixture : ClosedTransformFixture<AnyToAddPart>
	{
		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ValidateTransform()
		{
			var contextMock = new MessageContextMock();
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.EnvelopeSpecName))
				.Returns("envelope-name");

			using (var stream = new StringStream("<?xml version=\"1.0\" encoding=\"utf-16\" ?><root>content of a part is irrelevant here</root>"))
			{
				var setup = Given(input => input.Message(stream).Context(contextMock.Object))
					.Transform
					.OutputsXml(output => output.ConformingTo<AddPart>().WithStrictConformanceLevel());
				var result = setup.Validate();
				Assert.That(result.SelectSingleNode("//usp:envelopeSpecName/text()").Value, Is.EqualTo("envelope-name"));
				Assert.That(result.Select("//usp:partition").Count, Is.EqualTo(0));
				Assert.That(result.Select("//usp:messagingStepActivityId").Count, Is.EqualTo(0));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ValidateTransformWithEnvironmentTag()
		{
			var contextMock = new MessageContextMock();
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.EnvelopeSpecName))
				.Returns("envelope-name");
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.EnvironmentTag))
				.Returns("Tag");
			contextMock
				.Setup(c => c.GetProperty(TrackingProperties.MessagingStepActivityId))
				.Returns("D4D3A8E583024BAC9D35EC98C5422E82");

			using (var stream = new StringStream("<?xml version=\"1.0\" encoding=\"utf-16\" ?><root>content of a part is irrelevant here</root>"))
			{
				var setup = Given(input => input.Message(stream).Context(contextMock.Object))
					.Transform
					.OutputsXml(output => output.ConformingTo<AddPart>().WithStrictConformanceLevel());
				var result = setup.Validate();
				Assert.That(result.SelectSingleNode("//usp:envelopeSpecName/text()").Value, Is.EqualTo("envelope-name"));
				Assert.That(result.SelectSingleNode("//usp:environmentTag/text()").Value, Is.EqualTo("Tag"));
				Assert.That(result.SelectSingleNode("//usp:messagingStepActivityId/text()").Value, Is.EqualTo("D4D3A8E583024BAC9D35EC98C5422E82"));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ValidateTransformWithMessagingStepActivityId()
		{
			var contextMock = new MessageContextMock();
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.EnvelopeSpecName))
				.Returns("envelope-name");
			contextMock
				.Setup(c => c.GetProperty(TrackingProperties.MessagingStepActivityId))
				.Returns("D4D3A8E583024BAC9D35EC98C5422E82");

			using (var stream = new StringStream("<?xml version=\"1.0\" encoding=\"utf-16\" ?><root>content of a part is irrelevant here</root>"))
			{
				var setup = Given(input => input.Message(stream).Context(contextMock.Object))
					.Transform
					.OutputsXml(output => output.ConformingTo<AddPart>().WithStrictConformanceLevel());
				var result = setup.Validate();
				Assert.That(result.SelectSingleNode("//usp:envelopeSpecName/text()").Value, Is.EqualTo("envelope-name"));
				Assert.That(result.Select("//usp:partition").Count, Is.EqualTo(0));
				Assert.That(result.SelectSingleNode("//usp:messagingStepActivityId/text()").Value, Is.EqualTo("D4D3A8E583024BAC9D35EC98C5422E82"));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ValidateTransformWithPartition()
		{
			var contextMock = new MessageContextMock();
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.EnvelopeSpecName))
				.Returns("envelope-name");
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.EnvelopePartition))
				.Returns("A");
			contextMock
				.Setup(c => c.GetProperty(TrackingProperties.MessagingStepActivityId))
				.Returns("D4D3A8E583024BAC9D35EC98C5422E82");

			using (var stream = new StringStream("<?xml version=\"1.0\" encoding=\"utf-16\" ?><root>content of a part is irrelevant here</root>"))
			{
				var setup = Given(input => input.Message(stream).Context(contextMock.Object))
					.Transform
					.OutputsXml(output => output.ConformingTo<AddPart>().WithStrictConformanceLevel());
				var result = setup.Validate();
				Assert.That(result.SelectSingleNode("//usp:envelopeSpecName/text()").Value, Is.EqualTo("envelope-name"));
				Assert.That(result.SelectSingleNode("//usp:partition/text()").Value, Is.EqualTo("A"));
				Assert.That(result.SelectSingleNode("//usp:messagingStepActivityId/text()").Value, Is.EqualTo("D4D3A8E583024BAC9D35EC98C5422E82"));
			}
		}
	}
}

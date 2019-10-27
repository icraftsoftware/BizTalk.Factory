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
using Be.Stateless.BizTalk.Schemas.Sql.Procedures.Claim;
using Be.Stateless.BizTalk.Unit;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.BizTalk.Unit.Transform;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Transforms.ToSql.Procedures.Claim
{
	[TestFixture]
	public class ClaimToCheckInFixture : ClosedTransformFixture<ClaimToCheckIn>
	{
		[Test]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ValidateTransformClaimTokenWithContext()
		{
			var contextMock = new MessageContextMock();
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.ClaimedMessageType))
				.Returns("context-claimed-message-type");
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.CorrelationToken))
				.Returns("context-correlation-token");
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.EnvironmentTag))
				.Returns("context-environment-tag");

			using (var stream = ResourceManager.Load("Data.Token.1.xml"))
			{
				var setup = Given.Message(stream).Context(contextMock.Object).Transform.OutputsXml().ConformingTo<CheckIn>().WithStrictConformanceLevel();
				var result = setup.Execute();
				Assert.That(result.Select("//usp:url/text()").Count, Is.EqualTo(1));
				Assert.That(result.SelectSingleNode("//usp:messageType/text()").Value, Is.EqualTo("context-claimed-message-type"));
				Assert.That(result.SelectSingleNode("//usp:correlationToken/text()").Value, Is.EqualTo("context-correlation-token"));
				Assert.That(result.SelectSingleNode("//usp:environmentTag/text()").Value, Is.EqualTo("context-environment-tag"));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ValidateTransformClaimTokenWithEmbeddedDataAndContext()
		{
			var contextMock = new MessageContextMock();
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.ClaimedMessageType))
				.Returns("context-claimed-message-type");
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.CorrelationToken))
				.Returns("context-correlation-token");
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.EnvironmentTag))
				.Returns("context-environment-tag");
			contextMock
				.Setup(c => c.GetProperty(BizTalkFactoryProperties.ReceiverName))
				.Returns("context-receiver-name");

			using (var stream = ResourceManager.Load("Data.Token.3.xml"))
			{
				var setup = Given.Message(stream).Context(contextMock.Object).Transform.OutputsXml().ConformingTo<CheckIn>().WithStrictConformanceLevel();
				var result = setup.Execute();
				Assert.That(result.Select("//usp:url/text()").Count, Is.EqualTo(1));
				Assert.That(result.SelectSingleNode("//usp:correlationToken/text()").Value, Is.EqualTo("embedded-correlation-token"));
				Assert.That(result.SelectSingleNode("//usp:environmentTag/text()").Value, Is.EqualTo("embedded-environment-tag"));
				Assert.That(result.SelectSingleNode("//usp:messageType/text()").Value, Is.EqualTo("embedded-claimed-message-type"));
				Assert.That(result.SelectSingleNode("//usp:receiverName/text()").Value, Is.EqualTo("context-receiver-name"));
				Assert.That(result.SelectSingleNode("//usp:senderName/text()").Value, Is.EqualTo("embedded-sender-name"));
				Assert.That(
					result.SelectSingleNode("//usp:any/text()").Value,
					Is.EqualTo("<parent><child>one</child><child>two</child></parent><parent><child>six</child><child>ten</child></parent>"));
			}
		}

		[Test]
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public void ValidateTransformComplexClaimToken()
		{
			using (var stream = ResourceManager.Load("Data.Token.2.xml"))
			{
				var setup = Given.Message(stream).Context(new MessageContextMock().Object).Transform.OutputsXml().ConformingTo<CheckIn>().WithStrictConformanceLevel();
				var result = setup.Execute();
				Assert.That(result.Select("//usp:url/text()").Count, Is.EqualTo(1));
				Assert.That(result.Select("//usp:any").Count, Is.EqualTo(1));
				Assert.That(
					result.SelectSingleNode("//usp:any/text()").Value,
					Is.EqualTo("<parent><child>one</child><child>two</child></parent><parent><child>six</child><child>ten</child></parent>"));
			}
		}

		[Test]
		public void ValidateTransformSimpleClaimToken()
		{
			using (var stream = ResourceManager.Load("Data.Token.1.xml"))
			{
				var setup = Given.Message(stream).Context(new MessageContextMock().Object).Transform.OutputsXml().ConformingTo<CheckIn>().WithStrictConformanceLevel();
				var result = setup.Execute();
				Assert.That(result.Select("//usp:url/text()").Count, Is.EqualTo(1));
				Assert.That(result.Select("//usp:any").Count, Is.EqualTo(0));
			}
		}
	}
}

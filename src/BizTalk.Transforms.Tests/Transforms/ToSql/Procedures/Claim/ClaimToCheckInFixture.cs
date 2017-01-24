﻿#region Copyright & License

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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Schemas.Sql.Procedures.Claim;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.BizTalk.Unit.Transform;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Transforms.ToSql.Procedures.Claim
{
	[TestFixture]
	public class ClaimToCheckInFixture : TransformFixture<ClaimToCheckIn>
	{
		[Test]
		public void ValidateTransformClaimTokenWithContext()
		{
			var contextMock = new Mock<IBaseMessageContext>();
			contextMock
				.Setup(c => c.Read(BizTalkFactoryProperties.CorrelationToken.Name, BizTalkFactoryProperties.CorrelationToken.Namespace))
				.Returns("context-correlation-token");
			contextMock
				.Setup(c => c.Read(BizTalkFactoryProperties.ClaimedMessageType.Name, BizTalkFactoryProperties.ClaimedMessageType.Namespace))
				.Returns("context-claimed-message-type");

			using (var stream = ResourceManager.Load("Data.Token.1.xml"))
			{
				var result = Transform<CheckIn>(contextMock.Object, stream);
				Assert.That(result.Select("//usp:url/text()").Count, Is.EqualTo(1));
				Assert.That(result.Single("//usp:correlationToken/text()").Value, Is.EqualTo("context-correlation-token"));
				Assert.That(result.Single("//usp:messageType/text()").Value, Is.EqualTo("context-claimed-message-type"));
			}
		}

		[Test]
		public void ValidateTransformClaimTokenWithEmbeddedDataAndContext()
		{
			var contextMock = new Mock<IBaseMessageContext>();
			contextMock
				.Setup(c => c.Read(BizTalkFactoryProperties.CorrelationToken.Name, BizTalkFactoryProperties.CorrelationToken.Namespace))
				.Returns("context-correlation-token");
			contextMock
				.Setup(c => c.Read(BizTalkFactoryProperties.ClaimedMessageType.Name, BizTalkFactoryProperties.ClaimedMessageType.Namespace))
				.Returns("context-claimed-message-type");
			contextMock
				.Setup(c => c.Read(BizTalkFactoryProperties.ReceiverName.Name, BizTalkFactoryProperties.ReceiverName.Namespace))
				.Returns("context-receiver-name");

			using (var stream = ResourceManager.Load("Data.Token.3.xml"))
			{
				var result = Transform<CheckIn>(contextMock.Object, stream);
				Assert.That(result.Select("//usp:url/text()").Count, Is.EqualTo(1));
				Assert.That(result.Single("//usp:correlationToken/text()").Value, Is.EqualTo("embedded-correlation-token"));
				Assert.That(result.Single("//usp:messageType/text()").Value, Is.EqualTo("embedded-claimed-message-type"));
				Assert.That(result.Single("//usp:receiverName/text()").Value, Is.EqualTo("context-receiver-name"));
				Assert.That(result.Single("//usp:senderName/text()").Value, Is.EqualTo("embedded-sender-name"));
				Assert.That(
					result.Single("//usp:any/text()").Value,
					Is.EqualTo("<parent><child>one</child><child>two</child></parent><parent><child>six</child><child>ten</child></parent>"));
			}
		}

		[Test]
		public void ValidateTransformComplexClaimToken()
		{
			using (var stream = ResourceManager.Load("Data.Token.2.xml"))
			{
				var result = Transform<CheckIn>(new Mock<IBaseMessageContext>().Object, stream);
				Assert.That(result.Select("//usp:url/text()").Count, Is.EqualTo(1));
				Assert.That(result.Select("//usp:any").Count, Is.EqualTo(1));
				Assert.That(
					result.Single("//usp:any/text()").Value,
					Is.EqualTo("<parent><child>one</child><child>two</child></parent><parent><child>six</child><child>ten</child></parent>"));
			}
		}

		[Test]
		public void ValidateTransformSimpleClaimToken()
		{
			using (var stream = ResourceManager.Load("Data.Token.1.xml"))
			{
				var result = Transform<CheckIn>(new Mock<IBaseMessageContext>().Object, stream);
				Assert.That(result.Select("//usp:url/text()").Count, Is.EqualTo(1));
				Assert.That(result.Select("//usp:any").Count, Is.EqualTo(0));
			}
		}
	}
}

﻿#region Copyright & License

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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schema;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Unit.Constraints;
using Be.Stateless.BizTalk.Unit.Process;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.IO;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Linq.Extensions;
using NUnit.Framework;
using Path = System.IO.Path;

namespace Be.Stateless.BizTalk.Processes.Claim
{
	[TestFixture]
	public class ProcessFixture : ClaimBasedProcessFixture
	{
		[Test]
		public void AllTokensAvailableForCheckOutAreReleased()
		{
			var tokenUrls = new[] { "payload-data-file-13", "payload-data-file-23", "payload-data-file-33" };
			var correlationTokens = new[] { "correlation-token-13", "correlation-token-23", "correlation-token-33" };

			tokenUrls.Each(
				(i, tu) => {
					InsertToken(tu, correlationTokens[i]);
					new StringStream("This payload has been claimed to disk.").DropToFolder(CheckOutFolder, tu);
				});
			ReleaseTokens(tokenUrls);

			correlationTokens.Each(
				ct => {
					// Unidentified process because check-in phase has been skipped and no Claim.Check process name could have been inserted in context
					var process = TrackingRepository.SingleProcess(p => p.Name == Factory.DefaultProcesses.Unidentified && p.Value1 == ct && p.BeginTime > StartTime);
					process.SingleMessagingStep(s => s.Name == "BizTalk.Factory.RL1.Claim.CheckOut.WCF-SQL.XML" && s.Value1 == ct && s.Status == TrackingStatus.Received);
					process.SingleMessagingStep(s => s.Name == "BizTalk.Factory.SP1.UnitTest.Claim.Redeem.FILE.XML" && s.Value1 == ct && s.Status == TrackingStatus.Sent);
				});

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void CheckedOutTokenHasAnyExtraXmlContentRestored()
		{
			const string tokenUrl = "payload-data-file";
			const string correlationToken = "correlation-token-2";
			const string receiverName = "receiver-name-2";
			const string senderName = "sender-name-2";
			const string extraContent = "<extra-content>"
				+ "<node>value</node>"
				+ "<record><field1>value1</field1><field2>value2</field2><field3>value3</field3></record>"
				+ "</extra-content>"
				+ "<optional-content><node>value</node></optional-content>";
			InsertToken(tokenUrl, correlationToken, "text-message-type-2", receiverName, senderName, extraContent);
			new StringStream("This payload has been claimed to disk.").DropToFolder(CheckOutFolder, tokenUrl);
			ReleaseToken(tokenUrl);

			// Unidentified process because check-in phase has been skipped and no Claim.Check process name could have been inserted in context
			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.DefaultProcesses.Unidentified
					&& p.Value1 == correlationToken
					&& p.Value2 == receiverName
					&& p.Value3 == senderName
					&& p.BeginTime > StartTime);
			// ReSharper disable once ImplicitlyCapturedClosure
			var tokenMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Claim.CheckOut.WCF-SQL.XML"
					&& s.Value1 == correlationToken
					&& s.Value2 == receiverName
					&& s.Value3 == senderName
					&& s.Status == TrackingStatus.Received);
			// ReSharper disable once ImplicitlyCapturedClosure
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.UnitTest.Claim.Redeem.FILE.XML"
					&& s.Value1 == correlationToken
					&& s.Value2 == receiverName
					&& s.Value3 == senderName
					&& s.Status == TrackingStatus.Sent);
			Assert.That(tokenMessagingStep.Message.Body, Is.StringEnding("</clm:Url>" + extraContent + "</clm:CheckOut>"));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void ClaimBinaryMessage()
		{
			ResourceManager.Load("Data.Payload.bin").DropToFolder(DropFolders.INPUT_FOLDER, "Payload.bin.claim");

			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Processes.Claim.Check
					&& p.BeginTime > StartTime,
				TimeSpan.FromSeconds(60));
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.Claim.Desk.FILE"
					&& s.Status == TrackingStatus.Received
					&& s.MessageSize == ResourceManager.Load("Data.Payload.bin", stream => stream.Length));
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Claim.CheckIn.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Claim.CheckIn>().MessageType
					&& s.Status == TrackingStatus.Sent);

			var token = FindTokens().Single();
			Assert.That(File.Exists(Path.Combine(CheckInFolder, token.Url.Replace("\\", string.Empty) + ".chk")));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		[Test]
		public void ClaimXmlMessage()
		{
			new FakeClaimStream().DropToFolder(DropFolders.INPUT_FOLDER, "large_message.xml.claim");

			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Processes.Claim.Check
					&& p.BeginTime > StartTime
					&& p.Value1 == "embedded-correlation-token"
					&& p.Value2 == "embedded-receiver-name"
					&& p.Value3 == "embedded-sender-name",
				TimeSpan.FromSeconds(60));

			// message check-in phase
			var inboundMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.UnitTest.Claim.Desk.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Claim.CheckIn>().MessageType
					&& s.Status == TrackingStatus.Received
					&& s.Value1 == "embedded-correlation-token"
					&& s.Value2 == "embedded-receiver-name"
					&& s.Value3 == "embedded-sender-name");
			var tokenMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.Claim.CheckIn.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Claim.CheckIn>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& s.Value1 == "embedded-correlation-token"
					&& s.Value2 == "embedded-receiver-name"
					&& s.Value3 == "embedded-sender-name");

			// claim-specific context properties are captured
			Assert.That(tokenMessagingStep.Context.GetProperty(BizTalkFactoryProperties.CorrelationToken).Value, Is.EqualTo("embedded-correlation-token"));
			Assert.That(tokenMessagingStep.Context.GetProperty(BizTalkFactoryProperties.ReceiverName).Value, Is.EqualTo("embedded-receiver-name"));
			Assert.That(tokenMessagingStep.Context.GetProperty(BizTalkFactoryProperties.SenderName).Value, Is.EqualTo("embedded-sender-name"));

			// payload has been claimed into claim store
			var token = FindTokens().Single();
			Assert.That(token.MessageType, Is.EqualTo(new SchemaMetadata<Any>().MessageType));
			using (var reader = XmlReader.Create(tokenMessagingStep.Message.Stream))
			{
				var url = reader.AsMessageBodyCaptureDescriptor().Data;
				Assert.That(token.Url, Is.EqualTo(url));
			}
			var claimFileName = token.Url.Replace("\\", string.Empty) + ".chk";
			Assert.That(File.Exists(Path.Combine(CheckInFolder, claimFileName)));

			// move the claimed payload from the check-in folder to the check-out one as the ClaimStoreAgent would do
			// ReSharper disable once AssignNullToNotNullAttribute
			Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(CheckOutFolder, token.Url)));
			File.Move(Path.Combine(CheckInFolder, claimFileName), Path.Combine(CheckOutFolder, token.Url));
			ReleaseToken(token.Url);

			// message check-out phase
			tokenMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Claim.CheckOut.WCF-SQL.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Claim.CheckOut>().MessageType
					&& s.Status == TrackingStatus.Received
					&& s.Value1 == "embedded-correlation-token"
					&& s.Value2 == "embedded-receiver-name"
					&& s.Value3 == "embedded-sender-name");
			var outboundMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.UnitTest.Claim.Redeem.FILE.XML"
					&& s.MessageType == new SchemaMetadata<Schemas.Xml.Claim.CheckOut>().MessageType
					&& s.Status == TrackingStatus.Sent
					&& s.Value1 == "embedded-correlation-token"
					&& s.Value2 == "embedded-receiver-name"
					&& s.Value3 == "embedded-sender-name");

			// claim-specific context properties are restored and promoted
			Assert.That(
				tokenMessagingStep.Context.GetProperty(BizTalkFactoryProperties.ClaimedMessageType),
				Has.Property("Value").EqualTo(new SchemaMetadata<Any>().MessageType).And.Property("IsPromoted").True);
			Assert.That(
				tokenMessagingStep.Context.GetProperty(BizTalkFactoryProperties.CorrelationToken),
				Has.Property("Value").EqualTo("embedded-correlation-token").And.Property("IsPromoted").True);
			Assert.That(
				tokenMessagingStep.Context.GetProperty(BizTalkFactoryProperties.ReceiverName),
				Has.Property("Value").EqualTo("embedded-receiver-name").And.Property("IsPromoted").True);
			Assert.That(
				tokenMessagingStep.Context.GetProperty(BizTalkFactoryProperties.SenderName),
				Has.Property("Value").EqualTo("embedded-sender-name").And.Property("IsPromoted").True);

			// original inbound message payload has been restored
			Assert.That(inboundMessagingStep.Message.Body, Is.EqualTo(outboundMessagingStep.Message.Body));

			Assert.That(BizTalkServiceInstances, Has.No.UncompletedInstances());
		}

		protected override IEnumerable<string> SystemOutputFolders
		{
			get { return new[] { DropFolders.OUTPUT_FOLDER }; }
		}
	}
}
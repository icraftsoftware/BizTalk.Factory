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

using System.Linq;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.BizTalk.Tracking;
using Be.Stateless.BizTalk.Unit.Process;
using Be.Stateless.IO;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.Operations;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Processes.Claim
{
	// ReSharper disable ImplicitlyCapturedClosure

	[TestFixture]
	public class CheckOutFixture : ProcessFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			TokenAdapter.ClearTokens();
			DropFolders.ClearInputFolders();
		}

		[TearDown]
		public void TearDown()
		{
			TokenAdapter.ClearTokens();
			DropFolders.ClearOutputFolders();
			TerminateUncompletedBizTalkServiceInstances();
		}

		#endregion

		[Test]
		public void AllAvailableTokensAreReleased()
		{
			var tokenUrls = new[] { "payload-data-file-13", "payload-data-file-23", "payload-data-file-33" };
			var correlationTokens = new[] { "correlation-token-13", "correlation-token-23", "correlation-token-33" };

			tokenUrls.Each(
				(i, tu) => {
					TokenAdapter.InsertToken(tu, correlationTokens[i]);
					new StringStream("This payload has been claimed to disk.").SaveToClaimFolder(tu);
				});
			TokenAdapter.ReleaseTokens(tokenUrls);

			correlationTokens.Each(
				ct => {
					var process = TrackingRepository.SingleProcess(p => p.Name == Factory.Processes.Claim.CheckOut && p.Value1 == ct && p.BeginTime > StartTime);
					process.SingleMessagingStep(s => s.Name == "BizTalk.Factory.RL1.Claim.CheckOut.WCF-SQL.XML" && s.Value1 == ct && s.Status == TrackingStatus.Received);
					process.SingleMessagingStep(s => s.Name == "BizTalk.Factory.SP1.UnitTest.Claim.Redeem.FILE.XML" && s.Value1 == ct && s.Status == TrackingStatus.Sent);
				});

			Assert.That(UncompletedBizTalkServiceInstances, Is.EquivalentTo(Enumerable.Empty<MessageBoxServiceInstance>()));
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
			TokenAdapter.InsertToken(tokenUrl, correlationToken, "text-message-type-2", receiverName, senderName, extraContent);
			new StringStream("This payload has been claimed to disk.").SaveToClaimFolder(tokenUrl);
			TokenAdapter.ReleaseToken(tokenUrl);

			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Processes.Claim.CheckOut
					&& p.Value1 == correlationToken
					&& p.Value2 == receiverName
					&& p.Value3 == senderName
					&& p.BeginTime > StartTime);
			var tokenMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Claim.CheckOut.WCF-SQL.XML"
					&& s.Value1 == correlationToken
					&& s.Value2 == receiverName
					&& s.Value3 == senderName
					&& s.Status == TrackingStatus.Received);
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.UnitTest.Claim.Redeem.FILE.XML"
					&& s.Value1 == correlationToken
					&& s.Value2 == receiverName
					&& s.Value3 == senderName
					&& s.Status == TrackingStatus.Sent);
			Assert.That(tokenMessagingStep.Message.Body, Is.StringEnding("</clm:Url>" + extraContent + "</clm:CheckOut>"));

			Assert.That(UncompletedBizTalkServiceInstances, Is.EquivalentTo(Enumerable.Empty<MessageBoxServiceInstance>()));
		}

		[Test]
		public void CheckedOutTokenHasClaimContextPropertiesRestoredAndPromoted()
		{
			const string tokenUrl = "payload-data-file";
			const string correlationToken = "correlation-token-1";
			const string messageType = "text-message-type-1";
			const string receiverName = "receiver-name-1";
			const string senderName = "sender-name-1";

			TokenAdapter.InsertToken(tokenUrl, correlationToken, messageType, receiverName, senderName);
			new StringStream("This payload has been claimed to disk.").SaveToClaimFolder(tokenUrl);
			TokenAdapter.ReleaseToken(tokenUrl);

			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Processes.Claim.CheckOut
					&& p.Value1 == correlationToken
					&& p.Value2 == receiverName
					&& p.Value3 == senderName
					&& p.BeginTime > StartTime);
			var tokenMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Claim.CheckOut.WCF-SQL.XML"
					&& s.Value1 == correlationToken
					&& s.Value2 == receiverName
					&& s.Value3 == senderName
					&& s.Status == TrackingStatus.Received);
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.UnitTest.Claim.Redeem.FILE.XML"
					&& s.Value1 == correlationToken
					&& s.Value2 == receiverName
					&& s.Value3 == senderName
					&& s.Status == TrackingStatus.Sent);
			Assert.That(
				tokenMessagingStep.Context.GetProperty(BizTalkFactoryProperties.ClaimedMessageType),
				Has.Property("Value").EqualTo(messageType).And.Property("IsPromoted").True);
			Assert.That(
				tokenMessagingStep.Context.GetProperty(BizTalkFactoryProperties.CorrelationToken),
				Has.Property("Value").EqualTo(correlationToken).And.Property("IsPromoted").True);
			Assert.That(
				tokenMessagingStep.Context.GetProperty(BizTalkFactoryProperties.ReceiverName),
				Has.Property("Value").EqualTo(receiverName).And.Property("IsPromoted").True);
			Assert.That(
				tokenMessagingStep.Context.GetProperty(BizTalkFactoryProperties.SenderName),
				Has.Property("Value").EqualTo(senderName).And.Property("IsPromoted").True);

			Assert.That(UncompletedBizTalkServiceInstances, Is.EquivalentTo(Enumerable.Empty<MessageBoxServiceInstance>()));
		}

		[Test]
		public void OutgoingClaimedMessageHasOriginalPayloadRestored()
		{
			const string tokenUrl = "payload-data-file";
			const string correlationToken = "correlation-token-3";
			const string receiverName = "receiver-name-3";
			const string senderName = "sender-name-3";
			TokenAdapter.InsertToken(tokenUrl, correlationToken, "text-message-type-3", receiverName, senderName);
			new StringStream("This payload has been claimed to disk.").SaveToClaimFolder(tokenUrl);
			TokenAdapter.ReleaseToken(tokenUrl);

			var process = TrackingRepository.SingleProcess(
				p => p.Name == Factory.Processes.Claim.CheckOut
					&& p.Value1 == correlationToken
					&& p.Value2 == receiverName
					&& p.Value3 == senderName
					&& p.BeginTime > StartTime);
			process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.RL1.Claim.CheckOut.WCF-SQL.XML"
					&& s.Value1 == correlationToken
					&& s.Value2 == receiverName
					&& s.Value3 == senderName
					&& s.Status == TrackingStatus.Received);
			var claimedMessageMessagingStep = process.SingleMessagingStep(
				s => s.Name == "BizTalk.Factory.SP1.UnitTest.Claim.Redeem.FILE.XML"
					&& s.Value1 == correlationToken
					&& s.Value2 == receiverName
					&& s.Value3 == senderName
					&& s.Status == TrackingStatus.Sent);
			Assert.That(claimedMessageMessagingStep.Message.Body, Is.EqualTo("This payload has been claimed to disk."));

			Assert.That(UncompletedBizTalkServiceInstances, Is.EquivalentTo(Enumerable.Empty<MessageBoxServiceInstance>()));
		}
	}
}

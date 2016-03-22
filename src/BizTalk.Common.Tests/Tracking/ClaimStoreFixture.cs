#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Schemas.Xml;
using Be.Stateless.BizTalk.SsoClient;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.IO;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Xml.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;
using Path = System.IO.Path;

namespace Be.Stateless.BizTalk.Tracking
{
	[TestFixture]
	public class ClaimStoreFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			// reset ClaimStore's cached value
			ClaimStore.CheckInDirectory = null;
			ClaimStore.CheckOutDirectory = null;

			MessageMock = new Unit.Message.Mock<IBaseMessage> { DefaultValue = DefaultValue.Mock };
			ResourceTrackerMock = new Mock<IResourceTracker>();

			_ssoSettingsReaderinstance = SsoSettingsReader.Instance;
			SsoSettingsReaderMock = new Mock<ISsoSettingsReader>();
			SsoSettingsReader.Instance = SsoSettingsReaderMock.Object;
		}

		[TearDown]
		public void TearDown()
		{
			SsoSettingsReader.Instance = _ssoSettingsReaderinstance;

			File.Delete(Path.Combine(Path.GetTempPath(), "cca95baa39ab4e25a3c54971ea170911"));
			Directory.GetFiles(Path.GetTempPath(), "*.chk").Each(File.Delete);
			Directory.GetFiles(Path.GetTempPath(), "*.trk").Each(File.Delete);
			Directory.GetFiles(Path.GetTempPath(), "*.rchk").Each(File.Delete);
			Directory.GetFiles(Path.GetTempPath(), "*.rjob").Each(File.Delete);
			Directory.GetFiles(Path.GetTempPath(), "*.rtrk").Each(File.Delete);
		}

		#endregion

		[Test]
		public void ArchiveAndCaptureMessageBody()
		{
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				.Returns(@"\\network\share");

			using (var trackingStream = new TrackingStream(FakeTextStream.Create(1024 * 512)))
			{
				MessageMock.Object.BodyPart.Data = trackingStream;

				var ascertainedTracking = ClaimStore.Instance.SetupMessageBodyCapture(trackingStream, ActivityTrackingModes.Body | ActivityTrackingModes.Archive, null);
				Assert.That(ascertainedTracking, Is.EqualTo(ActivityTrackingModes.Body | ActivityTrackingModes.Archive));

				ClaimStore.Instance.SetupMessageBodyArchiving(trackingStream, "archive-target-location", null);
				MessageMock.Object.BodyPart.Data.Drain();

				// payload is claimed to disk and file extension is .rtrk
				var captureDescriptor = trackingStream.CaptureDescriptor;
				Assert.That(captureDescriptor.CaptureMode, Is.EqualTo(MessageBodyCaptureMode.Claimed));
				Assert.That(captureDescriptor.Data, Is.StringStarting(DateTime.Today.ToString(@"yyyyMMdd\\")));
				Assert.That(File.Exists(Path.Combine(Path.GetTempPath(), captureDescriptor.Data.Replace("\\", "") + ".rtrk")), Is.True);

				// archive job descriptor file is written next to captured payload
				var archiveDescriptor = trackingStream.ArchiveDescriptor;
				Assert.That(archiveDescriptor.Source, Is.EqualTo(captureDescriptor.Data));
				Assert.That(archiveDescriptor.Target, Is.EqualTo("archive-target-location"));
				Assert.That(File.Exists(Path.Combine(Path.GetTempPath(), captureDescriptor.Data.Replace("\\", "") + ".rjob")), Is.True);
			}
		}

		[Test]
		public void ArchiveAndClaimMessageBody()
		{
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				.Returns(@"\\network\share");

			using (var contentStream = FakeTextStream.Create(1024 * 1024))
			using (var trackingStream = new TrackingStream(contentStream))
			{
				MessageMock.Object.BodyPart.Data = trackingStream;

				var ascertainedTracking = ClaimStore.Instance.SetupMessageBodyCapture(trackingStream, ActivityTrackingModes.Claim | ActivityTrackingModes.Archive, null);
				Assert.That(ascertainedTracking, Is.EqualTo(ActivityTrackingModes.Claim | ActivityTrackingModes.Archive));

				ClaimStore.Instance.SetupMessageBodyArchiving(trackingStream, "archive-target-location", null);
				ClaimStore.Instance.Claim(MessageMock.Object, ResourceTrackerMock.Object);
				// message's actual body stream has been exhausted (i.e. saved to disk)
				Assert.That(contentStream.Position, Is.EqualTo(contentStream.Length));

				// message's body stream is replaced by a token message
				using (var reader = new StreamReader(MessageMock.Object.BodyPart.Data))
				{
					Assert.That(reader.ReadToEnd(), Is.EqualTo(MessageFactory.CreateClaimCheckIn(trackingStream.CaptureDescriptor.Data).OuterXml));
				}

				// MessageType of token message is promoted in message context
				var schemaMetadata = typeof(Claim.CheckIn).GetMetadata();
				MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, schemaMetadata.MessageType), Times.Once());
				MessageMock.Verify(m => m.Promote(BtsProperties.SchemaStrongName, schemaMetadata.DocumentSpec.DocSpecStrongName), Times.Once());

				// payload is claimed to disk and file extension is .rchk
				var captureDescriptor = trackingStream.CaptureDescriptor;
				Assert.That(captureDescriptor.CaptureMode, Is.EqualTo(MessageBodyCaptureMode.Claimed));
				Assert.That(captureDescriptor.Data, Is.StringStarting(DateTime.Today.ToString(@"yyyyMMdd\\")));
				Assert.That(File.Exists(Path.Combine(Path.GetTempPath(), captureDescriptor.Data.Replace("\\", "") + ".rchk")), Is.True);

				// archive job descriptor file is written next to captured payload
				var archiveDescriptor = trackingStream.ArchiveDescriptor;
				Assert.That(archiveDescriptor.Source, Is.EqualTo(captureDescriptor.Data));
				Assert.That(archiveDescriptor.Target, Is.EqualTo("archive-target-location"));
				Assert.That(File.Exists(Path.Combine(Path.GetTempPath(), captureDescriptor.Data.Replace("\\", "") + ".rjob")), Is.True);
			}
		}

		[Test]
		public void ArchiveAndClaimMessageBodyCanSkipClaimIfPayloadSizeIsSmallEnough()
		{
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				.Returns(@"\\network\share");

			using (var contentStream = FakeTextStream.Create(1024))
			using (var trackingStream = new TrackingStream(contentStream))
			{
				MessageMock.Object.BodyPart.Data = trackingStream;

				var ascertainedTracking = ClaimStore.Instance.SetupMessageBodyCapture(trackingStream, ActivityTrackingModes.Claim | ActivityTrackingModes.Archive, null);
				Assert.That(ascertainedTracking, Is.EqualTo(ActivityTrackingModes.Body | ActivityTrackingModes.Archive));

				ClaimStore.Instance.SetupMessageBodyArchiving(trackingStream, "archive-target-location", null);
				ClaimStore.Instance.Claim(MessageMock.Object, ResourceTrackerMock.Object);

				// message's actual body stream has been exhausted (i.e. saved to disk)
				Assert.That(contentStream.Position, Is.EqualTo(contentStream.Length));

				// message's body stream is replaced by a token message
				using (var reader = new StreamReader(MessageMock.Object.BodyPart.Data))
				{
					Assert.That(reader.ReadToEnd(), Is.EqualTo(MessageFactory.CreateClaimCheckIn(trackingStream.CaptureDescriptor.Data).OuterXml));
				}

				// MessageType of token message is promoted in message context
				var schemaMetadata = typeof(Claim.CheckIn).GetMetadata();
				MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, schemaMetadata.MessageType), Times.Once());
				MessageMock.Verify(m => m.Promote(BtsProperties.SchemaStrongName, schemaMetadata.DocumentSpec.DocSpecStrongName), Times.Once());

				// payload is claimed to disk and file extension is .rchk
				var captureDescriptor = trackingStream.CaptureDescriptor;
				Assert.That(captureDescriptor.CaptureMode, Is.EqualTo(MessageBodyCaptureMode.Claimed));
				Assert.That(captureDescriptor.Data, Is.StringStarting(DateTime.Today.ToString(@"yyyyMMdd\\")));
				Assert.That(File.Exists(Path.Combine(Path.GetTempPath(), captureDescriptor.Data.Replace("\\", "") + ".rtrk")), Is.True);

				// archive job descriptor file is written next to captured payload
				var archiveDescriptor = trackingStream.ArchiveDescriptor;
				Assert.That(archiveDescriptor.Source, Is.EqualTo(captureDescriptor.Data));
				Assert.That(archiveDescriptor.Target, Is.EqualTo("archive-target-location"));
				Assert.That(File.Exists(Path.Combine(Path.GetTempPath(), captureDescriptor.Data.Replace("\\", "") + ".rjob")), Is.True);
			}
		}

		[Test]
		public void CaptureMessageBody()
		{
			var trackingStream = new TrackingStream(FakeTextStream.Create(1024 * 1024));

			var messageMock = new Unit.Message.Mock<IBaseMessage> { DefaultValue = DefaultValue.Mock };
			messageMock.Object.BodyPart.Data = trackingStream;

			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				.Returns(@"\\network\share");

			ClaimStore.Instance.SetupMessageBodyCapture(trackingStream, ActivityTrackingModes.Body, null);

			messageMock.Object.BodyPart.Data.Drain();

			// payload is claimed to disk and file extension is .trk
			var captureDescriptor = trackingStream.CaptureDescriptor;
			Assert.That(captureDescriptor.CaptureMode, Is.EqualTo(MessageBodyCaptureMode.Claimed));
			Assert.That(captureDescriptor.Data, Is.StringStarting(DateTime.Today.ToString(@"yyyyMMdd\\")));
			Assert.That(File.Exists(Path.Combine(Path.GetTempPath(), captureDescriptor.Data.Replace("\\", "") + ".trk")), Is.True);
		}

		[Test]
		public void CaptureMessageBodyWillHaveMessageClaimed()
		{
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				.Returns(@"\\network\share");

			var trackingStreamMock = new Mock<TrackingStream>(FakeTextStream.Create(1024 * 1024)) { CallBase = true };

			ClaimStore.Instance.SetupMessageBodyCapture(trackingStreamMock.Object, ActivityTrackingModes.Body, null);

			trackingStreamMock.Verify(
				ts => ts.SetupCapture(It.Is<MessageBodyCaptureDescriptor>(cd => cd.CaptureMode == MessageBodyCaptureMode.Claimed)),
				Times.Never());
			trackingStreamMock.Verify(
				ts => ts.SetupCapture(It.Is<MessageBodyCaptureDescriptor>(cd => cd.CaptureMode == MessageBodyCaptureMode.Unclaimed)),
				Times.Never());
			trackingStreamMock.Verify(
				ts => ts.SetupCapture(It.Is<MessageBodyCaptureDescriptor>(cd => cd.CaptureMode == MessageBodyCaptureMode.Claimed), It.IsAny<Stream>()),
				Times.Once());
		}

		[Test]
		public void CaptureMessageBodyWillHaveMessageClaimedButSsoApplicationDoesNotExist()
		{
			// setup a mock's callback to ensure that, even if the BizTalk.Factory SSO store is deployed, the call will look for an SSO store that does not exist
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Callback(() => _ssoSettingsReaderinstance.ReadString("NONEXISTENT_APPLICATION", BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());

			var trackingStreamMock = new Mock<TrackingStream>(FakeTextStream.Create(1024 * 1024)) { CallBase = true };

			Assert.That(
				() => ClaimStore.Instance.SetupMessageBodyCapture(trackingStreamMock.Object, ActivityTrackingModes.Claim, null),
				Throws.TypeOf<COMException>().With.Message.EqualTo("The application does not exist.\r\n"));
		}

		[Test]
		public void CaptureMessageBodyWillHaveMessageUnclaimed()
		{
			var trackingStreamMock = new Mock<TrackingStream>(FakeTextStream.Create(1024)) { CallBase = true };

			ClaimStore.Instance.SetupMessageBodyCapture(trackingStreamMock.Object, ActivityTrackingModes.Claim, null);

			trackingStreamMock.Verify(
				ts => ts.SetupCapture(It.Is<MessageBodyCaptureDescriptor>(cd => cd.CaptureMode == MessageBodyCaptureMode.Unclaimed)),
				Times.Once());
			trackingStreamMock.Verify(
				ts => ts.SetupCapture(It.Is<MessageBodyCaptureDescriptor>(cd => cd.CaptureMode == MessageBodyCaptureMode.Claimed), It.IsAny<Stream>()),
				Times.Never());
			trackingStreamMock.Verify(
				ts => ts.SetupCapture(It.Is<MessageBodyCaptureDescriptor>(cd => cd.CaptureMode == MessageBodyCaptureMode.Unclaimed), It.IsAny<Stream>()),
				Times.Never());
		}

		[Test]
		public void CaptureMessageBodyWithEmptyStreamWillHaveMessageUnclaimed()
		{
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				.Returns(@"\\network\share");

			var trackingStreamMock = new Mock<TrackingStream>(new MemoryStream()) { CallBase = true };

			ClaimStore.Instance.SetupMessageBodyCapture(trackingStreamMock.Object, ActivityTrackingModes.Body, null);

			trackingStreamMock.Verify(
				ts => ts.SetupCapture(It.Is<MessageBodyCaptureDescriptor>(cd => cd.CaptureMode == MessageBodyCaptureMode.Claimed)),
				Times.Never());
			trackingStreamMock.Verify(
				ts => ts.SetupCapture(It.Is<MessageBodyCaptureDescriptor>(cd => cd.CaptureMode == MessageBodyCaptureMode.Unclaimed)),
				Times.Once());
			trackingStreamMock.Verify(
				ts => ts.SetupCapture(It.Is<MessageBodyCaptureDescriptor>(cd => cd.CaptureMode == MessageBodyCaptureMode.Claimed), It.IsAny<Stream>()),
				Times.Never());
		}

		[Test]
		public void ClaimLeavesMessageUnalteredWhenNoTrackingStreamHasBeenSetup()
		{
			using (var stream = new StringStream("content"))
			{
				MessageMock.Object.BodyPart.Data = stream;

				ClaimStore.Instance.Claim(MessageMock.Object, ResourceTrackerMock.Object);
			}

			Assert.That(MessageMock.Object.BodyPart.Data, Is.TypeOf<StringStream>());
			Assert.That(MessageMock.Object.BodyPart.Data.Position, Is.EqualTo(0));
		}

		[Test]
		public void ClaimMessageBody()
		{
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				.Returns(@"\\network\share");

			using (var contentStream = FakeTextStream.Create(1024 * 1024))
			using (var trackingStream = new TrackingStream(contentStream))
			{
				MessageMock.Object.BodyPart.Data = trackingStream;

				ClaimStore.Instance.SetupMessageBodyCapture(trackingStream, ActivityTrackingModes.Claim, null);
				ClaimStore.Instance.Claim(MessageMock.Object, ResourceTrackerMock.Object);

				// message's actual body stream has been exhausted (i.e. saved to disk)
				Assert.That(contentStream.Position, Is.EqualTo(contentStream.Length));

				// message's body stream is replaced by a token message
				using (var reader = new StreamReader(MessageMock.Object.BodyPart.Data))
				{
					Assert.That(reader.ReadToEnd(), Is.EqualTo(MessageFactory.CreateClaimCheckIn(trackingStream.CaptureDescriptor.Data).OuterXml));
				}

				// MessageType of token message is promoted in message context
				var schemaMetadata = typeof(Claim.CheckIn).GetMetadata();
				MessageMock.Verify(m => m.Promote(BtsProperties.MessageType, schemaMetadata.MessageType), Times.Once());
				MessageMock.Verify(m => m.Promote(BtsProperties.SchemaStrongName, schemaMetadata.DocumentSpec.DocSpecStrongName), Times.Once());

				// payload is claimed to disk and file extension is .chk
				var captureDescriptor = trackingStream.CaptureDescriptor;
				Assert.That(captureDescriptor.CaptureMode, Is.EqualTo(MessageBodyCaptureMode.Claimed));
				Assert.That(captureDescriptor.Data, Is.StringStarting(DateTime.Today.ToString(@"yyyyMMdd\\")));
				Assert.That(File.Exists(Path.Combine(Path.GetTempPath(), captureDescriptor.Data.Replace("\\", "") + ".chk")), Is.True);
			}
		}

		[Test]
		public void RedeemAndArchiveMessageBody()
		{
			const string content = "dummy";
			const string url = "cca95baa39ab4e25a3c54971ea170911";
			var path = Path.Combine(Path.GetTempPath(), url);
			using (var file = File.CreateText(path))
			{
				file.Write(content);
			}

			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				//.Returns(Path.GetTempPath());
				.Returns(@"\\network\share");

			using (var tokenStream = MessageFactory.CreateClaimCheck("file://" + path).AsStream())
			{
				MessageMock.Object.BodyPart.Data = tokenStream;

				ClaimStore.Instance.Redeem(MessageMock.Object, ResourceTrackerMock.Object);
				var trackingStream = (TrackingStream) MessageMock.Object.BodyPart.Data;
				ClaimStore.Instance.SetupMessageBodyArchiving(trackingStream, "archive-target-location", null);

				// archive job descriptor file is written next to captured payload
				var captureDescriptor = trackingStream.CaptureDescriptor;
				var archiveDescriptor = trackingStream.ArchiveDescriptor;
				Assert.That(archiveDescriptor.Source, Is.EqualTo(captureDescriptor.Data));
				Assert.That(archiveDescriptor.Target, Is.EqualTo("archive-target-location"));

				using (var reader = new StreamReader(MessageMock.Object.BodyPart.Data))
				{
					Assert.That(reader.ReadToEnd(), Is.EqualTo(content));
				}
			}

			// archive job descriptor file is written next to where payload would be captured if it was not checked out
			using (var reader = XmlReader.Create(Directory.GetFiles(Path.GetTempPath(), "*.rjob").Single()))
			{
				var archiveDescriptor = ArchiveDescriptor.Create(reader);
				Assert.That(archiveDescriptor.Source, Is.EqualTo("file://" + path));
				Assert.That(archiveDescriptor.Target, Is.EqualTo("archive-target-location"));
			}
		}

		[Test]
		public void RedeemClaimToken()
		{
			const string content = "dummy";
			const string url = "cca95baa39ab4e25a3c54971ea170911";
			using (var file = File.CreateText(Path.Combine(Path.GetTempPath(), url)))
			{
				file.Write(content);
			}

			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());

			using (var tokenStream = MessageFactory.CreateClaimCheck(url).AsStream())
			{
				MessageMock.Object.BodyPart.Data = tokenStream;

				ClaimStore.Instance.Redeem(MessageMock.Object, ResourceTrackerMock.Object);

				Assert.That(MessageMock.Object.BodyPart.Data, Is.TypeOf<TrackingStream>());
				// ReSharper disable once PossibleInvalidCastException
				var captureDescriptor = ((TrackingStream) MessageMock.Object.BodyPart.Data).CaptureDescriptor;
				Assert.That(captureDescriptor.CaptureMode, Is.EqualTo(MessageBodyCaptureMode.Claimed));
				// previously captured payload is reused and not captured/claimed anew
				Assert.That(captureDescriptor.Data, Is.EqualTo(url));
			}

			using (var reader = new StreamReader(MessageMock.Object.BodyPart.Data))
			{
				Assert.That(reader.ReadToEnd(), Is.EqualTo(content));
			}
		}

		[Test]
		public void RedeemClaimTokenThrowsWhenUnexpectedToken()
		{
			using (var tokenStream = MessageFactory.CreateClaimCheckIn("d59cd2ea045744f4a085b18be678e4f0").AsStream())
			{
				MessageMock.Object.BodyPart.Data = tokenStream;
				Assert.That(
					() => ClaimStore.Instance.Redeem(MessageMock.Object, ResourceTrackerMock.Object),
					Throws.InvalidOperationException.With.Message.EqualTo("Invalid token message, CheckIn token is not expected to be redeemed."));
			}
		}

		[Test]
		public void RedeemHttpClaimToken()
		{
			using (var tokenStream = MessageFactory.CreateClaimCheck("http://nothing/that/exists.xml").AsStream())
			{
				MessageMock.Object.BodyPart.Data = tokenStream;
				// fails, and still, shows that resources can be redeemed from somewhere else than the claim store
				Assert.That(
					() => ClaimStore.Instance.Redeem(MessageMock.Object, ResourceTrackerMock.Object),
					Throws.TypeOf<WebException>());
			}
		}

		[Test]
		public void RequiresCheckInAndOutIsCaseInsensitive()
		{
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath().ToUpper());
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath().ToLower());

			Assert.That(ClaimStore.RequiresCheckInAndOut, Is.False);
		}

		[Test]
		public void RequiresCheckInAndOutIsTrailingBackslashInsensitive()
		{
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				// makes sure there is one trailing '\'
				.Returns(Path.GetTempPath().ToUpper().TrimEnd('\\') + '\\');
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				// makes sure there is no trailing '\'
				.Returns(Path.GetTempPath().ToLower().TrimEnd('\\'));

			Assert.That(ClaimStore.RequiresCheckInAndOut, Is.False);
		}

		[Test]
		public void RequiresCheckInAndOutIsTrue()
		{
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath());
			SsoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME))
				.Returns(@"\\network\share");

			Assert.That(ClaimStore.RequiresCheckInAndOut);
		}

		private Unit.Message.Mock<IBaseMessage> MessageMock { get; set; }

		private Mock<IResourceTracker> ResourceTrackerMock { get; set; }

		private Mock<ISsoSettingsReader> SsoSettingsReaderMock { get; set; }

		private ISsoSettingsReader _ssoSettingsReaderinstance;
	}
}

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
using System.Linq;
using System.Text;
using Be.Stateless.BizTalk.SsoClient;
using Be.Stateless.BizTalk.Streaming;
using Be.Stateless.Linq.Extensions;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking
{
	[TestFixture]
	public class BodyTrackingPolicyFixture
	{
		[Test]
		public void CheckInWillClaimMessage()
		{
			var ssoSettingsReaderMock = new Mock<ISsoSettingsReader>();
			ssoSettingsReaderMock
				.Setup(ssr => ssr.ReadString(BizTalkFactorySettings.AFFILIATE_APPLICATION_NAME, BizTalkFactorySettings.LOCAL_CLAIM_STORE_DIRECTORY_PROPERTY_NAME))
				.Returns(Path.GetTempPath);
			SsoSettingsReader.Instance = ssoSettingsReaderMock.Object;

			var trackingStreamMock = new Mock<TrackingStream>(CreateContentStream(1024 * 512)) { CallBase = true };

			BodyTrackingPolicy.Instance.Assess(trackingStreamMock.Object, null);

			trackingStreamMock
				.Verify(ts => ts.InitiateAssessment());
			trackingStreamMock
				.Verify(
					ts => ts.CompleteAssessment(
						It.Is<BodyTrackingDescriptor>(claim => claim.TrackingMode == BodyTrackingMode.Claimed && claim.Data.StartsWith(DateTime.Now.ToString("yyyyMMdd") + "\\")),
						It.IsAny<Stream>()));
		}

		[Test]
		public void CheckInWillLeaveMessageUnclaimed()
		{
			var trackingStreamMock = new Mock<TrackingStream>(CreateContentStream(1024)) { CallBase = true };

			BodyTrackingPolicy.Instance.Assess(trackingStreamMock.Object, null);

			trackingStreamMock
				.Verify(ts => ts.InitiateAssessment());
			trackingStreamMock
				.Verify(ts => ts.CompleteAssessment(It.Is<BodyTrackingDescriptor>(claim => claim.TrackingMode == BodyTrackingMode.Unclaimed), null));
		}

		private static MemoryStream CreateContentStream(int messageSize)
		{
			var builder = new StringBuilder(messageSize);
			var length = Guid.NewGuid().ToString("N").Length;
			Enumerable.Range(0, messageSize / length).Each(i => builder.Append(Guid.NewGuid().ToString("N")));
			var stream = new MemoryStream(Encoding.Unicode.GetBytes(builder.ToString()));
			return stream;
		}
	}
}

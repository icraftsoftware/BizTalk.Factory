#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using Be.Stateless.BizTalk.Factory.Areas;
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Tracking
{
	[TestFixture]
	public class TrackingResolverFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			MessageMock = new Unit.Message.Mock<IBaseMessage>();
			MessageMock.Setup(m => m.GetProperty(BtsProperties.InboundTransportLocation)).Returns("inbound-transport-location");
		}

		#endregion

		[Test]
		public void ResolveProcessNameToUnidentified()
		{
			var sut = TrackingResolver.Create(MessageMock.Object);
			var processName = sut.ResolveProcessName();

			Assert.That(processName, Is.EqualTo(Default.Processes.Unidentified));

			MessageMock.Verify(m => m.SetProperty(TrackingProperties.ProcessName, Default.Processes.Unidentified), Times.Never());
		}

		[Test]
		public void ResolveProcessNameViaContext()
		{
			const string name = "context-process-name";
			MessageMock.Setup(m => m.GetProperty(TrackingProperties.ProcessName)).Returns(name);

			var sut = TrackingResolver.Create(MessageMock.Object);
			var processName = sut.ResolveProcessName();

			Assert.That(processName, Is.EqualTo(name));

			MessageMock.Verify(m => m.SetProperty(TrackingProperties.ProcessName, name), Times.Never());
		}

		private Unit.Message.Mock<IBaseMessage> MessageMock { get; set; }
	}
}

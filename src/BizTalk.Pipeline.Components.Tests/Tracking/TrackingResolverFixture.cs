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
using System.Linq;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.RuleEngine;
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

			_policyFactory = Policy.Factory;
			PolicyMock = new Mock<IPolicy>();
			Policy.Factory = rulseSetInfo => PolicyMock.Object;
		}

		[TearDown]
		public void TearDown()
		{
			Policy.Factory = _policyFactory;
		}

		#endregion

		[Test]
		public void PolicyIsExecutedOnceAndOnlyOnce()
		{
			var sut = TrackingResolver.Create(new PolicyName("name", 1, 0), MessageMock.Object);
			sut.ResolveArchiveTargetLocation();
			sut.ResolveProcessName();

			PolicyMock.Verify(p => p.Execute(It.IsAny<object[]>()), Times.Once());
		}

		[Test]
		public void ResolveArchiveTargetLocationViaContext()
		{
			const string location = "context-target-location";
			MessageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.ArchiveTargetLocation)).Returns(location);

			var sut = TrackingResolver.Create(new PolicyName("name", 1, 0), MessageMock.Object);
			var targetLocation = sut.ResolveArchiveTargetLocation();

			Assert.That(targetLocation, Is.EqualTo(location));

			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.ArchiveTargetLocation, location), Times.Never());
			MessageMock.Verify(m => m.DeleteProperty(BizTalkFactoryProperties.ArchiveTargetLocation), Times.Once());
			PolicyMock.Verify(p => p.Execute(It.IsAny<object[]>()), Times.Never());
		}

		[Test]
		public void ResolveArchiveTargetLocationViaPolicy()
		{
			const string location = "policy-target-location";

			PolicyMock
				.Setup(p => p.Execute(It.IsAny<object[]>()))
				.Callback<object[]>(
					facts => {
						// assert the ProcessName in the fact base to be used by the policy mock
						facts.OfType<Context>().Single().Write(BizTalkFactoryProperties.ArchiveTargetLocation.QName, location);
						// setup an expectation on mock to ensure subsequent read retrieves info just written
						MessageMock.Setup(m => m.GetProperty(BizTalkFactoryProperties.ArchiveTargetLocation)).Returns(location);
					})
				.Verifiable();

			var sut = TrackingResolver.Create(new PolicyName("name", 1, 0), MessageMock.Object);
			var targetLocation = sut.ResolveArchiveTargetLocation();

			Assert.That(targetLocation, Is.EqualTo(location));

			// ReSharper disable once ImplicitlyCapturedClosure
			MessageMock.Verify(m => m.SetProperty(BizTalkFactoryProperties.ArchiveTargetLocation, location), Times.Once());
			MessageMock.Verify(m => m.DeleteProperty(BizTalkFactoryProperties.ArchiveTargetLocation), Times.Once());
			PolicyMock.VerifyAll();
		}

		[Test]
		public void ResolveProcessNameToUnidentified()
		{
			var sut = TrackingResolver.Create(new PolicyName("name", 1, 0), MessageMock.Object);
			var processName = sut.ResolveProcessName();

			Assert.That(processName, Is.EqualTo(TrackingResolver.UNIDENTIFIED_PROCESS_NAME));

			MessageMock.Verify(m => m.SetProperty(TrackingProperties.ProcessName, TrackingResolver.UNIDENTIFIED_PROCESS_NAME), Times.Never());
			PolicyMock.Verify(p => p.Execute(It.IsAny<object[]>()), Times.Once());
		}

		[Test]
		public void ResolveProcessNameViaContext()
		{
			const string name = "context-process-name";
			MessageMock.Setup(m => m.GetProperty(TrackingProperties.ProcessName)).Returns(name);

			var sut = TrackingResolver.Create(new PolicyName("name", 1, 0), MessageMock.Object);
			var processName = sut.ResolveProcessName();

			Assert.That(processName, Is.EqualTo(name));

			MessageMock.Verify(m => m.SetProperty(TrackingProperties.ProcessName, name), Times.Never());
			PolicyMock.Verify(p => p.Execute(It.IsAny<object[]>()), Times.Never());
		}

		[Test]
		public void ResolveProcessNameViaPolicy()
		{
			const string name = "policy-process-name";

			PolicyMock
				.Setup(p => p.Execute(It.IsAny<object[]>()))
				.Callback<object[]>(
					facts => {
						// assert the ProcessName in the fact base to be used by the policy mock
						facts.OfType<Context>().Single().Write(TrackingProperties.ProcessName.QName, name);
						// setup an expectation on mock to ensure subsequent read retrieves info just written
						MessageMock.Setup(m => m.GetProperty(TrackingProperties.ProcessName)).Returns(name);
					})
				.Verifiable();

			var sut = TrackingResolver.Create(new PolicyName("name", 1, 0), MessageMock.Object);
			var processName = sut.ResolveProcessName();

			Assert.That(processName, Is.EqualTo(name));

			// ReSharper disable once ImplicitlyCapturedClosure
			MessageMock.Verify(m => m.SetProperty(TrackingProperties.ProcessName, name), Times.Once());
			PolicyMock.VerifyAll();
		}

		private Unit.Message.Mock<IBaseMessage> MessageMock { get; set; }

		private Mock<IPolicy> PolicyMock { get; set; }

		private Func<Microsoft.RuleEngine.RuleSetInfo, IPolicy> _policyFactory;
	}
}

#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Dsl.Binding.Diagnostics;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	[TestFixture]
	public class NamingConventionThunkFixture
	{
		[Test]
		public void ComputeApplicationNameViaApplicationBinding()
		{
			const string name = "Application Name";

			var applicationBindingMock = new Mock<IApplicationBinding<object>>();
			applicationBindingMock.Setup(m => m.Name).Returns(name);

			Assert.That(NamingConventionThunk.ComputeApplicationName(applicationBindingMock.Object), Is.EqualTo(name));
		}

		[Test]
		public void ComputeApplicationNameViaNamingConvention()
		{
			var conventionMock = new Mock<INamingConvention<object>>();

			var applicationBindingMock = new Mock<IApplicationBinding<object>>();
			applicationBindingMock.Setup(m => m.Name).Returns(conventionMock.Object);

			NamingConventionThunk.ComputeApplicationName(applicationBindingMock.Object);

			conventionMock.Verify(m => m.ComputeApplicationName(applicationBindingMock.Object), Times.Once());
		}

		[Test]
		public void ComputeReceiveLocationNameViaNamingConvention()
		{
			var conventionMock = new Mock<INamingConvention<object>>();

			var receiveLocationMock = new Mock<IReceiveLocation<object>>();
			receiveLocationMock.Setup(m => m.Name).Returns(conventionMock.Object);

			NamingConventionThunk.ComputeReceiveLocationName(receiveLocationMock.Object);

			conventionMock.Verify(m => m.ComputeReceiveLocationName(receiveLocationMock.Object), Times.Once());
		}

		[Test]
		public void ComputeReceiveLocationNameViaReceiveLocation()
		{
			const string name = "Receive Location Name";

			var receiveLocationMock = new Mock<IReceiveLocation<object>>();
			receiveLocationMock.Setup(m => m.Name).Returns(name);

			Assert.That(NamingConventionThunk.ComputeReceiveLocationName(receiveLocationMock.Object), Is.EqualTo(name));
		}

		[Test]
		public void ComputeReceivePortNameViaNamingConvention()
		{
			var conventionMock = new Mock<INamingConvention<object>>();

			var receivePortMock = new Mock<IReceivePort<object>>();
			receivePortMock.Setup(m => m.Name).Returns(conventionMock.Object);

			NamingConventionThunk.ComputeReceivePortName(receivePortMock.Object);

			conventionMock.Verify(m => m.ComputeReceivePortName(receivePortMock.Object), Times.Once());
		}

		[Test]
		public void ComputeReceivePortNameViaReceivePort()
		{
			const string name = "Receive Port Name";

			var receivePortMock = new Mock<IReceivePort<object>>();
			receivePortMock.Setup(m => m.Name).Returns(name);

			Assert.That(NamingConventionThunk.ComputeReceivePortName(receivePortMock.Object), Is.EqualTo(name));
		}

		[Test]
		public void ComputeSendPortNameViaNamingConvention()
		{
			var conventionMock = new Mock<INamingConvention<object>>();

			var sendPortMock = new Mock<ISendPort<object>>();
			sendPortMock.Setup(m => m.Name).Returns(conventionMock.Object);

			NamingConventionThunk.ComputeSendPortName(sendPortMock.Object);

			conventionMock.Verify(m => m.ComputeSendPortName(sendPortMock.Object), Times.Once());
		}

		[Test]
		public void ComputeSendPortNameViaSendPort()
		{
			const string name = "Send Port Name";

			var sendPortMock = new Mock<ISendPort<object>>();
			sendPortMock.Setup(m => m.Name).Returns(name);

			Assert.That(NamingConventionThunk.ComputeSendPortName(sendPortMock.Object), Is.EqualTo(name));
		}

		[Test]
		public void WrapExceptionInNamingConventionException()
		{
			var conventionMock = new Mock<INamingConvention<object>>();
			conventionMock.Setup(m => m.ComputeApplicationName(It.IsAny<IApplicationBinding<object>>())).Throws<NotSupportedException>();

			var applicationBindingMock = new Mock<IApplicationBinding<object>>();
			applicationBindingMock.As<IProvideSourceFileInformation>();
			applicationBindingMock.Setup(m => m.Name).Returns(conventionMock.Object);

			Assert.That(
				() => NamingConventionThunk.ComputeApplicationName(applicationBindingMock.Object),
				Throws.TypeOf<NamingConventionException>().With.InnerException.TypeOf<NotSupportedException>());
		}
	}
}

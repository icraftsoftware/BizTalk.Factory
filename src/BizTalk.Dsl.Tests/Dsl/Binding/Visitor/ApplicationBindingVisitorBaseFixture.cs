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

using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	[TestFixture]
	public class ApplicationBindingVisitorBaseFixture
	{
		[Test]
		public void VisitApplicationBindingAppliesEnvironmentOverrides()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportValidation>();
			var environmentSensitiveApplicationBindingMock = applicationBindingMock.As<ISupportEnvironmentOverride>();

			var visitorMock = new Mock<ApplicationBindingVisitorBase>("DEV");
			visitorMock.Object.VisitApplicationBinding(applicationBindingMock.Object);

			environmentSensitiveApplicationBindingMock.Verify(m => m.ApplyEnvironmentOverrides(It.IsAny<string>()), Times.Once);
			visitorMock.Verify(m => m.VisitApplicationCore(It.IsAny<IApplicationBinding<string>>()), Times.Once);
		}

		[Test]
		public void VisitOrchestrationAppliesEnvironmentOverrides()
		{
			var orchestrationBindingMock = new Mock<IOrchestrationBinding>();
			orchestrationBindingMock.As<ISupportValidation>();
			var environmentSensitiveOrchestrationBindingMock = orchestrationBindingMock.As<ISupportEnvironmentSensitivity>();
			environmentSensitiveOrchestrationBindingMock.Setup(m => m.IsDeployableForEnvironment(It.IsAny<string>())).Returns(true);

			var visitorMock = new Mock<ApplicationBindingVisitorBase>("DEV");
			visitorMock.Object.VisitOrchestration(orchestrationBindingMock.Object);

			environmentSensitiveOrchestrationBindingMock.Verify(m => m.IsDeployableForEnvironment(It.IsAny<string>()), Times.Once);
			environmentSensitiveOrchestrationBindingMock.Verify(m => m.ApplyEnvironmentOverrides(It.IsAny<string>()), Times.Once);
			visitorMock.Verify(m => m.VisitOrchestrationCore(It.IsAny<IOrchestrationBinding>()), Times.Once);
		}

		[Test]
		public void VisitOrchestrationDoesNotApplyEnvironmentOverrides()
		{
			var orchestrationBindingMock = new Mock<IOrchestrationBinding>();
			var environmentSensitiveOrchestrationBindingMock = orchestrationBindingMock.As<ISupportEnvironmentSensitivity>();
			environmentSensitiveOrchestrationBindingMock.Setup(m => m.IsDeployableForEnvironment(It.IsAny<string>())).Returns(false);

			var visitorMock = new Mock<ApplicationBindingVisitorBase>("DEV");
			visitorMock.Object.VisitOrchestration(orchestrationBindingMock.Object);

			environmentSensitiveOrchestrationBindingMock.Verify(m => m.IsDeployableForEnvironment(It.IsAny<string>()), Times.Once);
			environmentSensitiveOrchestrationBindingMock.Verify(m => m.ApplyEnvironmentOverrides(It.IsAny<string>()), Times.Never);
			visitorMock.Verify(m => m.VisitOrchestrationCore(It.IsAny<IOrchestrationBinding>()), Times.Never);
		}

		[Test]
		public void VisitReceiveLocationAppliesEnvironmentOverrides()
		{
			var receiveLocationMock = new Mock<IReceiveLocation<string>>();
			receiveLocationMock.As<ISupportValidation>();
			var environmentSensitiveReceiveLocationMock = receiveLocationMock.As<ISupportEnvironmentSensitivity>();
			environmentSensitiveReceiveLocationMock.Setup(m => m.IsDeployableForEnvironment(It.IsAny<string>())).Returns(true);

			var visitorMock = new Mock<ApplicationBindingVisitorBase>("DEV");
			visitorMock.Object.VisitReceiveLocation(receiveLocationMock.Object);

			environmentSensitiveReceiveLocationMock.Verify(m => m.IsDeployableForEnvironment(It.IsAny<string>()), Times.Once);
			environmentSensitiveReceiveLocationMock.Verify(m => m.ApplyEnvironmentOverrides(It.IsAny<string>()), Times.Once);
			visitorMock.Verify(m => m.VisitReceiveLocationCore(It.IsAny<IReceiveLocation<string>>()), Times.Once);
		}

		[Test]
		public void VisitReceiveLocationDoesNotApplyEnvironmentOverrides()
		{
			var receiveLocationMock = new Mock<IReceiveLocation<string>>();
			var environmentSensitiveReceiveLocationMock = receiveLocationMock.As<ISupportEnvironmentSensitivity>();
			environmentSensitiveReceiveLocationMock.Setup(m => m.IsDeployableForEnvironment(It.IsAny<string>())).Returns(false);

			var visitorMock = new Mock<ApplicationBindingVisitorBase>("DEV");
			visitorMock.Object.VisitReceiveLocation(receiveLocationMock.Object);

			environmentSensitiveReceiveLocationMock.Verify(m => m.IsDeployableForEnvironment(It.IsAny<string>()), Times.Once);
			environmentSensitiveReceiveLocationMock.Verify(m => m.ApplyEnvironmentOverrides(It.IsAny<string>()), Times.Never);
			visitorMock.Verify(m => m.VisitReceiveLocationCore(It.IsAny<IReceiveLocation<string>>()), Times.Never);
		}

		[Test]
		public void VisitReceivePortAppliesEnvironmentOverrides()
		{
			var receivePortMock = new Mock<IReceivePort<string>>();
			receivePortMock.As<ISupportValidation>();
			var environmentSensitiveReceivePortMock = receivePortMock.As<ISupportEnvironmentSensitivity>();
			environmentSensitiveReceivePortMock.Setup(m => m.IsDeployableForEnvironment(It.IsAny<string>())).Returns(true);

			var visitorMock = new Mock<ApplicationBindingVisitorBase>("DEV");
			visitorMock.Object.VisitReceivePort(receivePortMock.Object);

			environmentSensitiveReceivePortMock.Verify(m => m.IsDeployableForEnvironment(It.IsAny<string>()), Times.Once);
			environmentSensitiveReceivePortMock.Verify(m => m.ApplyEnvironmentOverrides(It.IsAny<string>()), Times.Once);
			visitorMock.Verify(m => m.VisitReceivePortCore(It.IsAny<IReceivePort<string>>()), Times.Once);
		}

		[Test]
		public void VisitReceivePortDoesNotApplyEnvironmentOverrides()
		{
			var receivePortMock = new Mock<IReceivePort<string>>();
			var environmentSensitiveReceivePortMock = receivePortMock.As<ISupportEnvironmentSensitivity>();
			environmentSensitiveReceivePortMock.Setup(m => m.IsDeployableForEnvironment(It.IsAny<string>())).Returns(false);

			var visitorMock = new Mock<ApplicationBindingVisitorBase>("DEV");
			visitorMock.Object.VisitReceivePort(receivePortMock.Object);

			environmentSensitiveReceivePortMock.Verify(m => m.IsDeployableForEnvironment(It.IsAny<string>()), Times.Once);
			environmentSensitiveReceivePortMock.Verify(m => m.ApplyEnvironmentOverrides(It.IsAny<string>()), Times.Never);
			visitorMock.Verify(m => m.VisitReceivePortCore(It.IsAny<IReceivePort<string>>()), Times.Never);
		}

		[Test]
		public void VisitSendPortAppliesEnvironmentOverrides()
		{
			var sendPortMock = new Mock<ISendPort<string>>();
			sendPortMock.As<ISupportValidation>();
			var environmentSensitiveSendPortMock = sendPortMock.As<ISupportEnvironmentSensitivity>();
			environmentSensitiveSendPortMock.Setup(m => m.IsDeployableForEnvironment(It.IsAny<string>())).Returns(true);

			var visitorMock = new Mock<ApplicationBindingVisitorBase>("DEV");
			visitorMock.Object.VisitSendPort(sendPortMock.Object);

			environmentSensitiveSendPortMock.Verify(m => m.IsDeployableForEnvironment(It.IsAny<string>()), Times.Once);
			environmentSensitiveSendPortMock.Verify(m => m.ApplyEnvironmentOverrides(It.IsAny<string>()), Times.Once);
			visitorMock.Verify(m => m.VisitSendPortCore(It.IsAny<ISendPort<string>>()), Times.Once);
		}

		[Test]
		public void VisitSendPortDoesNotApplyEnvironmentOverrides()
		{
			var sendPortMock = new Mock<ISendPort<string>>();
			var environmentSensitiveSendPortMock = sendPortMock.As<ISupportEnvironmentSensitivity>();
			environmentSensitiveSendPortMock.Setup(m => m.IsDeployableForEnvironment(It.IsAny<string>())).Returns(false);

			var visitorMock = new Mock<ApplicationBindingVisitorBase>("DEV");
			visitorMock.Object.VisitSendPort(sendPortMock.Object);

			environmentSensitiveSendPortMock.Verify(m => m.IsDeployableForEnvironment(It.IsAny<string>()), Times.Once);
			environmentSensitiveSendPortMock.Verify(m => m.ApplyEnvironmentOverrides(It.IsAny<string>()), Times.Never);
			visitorMock.Verify(m => m.VisitSendPortCore(It.IsAny<ISendPort<string>>()), Times.Never);
		}
	}
}

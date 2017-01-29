#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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

using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	[TestFixture]
	public class CurrentApplicationBindingVisitorFixture
	{
		[Test]
		public void VisitApplicationBindingSettlesTargetEnvironmentOverridesAndPerformsDecoratedVisitorVisit()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<ISupportValidation>();
			var visitableApplicationBindingMock = applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var decoraterVisitorMock = new Mock<IApplicationBindingVisitor>();

			var visitor = CurrentApplicationBindingVisitor.Create(decoraterVisitorMock.Object);
			visitor.VisitApplicationBinding(applicationBindingMock.Object);

			visitableApplicationBindingMock.Verify(a => a.Accept(It.IsAny<ApplicationBindingEnvironmentSettlerVisitor>()), Times.Once);
			decoraterVisitorMock.Verify(v => v.VisitApplicationBinding(applicationBindingMock.Object), Times.Once);
		}

		[Test]
		public void VisitOrchestrationDoesNotPerformDecoratedVisitorVisitForReferencedApplicationOrchestration()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<ISupportValidation>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var referencedApplicationBindingMock = new Mock<IApplicationBinding>();

			var orchestrationBindingMock = new Mock<IOrchestrationBinding>();
			orchestrationBindingMock.Setup(o => o.ApplicationBinding).Returns(referencedApplicationBindingMock.Object);

			var decoraterVisitorMock = new Mock<IApplicationBindingVisitor>();
			var visitor = CurrentApplicationBindingVisitor.Create(decoraterVisitorMock.Object);

			visitor.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.VisitOrchestration(orchestrationBindingMock.Object);

			decoraterVisitorMock.Verify(v => v.VisitOrchestration(orchestrationBindingMock.Object), Times.Never);
		}

		[Test]
		public void VisitOrchestrationPerformsDecoratedVisitorVisitForCurrentApplicationOrchestration()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<ISupportValidation>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var orchestrationBindingMock = new Mock<IOrchestrationBinding>();
			orchestrationBindingMock.Setup(o => o.ApplicationBinding).Returns(applicationBindingMock.Object);

			var decoraterVisitorMock = new Mock<IApplicationBindingVisitor>();
			var visitor = CurrentApplicationBindingVisitor.Create(decoraterVisitorMock.Object);

			visitor.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.VisitOrchestration(orchestrationBindingMock.Object);

			decoraterVisitorMock.Verify(v => v.VisitOrchestration(orchestrationBindingMock.Object), Times.Once);
		}

		[Test]
		public void VisitReceiveLocationDoesNotPerformDecoratedVisitorVisitForReferencedApplicationReceiveLocation()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<ISupportValidation>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var referencedApplicationBindingMock = new Mock<IApplicationBinding<string>>();

			var receivePortMock = new Mock<IReceivePort<string>>();
			receivePortMock.Setup(o => o.ApplicationBinding).Returns(referencedApplicationBindingMock.Object);

			var receiveLocationMock = new Mock<IReceiveLocation<string>>();
			receiveLocationMock.Setup(o => o.ReceivePort).Returns(receivePortMock.Object);

			var decoraterVisitorMock = new Mock<IApplicationBindingVisitor>();
			var visitor = CurrentApplicationBindingVisitor.Create(decoraterVisitorMock.Object);

			visitor.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.VisitReceiveLocation(receiveLocationMock.Object);

			decoraterVisitorMock.Verify(v => v.VisitReceiveLocation(receiveLocationMock.Object), Times.Never);
		}

		[Test]
		public void VisitReceiveLocationPerformsDecoratedVisitorVisitForCurrentApplicationReceiveLocation()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<ISupportValidation>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var receivePortMock = new Mock<IReceivePort<string>>();
			receivePortMock.Setup(o => o.ApplicationBinding).Returns(applicationBindingMock.Object);

			var receiveLocationMock = new Mock<IReceiveLocation<string>>();
			receiveLocationMock.Setup(o => o.ReceivePort).Returns(receivePortMock.Object);

			var decoraterVisitorMock = new Mock<IApplicationBindingVisitor>();
			var visitor = CurrentApplicationBindingVisitor.Create(decoraterVisitorMock.Object);

			visitor.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.VisitReceiveLocation(receiveLocationMock.Object);

			decoraterVisitorMock.Verify(v => v.VisitReceiveLocation(receiveLocationMock.Object), Times.Once);
		}

		[Test]
		public void VisitReceivePortDoesNotPerformDecoratedVisitorVisitForReferencedApplicationReceivePort()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<ISupportValidation>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var referencedApplicationBindingMock = new Mock<IApplicationBinding<string>>();

			var receivePortMock = new Mock<IReceivePort<string>>();
			receivePortMock.Setup(o => o.ApplicationBinding).Returns(referencedApplicationBindingMock.Object);

			var decoraterVisitorMock = new Mock<IApplicationBindingVisitor>();
			var visitor = CurrentApplicationBindingVisitor.Create(decoraterVisitorMock.Object);

			visitor.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.VisitReceivePort(receivePortMock.Object);

			decoraterVisitorMock.Verify(v => v.VisitReceivePort(receivePortMock.Object), Times.Never);
		}

		[Test]
		public void VisitReceivePortPerformsDecoratedVisitorVisitForCurrentApplicationReceivePort()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<ISupportValidation>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var receivePortMock = new Mock<IReceivePort<string>>();
			receivePortMock.Setup(o => o.ApplicationBinding).Returns(applicationBindingMock.Object);

			var decoraterVisitorMock = new Mock<IApplicationBindingVisitor>();
			var visitor = CurrentApplicationBindingVisitor.Create(decoraterVisitorMock.Object);

			visitor.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.VisitReceivePort(receivePortMock.Object);

			decoraterVisitorMock.Verify(v => v.VisitReceivePort(receivePortMock.Object), Times.Once);
		}

		[Test]
		public void VisitReferencedApplicationBindingDoesNotPropagateDecoratedVisitorNorPerformDecoratedVisitorVisit()
		{
			var referencedApplicationBindingMock = new Mock<IVisitable<IApplicationBindingVisitor>>();

			var decoraterVisitorMock = new Mock<IApplicationBindingVisitor>();

			var visitor = CurrentApplicationBindingVisitor.Create(decoraterVisitorMock.Object);
			visitor.VisitReferencedApplicationBinding(referencedApplicationBindingMock.Object);

			referencedApplicationBindingMock.Verify(a => a.Accept(visitor), Times.Never);
			decoraterVisitorMock.Verify(v => v.VisitReferencedApplicationBinding(referencedApplicationBindingMock.Object), Times.Never);
		}

		[Test]
		public void VisitSendPortDoesNotPerformDecoratedVisitorVisitForReferencedApplicationSendPort()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<ISupportValidation>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var referencedApplicationBindingMock = new Mock<IApplicationBinding<string>>();

			var sendPortMock = new Mock<ISendPort<string>>();
			sendPortMock.Setup(o => o.ApplicationBinding).Returns(referencedApplicationBindingMock.Object);

			var decoraterVisitorMock = new Mock<IApplicationBindingVisitor>();
			var visitor = CurrentApplicationBindingVisitor.Create(decoraterVisitorMock.Object);

			visitor.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.VisitSendPort(sendPortMock.Object);

			decoraterVisitorMock.Verify(v => v.VisitSendPort(sendPortMock.Object), Times.Never);
		}

		[Test]
		public void VisitSendPortPerformsDecoratedVisitorVisitForCurrentApplicationSendPort()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<ISupportValidation>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var sendPortMock = new Mock<ISendPort<string>>();
			sendPortMock.Setup(o => o.ApplicationBinding).Returns(applicationBindingMock.Object);

			var decoraterVisitorMock = new Mock<IApplicationBindingVisitor>();
			var visitor = CurrentApplicationBindingVisitor.Create(decoraterVisitorMock.Object);

			visitor.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.VisitSendPort(sendPortMock.Object);

			decoraterVisitorMock.Verify(v => v.VisitSendPort(sendPortMock.Object), Times.Once);
		}
	}
}

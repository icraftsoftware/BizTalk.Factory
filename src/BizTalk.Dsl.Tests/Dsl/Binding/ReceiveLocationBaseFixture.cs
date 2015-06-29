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
using System.Diagnostics;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Microsoft.BizTalk.DefaultPipelines;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	public class ReceiveLocationBaseFixture
	{
		[Test]
		public void AcceptsVisitor()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };

			var visitorMock = new Mock<IApplicationBindingVisitor>();

			((IVisitable<IApplicationBindingVisitor>) receiveLocationMock.Object).Accept(visitorMock.Object);

			visitorMock.Verify(m => m.VisitReceiveLocation(receiveLocationMock.Object), Times.Once);
		}

		[Test]
		public void AutomaticallyValidatesOnConfiguratorAction()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>>(
				(Action<IReceiveLocation<string>>) (rl => {
					rl.Name = "Receive Location Name";
					rl.ReceivePipeline = new ReceivePipeline<XMLReceive>();
					rl.Transport.Adapter = new InboundFileAdapter(ifa => { ifa.ReceiveFolder = @"c:\files\drops"; });
					rl.Transport.Host = "Host";
				})) { CallBase = true };
			var validatingReceiveLocationBindingMock = receiveLocationMock.As<ISupportValidation>();

			receiveLocationMock.Object.Description = "Force Moq to call ctor.";

			validatingReceiveLocationBindingMock.Verify(m => m.Validate(), Times.Once);
		}

		[Test]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };
			receiveLocationMock.Object.ReceivePipeline = new ReceivePipeline<XMLReceive>();

			((ISupportEnvironmentOverride) receiveLocationMock.Object).ApplyEnvironmentOverrides("ACC");

			receiveLocationMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) receiveLocationMock.Object).ApplyEnvironmentOverrides(string.Empty);

			receiveLocationMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToReceivePipeline()
		{
			var receivePipelineMock = new Mock<ReceivePipeline<XMLReceive>> { CallBase = true };

			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };
			receiveLocationMock.Object.ReceivePipeline = receivePipelineMock.Object;

			((ISupportEnvironmentOverride) receiveLocationMock.Object).ApplyEnvironmentOverrides("ACC");

			receivePipelineMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToSendPipeline()
		{
			var sendPipelineMock = new Mock<SendPipeline<XMLTransmit>> { CallBase = true };

			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };
			receiveLocationMock.Object.ReceivePipeline = new ReceivePipeline<XMLReceive>();
			receiveLocationMock.Object.SendPipeline = sendPipelineMock.Object;

			((ISupportEnvironmentOverride) receiveLocationMock.Object).ApplyEnvironmentOverrides("ACC");

			sendPipelineMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToTransport()
		{
			var adapterMock = new Mock<IInboundAdapter>();
			var environmentSensitiveAdapterMock = adapterMock.As<ISupportEnvironmentOverride>();

			var receiveLocationMock = new Mock<ReceiveLocationBase<object>> { CallBase = true };
			receiveLocationMock.Object.Name = "Receive Location Name";
			receiveLocationMock.Object.ReceivePipeline = new ReceivePipeline<XMLReceive>();
			receiveLocationMock.Object.Transport.Host = "Host";
			receiveLocationMock.Object.Transport.Adapter = adapterMock.Object;

			((ISupportEnvironmentOverride) receiveLocationMock.Object).ApplyEnvironmentOverrides("ACC");

			// indirectly verifies that ReceiveLocationBase forwards ApplyEnvironmentOverrides() call to Transport, which forwards it to its adapter
			environmentSensitiveAdapterMock.Verify(m => m.ApplyEnvironmentOverrides("ACC"), Times.Once);
		}

		[Test]
		public void IsDeployableForEnvironmentIsCheckedForGivenEnvironment()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };

			((ISupportEnvironmentDeploymentPredicate) receiveLocationMock.Object).IsDeployableForEnvironment("ACC");

			receiveLocationMock.Protected().Verify("IsDeployableForEnvironment", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void IsDeployableForEnvironmentIsNotCheckedWhenNoGivenEnvironment()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };

			((ISupportEnvironmentDeploymentPredicate) receiveLocationMock.Object).IsDeployableForEnvironment(string.Empty);

			receiveLocationMock.Protected().Verify("IsDeployableForEnvironment", Times.Never(), ItExpr.IsAny<string>());
		}

		[Test]
		public void NameIsMandatory()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };
			var stackFrame = new StackFrame(0, true);
			receiveLocationMock.Object.Description = "Force Moq to call ctor.";

			Assert.That(
				() => ((ISupportValidation) receiveLocationMock.Object).Validate(),
				Throws.InstanceOf<BindingException>().With.Message.EqualTo(
					string.Format(
						"Receive Location's Name is not defined.\r\n{0}, line {1}, column {2}.",
						stackFrame.GetFileName(),
						stackFrame.GetFileLineNumber() + 1,
						stackFrame.GetFileColumnNumber())));
		}

		[Test]
		public void ReceivePipelineIsMandatory()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };
			var stackFrame = new StackFrame(0, true);
			receiveLocationMock.Object.Name = "Receive Location Name";

			Assert.That(
				() => ((ISupportValidation) receiveLocationMock.Object).Validate(),
				Throws.InstanceOf<BindingException>().With.Message.EqualTo(
					string.Format(
						"'Receive Location Name' Receive Location's Receive Pipeline is not defined.\r\n{0}, line {1}, column {2}.",
						stackFrame.GetFileName(),
						stackFrame.GetFileLineNumber() + 1,
						stackFrame.GetFileColumnNumber())));
		}

		[Test]
		public void SupportINamingConvention()
		{
			const string name = "Receive Location Name";

			var conventionMock = new Mock<INamingConvention<object>>();
			conventionMock.Setup(m => m.ComputeReceiveLocationName(It.IsAny<IReceiveLocation<object>>())).Returns(name);

			var receiveLocationMock = new Mock<ReceiveLocationBase<object>> { CallBase = true };
			receiveLocationMock.Object.Name = conventionMock.Object;

			Assert.That(((ISupportNamingConvention) receiveLocationMock.Object).Name, Is.EqualTo(name));
		}

		[Test]
		public void SupportStringNamingConvention()
		{
			const string name = "Receive Location Name";
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };

			receiveLocationMock.Object.Name = name;

			Assert.That(((ISupportNamingConvention) receiveLocationMock.Object).Name, Is.EqualTo(name));
		}

		[Test]
		public void TransportIsValidated()
		{
			var adapterMock = new Mock<IInboundAdapter>();
			var validatingAdapterMock = adapterMock.As<ISupportValidation>();

			var receiveLocationMock = new Mock<ReceiveLocationBase<object>> { CallBase = true };
			receiveLocationMock.Object.Name = "Receive Location Name";
			receiveLocationMock.Object.ReceivePipeline = new ReceivePipeline<XMLReceive>();
			receiveLocationMock.Object.Transport.Host = "Host";
			receiveLocationMock.Object.Transport.Adapter = adapterMock.Object;

			((ISupportValidation) receiveLocationMock.Object).Validate();

			// indirectly verifies that ReceiveLocationBase forwards Validate() call to Transport, which forwards it to its adapter
			validatingAdapterMock.Verify(m => m.Validate(), Times.Once);
		}
	}
}

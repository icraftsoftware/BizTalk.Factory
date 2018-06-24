﻿#region Copyright & License

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

using System;
using System.Linq;
using Be.Stateless.BizTalk.Dsl.Binding.CodeDom;
using Be.Stateless.BizTalk.Orchestrations.Dummy;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	public class OrchestrationBindingBaseFixture
	{
		[Test]
		public void AcceptsVisitor()
		{
			var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
			var visitorMock = new Mock<IApplicationBindingVisitor>();

			((IVisitable<IApplicationBindingVisitor>) orchestrationBindingMock.Object).Accept(visitorMock.Object);

			visitorMock.Verify(m => m.VisitOrchestration(orchestrationBindingMock.Object), Times.Once);
		}

		[Test]
		public void AutomaticallyValidatesOnConfiguratorAction()
		{
			var orchestrationBindingMock = new Mock<ProcessOrchestrationBinding>((Action<IProcessOrchestrationBinding>) (o => { })) { CallBase = true };
			var validatingOrchestrationBindingMock = orchestrationBindingMock.As<ISupportValidation>();
			validatingOrchestrationBindingMock.Setup(o => o.Validate()).Verifiable();

			orchestrationBindingMock.Object.Host = "Force Moq to call ctor.";

			validatingOrchestrationBindingMock.Verify(m => m.Validate(), Times.Once);
		}

		[Test]
		public void DirectPortsHaveNoMatchingPropertyInGeneratedOrchestrationBinding()
		{
			var assembly = typeof(Process).CompileToDynamicAssembly();
			var orchestrationBinding = assembly.CreateInstance(typeof(Process).FullName + "OrchestrationBinding");

			Assert.That(
				// ReSharper disable once PossibleNullReferenceException
				orchestrationBinding.GetType().GetProperties().Any(p => p.Name.StartsWith("Direct")),
				Is.False);
		}

		[Test]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };

			((ISupportEnvironmentOverride) orchestrationBindingMock.Object).ApplyEnvironmentOverrides("ACC");

			orchestrationBindingMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };

			((ISupportEnvironmentOverride) orchestrationBindingMock.Object).ApplyEnvironmentOverrides(string.Empty);

			orchestrationBindingMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[Test]
		public void HostIsMandatory()
		{
			var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
			orchestrationBindingMock.Object.Description = "Force Moq to call ctor.";

			Assert.That(
				() => ((ISupportValidation) orchestrationBindingMock.Object).Validate(),
				Throws.InstanceOf<BindingException>().With.Message.EqualTo("Orchestration's Host is not defined."));
		}

		[Test]
		public void LogicalOneWayReceivePortMustBeBoundToOneWayReceivePort()
		{
			IProcessOrchestrationBinding orchestrationBinding = new ProcessOrchestrationBinding();
			orchestrationBinding.Host = "Host";
			orchestrationBinding.ReceivePort = new TestApplication.TwoWayReceivePort();
			orchestrationBinding.SendPort = new TestApplication.OneWaySendPort();
			orchestrationBinding.SolicitResponsePort = new TestApplication.TwoWaySendPort();
			orchestrationBinding.RequestResponsePort = new TestApplication.TwoWayReceivePort();

			Assert.That(
				() => ((ISupportValidation) orchestrationBinding).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Orchestration's one-way logical port 'ReceivePort' is bound to two-way port 'TwoWayReceivePort'."));
		}

		[Test]
		public void LogicalOneWaySendPortMustBeBoundToOneWaySendPort()
		{
			IProcessOrchestrationBinding orchestrationBinding = new ProcessOrchestrationBinding();
			orchestrationBinding.Host = "Host";
			orchestrationBinding.ReceivePort = new TestApplication.OneWayReceivePort();
			orchestrationBinding.SendPort = new TestApplication.TwoWaySendPort();
			orchestrationBinding.SolicitResponsePort = new TestApplication.TwoWaySendPort();
			orchestrationBinding.RequestResponsePort = new TestApplication.TwoWayReceivePort();

			Assert.That(
				() => ((ISupportValidation) orchestrationBinding).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Orchestration's one-way logical port 'SendPort' is bound to two-way port 'TwoWaySendPort'."));
		}

		[Test]
		public void LogicalPortsMustAllBeBound()
		{
			IProcessOrchestrationBinding orchestrationBinding = new ProcessOrchestrationBinding();
			orchestrationBinding.Host = "Host";
			orchestrationBinding.ReceivePort = new Mock<ReceivePort>().Object;
			orchestrationBinding.SendPort = new Mock<SendPort>().Object;

			Assert.That(
				() => ((ISupportValidation) orchestrationBinding).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo(
						string.Format(
							"The '{0}' orchestration has unbound logical ports: 'RequestResponsePort', 'SolicitResponsePort'.",
							typeof(Process).FullName)));
		}

		[Test]
		public void LogicalRequestResponsePortMustBeBoundToTwoWayReceivePort()
		{
			IProcessOrchestrationBinding orchestrationBinding = new ProcessOrchestrationBinding();
			orchestrationBinding.Host = "Host";
			orchestrationBinding.ReceivePort = new TestApplication.OneWayReceivePort();
			orchestrationBinding.SendPort = new TestApplication.OneWaySendPort();
			orchestrationBinding.SolicitResponsePort = new TestApplication.TwoWaySendPort();
			orchestrationBinding.RequestResponsePort = new TestApplication.OneWayReceivePort();

			Assert.That(
				() => ((ISupportValidation) orchestrationBinding).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Orchestration's two-way logical port 'RequestResponsePort' is bound to one-way port 'OneWayReceivePort'."));
		}

		[Test]
		public void LogicalSolicitResponsePortMustBeBoundToTwoWaySendPort()
		{
			IProcessOrchestrationBinding orchestrationBinding = new ProcessOrchestrationBinding();
			orchestrationBinding.Host = "Host";
			orchestrationBinding.ReceivePort = new TestApplication.OneWayReceivePort();
			orchestrationBinding.SendPort = new TestApplication.OneWaySendPort();
			orchestrationBinding.SolicitResponsePort = new TestApplication.OneWaySendPort();
			orchestrationBinding.RequestResponsePort = new TestApplication.TwoWayReceivePort();

			Assert.That(
				() => ((ISupportValidation) orchestrationBinding).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Orchestration's two-way logical port 'SolicitResponsePort' is bound to one-way port 'OneWaySendPort'."));
		}

		[Test]
		public void OperationName()
		{
			Assert.That(ProcessOrchestrationBinding.SolicitResponsePort.Operations.SolicitResponseOperation.Name, Is.Not.Empty);
		}
	}
}

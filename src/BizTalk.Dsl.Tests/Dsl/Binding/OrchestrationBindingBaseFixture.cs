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
using System.Diagnostics;
using System.Linq;
using Be.Stateless.BizTalk.Dsl.Binding.CodeDom;
using Be.Stateless.BizTalk.Orchestrations.Dummy;
using Be.Stateless.Reflection;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Process = Be.Stateless.BizTalk.Orchestrations.Dummy.Process;

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
			var orchestrationBindingMock = new Mock<ProcessOrchestrationBinding>((Action<ProcessOrchestrationBinding>) (o => { })) { CallBase = true };
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
			var stackFrame = new StackFrame(0, true);
			orchestrationBindingMock.Object.Description = "Force Moq to call ctor.";

			Assert.That(
				() => ((ISupportValidation) orchestrationBindingMock.Object).Validate(),
				Throws.InstanceOf<BindingException>().With.Message.EqualTo(
					string.Format(
						"'{0}' Orchestration's Host is not defined.\r\n{1}, line {2}, column {3}.",
						typeof(Process).FullName,
						stackFrame.GetFileName(),
						stackFrame.GetFileLineNumber() + 1,
						stackFrame.GetFileColumnNumber())));
		}

		[Test]
		public void IsDeployableForEnvironmentIsCheckedForGivenEnvironment()
		{
			var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };

			((ISupportEnvironmentDeploymentPredicate) orchestrationBindingMock.Object).IsDeployableForEnvironment("ACC");

			orchestrationBindingMock.Protected().Verify("IsDeployableForEnvironment", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void IsDeployableForEnvironmentIsNotCheckedWhenNoGivenEnvironment()
		{
			var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };

			((ISupportEnvironmentDeploymentPredicate) orchestrationBindingMock.Object).IsDeployableForEnvironment(string.Empty);

			orchestrationBindingMock.Protected().Verify("IsDeployableForEnvironment", Times.Never(), ItExpr.IsAny<string>());
		}

		[Test]
		public void LogicalOneWayReceivePortMustBeBoundToOneWayReceivePort()
		{
			var stackFrame = new StackFrame(0, true);
			var orchestrationBinding = new ProcessOrchestrationBinding {
				Host = "Host",
				ReceivePort = new TestApplication.TwoWayReceivePort(),
				SendPort = new TestApplication.OneWaySendPort(),
				SolicitResponsePort = new TestApplication.TwoWaySendPort(),
				RequestResponsePort = new TestApplication.TwoWayReceivePort()
			};

			Assert.That(
				() => ((ISupportValidation) orchestrationBinding).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo(
						string.Format(
							"'{0}' Orchestration's one-way logical port 'ReceivePort' is bound to two-way port 'TwoWayReceivePort'.\r\n{1}, line {2}, column {3}.",
							typeof(Process).FullName,
							stackFrame.GetFileName(),
							stackFrame.GetFileLineNumber() + 1,
							stackFrame.GetFileColumnNumber())));
		}

		[Test]
		public void LogicalOneWaySendPortMustBeBoundToOneWaySendPort()
		{
			var stackFrame = new StackFrame(0, true);
			var orchestrationBinding = new ProcessOrchestrationBinding {
				Host = "Host",
				ReceivePort = new TestApplication.OneWayReceivePort(),
				SendPort = new TestApplication.TwoWaySendPort(),
				SolicitResponsePort = new TestApplication.TwoWaySendPort(),
				RequestResponsePort = new TestApplication.TwoWayReceivePort()
			};

			Assert.That(
				() => ((ISupportValidation) orchestrationBinding).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo(
						string.Format(
							"'{0}' Orchestration's one-way logical port 'SendPort' is bound to two-way port 'TwoWaySendPort'.\r\n{1}, line {2}, column {3}.",
							typeof(Process).FullName,
							stackFrame.GetFileName(),
							stackFrame.GetFileLineNumber() + 1,
							stackFrame.GetFileColumnNumber())));
		}

		[Test]
		public void LogicalPortsMustAllBeBound()
		{
			var assembly = typeof(Process).CompileToDynamicAssembly();
			var orchestrationBinding = (IOrchestrationBinding) assembly.CreateInstance(typeof(Process).FullName + "OrchestrationBinding");
			// ReSharper disable once PossibleNullReferenceException
			orchestrationBinding.Host = "Host";
			Reflector.SetProperty(orchestrationBinding, "ReceivePort", new Mock<ReceivePort>().Object);
			Reflector.SetProperty(orchestrationBinding, "SendPort", new Mock<SendPort>().Object);

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
			var stackFrame = new StackFrame(0, true);
			var orchestrationBinding = new ProcessOrchestrationBinding {
				Host = "Host",
				ReceivePort = new TestApplication.OneWayReceivePort(),
				SendPort = new TestApplication.OneWaySendPort(),
				SolicitResponsePort = new TestApplication.TwoWaySendPort(),
				RequestResponsePort = new TestApplication.OneWayReceivePort()
			};

			Assert.That(
				() => ((ISupportValidation) orchestrationBinding).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo(
						string.Format(
							"'{0}' Orchestration's two-way logical port 'RequestResponsePort' is bound to one-way port 'OneWayReceivePort'.\r\n{1}, line {2}, column {3}.",
							typeof(Process).FullName,
							stackFrame.GetFileName(),
							stackFrame.GetFileLineNumber() + 1,
							stackFrame.GetFileColumnNumber())));
		}

		[Test]
		public void LogicalSolicitResponsePortMustBeBoundToTwoWaySendPort()
		{
			var stackFrame = new StackFrame(0, true);
			var orchestrationBinding = new ProcessOrchestrationBinding {
				Host = "Host",
				ReceivePort = new TestApplication.OneWayReceivePort(),
				SendPort = new TestApplication.OneWaySendPort(),
				SolicitResponsePort = new TestApplication.OneWaySendPort(),
				RequestResponsePort = new TestApplication.TwoWayReceivePort()
			};

			Assert.That(
				() => ((ISupportValidation) orchestrationBinding).Validate(),
				Throws.TypeOf<BindingException>()
					.With.Message.EqualTo(
						string.Format(
							"'{0}' Orchestration's two-way logical port 'SolicitResponsePort' is bound to one-way port 'OneWaySendPort'.\r\n{1}, line {2}, column {3}.",
							typeof(Process).FullName,
							stackFrame.GetFileName(),
							stackFrame.GetFileLineNumber() + 1,
							stackFrame.GetFileColumnNumber())));
		}
	}
}

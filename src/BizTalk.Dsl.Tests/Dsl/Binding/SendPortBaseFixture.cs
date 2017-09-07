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

using System;
using System.Linq.Expressions;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Microsoft.BizTalk.DefaultPipelines;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	// http://social.msdn.microsoft.com/Forums/en-US/fec0c1c2-a3fd-4b40-8f12-deab25a91c92/sendport-bindingoption
	// A Send Port BindingOption="0" means that no Orchestrations are bound to this Send Port. A Send Port
	// BindingOption="1" indicates that at least one Orchestration is bound to this Send Port. Therefore if
	// Send Port BindingOption="0" make sure Send Port subscribes to messages via a Filter, because if it
	// doesn't then it has no means of subscribing to messages (no Orchestration binding, no Filter expression).

	[TestFixture]
	public class SendPortBaseFixture
	{
		[Test]
		public void AcceptsVisitor()
		{
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			var visitorMock = new Mock<IApplicationBindingVisitor>();

			((IVisitable<IApplicationBindingVisitor>) sendPortMock.Object).Accept(visitorMock.Object);

			visitorMock.Verify(m => m.VisitSendPort(sendPortMock.Object), Times.Once);
		}

		[Test]
		public void AutomaticallyValidatesOnConfiguratorAction()
		{
			var sendPortMock = new Mock<SendPortBase<string>>(
				(Action<ISendPort<string>>) (sp => {
					sp.Name = "Send Port Name";
					sp.SendPipeline = new SendPipeline<XMLTransmit>();
					sp.Transport.Adapter = new FileAdapter.Outbound(ifa => { ifa.DestinationFolder = @"c:\files\drops"; });
					sp.Transport.Host = "Host";
				})) { CallBase = true };
			var validatingSendPortMock = sendPortMock.As<ISupportValidation>();

			sendPortMock.Object.Description = "Force Moq to call ctor.";

			validatingSendPortMock.Verify(m => m.Validate(), Times.Once);
		}

		[Test]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides("ACC");

			sendPortMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides(string.Empty);

			sendPortMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToFilter()
		{
			var filterMock = new Mock<Filter>((Expression<Func<bool>>) (() => false));
			var environmentSensitiveFilterMock = filterMock.As<ISupportEnvironmentOverride>();

			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };
			sendPortMock.Object.Filter = filterMock.Object;

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides("ACC");

			environmentSensitiveFilterMock.Verify(m => m.ApplyEnvironmentOverrides("ACC"), Times.Once);
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToReceivePipeline()
		{
			var receivePipelineMock = new Mock<ReceivePipeline<XMLReceive>> { CallBase = true };

			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };
			sendPortMock.Object.ReceivePipeline = receivePipelineMock.Object;

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides("ACC");

			receivePipelineMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToSendPipeline()
		{
			var sendPipelineMock = new Mock<SendPipeline<XMLTransmit>> { CallBase = true };

			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };
			sendPortMock.Object.SendPipeline = sendPipelineMock.Object;

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides("ACC");

			sendPipelineMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToTransport()
		{
			var backupAdapterMock = new Mock<IOutboundAdapter>();
			var environmentSensitiveBackupAdapterMock = backupAdapterMock.As<ISupportEnvironmentOverride>();

			var adapterMock = new Mock<IOutboundAdapter>();
			var environmentSensitiveAdapterMock = adapterMock.As<ISupportEnvironmentOverride>();

			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };
			sendPortMock.Object.Name = "Send Port Name";
			sendPortMock.Object.Transport.Host = "Host";
			sendPortMock.Object.Transport.Adapter = adapterMock.Object;
			sendPortMock.Object.BackupTransport.Host = "Host";
			sendPortMock.Object.BackupTransport.Adapter = backupAdapterMock.Object;

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides("ACC");

			// indirectly verifies that SendPortBase forwards ApplyEnvironmentOverrides() call to Transport, which forwards it to its adapter
			environmentSensitiveAdapterMock.Verify(m => m.ApplyEnvironmentOverrides("ACC"), Times.Once);
			environmentSensitiveBackupAdapterMock.Verify(m => m.ApplyEnvironmentOverrides("ACC"), Times.Once);
		}

		[Test]
		public void NameIsMandatory()
		{
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			sendPortMock.Object.Description = "Force Moq to call ctor.";

			Assert.That(
				() => ((ISupportValidation) sendPortMock.Object).Validate(),
				Throws.InstanceOf<BindingException>().With.Message.EqualTo("Send Port's Name is not defined."));
		}

		[Test]
		public void SendPipelineIsMandatory()
		{
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			sendPortMock.Object.Name = "Send Port Name";

			Assert.That(
				() => ((ISupportValidation) sendPortMock.Object).Validate(),
				Throws.InstanceOf<BindingException>().With.Message.EqualTo("Send Port's Send Pipeline is not defined."));
		}

		[Test]
		public void SupportINamingConvention()
		{
			const string name = "Send Port Name";

			var conventionMock = new Mock<INamingConvention<object>>();
			conventionMock.Setup(m => m.ComputeSendPortName(It.IsAny<ISendPort<object>>())).Returns(name);

			var sendPortMock = new Mock<SendPortBase<object>> { CallBase = true };
			sendPortMock.Object.Name = conventionMock.Object;

			Assert.That(((ISupportNamingConvention) sendPortMock.Object).Name, Is.EqualTo(name));
		}

		[Test]
		public void SupportStringNamingConvention()
		{
			const string name = "Send Port Name";
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			sendPortMock.Object.Name = name;

			Assert.That(((ISupportNamingConvention) sendPortMock.Object).Name, Is.EqualTo(name));
		}

		[Test]
		public void TransportIsValidated()
		{
			var adapterMock = new Mock<IOutboundAdapter>();
			var validatingAdapterMock = adapterMock.As<ISupportValidation>();

			var backupAdapterMock = new Mock<IOutboundAdapter>();
			var validatingBackupAdapterMock = backupAdapterMock.As<ISupportValidation>();

			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };
			sendPortMock.Object.Name = "Send Port Name";
			sendPortMock.Object.SendPipeline = new SendPipeline<XMLTransmit>();
			sendPortMock.Object.Transport.Host = "Host";
			sendPortMock.Object.Transport.Adapter = adapterMock.Object;
			sendPortMock.Object.BackupTransport.Host = "Host";
			sendPortMock.Object.BackupTransport.Adapter = backupAdapterMock.Object;

			((ISupportValidation) sendPortMock.Object).Validate();

			// indirectly verifies that SendPortBase forwards Validate() call to Transport, which forwards it to its adapter
			validatingAdapterMock.Verify(m => m.Validate(), Times.Once);
			// indirectly verifies that SendPortBase forwards Validate() call to BackupTransport, which forwards it to its adapter
			validatingBackupAdapterMock.Verify(m => m.Validate(), Times.Once);
		}
	}
}

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

using System.Diagnostics;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	public class TransportBaseFixture
	{
		[Test]
		public void ForwardsApplyEnvironmentOverridesToAdapter()
		{
			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			var environmentSensitiveTransportMock = transportMock.As<ISupportEnvironmentOverride>();
			var adapterMock = new Mock<IAdapter>();
			var environmentSensitiveAdapterMock = adapterMock.As<ISupportEnvironmentOverride>();

			transportMock.Object.Adapter = adapterMock.Object;
			environmentSensitiveTransportMock.Object.ApplyEnvironmentOverrides("ACC");

			environmentSensitiveAdapterMock.Verify(m => m.ApplyEnvironmentOverrides("ACC"), Times.Once);
		}

		[Test]
		public void ForwardsValidateToAdapter()
		{
			var adapterMock = new Mock<IAdapter>();
			var validatingAdapterMock = adapterMock.As<ISupportValidation>();

			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			var validatingTransportMock = transportMock.As<ISupportValidation>();
			transportMock.Object.Host = "Host";
			transportMock.Object.Adapter = adapterMock.Object;

			validatingTransportMock.Object.Validate();

			validatingAdapterMock.Verify(m => m.Validate(), Times.Once);
		}

		[Test]
		public void IgnoresEnvironmentOverrides()
		{
			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			var environmentSensitiveTransportMock = transportMock.As<ISupportEnvironmentOverride>();

			environmentSensitiveTransportMock.Object.ApplyEnvironmentOverrides(string.Empty);

			transportMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[Test]
		public void SupportsEnvironmentOverrides()
		{
			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			var environmentSensitiveTransportMock = transportMock.As<ISupportEnvironmentOverride>();

			environmentSensitiveTransportMock.Object.ApplyEnvironmentOverrides("ACC");

			transportMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Test]
		public void TransportAdapterIsMandatory()
		{
			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			var stackFrame = new StackFrame(0, true);
			transportMock.Object.Host = "Host";

			Assert.That(
				() => ((ISupportValidation) transportMock.Object).Validate(),
				Throws.InstanceOf<BindingException>().With.Message.EqualTo(
					string.Format(
						"'{0}' Transport's Adapter is not defined.\r\n{1}, line {2}, column {3}.",
						transportMock.Object.GetType().FullName,
						stackFrame.GetFileName(),
						stackFrame.GetFileLineNumber() + 1,
						stackFrame.GetFileColumnNumber())));
		}

		[Test]
		public void TransportHostIsMandatory()
		{
			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			var stackFrame = new StackFrame(0, true);
			transportMock.Object.Adapter = new OutboundFileAdapter(a => { });

			Assert.That(
				() => ((ISupportValidation) transportMock.Object).Validate(),
				Throws.InstanceOf<BindingException>().With.Message.EqualTo(
					string.Format(
						"'{0}' Transport's Host is not defined.\r\n{1}, line {2}, column {3}.",
						transportMock.Object.GetType().FullName,
						stackFrame.GetFileName(),
						stackFrame.GetFileLineNumber() + 1,
						stackFrame.GetFileColumnNumber())));
		}

		[Test]
		public void TransportUnknownAdapterIsInvalid()
		{
			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			var stackFrame = new StackFrame(0, true);
			transportMock.Object.Host = "Host";

			var adapterMock = new Mock<TransportBase<IAdapter>.UnknownAdapter>();
			transportMock.Object.Adapter = adapterMock.Object;

			Assert.That(
				() => ((ISupportValidation) transportMock.Object).Validate(),
				Throws.InstanceOf<BindingException>().With.Message.EqualTo(
					string.Format(
						"'{0}' Transport's Adapter is not defined.\r\n{1}, line {2}, column {3}.",
						transportMock.Object.GetType().FullName,
						stackFrame.GetFileName(),
						stackFrame.GetFileLineNumber() + 1,
						stackFrame.GetFileColumnNumber())));
		}
	}
}

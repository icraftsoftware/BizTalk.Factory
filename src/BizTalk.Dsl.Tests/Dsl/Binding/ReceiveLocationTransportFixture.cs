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

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	public class ReceiveLocationTransportFixture
	{
		[Test]
		public void DefaultUnknownInboundAdapterFailsValidate()
		{
			var rlt = new ReceiveLocationTransport { Host = "Host" };
			Assert.That(
				() => ((ISupportValidation) rlt).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Transport's Adapter is not defined."));
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToSchedule()
		{
			var scheduleMock = new Mock<Schedule>();
			var environmentSensitiveScheduleMock = scheduleMock.As<ISupportEnvironmentOverride>();

			var rlt = new ReceiveLocationTransport { Schedule = scheduleMock.Object };
			((ISupportEnvironmentOverride) rlt).ApplyEnvironmentOverrides("ACC");

			environmentSensitiveScheduleMock.Verify(m => m.ApplyEnvironmentOverrides("ACC"), Times.Once);
		}
	}
}

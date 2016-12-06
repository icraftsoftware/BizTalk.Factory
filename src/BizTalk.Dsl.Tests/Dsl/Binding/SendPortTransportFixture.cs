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
	public class SendPortTransportFixture
	{
		[Test]
		public void DefaultUnknownOutboundAdapterFailsValidate()
		{
			var spt = new SendPortTransport { Host = "Host" };
			Assert.That(
				() => ((ISupportValidation) spt).Validate(),
				Throws.TypeOf<BindingException>().With.Message.EqualTo("Transport's Adapter is not defined."));
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToRetryPolicy()
		{
			var retryPolicyMock = new Mock<RetryPolicy>();
			var environmentSensitiveRetryPolicyMock = retryPolicyMock.As<ISupportEnvironmentOverride>();

			var spt = new SendPortTransport { RetryPolicy = retryPolicyMock.Object };
			((ISupportEnvironmentOverride) spt).ApplyEnvironmentOverrides("ACC");

			environmentSensitiveRetryPolicyMock.Verify(m => m.ApplyEnvironmentOverrides("ACC"), Times.Once);
		}

		[Test]
		public void ForwardsApplyEnvironmentOverridesToServiceWindow()
		{
			var serviceWindowMock = new Mock<ServiceWindow>();
			var environmentSensitiveServiceWindowMock = serviceWindowMock.As<ISupportEnvironmentOverride>();

			var spt = new SendPortTransport { ServiceWindow = serviceWindowMock.Object };
			((ISupportEnvironmentOverride) spt).ApplyEnvironmentOverrides("ACC");

			environmentSensitiveServiceWindowMock.Verify(m => m.ApplyEnvironmentOverrides("ACC"), Times.Once);
		}
	}
}

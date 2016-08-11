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
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration
{
	[TestFixture]
	public class NetMsmqBindingElementFixture
	{
		[Test]
		public void NetMsmqBindingElementDecoratorCanBeUsedAsWcfCustomAdapterTypeParameter()
		{
			Assert.That(
				() => new WcfCustomAdapter.Inbound<NetMsmqBindingElement>(a => { a.Binding.RetryPolicy = Convention.BizTalkFactory.NetMsmqRetryPolicy.LongRunning; }),
				Throws.Nothing);
		}

		[Test]
		public void NetMsmqRetryPolicyIsEnvironmentSensitive()
		{
			var wca = Convention.BizTalkFactory.NetMsmqRetryPolicy.LongRunning;

			((ISupportEnvironmentOverride) wca).ApplyEnvironmentOverrides("BLD");
			Assert.That(wca.MaxRetryCycles, Is.EqualTo(0));
			Assert.That(wca.ReceiveRetryCount, Is.EqualTo(2));
			Assert.That(wca.RetryCycleDelay, Is.EqualTo(TimeSpan.Zero));
			Assert.That(wca.TimeToLive, Is.EqualTo(TimeSpan.FromMinutes(1)));

			((ISupportEnvironmentOverride) wca).ApplyEnvironmentOverrides("ACC");
			Assert.That(wca.MaxRetryCycles, Is.EqualTo(3));
			Assert.That(wca.ReceiveRetryCount, Is.EqualTo(3));
			Assert.That(wca.RetryCycleDelay, Is.EqualTo(TimeSpan.FromMinutes(9)));
			Assert.That(wca.TimeToLive, Is.EqualTo(TimeSpan.FromMinutes(30)));

			((ISupportEnvironmentOverride) wca).ApplyEnvironmentOverrides("PRD");
			Assert.That(wca.MaxRetryCycles, Is.EqualTo(71));
			Assert.That(wca.ReceiveRetryCount, Is.EqualTo(1));
			Assert.That(wca.RetryCycleDelay, Is.EqualTo(TimeSpan.FromHours(1)));
			Assert.That(wca.TimeToLive, Is.EqualTo(TimeSpan.FromDays(3)));
		}

		[Test]
		public void SerializeToXml()
		{
			var binding = new NetMsmqBindingElement { RetryPolicy = Convention.BizTalkFactory.NetMsmqRetryPolicy.LongRunning };
			binding.ApplyEnvironmentOverrides("ACC");
			Assert.That(
				binding.GetBindingElementXml("netMsmqBinding"),
				Is.EqualTo("<binding name=\"netMsmqBinding\" maxRetryCycles=\"3\" receiveRetryCount=\"3\" retryCycleDelay=\"00:09:00\" timeToLive=\"00:30:00\" />"));
		}
	}
}

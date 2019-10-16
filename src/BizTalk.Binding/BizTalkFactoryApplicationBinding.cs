#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Constants;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple;

namespace Be.Stateless.BizTalk
{
	[SuppressMessage("ReSharper", "FunctionComplexityOverflow")]
	public class BizTalkFactoryApplicationBinding : Dsl.Binding.Convention.ApplicationBinding<NamingConvention>
	{
		public BizTalkFactoryApplicationBinding()
		{
			Name = ApplicationName.Is(BizTalkFactorySettings.APPLICATION_NAME);
			Description = "Library to speed up the development of BizTalk Server applications.";
			ReceivePorts.Add(
				new BatchReceivePort(),
				new ClaimReceivePort());
			SendPorts.Add(
				new BatchAddPartSendPort(),
				new BatchQueueControlledReleaseSendPort(),
				new ClaimCheckInSendPort(),
				new SinkFailedMessageSendPort());
		}

		#region Base Class Member Overrides

		protected override void ApplyEnvironmentOverrides(string environment)
		{
			if (environment.IsDevelopment())
			{
				ReceivePorts.Add(
					new UnitTestReceivePort());
				SendPorts.Add(
					new UnitTestBatchReleaseSendPort(),
					new UnitTestClaimRedeemSendPort(),
					new UnitTestStubSendPort());
			}
		}

		#endregion
	}
}

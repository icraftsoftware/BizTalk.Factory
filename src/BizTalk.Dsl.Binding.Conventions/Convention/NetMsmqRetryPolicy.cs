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
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Constants;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public static class NetMsmqRetryPolicy
	{
		#region Nested Type: EnvironmentSensitiveNetMsmqRetryPolicy

		private class EnvironmentSensitiveNetMsmqRetryPolicy : ServiceModel.Configuration.NetMsmqRetryPolicy, ISupportEnvironmentOverride
		{
			public EnvironmentSensitiveNetMsmqRetryPolicy(Func<string, ServiceModel.Configuration.NetMsmqRetryPolicy> policySelector)
			{
				_policySelector = policySelector;
			}

			#region ISupportEnvironmentOverride Members

			public void ApplyEnvironmentOverrides(string environment)
			{
				_policy = _policySelector(environment);
			}

			#endregion

			#region Base Class Member Overrides

			public override int MaxRetryCycles
			{
				get { return _policy.MaxRetryCycles; }
			}

			public override int ReceiveRetryCount
			{
				get { return _policy.ReceiveRetryCount; }
			}

			public override TimeSpan RetryCycleDelay
			{
				get { return _policy.RetryCycleDelay; }
			}

			public override TimeSpan TimeToLive
			{
				get { return _policy.TimeToLive; }
			}

			#endregion

			private readonly Func<string, ServiceModel.Configuration.NetMsmqRetryPolicy> _policySelector;

			private ServiceModel.Configuration.NetMsmqRetryPolicy _policy;
		}

		#endregion

		static NetMsmqRetryPolicy()
		{
			_longRunning = new ServiceModel.Configuration.NetMsmqRetryPolicy {
				MaxRetryCycles = 71,
				ReceiveRetryCount = 1,
				RetryCycleDelay = TimeSpan.FromHours(1),
				TimeToLive = TimeSpan.FromDays(3)
			};
			_realTime = new ServiceModel.Configuration.NetMsmqRetryPolicy {
				MaxRetryCycles = 0,
				ReceiveRetryCount = 2,
				RetryCycleDelay = TimeSpan.Zero,
				TimeToLive = TimeSpan.FromMinutes(1)
			};
			_shortRunning = new ServiceModel.Configuration.NetMsmqRetryPolicy {
				MaxRetryCycles = 3,
				ReceiveRetryCount = 3,
				RetryCycleDelay = TimeSpan.FromMinutes(9),
				TimeToLive = TimeSpan.FromMinutes(30)
			};
		}

		public static ServiceModel.Configuration.NetMsmqRetryPolicy LongRunning
		{
			get
			{
				return new EnvironmentSensitiveNetMsmqRetryPolicy(
					environment => environment.IsOneOf(TargetEnvironment.DEVELOPMENT, TargetEnvironment.BUILD)
						? _realTime
						: environment.IsAcceptance()
							? _shortRunning
							: _longRunning);
			}
		}

		public static ServiceModel.Configuration.NetMsmqRetryPolicy RealTime
		{
			get { return _realTime; }
		}

		public static ServiceModel.Configuration.NetMsmqRetryPolicy ShortRunning
		{
			get
			{
				return new EnvironmentSensitiveNetMsmqRetryPolicy(
					environment => environment.IsOneOf(TargetEnvironment.DEVELOPMENT, TargetEnvironment.BUILD, TargetEnvironment.ACCEPTANCE)
						? _realTime
						: _shortRunning);
			}
		}

		private static readonly ServiceModel.Configuration.NetMsmqRetryPolicy _longRunning;
		private static readonly ServiceModel.Configuration.NetMsmqRetryPolicy _realTime;
		private static readonly ServiceModel.Configuration.NetMsmqRetryPolicy _shortRunning;
	}
}

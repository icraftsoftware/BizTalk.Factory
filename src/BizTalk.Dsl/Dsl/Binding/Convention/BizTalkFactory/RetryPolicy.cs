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
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.BizTalkFactory
{
	public static class RetryPolicy
	{
		#region Nested Type: EnvironmentSensitiveRetryPolicy

		private class EnvironmentSensitiveRetryPolicy : Binding.RetryPolicy, ISupportEnvironmentOverride
		{
			public EnvironmentSensitiveRetryPolicy(Func<string, Binding.RetryPolicy> policySelector)
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

			public override int Count
			{
				get { return _policy.Count; }
			}

			public override TimeSpan Interval
			{
				get { return _policy.Interval; }
			}

			#endregion

			private readonly Func<string, Binding.RetryPolicy> _policySelector;

			private Binding.RetryPolicy _policy;
		}

		#endregion

		static RetryPolicy()
		{
			_longRunning = new Binding.RetryPolicy { Count = 300, Interval = TimeSpan.FromMinutes(15) };
			_realTime = new Binding.RetryPolicy { Count = 0 };
			_shortRunning = new Binding.RetryPolicy { Count = 15, Interval = TimeSpan.FromMinutes(2) };
		}

		public static Binding.RetryPolicy LongRunning
		{
			get
			{
				return new EnvironmentSensitiveRetryPolicy(
					environment => environment.IsOneOf("DEV", "BLD")
						? _realTime
						: environment == "ACC"
							? _shortRunning
							: _longRunning);
			}
		}

		public static Binding.RetryPolicy RealTime
		{
			get { return _realTime; }
		}

		public static Binding.RetryPolicy ShortRunning
		{
			get
			{
				return new EnvironmentSensitiveRetryPolicy(
					environment => environment.IsOneOf("DEV", "BLD", "ACC")
						? _realTime
						: _shortRunning);
			}
		}

		private static readonly Binding.RetryPolicy _longRunning;
		private static readonly Binding.RetryPolicy _realTime;
		private static readonly Binding.RetryPolicy _shortRunning;
	}
}

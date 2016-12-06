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

using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public sealed class SendPortTransport : TransportBase<IOutboundAdapter>
	{
		#region Nested Type: UnknownOutboundAdapter

		internal class UnknownOutboundAdapter : UnknownAdapter, IOutboundAdapter
		{
			public static readonly IOutboundAdapter Instance = new UnknownOutboundAdapter();
		}

		#endregion

		public SendPortTransport()
		{
			Adapter = UnknownOutboundAdapter.Instance;
			RetryPolicy = RetryPolicy.Default;
			ServiceWindow = ServiceWindow.None;
		}

		#region Base Class Member Overrides

		protected override void ApplyEnvironmentOverrides(string environment)
		{
			// ReSharper disable SuspiciousTypeConversion.Global
			(RetryPolicy as ISupportEnvironmentOverride).IfNotNull(s => s.ApplyEnvironmentOverrides(environment));
			(ServiceWindow as ISupportEnvironmentOverride).IfNotNull(s => s.ApplyEnvironmentOverrides(environment));
			// ReSharper restore SuspiciousTypeConversion.Global
		}

		#endregion

		public RetryPolicy RetryPolicy { get; set; }

		/// <summary>
		/// <see cref="ServiceWindow"/> restricts the <see cref="SendPortBase{TNamingConvention}"/> to work during certain
		/// hours of the day.
		/// </summary>
		public ServiceWindow ServiceWindow { get; set; }
	}
}

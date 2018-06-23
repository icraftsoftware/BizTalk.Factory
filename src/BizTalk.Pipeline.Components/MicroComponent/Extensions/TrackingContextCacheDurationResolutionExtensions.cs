#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent.Extensions
{
	internal static class TrackingContextCacheDurationResolutionExtensions
	{
		public static int ResolveTrackingContextCacheDuration(this IBaseMessage message, TimeSpan configuredDuration)
		{
			// TODO ? assert that WCF transport is used
			var wcfTimeout = message.GetProperty(WcfProperties.SendTimeout);
			if (!wcfTimeout.IsNullOrEmpty()) return (int) TimeSpan.Parse(wcfTimeout).TotalSeconds;

			// TODO ? assert that HTTP transport is used
			var httpTimeout = message.GetProperty(HttpProperties.RequestTimeout);
			// TODO ? assert Timeout.Value > 0
			if (httpTimeout.HasValue) return httpTimeout.Value;

			return (int) configuredDuration.TotalSeconds;
		}
	}
}

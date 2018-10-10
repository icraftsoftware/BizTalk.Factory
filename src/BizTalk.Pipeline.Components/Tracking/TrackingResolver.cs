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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Factory.Areas;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Ensure process name resolution while <see cref="ActivityTrackerComponent"/> is tracking.
	/// </summary>
	// TODO refactor off
	[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
	internal class TrackingResolver
	{
		internal static TrackingResolver Create(IBaseMessage message)
		{
			return Factory(message);
		}

		#region Mock's Factory Hook Point

		internal static Func<IBaseMessage, TrackingResolver> Factory
		{
			get { return _factory; }
			set { _factory = value; }
		}

		#endregion

		// ReSharper disable once MemberCanBePrivate.Global
		// ctor is protected for mocking purposes
		protected TrackingResolver(IBaseMessage message)
		{
			_message = message;
		}

		internal virtual string ResolveProcessName()
		{
			var processName = _message.GetProperty(TrackingProperties.ProcessName);
			if (processName.IsNullOrEmpty())
			{
				// could fallback on an UnidentifiedProcessResolver policy if process name remains unresolved but that would
				// be overkill just to get back constant default process name; besides what if policy execution fails
				processName = Default.Processes.Unidentified;
			}
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Resolved process name to '{0}'.", processName);
			return processName;
		}

		private static Func<IBaseMessage, TrackingResolver> _factory = message => new TrackingResolver(message);
		private static readonly ILog _logger = LogManager.GetLogger(typeof(TrackingResolver));
		private readonly IBaseMessage _message;
	}
}

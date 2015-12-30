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

using System;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Factory;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Ensure <see cref="ActivityTrackerComponent"/>'s <see cref="ActivityTrackerComponent.TrackingResolutionPolicy"/>
	/// is executed both lazily and one and only once.
	/// </summary>
	// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
	internal class TrackingResolver
	{
		internal static TrackingResolver Create(PolicyName policyName, IBaseMessage message)
		{
			return Factory(policyName, message);
		}

		#region Mock's Factory Hook Point

		internal static Func<PolicyName, IBaseMessage, TrackingResolver> Factory
		{
			get { return _factory; }
			set { _factory = value; }
		}

		#endregion

		// ReSharper disable once MemberCanBePrivate.Global
		// ctor is protected for mocking purposes
		protected TrackingResolver(PolicyName policyName, IBaseMessage message)
		{
			_policyName = policyName;
			_message = message;
		}

		internal virtual string ResolveProcessName()
		{
			var processName = ResolveProperty(TrackingProperties.ProcessName);
			if (processName.IsNullOrEmpty())
			{
				// could fallback on an UnidentifiedProcessResolver policy if process name remains unresolved but that'd
				// be overkill just to get back constant default process name; besides what if policy execution fails
				processName = GlobalArea.Processes.Unidentified;
			}
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Resolved process name to '{0}'.", processName);
			return processName;
		}

		internal virtual string ResolveArchiveTargetLocation()
		{
			var targetLocation = ResolveProperty(BizTalkFactoryProperties.ArchiveTargetLocation);
			// remove it from ctxt to avoid unintended side effects
			_message.DeleteProperty(BizTalkFactoryProperties.ArchiveTargetLocation);
			return targetLocation;
		}

		private string ResolveProperty<T>(MessageContextProperty<T, string> property)
			where T : MessageContextPropertyBase, new()
		{
			var value = _message.GetProperty(property);
			if (value.IsNullOrEmpty() && _policyName != null && !_hasPolicyRun)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Executing {0}'s policy '{1}'.", typeof(ActivityTrackerComponent).Name, _policyName);
				Policy.Execute(_policyName, new Context(_message.Context));
				value = _message.GetProperty(property);
				_hasPolicyRun = true;
			}
			return value;
		}

		private static Func<PolicyName, IBaseMessage, TrackingResolver> _factory =
			(trackingResolutionPolicy, message) => new TrackingResolver(trackingResolutionPolicy, message);

		private static readonly ILog _logger = LogManager.GetLogger(typeof(TrackingResolver));
		private readonly IBaseMessage _message;
		private readonly PolicyName _policyName;
		private bool _hasPolicyRun;
	}
}

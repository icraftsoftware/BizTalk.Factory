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
using SBMessaging;

namespace Be.Stateless.BizTalk.ContextProperties
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class SBMessagingProperties
	{
		public static readonly MessageContextProperty<ContentType, string> ContentType
			= new MessageContextProperty<ContentType, string>();

		public static readonly MessageContextProperty<CorrelationId, string> CorrelationId
			= new MessageContextProperty<CorrelationId, string>();

		public static readonly MessageContextProperty<CustomBrokeredMessagePropertyNamespace, string> CustomBrokeredMessagePropertyNamespace
			= new MessageContextProperty<CustomBrokeredMessagePropertyNamespace, string>();

		public static readonly MessageContextProperty<DeliveryCount, int> DeliveryCount
			= new MessageContextProperty<DeliveryCount, int>();

		public static readonly MessageContextProperty<EnqueuedTimeUtc, DateTime> EnqueuedTimeUtc
			= new MessageContextProperty<EnqueuedTimeUtc, DateTime>();

		public static readonly MessageContextProperty<ExpiresAtUtc, DateTime> ExpiresAtUtc
			= new MessageContextProperty<ExpiresAtUtc, DateTime>();

		public static readonly MessageContextProperty<Label, string> Label
			= new MessageContextProperty<Label, string>();

		public static readonly MessageContextProperty<MessageId, string> MessageId
			= new MessageContextProperty<MessageId, string>();

		public static readonly MessageContextProperty<ReplyTo, string> ReplyTo
			= new MessageContextProperty<ReplyTo, string>();

		public static readonly MessageContextProperty<ScheduledEnqueueTimeUtc, DateTime> ScheduledEnqueueTimeUtc
			= new MessageContextProperty<ScheduledEnqueueTimeUtc, DateTime>();

		public static readonly MessageContextProperty<TimeToLive, string> TimeToLive
			= new MessageContextProperty<TimeToLive, string>();

		public static readonly MessageContextProperty<To, string> To
			= new MessageContextProperty<To, string>();
	}
}

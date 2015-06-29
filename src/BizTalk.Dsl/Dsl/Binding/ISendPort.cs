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

using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.Dsl.Pipeline;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public interface ISendPort
	{
		bool IsTwoWay { get; }
	}

	public interface ISendPort<TNamingConvention> : ISendPort, IObjectBinding<TNamingConvention> where TNamingConvention : class
	{
		IApplicationBinding<TNamingConvention> ApplicationBinding { get; }

		SendPortTransport BackupTransport { get; }

		Filter Filter { get; set; }

		/// <summary>
		/// Whether to send messages in order of receipt.
		/// </summary>
		/// <remarks>
		/// <para>
		/// See also:
		/// <list type="bullet">
		/// <item>
		/// How to Configure Transport Advanced Options for a Send Port,
		/// https://msdn.microsoft.com/en-us/library/aa578109.aspx.
		/// </item>
		/// <item>
		/// SendPort Properties,
		/// https://msdn.microsoft.com/en-us/library/microsoft.biztalk.explorerom.sendport_properties.aspx.
		/// </item>
		/// </list>
		/// </para>
		/// </remarks>
		bool OrderedDelivery { get; }

		/// <summary>
		/// Priority of the resend attempt.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Internally, BizTalk Server assigns a priority to every subscription. Priority can be any number from 1
		/// (highest priority) to 10 (lowest priority). Because the default priority is 7 for activating subscriptions and
		/// 5 for correlating subscriptions, correlating messages will be delivered earlier than activating
		/// subscriptionsInternally, BizTalk Server assigns a priority to every subscription. Priority can be any number
		/// from 1 (<see cref="Binding.Priority.Highest"/> priority) to 10 (<see cref="Binding.Priority.Lowest"/>
		/// priority). Because the default priority is 7 for activating subscriptions and 5 for correlating subscriptions,
		/// correlating messages will be delivered earlier than activating subscriptions.
		/// </para>
		/// <para>
		/// See also:
		/// <list type="bullet">
		/// <item>
		/// How to Configure Transport Advanced Options for a Send Port,
		/// https://msdn.microsoft.com/en-us/library/aa578109.aspx.
		/// </item>
		/// </list>
		/// </para>
		/// </remarks>
		Priority Priority { get; }

		ReceivePipeline ReceivePipeline { get; set; }

		SendPipeline SendPipeline { get; set; }

		/// <summary>
		/// Whether to stop sending subsequent messages that follow a failed message when <see cref="OrderedDelivery"/>
		/// option is enabled.
		/// </summary>
		/// <remarks>
		/// <para>
		/// See also:
		/// <list type="bullet">
		/// <item>
		/// How to Configure Transport Advanced Options for a Send Port,
		/// https://msdn.microsoft.com/en-us/library/aa578109.aspx.
		/// </item>
		/// </list>
		/// </para>
		/// </remarks>
		bool StopSendingOnOrderedDeliveryFailure { get; }

		SendPortTransport Transport { get; }
	}
}

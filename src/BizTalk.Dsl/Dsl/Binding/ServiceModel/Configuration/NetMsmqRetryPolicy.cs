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

namespace Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration
{
	public class NetMsmqRetryPolicy
	{
		static NetMsmqRetryPolicy()
		{
			_default = new NetMsmqRetryPolicy { MaxRetryCycles = 2, ReceiveRetryCount = 5, RetryCycleDelay = TimeSpan.FromMinutes(10), TimeToLive = TimeSpan.FromDays(1) };
		}

		public static NetMsmqRetryPolicy Default
		{
			get { return _default; }
		}

		/// <summary>
		/// Gets or sets the maximum number of retry cycles to attempt delivery of messages to the receiving application.
		/// </summary>
		/// <returns>
		/// The maximum number of retry cycles to attempt prior to transferring a message to the poison-message queue.
		/// </returns>
		/// <remarks>
		/// It defaults to 2.
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/system.servicemodel.msmqbindingbase.maxretrycycles">MsmqBindingBase.MaxRetryCycles</seealso>
		public virtual int MaxRetryCycles { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of immediate retries that the queue manager should attempt if transmission of
		/// a message from the application queue to the application fails.
		/// </summary>
		/// <returns>
		/// The maximum number of times the queue manager should attempt to send a message before transferring it to the
		/// retry queue.
		/// </returns>
		/// <remarks>
		/// It defaults to 5.
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/system.servicemodel.msmqbindingbase.ReceiveRetryCount">MsmqBindingBase.ReceiveRetryCount</seealso>
		public virtual int ReceiveRetryCount { get; set; }

		/// <summary>
		/// Gets or sets a value that specifies how long to wait before attempting another retry cycle when attempting to
		/// deliver a message that could not be delivered.
		/// </summary>
		/// <returns>
		/// The <see cref="TimeSpan"/> that specifies the interval of time to wait before starting the next cycle of
		/// delivery attempts to the receiving application.
		/// </returns>
		/// <remarks>
		/// It defaults to 10 minutes.
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/system.servicemodel.msmqbindingbase.retrycycledelay">MsmqBindingBase.RetryCycleDelay</seealso>
		public virtual TimeSpan RetryCycleDelay { get; set; }

		/// <summary>
		/// Gets or sets a value that specifies how long messages are valid. When this time has elapsed, the message is
		/// placed in a dead-letter queue (if available).
		/// </summary>
		/// <returns>
		/// A <see cref="TimeSpan"/> value that specifies how long messages are valid.
		/// </returns>
		/// <remarks>
		/// It defaults to 1 day.
		/// </remarks>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/system.servicemodel.msmqbindingbase.timetolive">MsmqBindingBase.TimeToLive</seealso>
		public virtual TimeSpan TimeToLive { get; set; }

		private static readonly NetMsmqRetryPolicy _default;
	}
}

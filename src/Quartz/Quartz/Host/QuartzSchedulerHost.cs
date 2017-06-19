#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Threading;
using Be.Stateless.Quartz.Host.Core;
using Common.Logging;
using Quartz;
using Quartz.Impl;

namespace Be.Stateless.Quartz.Host
{
	/// <summary>
	/// Quartz Scheduler main host logic.
	/// </summary>
	[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
	public class QuartzSchedulerHost : IQuartzSchedulerHost
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QuartzSchedulerHost"/> class.
		/// </summary>
		public QuartzSchedulerHost()
		{
			_logger = LogManager.GetLogger(GetType());
		}

		#region IQuartzSchedulerHost Members

		/// <summary>
		/// Initializes the instance of the <see cref="QuartzSchedulerHost"/> class.
		/// </summary>
		public virtual void Initialize()
		{
			try
			{
				_schedulerFactory = CreateSchedulerFactory();
				_scheduler = GetScheduler();
			}
			catch (Exception e)
			{
				_logger.Error("Scheduler initialization failed:" + e.Message, e);
				throw;
			}
		}

		/// <summary>
		/// Starts this instance, delegates to scheduler.
		/// </summary>
		public virtual void Start()
		{
			_scheduler.Start();
			try
			{
				Thread.Sleep(3000);
			}
			catch (ThreadInterruptedException) { }
			_logger.Info("Scheduler started successfully.");
		}

		/// <summary>
		/// Stops this instance, delegates to scheduler.
		/// </summary>
		public virtual void Stop()
		{
			_scheduler.Shutdown(true);
			_logger.Info("Scheduler shutdown complete.");
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			// no-op for now
		}

		#endregion

		/// <summary>
		/// Returns the current scheduler instance (usually created in <see cref="Initialize" /> using the <see
		/// cref="GetScheduler" /> method).
		/// </summary>
		protected virtual IScheduler Scheduler
		{
			get { return _scheduler; }
		}

		/// <summary>
		/// Gets the scheduler with which this server should operate with.
		/// </summary>
		/// <returns></returns>
		protected virtual IScheduler GetScheduler()
		{
			return _schedulerFactory.GetScheduler();
		}

		/// <summary>
		/// Creates the scheduler factory that will be the factory
		/// for all schedulers on this instance.
		/// </summary>
		/// <returns></returns>
		protected virtual ISchedulerFactory CreateSchedulerFactory()
		{
			return new StdSchedulerFactory();
		}

		private readonly ILog _logger;
		private IScheduler _scheduler;
		private ISchedulerFactory _schedulerFactory;
	}
}

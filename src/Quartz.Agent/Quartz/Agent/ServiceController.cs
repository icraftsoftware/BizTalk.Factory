#region Copyright & License

// Copyright � 2012 - 2017 Fran�ois Chabot, Yves Dierick
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

using System.ServiceProcess;
using Be.Stateless.Quartz.Host;
using Be.Stateless.Quartz.Host.Core;
using Common.Logging;

namespace Be.Stateless.Quartz.Agent
{
	/// <summary>
	/// Main windows service to delegate calls to <see cref="IQuartzSchedulerHost" />.
	/// </summary>
	public class ServiceController : ServiceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceController"/> class.
		/// </summary>
		public ServiceController()
		{
			_logger = LogManager.GetLogger(GetType());

			_logger.Debug("Creating IQuartzSchedulerHost instance.");
			_host = QuartzSchedulerHostFactory.CreateHost();
			_logger.Debug("Initializing Quartz.NET Scheduler host");
			_host.Initialize();
			_logger.Debug("Quartz.NET Scheduler initialized");
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_logger.Debug("Disposing service");
				_host.Dispose();
				_logger.Debug("Service disposed");
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			_logger.Debug("Starting service");
			_host.Start();
			_logger.Debug("Service started");
		}

		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			_logger.Debug("Stopping service");
			_host.Stop();
			_logger.Debug("Service stopped");
		}

		#endregion

		private readonly IQuartzSchedulerHost _host;
		private readonly ILog _logger;
	}
}

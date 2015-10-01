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

using System.ServiceProcess;
using Common.Logging;
using Quartz.Server.Core;

namespace Quartz.Server
{
	/// <summary>
	/// Main windows service to delegate calls to <see cref="IQuartzServer" />.
	/// </summary>
	public class QuartzService : ServiceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QuartzService"/> class.
		/// </summary>
		public QuartzService()
		{
			_logger = LogManager.GetLogger(GetType());

			_logger.Debug("Obtaining instance of an IQuartzServer");
			_server = QuartzServerFactory.CreateServer();

			_logger.Debug("Initializing server");
			_server.Initialize();
			_logger.Debug("Server initialized");
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
				_server.Dispose();
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
			_server.Start();
			_logger.Debug("Service started");
		}

		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			_logger.Debug("Stopping service");
			_server.Stop();
			_logger.Debug("Service stopped");
		}

		#endregion

		private readonly ILog _logger;
		private readonly IQuartzServer _server;
	}
}

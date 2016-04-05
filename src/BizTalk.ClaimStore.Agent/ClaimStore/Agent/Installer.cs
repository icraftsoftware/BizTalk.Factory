#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace Be.Stateless.BizTalk.ClaimStore.Agent
{
	[RunInstaller(true)]
	public partial class Installer : System.Configuration.Install.Installer
	{
		public Installer()
		{
			InitializeComponent();

			_isServiceInstalled = System.ServiceProcess.ServiceController
				.GetServices()
				.Any(sc => sc.ServiceName == _serviceInstaller.ServiceName);

			BeforeInstall += SetupInstaller;
			AfterInstall += StartService;

			BeforeUninstall += StopService;
		}

		#region Base Class Member Overrides

		public override void Install(System.Collections.IDictionary stateSaver)
		{
			if (_isServiceInstalled)
			{
				// clearing Installers collection similarly to what is being done in Uninstall() does not work as base class
				// will commit the state anyway, which will be corrupted if the installers are skipped/have been cleared...
				Console.WriteLine("\r\nPerforming reinstallation of '{0}' service.", _serviceInstaller.ServiceName);
				// ReSharper disable AssignNullToNotNullAttribute
				base.Uninstall(null);
				// ReSharper restore AssignNullToNotNullAttribute
			}
			base.Install(stateSaver);
		}

		public override void Uninstall(System.Collections.IDictionary savedState)
		{
			if (!_isServiceInstalled)
			{
				Console.WriteLine("\r\nSkipping uninstall: '{0}' service is not installed.", _serviceInstaller.ServiceName);
				Installers.Clear();
			}
			base.Uninstall(savedState);
		}

		#endregion

		private void SetupInstaller(object sender, InstallEventArgs installEventArgs)
		{
			_serviceProcessInstaller.Account = ServiceAccount.User;
			_serviceProcessInstaller.Username = Context.Parameters["ServiceAccountName"];
			_serviceProcessInstaller.Password = Context.Parameters["ServiceAccountPassword"];
			switch (Context.Parameters["ServiceStartMode"].ToLowerInvariant())
			{
				case "a":
				case "auto":
				case "automatic":
					_serviceInstaller.StartType = ServiceStartMode.Automatic;
					break;
				case "disabled":
					_serviceInstaller.StartType = ServiceStartMode.Disabled;
					break;
				default:
					_serviceInstaller.StartType = ServiceStartMode.Manual;
					break;
			}
		}

		private void StartService(object sender, InstallEventArgs installEventArgs)
		{
			try
			{
				if (_serviceInstaller.StartType != ServiceStartMode.Automatic) return;
				using (var serviceController = new System.ServiceProcess.ServiceController(_serviceInstaller.ServiceName))
				{
					if (serviceController.Status == ServiceControllerStatus.Running) return;
					if (serviceController.Status == ServiceControllerStatus.StartPending) return;
					serviceController.Start();
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine("Warning! Could not start service with automatic starting mode after install:");
				Console.WriteLine(exception);
			}
		}

		private void StopService(object sender, InstallEventArgs installEventArgs)
		{
			try
			{
				if (!_isServiceInstalled) return;
				using (var serviceController = new System.ServiceProcess.ServiceController(_serviceInstaller.ServiceName))
				{
					serviceController.Stop();
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine("Warning! Could not stop running service before uninstalling it:");
				Console.WriteLine(exception);
			}
		}

		private readonly bool _isServiceInstalled;
	}
}

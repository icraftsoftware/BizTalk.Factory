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
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Quartz.Server
{
	/// <summary>
	/// Service installer for the Quartz server.
	/// </summary>
	[RunInstaller(true)]
	public class QuartzServiceInstaller : Installer
	{
		public QuartzServiceInstaller()
		{
			InitializeComponent();

			BeforeInstall += QuartzServiceInstallerBeforeInstall;
			BeforeUninstall += QuartzServiceInstallerBeforeUninstall;
			AfterInstall += QuartzServiceInstallerAfterInstall;

			_serviceInstaller.ServiceName = Configuration.ServiceName;
			_serviceInstaller.DisplayName = Configuration.ServiceDisplayName;
			_serviceInstaller.Description = Configuration.ServiceDescription;
		}

		private void QuartzServiceInstallerBeforeInstall(object sender, InstallEventArgs e)
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

		private void QuartzServiceInstallerBeforeUninstall(object sender, InstallEventArgs e)
		{
			_serviceProcessInstaller.Account = ServiceAccount.User;
			_serviceProcessInstaller.Username = Context.Parameters["ServiceAccountName"];
			_serviceProcessInstaller.Password = Context.Parameters["ServiceAccountPassword"];
			// stop a running service before uninstalling it
			try
			{
				using (var serviceController = new ServiceController(_serviceInstaller.ServiceName))
				{
					if (serviceController.Status == ServiceControllerStatus.Running)
					{
						serviceController.Stop();
					}
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine("Warning: could not stop running service before uninstalling it: " + exception);
			}
		}

		private void QuartzServiceInstallerAfterInstall(object sender, InstallEventArgs e)
		{
			if (_serviceInstaller.StartType == ServiceStartMode.Automatic)
			{
				try
				{
					using (var serviceController = new ServiceController(_serviceInstaller.ServiceName))
					{
						serviceController.Start();
					}
				}
				catch (Exception exception)
				{
					Console.WriteLine("Warning: could not start service set in automatic start mode after install: " + exception);
				}
			}
		}

		#region Component Designer generated code

		private void InitializeComponent()
		{
			this._serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this._serviceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// serviceProcessInstaller
			// 
			this._serviceProcessInstaller.Password = null;
			this._serviceProcessInstaller.Username = null;
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(
				new System.Configuration.Install.Installer[] {
					this._serviceProcessInstaller,
					this._serviceInstaller
				});
		}

		#endregion

		private ServiceInstaller _serviceInstaller;
		private ServiceProcessInstaller _serviceProcessInstaller;
	}
}

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
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using Be.Stateless.Quartz;
using Be.Stateless.Quartz.Server;

namespace Be.Stateless
{
	/// <summary>
	/// Summary description for Program.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		private static void Main()
		{
			// change from service account's dir to more logical one
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			if (Debugger.IsAttached)
			{
				var server = QuartzServerFactory.CreateServer();
				server.Initialize();
				server.Start();
				Console.WriteLine("Server started. Press <ENTER> to stop.");
				Console.ReadLine();
				server.Stop();
			}
			else
			{
				var servicesToRun = new ServiceBase[] { new QuartzService() };
				ServiceBase.Run(servicesToRun);
			}
		}
	}
}

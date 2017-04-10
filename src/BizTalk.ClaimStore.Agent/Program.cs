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
using System.Diagnostics.CodeAnalysis;
using System.ServiceProcess;
using Be.Stateless.BizTalk.ClaimStore;
using ServiceController = Be.Stateless.BizTalk.ClaimStore.Agent.ServiceController;

namespace Be.Stateless.BizTalk
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[SuppressMessage("ReSharper", "LocalizableElement")]
		private static void Main()
		{
			// avoid running as a service when debugging
			if (Debugger.IsAttached)
			{
				var collector = new MessageBodyCollector();
				collector.Start();
				Console.WriteLine("Press <ENTER> to stop...");
				Console.ReadLine();
				collector.Stop();
			}
			else
			{
				ServiceBase.Run(new ServiceController());
			}
		}
	}
}

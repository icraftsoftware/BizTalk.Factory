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
using Be.Stateless.Quartz.Configuration;
using Be.Stateless.Quartz.Server.Core;
using Common.Logging;

namespace Be.Stateless.Quartz.Server
{
	/// <summary>
	/// Factory class to create Quartz server implementations from.
	/// </summary>
	public class QuartzServerFactory
	{
		/// <summary>
		/// Creates a new instance of an Quartz.NET server core.
		/// </summary>
		/// <returns></returns>
		public static IQuartzServer CreateServer()
		{
			var typeName = QuartzConfigurationSection.ServerImplementationType;
			var type = Type.GetType(typeName, true);
			_logger.Debug("Creating new instance of server type '" + typeName + "'");
			var retValue = (IQuartzServer) Activator.CreateInstance(type);
			_logger.Debug("Instance successfully created");
			return retValue;
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(QuartzServerFactory));
	}
}

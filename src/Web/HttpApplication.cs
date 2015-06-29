#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using log4net.Config;

namespace Be.Stateless.Web
{
	public class HttpApplication : System.Web.HttpApplication
	{
		// use a static ctor to have this done *once* and *thread-safely*
		static HttpApplication()
		{
			// setup log4net at application start-up to prevent any log entry loss
			var fileName = HostingEnvironment.MapPath("~/log4net.config");
			if (fileName != null) XmlConfigurator.ConfigureAndWatch(new FileInfo(fileName));
			_logger = LogManager.GetLogger(typeof(HttpApplication));
		}

		protected virtual void Application_Start(object sender, EventArgs e)
		{
			_logger.InfoFormat("AppVDir '{1}' of site '{0}' is starting up.", HostingEnvironment.SiteName, HostingEnvironment.ApplicationVirtualPath);
		}

		protected virtual void Application_End(object sender, EventArgs e)
		{
			_logger.InfoFormat(
				"AppVDir '{1}' of site '{0}' is shutting down for the following reason {2}.",
				HostingEnvironment.SiteName,
				HostingEnvironment.ApplicationVirtualPath,
				HostingEnvironment.ShutdownReason);

			if (!_logger.IsDebugEnabled) return;

			// Logging ASP.NET Application Shutdown Events
			// see http://weblogs.asp.net/scottgu/archive/2005/12/14/433194.aspx
			// see http://blogs.msdn.com/tess/archive/2006/08/02/asp-net-case-study-lost-session-variables-and-appdomain-recycles.aspx
			// see http://blogs.msdn.com/toddca/archive/2005/12/01/499144.aspx
			// see http://blogs.msdn.com/toddca/archive/2006/07/17/668412.aspx
			// see http://weblogs.asp.net/owscott/archive/2006/02/21/ASP.NET-v2.0-_2D00_-AppDomain-recycles_2C00_-more-common-than-before.aspx
			var httpRuntimeType = typeof(HttpRuntime);
			var httpRuntime = (HttpRuntime) httpRuntimeType
				.GetField("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static)
				.IfNotNull(fi => fi.GetValue(null));
			if (httpRuntime == null) return;

			var shutDownMessage = (string) httpRuntimeType
				.GetField("_shutDownMessage", BindingFlags.NonPublic | BindingFlags.Instance)
				.IfNotNull(fi => fi.GetValue(httpRuntime));
			var shutDownStack = (string) httpRuntimeType
				.GetField("_shutDownStack", BindingFlags.NonPublic | BindingFlags.Instance)
				.IfNotNull(fi => fi.GetValue(httpRuntime));
			_logger.DebugFormat(
				"Extra AppDomain shutdown information. Message: {0}\r\nCall Stack: {1}",
				shutDownMessage,
				shutDownStack);
		}

		private static readonly ILog _logger;
	}
}

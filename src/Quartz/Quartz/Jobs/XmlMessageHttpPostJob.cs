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
using System.Net;
using System.Text;
using Be.Stateless.Logging;
using Quartz;

namespace Be.Stateless.Quartz.Jobs
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Quartz Job")]
	public class XmlMessageHttpPostJob : IJob
	{
		#region IJob Members

		public void Execute(IJobExecutionContext context)
		{
			try
			{
				var url = context.MergedJobDataMap.GetString(URL_KEY);
				var xmlMessage = context.MergedJobDataMap.GetString(XML_MESSAGE_KEY);
				var encodedXmlMessage = Encoding.UTF8.GetBytes(xmlMessage);
				using (var webClient = new WebClient())
				{
					webClient.Credentials = CredentialCache.DefaultNetworkCredentials;
					webClient.Headers.Add(HttpRequestHeader.ContentType, "text/xml; charset=utf-8");
					if (_logger.IsFineEnabled) _logger.FineFormat("About to post an XML message to the following URL: {0}.", url);
					webClient.UploadData(url, encodedXmlMessage);
				}
			}
			catch (Exception exception)
			{
				if (_logger.IsWarnEnabled) _logger.Warn("XmlMessageHttpPostJob has failed.", exception);
				throw;
			}
		}

		#endregion

		private const string URL_KEY = "url";
		private const string XML_MESSAGE_KEY = "xmlMessage";
		private static readonly ILog _logger = LogManager.GetLogger(typeof(XmlMessageHttpPostJob));
	}
}

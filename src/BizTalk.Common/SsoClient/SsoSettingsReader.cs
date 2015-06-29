#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using System.Collections;
using Be.Stateless.Logging;

namespace Be.Stateless.BizTalk.SsoClient
{
	/// <summary>
	/// Wraps a <see cref="SSOSettingsFileReader"/> for the sake of mocking and unit testing.
	/// </summary>
	public class SsoSettingsReader : ISsoSettingsReader
	{
		public static ISsoSettingsReader Instance
		{
			get { return _instance; }
			internal set { _instance = value; }
		}

		private SsoSettingsReader() { }

		#region ISsoSettingsReader Members

		public void ClearCache(string affiliateApplication)
		{
			SSOSettingsFileReader.ClearCache(affiliateApplication);
		}

		public Hashtable Read(string affiliateApplication)
		{
			return SSOSettingsFileReader.Read(affiliateApplication);
		}

		public int ReadInt32(string affiliateApplication, string valueName)
		{
			try
			{
				return SSOSettingsFileReader.ReadInt32(affiliateApplication, valueName);
			}
			catch (Exception exception)
			{
				if (_logger.IsWarnEnabled)
					_logger.Warn(string.Format("Cannot read int '{0}' from '{1}' application's SSO settings.", valueName, affiliateApplication), exception);
				throw;
			}
		}

		public string ReadString(string affiliateApplication, string valueName)
		{
			try
			{
				return SSOSettingsFileReader.ReadString(affiliateApplication, valueName);
			}
			catch (Exception exception)
			{
				if (_logger.IsWarnEnabled)
					_logger.Warn(string.Format("Cannot read string '{0}' from '{1}' application's SSO settings.", valueName, affiliateApplication), exception);
				throw;
			}
		}

		#endregion

		private static readonly ILog _logger = LogManager.GetLogger(typeof(SsoSettingsReader));
		private static ISsoSettingsReader _instance = new SsoSettingsReader();
	}
}

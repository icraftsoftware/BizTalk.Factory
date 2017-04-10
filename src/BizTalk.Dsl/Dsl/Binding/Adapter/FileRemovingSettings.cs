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

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class FileRemovingSettings
	{
		public FileRemovingSettings()
		{
			RetryCount = 5;
			RetryInterval = TimeSpan.FromMilliseconds(10);
			MaxRetryInterval = TimeSpan.FromMinutes(5);
		}

		/// <summary>
		/// Removing of files retry interval.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Specify the maximum retry interval time that the file adapter waits before attempting to delete a file that it
		/// has read and submitted to BizTalk Server.
		/// </para>
		/// <para>
		/// See also File Transport Properties Dialog Box, Receive, Advanced Settings Dialog Box,
		/// https://msdn.microsoft.com/en-us/library/aa559438.aspx.
		/// </para>
		/// </remarks>
		public TimeSpan MaxRetryInterval { get; set; }

		/// <summary>
		/// Removing of files retry count.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Specify the number of times that the file adapter will attempt to delete a file that it has read and submitted
		/// to BizTalk Server.
		/// </para>
		/// <para>
		/// See also File Transport Properties Dialog Box, Receive, Advanced Settings Dialog Box,
		/// https://msdn.microsoft.com/en-us/library/aa559438.aspx.
		/// </para>
		/// </remarks>
		public uint RetryCount { get; set; }

		/// <summary>
		/// Removing of files retry interval.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Specify the initial interval that the file adapter waits before attempting to delete a file that it has read
		/// and submitted to BizTalk Server. This interval will double after each retry interval up to the specified
		/// maximum retry interval value.
		/// </para>
		/// <para>
		/// See also File Transport Properties Dialog Box, Receive, Advanced Settings Dialog Box,
		/// https://msdn.microsoft.com/en-us/library/aa559438.aspx.
		/// </para>
		/// </remarks>
		public TimeSpan RetryInterval { get; set; }
	}
}

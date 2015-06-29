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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Be.Stateless.BizTalk.ClaimStore.States;
using Be.Stateless.Logging;

namespace Be.Stateless.BizTalk.ClaimStore
{
	internal static class MessageBodyEnumerable
	{
		/// <summary>
		/// Enumerates message bodies that are collectible, i.e. having either no lock or a lock that has expired/timed out.
		/// </summary>
		/// <param name="directories"></param>
		/// <param name="fileLockTimeout"></param>
		/// <returns></returns>
		public static IEnumerable<MessageBody> EnumerateMessageBodies(this IEnumerable<string> directories, TimeSpan fileLockTimeout)
		{
			var troublesomeLockTimeout = TimeSpan.FromMinutes(fileLockTimeout.TotalMinutes * 3);
			foreach (var messageBody in directories.EnumerateAllMessageBodies())
			{
				var lockTime = messageBody.DataFile.LockTime;
				var now = DateTime.UtcNow;
				var lockDuration = lockTime.HasValue ? now - lockTime.Value : TimeSpan.FromMinutes(0);
				if (lockDuration > troublesomeLockTimeout && _logger.IsWarnEnabled)
					_logger.WarnFormat(
						"Lock on message body's data file '{0}' has been acquired {1} minutes ago and ClaimStore.Agent was not able to pursue its processing since then.",
						messageBody.DataFile,
						lockDuration.TotalMinutes);

				var availabilityDuration = now - messageBody.DataFile.CreationTime;
				if (availabilityDuration.TotalHours > 12 && _logger.IsErrorEnabled)
					_logger.ErrorFormat(
						"Message body's data file '{0}' was captured {1} hours ago and and ClaimStore.Agent was not able to collect it since then.",
						messageBody.DataFile,
						availabilityDuration.TotalHours);

				// if just locked or lock timed out, i.e lock not acquired in time range ]0 ; FileLockTimeout], which means
				// the file might have been locked by a remote Claim Store Agent that is still busy working
				if (TimeSpan.FromMinutes(0) == lockDuration || lockDuration > fileLockTimeout)
				{
					yield return messageBody;
				}
				else if (_logger.IsDebugEnabled)
				{
					_logger.DebugFormat("Skipping locked message body's data file '{0}'.", messageBody.DataFile);
				}
			}
		}

		/// <summary>
		/// Gather message body data files to the central Claim Store.
		/// </summary>
		/// <param name="messageBodies"></param>
		/// <param name="gatheringDirectory"></param>
		public static void Collect(this IEnumerable<MessageBody> messageBodies, string gatheringDirectory)
		{
			foreach (var messageBody in messageBodies)
			{
				if (_logger.IsDebugEnabled) _logger.DebugFormat("Collecting message body's data file '{0}'.", messageBody.DataFile);
				messageBody.Collect(gatheringDirectory);
			}
		}

		private static IEnumerable<MessageBody> EnumerateAllMessageBodies(this IEnumerable<string> directories)
		{
			return directories.SelectMany(EnumerateFiles)
				.Where(filePath => filePath.IsValidDataFilePath())
				.Select(DataFile.Create)
				.Select(MessageBody.Create);
		}

		private static IEnumerable<string> EnumerateFiles(string path)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Enumerating message body's data files in folder '{0}'.", path);
			return _fileEnumerator(path, "*.*");
		}

		internal static Func<string, string, IEnumerable<string>> _fileEnumerator = (path, searchPattern) => Directory.EnumerateFiles(path, searchPattern);
		private static readonly ILogEx _logger = (ILogEx) LogManager.GetLogger(typeof(MessageBodyEnumerable));
	}
}

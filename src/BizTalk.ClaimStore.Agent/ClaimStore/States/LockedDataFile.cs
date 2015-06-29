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
using Be.Stateless.Extensions;
using Be.Stateless.Logging;

namespace Be.Stateless.BizTalk.ClaimStore.States
{
	internal class LockedDataFile : DataFile
	{
		public LockedDataFile(string filePath) : base(filePath)
		{
			var state = Path.Tokenize().State;
			if (state != STATE_TOKEN)
				throw new InvalidOperationException(
					string.Format(
						"{0} cannot handle a data file in the '{1}' state.",
						GetType().Name,
						state.IsNullOrEmpty() ? "undefined" : state));
		}

		#region Base Class Member Overrides

		internal override void Gather(MessageBody messageBody, string gatheringDirectory)
		{
			if (gatheringDirectory.IsNullOrEmpty()) throw new ArgumentNullException("gatheringDirectory");

			if (_logger.IsDebugEnabled) _logger.DebugFormat("Gathering {0}.", this);

			var claimStoreAbsolutePath = System.IO.Path.Combine(gatheringDirectory, ClaimStoreRelativePath);
			var gatheredFilePath = Path.Tokenize().UnlockedFilePath + "." + NewTimestamp() + "." + GatheredDataFile.STATE_TOKEN;
			// message body is not moved but copied to central claim store; it will be deleted only after it has been
			// successfully gathered and released
			var result = DataFileServant.Instance.TryCreateDirectory(claimStoreAbsolutePath)
				&& DataFileServant.Instance.TryCopyFile(Path, claimStoreAbsolutePath)
				&& DataFileServant.Instance.TryMoveFile(Path, gatheredFilePath);
			messageBody.DataFile = result
				? (DataFile) new GatheredDataFile(gatheredFilePath)
				// make sure data file will be unlocked and ownership relinquished so that it can be processed again during next collection
				: this;
		}

		internal override void Lock(MessageBody messageBody)
		{
			throw new InvalidOperationException();
		}

		internal override void Release(MessageBody messageBody)
		{
			throw new InvalidOperationException();
		}

		internal override void Unlock(MessageBody messageBody)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Unlocking {0}.", this);

			// try to revert file name to what it was before locking, i.e. remove .timestamp.locked extension from file
			// name to release exclusive ownership and reflect this file has yet to be processed
			var unlockedFilePath = Path.Tokenize().UnlockedFilePath;
			var result = DataFileServant.Instance.TryMoveFile(Path, unlockedFilePath);
			messageBody.DataFile = result
				? (DataFile) new UnlockedDataFile(unlockedFilePath)
				: new AwaitingRetryDataFile(Path);
		}

		#endregion

		internal const string STATE_TOKEN = "locked";
		private static readonly ILog _logger = LogManager.GetLogger(typeof(LockedDataFile));
	}
}

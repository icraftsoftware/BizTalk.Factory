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
using System.Transactions;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;

namespace Be.Stateless.BizTalk.ClaimStore.States
{
	internal class GatheredDataFile : DataFile
	{
		internal GatheredDataFile(string filePath) : base(filePath)
		{
			var state = Path.Tokenize().State;
			if (state != STATE_TOKEN)
				throw new InvalidOperationException(
					string.Format(
						"{0} cannot handle a data file in the '{1}' state.",
						GetType().Name,
						state.IsNullOrEmpty() ? "undefined" : state));
		}

		internal GatheredDataFile(DataFile dataFile) : base(dataFile.Path.NewNameForState(STATE_TOKEN)) { }

		#region Base Class Member Overrides

		internal override void Gather(MessageBody messageBody, string gatheringDirectory)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Skipping gathering transition of already gathered data file for {0}.", this);
		}

		internal override void Lock(MessageBody messageBody)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Locking {0}.", this);

			// take a new lock ---update timestamp while staying in gathered state--- so that this agent instance get
			// exclusive ownership of the data file should there be another remote agent instance working concurrently
			var gatheredDataFile = new GatheredDataFile(this);
			var result = DataFileServant.Instance.TryMoveFile(Path, gatheredDataFile.Path);
			messageBody.DataFile = result
				? (DataFile) gatheredDataFile
				: new AwaitingRetryDataFile(this);
		}

		internal override void Release(MessageBody messageBody)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Releasing {0}.", this);

			var releasedDataFile = new ReleasedDataFile(this);
			// release claim token from database and transition to ReleasedDataFile (i.e. rename data file) within a transaction scope
			using (var tx = new TransactionScope())
			{
				var result = DataFileServant.Instance.TryReleaseToken(ClaimStoreRelativePath)
					&& DataFileServant.Instance.TryMoveFile(Path, releasedDataFile.Path);
				if (result)
				{
					messageBody.DataFile = releasedDataFile;
					tx.Complete();
				}
				else
				{
					messageBody.DataFile = new AwaitingRetryDataFile(this);
				}
			}
		}

		internal override void Unlock(MessageBody messageBody)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Unlocking {0}.", this);

			// if gathered without needing to be released then data file has to be deleted
			var result = DataFileServant.Instance.TryDeleteFile(Path);
			if (!result) messageBody.DataFile = new AwaitingRetryDataFile(this);
		}

		#endregion

		internal const string STATE_TOKEN = "gathered";
		private static readonly ILog _logger = LogManager.GetLogger(typeof(GatheredDataFile));
	}
}

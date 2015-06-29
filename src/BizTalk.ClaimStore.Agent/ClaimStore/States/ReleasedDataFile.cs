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
	internal class ReleasedDataFile : DataFile
	{
		public ReleasedDataFile(string filePath) : base(filePath)
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
			throw new InvalidOperationException();
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

			// if gathered and released then data file has to be deleted
			var result = DataFileServant.Instance.TryDeleteFile(Path);
			if (!result) messageBody.DataFile = new AwaitingRetryDataFile(Path);
		}

		#endregion

		internal const string STATE_TOKEN = "released";
		private static readonly ILog _logger = LogManager.GetLogger(typeof(ReleasedDataFile));
	}
}

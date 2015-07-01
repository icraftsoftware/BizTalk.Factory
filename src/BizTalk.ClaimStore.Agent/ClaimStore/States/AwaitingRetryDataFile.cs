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

using Be.Stateless.Logging;

namespace Be.Stateless.BizTalk.ClaimStore.States
{
	/// <summary>
	/// </summary>
	/// <remarks>
	/// The purpose of the class is to stop any further state transition, so that the message body data file will try
	/// again to perform its failed operation when the Claim Store Agent will carry out
	///  its next collection.
	/// </remarks>
	internal class AwaitingRetryDataFile : DataFile
	{
		internal AwaitingRetryDataFile(DataFile dataFile) : base(dataFile.Path) { }

		#region Base Class Member Overrides

		internal override void Gather(MessageBody messageBody, string gatheringDirectory)
		{
			if (_logger.IsFineEnabled) _logger.FineFormat("Skipping gathering transition and awaiting retry for {0}.", this);
		}

		internal override void Lock(MessageBody messageBody)
		{
			if (_logger.IsFineEnabled) _logger.FineFormat("Skipping locking transition and awaiting retry for {0}.", this);
		}

		internal override void Release(MessageBody messageBody)
		{
			if (_logger.IsFineEnabled) _logger.FineFormat("Skipping releasing transition and awaiting retry for {0}.", this);
		}

		internal override void Unlock(MessageBody messageBody)
		{
			if (_logger.IsFineEnabled) _logger.FineFormat("Skipping unlocking transition and awaiting retry for {0}.", this);
		}

		#endregion

		private static readonly ILog _logger = LogManager.GetLogger(typeof(AwaitingRetryDataFile));
	}
}

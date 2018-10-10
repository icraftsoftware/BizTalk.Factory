#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using Be.Stateless.BizTalk.ClaimStore.States;

namespace Be.Stateless.BizTalk.ClaimStore
{
	internal abstract class MessageBody
	{
		internal static MessageBody Create(DataFile dataFile)
		{
			if (dataFile == null) throw new ArgumentNullException("dataFile");

			var trackingMode = dataFile.TrackingMode;
			if (trackingMode.Equals("chk", StringComparison.OrdinalIgnoreCase))
			{
				return new ClaimedMessageBody(dataFile);
			}
			if (trackingMode.Equals("trk", StringComparison.OrdinalIgnoreCase))
			{
				return new TrackedMessageBody(dataFile);
			}
			throw new ArgumentException(
				string.Format(
					"Claim Store Agent does not support the tracking mode '{0}' of message body data file name '{1}'.",
					trackingMode,
					dataFile),
				"dataFile");
		}

		protected internal MessageBody(DataFile dataFile)
		{
			DataFile = dataFile;
		}

		internal DataFile DataFile { get; set; }

		internal abstract void Collect(string gatheringDirectory);
	}
}

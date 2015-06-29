#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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

namespace Be.Stateless.BizTalk.Tracking
{
	public static class TrackingStatus
	{
		// ReSharper disable InconsistentNaming
		public const string Completed = "Completed";
		public const string Failed = "Failed";
		public const string FailedMessage = "FailedMessage";
		public const string Pending = "Pending";
		public const string Received = "Received";
		public const string Sent = "Sent";
	}
}

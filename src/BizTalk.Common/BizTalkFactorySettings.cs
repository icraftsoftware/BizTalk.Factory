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

using Be.Stateless.BizTalk.SsoClient;

namespace Be.Stateless.BizTalk
{
	/// <summary>
	/// Single access point for all BizTalk Factory settings coming either from the BizTalkFactoryMgmtDb or SSODB.
	/// </summary>
	/// <remarks>
	/// It's purpose is twofold: avoid the dissemination of magic stings all around, on one side, and support mocking and
	/// unit testing, on the other side.
	/// </remarks>
	public static class BizTalkFactorySettings
	{
		public static string ClaimStoreCheckInDirectory
		{
			get { return SsoSettingsReader.Instance.ReadString(AFFILIATE_APPLICATION_NAME, CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME); }
		}

		public static string ClaimStoreCheckOutDirectory
		{
			get { return SsoSettingsReader.Instance.ReadString(AFFILIATE_APPLICATION_NAME, CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME); }
		}

		internal const string AFFILIATE_APPLICATION_NAME = "BizTalk.Factory";
		internal const string CLAIM_STORE_CHECK_IN_DIRECTORY_PROPERTY_NAME = "ClaimStoreCheckInDirectory";
		internal const string CLAIM_STORE_CHECK_OUT_DIRECTORY_PROPERTY_NAME = "ClaimStoreCheckOutDirectory";
	}
}

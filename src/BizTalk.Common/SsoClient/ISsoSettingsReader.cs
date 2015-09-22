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

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Be.Stateless.BizTalk.SsoClient
{
	/// <summary>
	/// <see cref="SSOSettingsFileReader"/> indirection for the sake of mocking.
	/// </summary>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public interface ISsoSettingsReader
	{
		void ClearCache(string affiliateApplication);

		Hashtable Read(string affiliateApplication);

		int ReadInt32(string affiliateApplication, string valueName);

		string ReadString(string affiliateApplication, string valueName);
	}
}

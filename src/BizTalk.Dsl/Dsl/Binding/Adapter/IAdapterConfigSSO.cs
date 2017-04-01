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

using System.Diagnostics.CodeAnalysis;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Casing required by BizTalk Server's adapter serializtion.")]
	public interface IAdapterConfigSSO
	{
		#region Credentials Tab - User Name Credentials Settings

		/// <summary>
		/// Specify whether to use Enterprise Single Sign-On (SSO) to retrieve client credentials for authentication with
		/// the destination server.
		/// </summary>
		/// <remarks>
		/// It defaults to <c>False</c>.
		/// </remarks>
		[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Casing required by BizTalk Server's adapter serializtion.")]
		bool UseSSO { get; set; }

		#endregion
	}
}

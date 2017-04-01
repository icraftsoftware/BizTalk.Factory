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

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public interface IAdapterConfigOutboundCredentials : IAdapterConfigSSO
	{
		#region Credentials Tab - User Name Credentials Settings

		/// <summary>
		/// Specify the affiliate application to use for Enterprise Single Sign-On (SSO).
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="string.Empty"/>.
		/// </remarks>
		string AffiliateApplicationName { get; set; }

		/// <summary>
		/// Specify the password to use for authentication with the destination server when the <see
		/// cref="IAdapterConfigSSO.UseSSO"/> property is set to <c>False</c>.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="string.Empty"/>.
		/// </remarks>
		string Password { get; set; }

		/// <summary>
		/// Specify the user name to use for authentication with the destination server when the <see
		/// cref="IAdapterConfigSSO.UseSSO"/> property is set to <c>False</c>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// You do not have to use the domain\user format for this property.
		/// </para>
		/// <para>
		/// It defaults to <see cref="string.Empty"/>.
		/// </para>
		/// </remarks>
		string UserName { get; set; }

		#endregion
	}
}

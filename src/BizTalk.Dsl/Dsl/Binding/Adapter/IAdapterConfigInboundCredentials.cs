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

using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public interface IAdapterConfigInboundCredentials
	{
		#region Other Tab - Credentials Settings

		/// <summary>
		/// Specify the SSO affiliate application to return external credentials to be used when this receive location
		/// sends solicit messages to poll an external service. The specified SSO affiliate application must have a
		/// mapping between the Windows account under which this receive location runs and the account for the external
		/// service. This property is required when the <see cref="CredentialType"/> property is set to <see
		/// cref="CredentialSelection.GetCredentials"/>.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="string.Empty"/>.
		/// </remarks>
		string AffiliateApplicationName { get; set; }

		/// <summary>
		/// Specify the type of credentials for this receive location to use when polling an external service.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <list type="bullet">
		/// <item>
		/// <see cref="CredentialSelection.None"/> &#8212; Do not use any credentials when this receive location sends
		/// solicit messages to poll an external service, or this receive location does not need to poll any external
		/// service.
		/// </item>
		/// <item>
		/// <see cref="CredentialSelection.IssueTicket"/> &#8212; Use Enterprise Single Sign-On (SSO) to retrieve
		/// client credentials to issue an SSO ticket. This option requires using the security mode that allows this
		/// receive location to impersonate the user account to issue an SSO ticket.
		/// </item>
		/// <item>
		/// <see cref="CredentialSelection.UserAccount"/> &#8212; Use the credentials specified in the <see
		/// cref="UserName"/> and <see cref="Password"/> properties when this receive location sends solicit messages
		/// to poll an external service.
		/// </item>
		/// <item>
		/// <see cref="CredentialSelection.GetCredentials"/> &#8212; Use the SSO affiliate application specified in the
		/// <see cref="AffiliateApplicationName"/> property when this receive location sends solicit messages to poll
		/// an external service.
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// It defaults to <see cref="CredentialSelection.None"/>.
		/// </para>
		/// </remarks>
		CredentialSelection CredentialType { get; set; }

		/// <summary>
		/// Specify the password for this receive location to use when polling an external service to retrieve response
		/// messages. This property is required when the <see cref="CredentialType"/> property is set to <see
		/// cref="CredentialSelection.UserAccount"/>.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="string.Empty"/>.
		/// </remarks>
		string Password { get; set; }

		/// <summary>
		/// Specify the user name for this receive location to use when polling an external service to retrieve
		/// response messages. This property is required when the <see cref="CredentialType"/> property is set to <see
		/// cref="CredentialSelection.UserAccount"/>.
		/// </summary>
		/// <remarks>
		/// It defaults to <see cref="string.Empty"/>.
		/// </remarks>
		string UserName { get; set; }

		#endregion
	}
}

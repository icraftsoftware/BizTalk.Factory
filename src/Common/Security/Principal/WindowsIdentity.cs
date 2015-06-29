#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Be.Stateless.Security.Principal
{
	internal enum LogonProvider
	{
		// ReSharper disable InconsistentNaming
		Default = 0,
		WinNT35 = 1,
		WinNT40 = 2,
		WinNT50 = 3
		// ReSharper restore InconsistentNaming
	}

	internal enum LogonType
	{
		Interactive = 2,
		Network = 3,
		Batch = 4,
		Service = 5,
		Unlock = 7,
		NetworkClearText = 8,
		NewCredentials = 9
	}

	internal class ImpersonationContext : IDisposable
	{
		public ImpersonationContext(string username, string domain, string password)
		{
			var result = LogonUser(username, domain, password, (int) LogonType.NewCredentials, (int) LogonProvider.WinNT50, out _safeTokenHandle);
			if (!result) throw new Win32Exception(Marshal.GetLastWin32Error());
			_windowsImpersonationContext = new System.Security.Principal.WindowsIdentity(_safeTokenHandle.DangerousGetHandle()).Impersonate();
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_windowsImpersonationContext != null) _windowsImpersonationContext.Dispose();
			if (_safeTokenHandle != null) _safeTokenHandle.Dispose();
			GC.SuppressFinalize(this);
		}

		#endregion

		~ImpersonationContext()
		{
			Dispose();
		}

		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern bool LogonUser(
			string lpszUsername,
			string lpszDomain,
			string lpszPassword,
			int dwLogonType,
			int dwLogonProvider,
			out SafeTokenHandle phToken);

		private readonly SafeTokenHandle _safeTokenHandle;
		private readonly WindowsImpersonationContext _windowsImpersonationContext;
	}

	/// <summary>
	/// </summary>
	/// <seealso href="http://msdn.microsoft.com/en-us/library/system.security.principal.windowsimpersonationcontext.aspx"/>
	/// <seealso href="http://www.cstruter.com/blog/270"/>
	internal class WindowsIdentity
	{
		public WindowsIdentity(string userName, string password)
		{
			UserName = userName;
			Password = password;
		}

		public string UserName { get; set; }
		public string Password { get; set; }

		#region CredUIParseUserName
		// ReSharper disable InconsistentNaming

		/// <summary>
		/// http://www.pinvoke.net/default.aspx/Enums.CredUIReturnCodes
		/// </summary>
		private enum CredUIReturnCode
		{
			NO_ERROR = 0,
			ERROR_INVALID_PARAMETER = 87,
			ERROR_INSUFFICIENT_BUFFER = 122,
			ERROR_BAD_ARGUMENTS = 160,
			ERROR_INVALID_FLAGS = 1004,
			ERROR_NOT_FOUND = 1168,
			ERROR_CANCELLED = 1223,
			ERROR_NO_SUCH_LOGON_SESSION = 1312,
			ERROR_INVALID_ACCOUNT_NAME = 1315
		}

		/// <summary>
		/// http://www.pinvoke.net/default.aspx/credui/CredUIParseUserName.html
		/// </summary>
		[DllImport("credui.dll", EntryPoint = "CredUIParseUserNameW", CharSet = CharSet.Unicode)]
		private static extern CredUIReturnCode CredUIParseUserName(
			string userName,
			StringBuilder user,
			int userMaxChars,
			StringBuilder domain,
			int domainMaxChars);

		// ReSharper restore InconsistentNaming
		#endregion

		/// <summary>
		/// Extracts the domain and user account name from a fully qualified user name.
		/// </summary>
		/// <param name="username">
		/// A <see cref="string"/> that contains the user name to be parsed. The name must be in UPN or down-level format,
		/// or a certificate.
		/// </param>
		/// <param name="user">
		/// A <see cref="string"/> that receives the user account name.
		/// </param>
		/// <param name="domain">
		/// A <see cref="string"/> that receives the domain name. If <paramref name="username"/> specifies a certificate,
		/// pszDomain will be <see langword="null"/>.
		/// </param>
		/// <returns>
		/// <see langword="true"/> if the <paramref name="username"/> contains a domain and a user-name; otherwise, <see
		/// langword="false"/>.
		/// </returns>
		/// <seealso href="http://www.pinvoke.net/default.aspx/credui.creduiparseusername"/>
		public static void ParseUserName(string username, out string user, out string domain)
		{
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username");

			var userBuilder = new StringBuilder();
			var domainBuilder = new StringBuilder();
			var code = CredUIParseUserName(username, userBuilder, int.MaxValue, domainBuilder, int.MaxValue);
			switch (code)
			{
				case CredUIReturnCode.NO_ERROR:
					user = userBuilder.ToString();
					domain = domainBuilder.ToString();
					break;

				case CredUIReturnCode.ERROR_CANCELLED:
					throw new Exception("Canceled.");

				case CredUIReturnCode.ERROR_NO_SUCH_LOGON_SESSION:
					throw new Exception("No such logon session.");

				case CredUIReturnCode.ERROR_NOT_FOUND:
					throw new Exception("Not found.");

				case CredUIReturnCode.ERROR_INVALID_ACCOUNT_NAME:
					throw new Exception(string.Format("Invalid account name ({0}).", username));

				case CredUIReturnCode.ERROR_INSUFFICIENT_BUFFER:
					throw new Exception("Insufficient buffer.");

				case CredUIReturnCode.ERROR_BAD_ARGUMENTS:
					throw new Exception("Bad arguments.");

				case CredUIReturnCode.ERROR_INVALID_PARAMETER:
					throw new Exception("Invalid parameter.");

				case CredUIReturnCode.ERROR_INVALID_FLAGS:
					throw new Exception("Invalid flags.");

				default:
					throw new Exception("Unknown credential result encountered.");
			}
		}

		public IDisposable Impersonate()
		{
			string username, domain;
			ParseUserName(UserName, out username, out domain);
			return new ImpersonationContext(username, domain, Password);
		}
	}
}
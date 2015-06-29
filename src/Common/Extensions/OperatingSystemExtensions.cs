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

namespace Be.Stateless.Extensions
{
	public static class OperatingSystemExtensions
	{
		public static bool SupportTransactionalFileSystem(this OperatingSystem system)
		{
			// TODO rewrite as GetVolumeInformation, see http://msdn.microsoft.com/en-us/library/windows/desktop/aa364993(v=vs.85).aspx
			// If the volume supports file system transactions, the function returns FILE_SUPPORTS_TRANSACTIONS in lpFileSystemFlags
			// Transactional NTFS (TxF) is supported starting with Vista
			return system.Platform == PlatformID.Win32NT && system.Version.Major >= 6;
		}
	}
}

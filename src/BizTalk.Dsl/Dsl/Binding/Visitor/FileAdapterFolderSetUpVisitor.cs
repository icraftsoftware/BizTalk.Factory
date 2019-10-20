﻿#region Copyright & License

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

using System;
using System.IO;
using System.Security.AccessControl;
using Be.Stateless.Logging;
using Path = Be.Stateless.IO.Path;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	/// <summary>
	/// <see cref="IApplicationBindingVisitor"/> implementation that sets up file adapters' physical paths.
	/// </summary>
	public class FileAdapterFolderSetUpVisitor : FileAdapterFolderVisitorBase
	{
		public static IApplicationBindingVisitor Create(string[] users)
		{
			if (users == null) throw new ArgumentNullException("users");
			return new FileAdapterFolderSetUpVisitor(users);
		}

		private FileAdapterFolderSetUpVisitor(string[] users)
		{
			_users = users;
		}

		#region Base Class Member Overrides

		protected override void VisitDirectory(string path)
		{
			_logger.InfoFormat("Setting up directory '{0}'.", path);
			CreateDirectory(path);
			SecureDirectory(path);
		}

		#endregion

		private void CreateDirectory(string path)
		{
			if (Directory.Exists(path))
			{
				_logger.InfoFormat("Directory '{0}' already exists.", path);
				return;
			}

			try
			{
				Directory.CreateDirectory(path);
				_logger.InfoFormat("Created directory '{0}'.", path);
			}
			catch (Exception exception)
			{
				_logger.WarnFormat(string.Format("Could not create directory '{0}'.", path), exception);
			}
		}

		private void SecureDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				_logger.InfoFormat("Cannot grant permissions because directory '{0}' does not exist.", path);
				return;
			}

			if (Path.IsNetworkPath(path))
			{
				_logger.InfoFormat("Cannot grant permissions because directory '{0}' is a network path.", path);
				return;
			}

			foreach (var user in _users)
			{
				try
				{
					var acl = Directory.GetAccessControl(path);
					acl.AddAccessRule(
						new FileSystemAccessRule(
							user,
							FileSystemRights.FullControl,
							InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
							PropagationFlags.None,
							AccessControlType.Allow));
					Directory.SetAccessControl(path, acl);
					_logger.InfoFormat("Granted Full Control permission to '{0}' on directory '{1}'.", user, path);
				}
				catch (Exception exception)
				{
					_logger.WarnFormat(string.Format("Could not grant Full Control permission to '{0}' on directory '{1}'.", user, path), exception);
				}
			}
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(FileAdapterFolderSetUpVisitor));
		private readonly string[] _users;
	}
}

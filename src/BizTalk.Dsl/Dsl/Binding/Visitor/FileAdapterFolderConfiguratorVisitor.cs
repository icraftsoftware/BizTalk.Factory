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

using System;
using System.IO;
using System.Security.AccessControl;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Path = Be.Stateless.IO.Path;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	public class FileAdapterFolderConfiguratorVisitor : IApplicationBindingVisitor
	{
		public static FileAdapterFolderConfiguratorVisitor CreateInstaller(string targetEnvironment, string[] users)
		{
			if (targetEnvironment.IsNullOrEmpty()) throw new ArgumentNullException("targetEnvironment");
			if (users == null) throw new ArgumentNullException("users");
			return new FileAdapterFolderConfiguratorVisitor(targetEnvironment, users);
		}

		public static IApplicationBindingVisitor CreateUninstaller(string targetEnvironment, bool recurse)
		{
			if (targetEnvironment.IsNullOrEmpty()) throw new ArgumentNullException("targetEnvironment");
			return new FileAdapterFolderConfiguratorVisitor(targetEnvironment, recurse);
		}

		private FileAdapterFolderConfiguratorVisitor(string environment, string[] users)
		{
			_operation = SetupDirectory;
			_environment = environment;
			_users = users;
		}

		private FileAdapterFolderConfiguratorVisitor(string environment, bool recurse)
		{
			_operation = path => TeardownDirectory(path, recurse);
			_environment = environment;
		}

		#region IApplicationBindingVisitor Members

		public void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding) where TNamingConvention : class
		{
			((ISupportEnvironmentOverride) applicationBinding).ApplyEnvironmentOverrides(_environment);
		}

		public void VisitOrchestration(IOrchestrationBinding orchestrationBinding) { }

		public void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort) where TNamingConvention : class
		{
			// TODO ?? remove this test and check if there are any Receive Location to deploy for the environment ??
			if (((ISupportEnvironmentDeploymentPredicate) receivePort).IsDeployableForEnvironment(_environment))
			{
				((ISupportEnvironmentOverride) receivePort).ApplyEnvironmentOverrides(_environment);
			}
		}

		public void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation) where TNamingConvention : class
		{
			if (((ISupportEnvironmentDeploymentPredicate) receiveLocation).IsDeployableForEnvironment(_environment))
			{
				((ISupportEnvironmentOverride) receiveLocation).ApplyEnvironmentOverrides(_environment);
				var fileAdapter = receiveLocation.Transport.Adapter as FileAdapter.Inbound;
				if (fileAdapter != null) _operation(fileAdapter.ReceiveFolder);
			}
		}

		public void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort) where TNamingConvention : class
		{
			if (((ISupportEnvironmentDeploymentPredicate) sendPort).IsDeployableForEnvironment(_environment))
			{
				((ISupportEnvironmentOverride) sendPort).ApplyEnvironmentOverrides(_environment);
				var fileAdapter = sendPort.Transport.Adapter as FileAdapter.Outbound;
				if (fileAdapter != null) _operation(fileAdapter.DestinationFolder);
			}
		}

		#endregion

		#region Directory setup

		private void SetupDirectory(string path)
		{
			_logger.InfoFormat("Setting up directory '{0}'.", path);
			CreateDirectory(path);
			SecureDirectory(path);
		}

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

		#endregion

		#region Directory teardown

		private void TeardownDirectory(string path, bool recurse)
		{
			_logger.InfoFormat("Tearing down directory '{0}'.", path);
			try
			{
				Directory.Delete(path, recurse);
				_logger.InfoFormat("Deleted directory '{0}'.", path);
			}
			catch (Exception exception)
			{
				_logger.WarnFormat(string.Format("Could not delete directory '{0}'.", path), exception);
			}
		}

		#endregion

		private static readonly ILog _logger = LogManager.GetLogger(typeof(FileAdapterFolderConfiguratorVisitor));

		private readonly string _environment;
		private readonly Action<string> _operation;
		private readonly string[] _users;
	}
}

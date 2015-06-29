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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using Be.Stateless.Logging;
using Path = System.IO.Path;

namespace Be.Stateless.BizTalk.ClaimStore.States
{
	// virtual members intended for mocking
	// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
	internal class DataFileServant
	{
		public static DataFileServant Instance
		{
			get { return _instance; }
			internal set { _instance = value; }
		}

		#region File System Operations

		internal virtual bool TryCreateDirectory(string filePath)
		{
			if (filePath.IsNullOrEmpty()) throw new ArgumentNullException("filePath");
			var targetDirectory = Path.GetDirectoryName(filePath);
			return TryIoOperation(
				// ReSharper disable AssignNullToNotNullAttribute
				() => Directory.CreateDirectory(targetDirectory),
				string.Format("Failed to create directory '{0}'.", targetDirectory));
		}

		internal virtual bool TryCopyFile(string sourceFilePath, string targetFilePath)
		{
			if (sourceFilePath.IsNullOrEmpty()) throw new ArgumentNullException("sourceFilePath");
			if (targetFilePath.IsNullOrEmpty()) throw new ArgumentNullException("targetFilePath");
			return TryIoOperation(
				() => File.Copy(sourceFilePath, targetFilePath, true),
				string.Format("Failed to copy file from '{0}' to '{1}'.", sourceFilePath, targetFilePath));
		}

		internal virtual bool TryDeleteFile(string filePath)
		{
			if (filePath.IsNullOrEmpty()) throw new ArgumentNullException("filePath");
			return TryIoOperation(
				() => File.Delete(filePath),
				string.Format("Failed to delete file '{0}'.", filePath));
		}

		internal virtual bool TryMoveFile(string sourceFilePath, string targetFilePath)
		{
			if (sourceFilePath.IsNullOrEmpty()) throw new ArgumentNullException("sourceFilePath");
			if (targetFilePath.IsNullOrEmpty()) throw new ArgumentNullException("targetFilePath");

			var transaction = Transaction.Current;
			if (transaction == null)
			{
				return TryIoOperation(
					() => File.Move(sourceFilePath, targetFilePath),
					string.Format("Failed to move file from '{0}' to '{1}'.", sourceFilePath, targetFilePath));
			}

			// ReSharper disable once SuspiciousTypeConversion.Global
			var kernelTransaction = (IKernelTransaction) TransactionInterop.GetDtcTransaction(transaction);
			return TryIoOperation(
				() => FileTransacted.Move(sourceFilePath, targetFilePath, kernelTransaction),
				string.Format("Failed to transactionally move file from '{0}' to '{1}'.", sourceFilePath, targetFilePath));
		}

		private bool TryIoOperation(Action operation, string message)
		{
			// resilient to IOException and UnauthorizedAccessException but not to any other exception
			try
			{
				operation();
				return true;
			}
			catch (IOException exception)
			{
				if (_logger.IsWarnEnabled) _logger.Warn(message, exception);
				return false;
			}
			catch (UnauthorizedAccessException exception)
			{
				if (_logger.IsWarnEnabled) _logger.Warn(message, exception);
				return false;
			}
		}

		#endregion

		#region Database Operations

		internal virtual bool TryReleaseToken(string token)
		{
			if (token.IsNullOrEmpty()) throw new ArgumentNullException("token");
			return TrySqlOperation(
				() => {
					using (var cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["TransientStateDb"].ConnectionString))
					using (var cmd = new SqlCommand("usp_claim_Release", cnx))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@url", token);
						cnx.Open();
						var rowsAffected = cmd.ExecuteNonQuery();
						return rowsAffected == 1;
					}
				},
				string.Format("Failed to release claim check token '{0}'.", token));
		}

		private bool TrySqlOperation(Func<bool> operation, string message)
		{
			// tolerant to SqlException but let through any other exception
			try
			{
				return operation();
			}
			catch (SqlException exception)
			{
				if (_logger.IsWarnEnabled) _logger.Warn(message, exception);
				return false;
			}
		}

		#endregion

		private static DataFileServant _instance = new DataFileServant();
		private static readonly ILog _logger = LogManager.GetLogger(typeof(MessageBody));
	}
}

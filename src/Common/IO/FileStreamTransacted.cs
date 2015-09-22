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
using System.Runtime.InteropServices;
using System.Transactions;
using Be.Stateless.Logging;

namespace Be.Stateless.IO
{
	/// <summary>
	/// <see cref="FileStream"/> subclass to be used when the underlying stream is transacted and its scoping transaction
	/// requires and explicit commit because it is not DTC enlisted.
	/// </summary>
	public class FileStreamTransacted : FileStream, IStreamTransacted
	{
		#region TransactionState Enum

		private enum TransactionState
		{
			Completed,
			Faulted,
			Running
		}

		#endregion

		internal FileStreamTransacted(IntPtr transactionHandle, string path, int bufferSize)
			: base(FileTransacted.CreateFileTransactedHandle(transactionHandle, path), FileAccess.Write, bufferSize)
		{
			if (_logger.IsDebugEnabled) _logger.DebugFormat("Transacted file stream created for writing at path '{0}'.", path);
			_transactionHandle = transactionHandle;
		}

		#region IStreamTransacted Members

		public void Commit()
		{
			_state = TransactionState.Completed;
			Close();
		}

		public void Rollback()
		{
			_state = TransactionState.Faulted;
			Close();
		}

		#endregion

		#region Base Class Member Overrides

		/// <summary>
		/// Note that because the base class does mix the Close() and Dispose() operations, one can commit the transaction
		/// if properly closed, and rollback it if disposed...
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			// <see href="http://msdn.microsoft.com/en-us/magazine/cc163392.aspx"/>
			// <see href="http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P"/>

			if (_logger.IsDebugEnabled) _logger.DebugFormat("FileStreamTransacted is {0}.", disposing ? "disposing" : "finalizing");

			// stream has to flushed and closed prior to transaction completion
			base.Dispose(disposing);

			// clean up managed resources and ensure transaction gets completed
			if (disposing && !Completed)
			{
				if (_state == TransactionState.Completed) CommitTransactionCore();
				else RollbackTransactionCore();
			}

			// clean up native resources
			if (!Completed)
			{
				if (_logger.IsWarnEnabled) _logger.Warn("Finalizer with a transaction yet unresolved, that is neither committed nor rolled back.");
				var result = CloseHandle(_transactionHandle);
				if (!result && _logger.IsWarnEnabled) _logger.WarnFormat("Cannot close kernel transaction handle in finalizer. Win32 error code: {0}.", Marshal.GetLastWin32Error());
				_transactionHandle = IntPtr.Zero;
			}
		}

		#endregion

		private bool Completed
		{
			get { return _transactionHandle == IntPtr.Zero; }
		}

		private void CommitTransactionCore()
		{
			if (Completed) return;
			if (_logger.IsDebugEnabled) _logger.Debug("Committing FileStreamTransacted...");
			var result = CommitTransaction(_transactionHandle);
			if (!result) throw new TransactionException(string.Format("Cannot commit kernel transaction. Win32 error code: {0}.", Marshal.GetLastWin32Error()));
			result = CloseHandle(_transactionHandle);
			if (!result && _logger.IsWarnEnabled) _logger.WarnFormat("Cannot close kernel transaction handle. Win32 error code: {0}.", Marshal.GetLastWin32Error());
			_transactionHandle = IntPtr.Zero;
		}

		private void RollbackTransactionCore()
		{
			if (Completed) return;
			if (_logger.IsDebugEnabled) _logger.Debug("Rolling back FileStreamTransacted...");
			var result = RollbackTransaction(_transactionHandle);
			if (!result)
			{
				var lastWin32Error = Marshal.GetLastWin32Error();
				if (_logger.IsWarnEnabled) _logger.WarnFormat("Cannot rollback kernel transaction. Win32 error code: {0}.", lastWin32Error);
				throw new TransactionException(string.Format("Could not commit a manually managed kernel transaction. Win32 error code: {0}.", lastWin32Error));
			}
			result = CloseHandle(_transactionHandle);
			if (!result && _logger.IsWarnEnabled) _logger.WarnFormat("Cannot close kernel transaction handle. Win32 error code: {0}.", Marshal.GetLastWin32Error());
			_transactionHandle = IntPtr.Zero;
		}

		#region External Imports

		[DllImport("Kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr handle);

		[DllImport("Ktmw32.dll", SetLastError = true)]
		private static extern bool CommitTransaction(IntPtr transaction);

		[DllImport("Ktmw32.dll", SetLastError = true)]
		private static extern bool RollbackTransaction(IntPtr transaction);

		#endregion

		private static readonly ILog _logger = LogManager.GetLogger(typeof(FileStreamTransacted));
		private TransactionState _state = TransactionState.Running;
		private IntPtr _transactionHandle;
	}
}

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
using Be.Stateless.Extensions;
using Be.Stateless.Logging;
using Microsoft.Win32.SafeHandles;

namespace Be.Stateless.IO
{
	/// <summary>
	/// Provides static methods for the creation of transactional <see cref="FileStream"/> or <see
	/// cref="FileStreamTransacted"/> objects.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Microsoft strongly recommends developers utilize alternative means to achieve your application’s needs. Many
	/// scenarios that TxF was developed for can be achieved through simpler and more readily available techniques.
	/// Furthermore, TxF may not be available in future versions of Microsoft Windows. For more information, and
	/// alternatives to TxF, please see <a
	/// href="https://msdn.microsoft.com/en-us/library/windows/desktop/hh802690(v=vs.85).aspx">Alternatives to using
	/// Transactional NTFS</a>.
	/// </para>
	/// <para>
	/// <a href="https://transactionalfilemgr.codeplex.com/">.NET Transactional File Manager</a>.
	/// </para>
	/// </remarks>
	public static class FileTransacted
	{
		/// <summary>
		/// Creates a new file in the specified path.
		/// </summary>
		/// <param name="path">
		/// The path and name of the file to create.
		/// </param>
		/// <param name="bufferSize">
		/// The number of bytes buffered for writes to the file. It defaults to 4KB.
		/// </param>
		/// <param name="transaction">
		/// An <see cref="IKernelTransaction"/> transaction that is already enlisted with a DTC, or <c>null</c> otherwise.
		/// It defaults to <c>null</c>.
		/// </param>
		/// <returns>
		/// A <see cref="FileStream"/> or <see cref="FileStreamTransacted"/>, depending on whether the transaction has to
		/// be explicitly (i.e. not DTC enlisted) managed or not, with the specified buffer size that provides write
		/// access to the file specified in path.
		/// </returns>
		/// <remarks>
		/// <para>
		/// Notice that a <see cref="FileStream"/> can only be transactional if both the file system supports transaction
		/// &#8212; <see cref="OperatingSystemExtensions"/>.<see
		/// cref="OperatingSystemExtensions.SupportTransactionalFileSystem"/> &#8212; and the file is not created in a
		/// network volume &#8212; <see cref="Path"/>.<see cref="Path.IsNetworkPath"/>. For these reasons, the <see
		/// cref="FileStream"/> created may not be transactional at all if one of these transactional requirements is not
		/// met.
		/// </para>
		/// <para>
		/// If both transactional requirements are met and a <c>null</c> <paramref name="transaction"/> has been passed,
		/// the stream created will be a <see cref="FileStreamTransacted"/> instance. In this case, the client will be in
		/// charge of <i>explicitly</i> managing the transaction outcome, that is either <see
		/// cref="FileStreamTransacted.Commit"/> or <see cref="FileStreamTransacted.Rollback"/>.
		/// </para>
		/// <para>
		/// If both transactional requirements are met and a non <c>null</c> <paramref name="transaction"/> has been
		/// passed, the stream created will be a regular <see cref="FileStream"/> instance. However it will have been
		/// created in such a way that the <see cref="FileStream"/> piggybacks the <paramref name="transaction"/> already
		/// enlisted with a DTC. <b>It is therefore the responsibility of the client to ensure that the non <c>null</c>
		/// <paramref name="transaction"/> is indeed correctly enlisted with a DTC; the client would have no way to
		/// determine the outcome of the transaction otherwise.</b>
		/// </para>
		/// <para>
		/// If not both transactional requirements are met, and irrelevantly of the passed <paramref name="transaction"/>,
		/// the stream created will always be a regular <see cref="FileStream"/> instance with no underlying transaction.
		/// The client has therefore no way of distinguishing a transactional <see cref="FileStream"/> enlisted with a DTC
		/// from non transactional one.
		/// </para>
		/// </remarks>
		/// <seealso cref="OperatingSystemExtensions"/>
		/// <seealso cref="OperatingSystemExtensions.SupportTransactionalFileSystem"/>
		public static FileStream Create(string path, int bufferSize = 4 * 1024, IKernelTransaction transaction = null)
		{
			if (path.IsNullOrEmpty()) throw new ArgumentNullException("path");
			var fileStream = !_operatingSystem.SupportTransactionalFileSystem() || Path.IsNetworkPath(path)
				? new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize)
				: transaction != null
					? CreateTransacted(path, bufferSize, transaction)
					: CreateTransacted(path, bufferSize);
			return fileStream;
		}

		/// <summary>
		/// Creates a new file in the specified path.
		/// </summary>
		/// <param name="path">
		/// The path and name of the file to create.
		/// </param>
		/// <param name="bufferSize">
		/// The number of bytes buffered for writes to the file.
		/// </param>
		/// <returns>
		/// A <see cref="FileStream"/> with the specified buffer size that provides write access to the file specified in
		/// path.
		/// </returns>
		private static FileStream CreateTransacted(string path, int bufferSize)
		{
			if (Path.IsNetworkPath(path)) throw new ArgumentException("Cannot create a transacted file in a network volume.", "path");
			if (!_operatingSystem.SupportTransactionalFileSystem()) throw new InvalidOperationException("File system is not transactional.");

			// FileStreamTransacted necessary as the transaction is not DTC enlisted: transaction must be explicitly managed
			return new FileStreamTransacted(CreateTransactionHandle(), path, bufferSize);
		}

		/// <summary>
		/// Creates a new file in the specified path.
		/// </summary>
		/// <param name="path">
		/// The path and name of the file to create.
		/// </param>
		/// <param name="bufferSize">
		/// The number of bytes buffered for writes to the file.
		/// </param>
		/// <param name="transaction">
		/// </param>
		/// A non-<c>null</c> <see cref="IKernelTransaction"/> transaction that must already be enlisted with a DTC.
		/// <returns>
		/// A <see cref="FileStream"/> with the specified buffer size that provides write access to the file specified in
		/// path.
		/// </returns>
		private static FileStream CreateTransacted(string path, int bufferSize, IKernelTransaction transaction)
		{
			if (Path.IsNetworkPath(path)) throw new ArgumentException("Cannot create a transacted file in a network volume.", "path");
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (!_operatingSystem.SupportTransactionalFileSystem()) throw new InvalidOperationException("File system is not transactional.");

			IntPtr transactionHandle;
			transaction.GetHandle(out transactionHandle);
			if (transactionHandle == IntPtr.Zero) throw new TransactionException("Cannot get handle to kernel transaction.");

			// FileStreamTransacted unnecessary as the transaction is DTC enlisted: transaction is implicitly managed
			return new FileStream(CreateFileTransactedHandle(transactionHandle, path), FileAccess.Write, bufferSize);
		}

		/// <summary>
		/// Moves an existing file or a directory, including its children, as a transacted operation.
		/// </summary>
		/// <param name="sourceFilePath">
		/// The current name of the existing file or directory on the local computer.
		/// </param>
		/// <param name="targetFilePath">
		/// The new name for the file or directory. The new name must not already exist. A new file may be on a different
		/// file system or drive. A new directory must be on the same drive.
		/// </param>
		/// <param name="transaction">
		/// A handle to the transaction.
		/// </param>
		/// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa365241(v=vs.85).aspx"/>
		public static void Move(string sourceFilePath, string targetFilePath, IKernelTransaction transaction)
		{
			if (sourceFilePath.IsNullOrEmpty()) throw new ArgumentNullException("sourceFilePath");
			if (targetFilePath.IsNullOrEmpty()) throw new ArgumentNullException("targetFilePath");
			if (!_operatingSystem.SupportTransactionalFileSystem()) throw new InvalidOperationException("File system is not transactional.");

			IntPtr transactionHandle;
			transaction.GetHandle(out transactionHandle);
			if (transactionHandle == IntPtr.Zero) throw new TransactionException("Cannot get handle to kernel transaction.");

			var result = MoveFileTransacted(sourceFilePath, targetFilePath, IntPtr.Zero, IntPtr.Zero, 0, transactionHandle);
			if (!result) Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
		}

		#region Helpers

		internal static SafeFileHandle CreateFileTransactedHandle(IntPtr transactionHandle, string path)
		{
			if (transactionHandle == IntPtr.Zero) throw new ArgumentNullException("transactionHandle");
			var fileHandle = CreateFileTransacted(
				path,
				GENERIC_WRITE,
				0,
				IntPtr.Zero,
				CREATE_NEW,
				0,
				IntPtr.Zero,
				transactionHandle,
				IntPtr.Zero,
				IntPtr.Zero);
			if (fileHandle.IsInvalid) throw new IOException(string.Format("Null transacted file handle for file '{0}'.", path));
			if (_logger.IsFineEnabled) _logger.FineFormat("Transacted file handle created for writing at path '{0}'.", path);
			return fileHandle;
		}

		private static IntPtr CreateTransactionHandle()
		{
			var transactionHandle = CreateTransaction(IntPtr.Zero, IntPtr.Zero, 0, 0, 0, 0, null);
			if (transactionHandle == IntPtr.Zero) throw new TransactionException("Cannot create kernel transaction.");
			return transactionHandle;
		}

		#endregion

		#region External Imports

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern SafeFileHandle CreateFileTransacted(
			string lpFileName,
			uint dwDesiredAccess,
			uint dwShareMode,
			IntPtr lpSecurityAttributes,
			uint dwCreationDisposition,
			uint dwFlagsAndAttributes,
			IntPtr hTemplateFile,
			IntPtr hTransaction,
			IntPtr pusMiniVersion,
			IntPtr pExtendedParameter);

		[DllImport("Ktmw32.dll")]
		private static extern IntPtr CreateTransaction(
			IntPtr securityAttributes,
			IntPtr guid,
			int options,
			int isolationLevel,
			int isolationFlags,
			int milliseconds,
			string description);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool MoveFileTransacted(
			string lpExistingFileName,
			string lpNewFileName,
			IntPtr lpProgressRoutine,
			IntPtr lpData,
			uint dwFlags,
			IntPtr hTransaction);

		private const uint GENERIC_WRITE = 0x40000000;
		private const uint CREATE_NEW = 1;

		#endregion

		/// <summary>
		/// <c>internal</c> for the sake of unit testing only.
		/// </summary>
		internal static OperatingSystem _operatingSystem = Environment.OSVersion;

		private static readonly ILog _logger = LogManager.GetLogger(typeof(FileTransacted));
	}
}

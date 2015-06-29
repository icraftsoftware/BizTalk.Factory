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
using System.Transactions;
using Be.Stateless.Extensions;
using NUnit.Framework;

namespace Be.Stateless.IO
{
	[TestFixture]
	public class FileTransactedFixture
	{
		#region Setup/Teardown

		[TearDown]
		public void TearDown()
		{
			File.Delete(_filename);
			File.Delete(_filename + ".moved");
		}

		#endregion

		[Test]
		public void CreateFileStreamTransactedWhenTransactionalFileSystemSupported()
		{
			FileTransacted._operatingSystem = OperatingSystemExtensionsFixture.Windows7;
			using (var file = FileTransacted.Create(_filename))
			{
				Assert.That(file, Is.TypeOf<FileStreamTransacted>());
			}
		}

		[Test]
		public void CreateFileStreamWhenGivenAmbientTransactionAndTransactionalFileSystemSupported()
		{
			FileTransacted._operatingSystem = OperatingSystemExtensionsFixture.Windows7;
			using (new TransactionScope())
			{
				// grab kernel level transaction handle
				var dtcTransaction = TransactionInterop.GetDtcTransaction(Transaction.Current);
				// ReSharper disable once SuspiciousTypeConversion.Global
				var kernelTransaction = (IKernelTransaction) dtcTransaction;
				var file = FileTransacted.Create(_filename, 1024, kernelTransaction);
				Assert.That(file, Is.TypeOf<FileStream>());
			}
		}

		[Test]
		public void CreateFileStreamWhenNetworkPath()
		{
			var uncFilename = @"\\localhost\" + _filename.Replace(':', '$');
			FileTransacted._operatingSystem = OperatingSystemExtensionsFixture.Windows7;
			using (var file = FileTransacted.Create(uncFilename))
			{
				Assert.That(file, Is.TypeOf<FileStream>());
			}
		}

		[Test]
		public void CreateFileStreamWhenTransactionalFileSystemUnsupported()
		{
			FileTransacted._operatingSystem = OperatingSystemExtensionsFixture.WindowsXP;
			using (var file = FileTransacted.Create(_filename))
			{
				Assert.That(file, Is.TypeOf<FileStream>());
			}
		}

		[Test]
		public void MoveWhenAmbientTransactionCompletes()
		{
			using (var writer = File.CreateText(_filename))
			{
				writer.WriteLine("test");
			}

			Assert.That(File.Exists(_filename), Is.True);
			Assert.That(File.Exists(_filename + ".moved"), Is.False);

			FileTransacted._operatingSystem = OperatingSystemExtensionsFixture.Windows7;
			using (var scope = new TransactionScope())
			{
				var dtcTransaction = TransactionInterop.GetDtcTransaction(Transaction.Current);
				// ReSharper disable once SuspiciousTypeConversion.Global
				var kernelTransaction = (IKernelTransaction) dtcTransaction;
				FileTransacted.Move(_filename, _filename + ".moved", kernelTransaction);
				// this is the root scope and it has to cast a vote
				scope.Complete();
			}

			Assert.That(File.Exists(_filename), Is.False);
			Assert.That(File.Exists(_filename + ".moved"), Is.True);
		}

		[Test]
		public void MoveWhenAmbientTransactionDoesNotComplete()
		{
			using (var writer = File.CreateText(_filename))
			{
				writer.WriteLine("test");
			}

			Assert.That(File.Exists(_filename), Is.True);
			Assert.That(File.Exists(_filename + ".moved"), Is.False);

			FileTransacted._operatingSystem = OperatingSystemExtensionsFixture.Windows7;
			using (new TransactionScope())
			{
				var dtcTransaction = TransactionInterop.GetDtcTransaction(Transaction.Current);
				// ReSharper disable once SuspiciousTypeConversion.Global
				var kernelTransaction = (IKernelTransaction) dtcTransaction;
				FileTransacted.Move(_filename, _filename + ".moved", kernelTransaction);
				// this is the root scope and failing to cast a vote will abort the ambient transaction
			}

			Assert.That(File.Exists(_filename), Is.True);
			Assert.That(File.Exists(_filename + ".moved"), Is.False);
		}

		private static string GetTempFileName()
		{
			return System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".txt");
		}

		private static readonly string _filename = GetTempFileName();
	}
}

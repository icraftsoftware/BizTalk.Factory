#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using System.Text;
using NUnit.Framework;

namespace Be.Stateless.IO
{
	[TestFixture]
	public class FileStreamTransactedFixture
	{
		#region Setup/Teardown

		[TearDown]
		public void TearDown()
		{
			File.Delete(_filename);
		}

		#endregion

		[Test]
		public void TransactionCommitHasToBeExplicit()
		{
			using (var file = FileTransacted.Create(_filename))
			{
				Assert.That(file, Is.TypeOf<FileStreamTransacted>());
				file.Write(_buffer, 0, _buffer.Length);
				((FileStreamTransacted) file).Commit();
			}
			Assert.That(File.Exists(_filename), "Transaction should have been committed: file is not found.");
		}

		[Test]
		public void TransactionRollbackCanBeExplicit()
		{
			using (var file = FileTransacted.Create(_filename))
			{
				Assert.That(file, Is.TypeOf<FileStreamTransacted>());
				file.Write(_buffer, 0, _buffer.Length);
				((FileStreamTransacted) file).Rollback();
			}
			Assert.That(File.Exists(_filename), Is.False, "Transaction should have been rolled back: file is found.");
		}

		[Test]
		public void TransactionRollbackOnClose()
		{
			using (var file = FileTransacted.Create(_filename))
			{
				Assert.That(file, Is.TypeOf<FileStreamTransacted>());
				file.Write(_buffer, 0, _buffer.Length);
				file.Close();
			}
			Assert.That(File.Exists(_filename), Is.False, "Transaction should have been rolled back: file is found.");
		}

		[Test]
		public void TransactionRollbackOnDispose()
		{
			using (var file = FileTransacted.Create(_filename))
			{
				Assert.That(file, Is.TypeOf<FileStreamTransacted>());
				file.Write(_buffer, 0, _buffer.Length);
			}
			Assert.That(File.Exists(_filename), Is.False, "Transaction should have been rolled back: file is found.");
		}

		[Test]
		public void TransactionRollbackOnFinalize()
		{
			var file = FileTransacted.Create(_filename);
			Assert.That(file, Is.TypeOf<FileStreamTransacted>());
			file.Write(_buffer, 0, _buffer.Length);
			// ReSharper disable RedundantAssignment
			file = null;
			// ReSharper restore RedundantAssignment
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Assert.That(File.Exists(_filename), Is.False, "Transaction should have been rolled back: file is found.");
		}

		private static string GetTempFileName()
		{
			return System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".txt");
		}

		private static readonly string _filename = GetTempFileName();
		private readonly byte[] _buffer = Encoding.Unicode.GetBytes("foobar");
	}
}

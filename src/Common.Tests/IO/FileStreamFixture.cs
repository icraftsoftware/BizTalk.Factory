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
using System.Text;
using System.Transactions;
using NUnit.Framework;

namespace Be.Stateless.IO
{
	[TestFixture]
	public class FileStreamFixture
	{
		#region Setup/Teardown

		[TearDown]
		public void TearDown()
		{
			File.Delete(_filename);
		}

		#endregion

		[Test]
		public void TransactionCommitWithAmbientTransaction()
		{
			using (var scope = new TransactionScope())
			{
				// ReSharper disable once SuspiciousTypeConversion.Global
				var kernelTransaction = (IKernelTransaction) TransactionInterop.GetDtcTransaction(Transaction.Current);
				using (var file = FileTransacted.Create(_filename, 1024, kernelTransaction))
				{
					Assert.That(file, Is.TypeOf<FileStream>());
					file.Write(_buffer, 0, _buffer.Length);
				}
				// this is the root scope and it has to cast a vote
				scope.Complete();
			}
			Assert.That(File.Exists(_filename));
		}

		[Test]
		public void TransactionRollbackWithAmbientTransaction()
		{
			using (new TransactionScope())
			{
				// ReSharper disable once SuspiciousTypeConversion.Global
				var kernelTransaction = (IKernelTransaction) TransactionInterop.GetDtcTransaction(Transaction.Current);
				using (var file = FileTransacted.Create(_filename, 1024, kernelTransaction))
				{
					Assert.That(file, Is.TypeOf<FileStream>());
					file.Write(_buffer, 0, _buffer.Length);
				}
				// this is the root scope and failing to cast a vote will abort the ambient transaction
			}
			Assert.That(File.Exists(_filename), Is.False);
		}

		private static string GetTempFileName()
		{
			return System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".txt");
		}

		private readonly byte[] _buffer = Encoding.Unicode.GetBytes("foobar");
		private static readonly string _filename = GetTempFileName();
	}
}

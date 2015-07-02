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

using System.IO;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.ClaimStore.States
{
	[TestFixture]
	public class LockedDataFileFixture
	{
		[Test]
		public void GatherCopiesDataFileToCentralClaimStoreAndRenamesLocalDataFile()
		{
			var servantMock = new Mock<DataFileServant>();
			DataFileServant.Instance = servantMock.Object;

			const string filePath = "201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150627111406.locked";
			var sut = new LockedDataFile(filePath);
			var messageBodyMock = new Mock<MessageBody>(sut);

			servantMock.Setup(s => s.TryCreateDirectory(Path.Combine(Path.GetTempPath(), sut.ClaimStoreRelativePath))).Returns(true).Verifiable();
			servantMock.Setup(s => s.TryCopyFile(filePath, It.Is<string>(path => path == Path.Combine(Path.GetTempPath(), sut.ClaimStoreRelativePath)))).Returns(true).Verifiable();
			servantMock.Setup(s => s.TryMoveFile(filePath, It.Is<string>(path => path.Tokenize().State == GatheredDataFile.STATE_TOKEN))).Returns(true).Verifiable();

			sut.Gather(messageBodyMock.Object, Path.GetTempPath());

			servantMock.VerifyAll();
		}

		[Test]
		public void GatherTransitionsToAwaitingRetryDataFileWhenOperationFails()
		{
			var servantMock = new Mock<DataFileServant>();
			DataFileServant.Instance = servantMock.Object;

			const string filePath = "201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150627111406.locked";
			var sut = new LockedDataFile(filePath);
			var messageBodyMock = new Mock<MessageBody>(sut);
			messageBodyMock.SetupAllProperties();

			servantMock.Setup(s => s.TryCreateDirectory(Path.Combine(Path.GetTempPath(), sut.ClaimStoreRelativePath))).Returns(true);
			servantMock.Setup(s => s.TryCopyFile(filePath, It.Is<string>(path => path == Path.Combine(Path.GetTempPath(), sut.ClaimStoreRelativePath)))).Returns(true);
			servantMock.Setup(s => s.TryMoveFile(filePath, It.Is<string>(path => path.Tokenize().State == GatheredDataFile.STATE_TOKEN))).Returns(false);

			sut.Gather(messageBodyMock.Object, Path.GetTempPath());

			Assert.That(messageBodyMock.Object.DataFile, Is.TypeOf<AwaitingRetryDataFile>());
		}

		[Test]
		public void GatherTransitionsToGatheredDataFileWhenOperationSucceeds()
		{
			var servantMock = new Mock<DataFileServant>();
			DataFileServant.Instance = servantMock.Object;

			const string filePath = "201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150627111406.locked";
			var sut = new LockedDataFile(filePath);
			var messageBodyMock = new Mock<MessageBody>(sut);
			messageBodyMock.SetupAllProperties();

			servantMock.Setup(s => s.TryCreateDirectory(Path.Combine(Path.GetTempPath(), sut.ClaimStoreRelativePath))).Returns(true);
			servantMock.Setup(s => s.TryCopyFile(filePath, It.Is<string>(path => path == Path.Combine(Path.GetTempPath(), sut.ClaimStoreRelativePath)))).Returns(true);
			servantMock.Setup(s => s.TryMoveFile(filePath, It.Is<string>(path => path.Tokenize().State == GatheredDataFile.STATE_TOKEN))).Returns(true);

			sut.Gather(messageBodyMock.Object, Path.GetTempPath());

			Assert.That(messageBodyMock.Object.DataFile, Is.TypeOf<GatheredDataFile>());
		}

		[Test]
		public void LockTransitionsToAwaitingRetryDataFileWhenOperationFails()
		{
			var servantMock = new Mock<DataFileServant>();
			DataFileServant.Instance = servantMock.Object;

			const string filePath = "201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150627111406.locked";
			var sut = new LockedDataFile(filePath);
			var messageBodyMock = new Mock<MessageBody>(sut);
			messageBodyMock.SetupAllProperties();

			servantMock.Setup(s => s.TryMoveFile(filePath, It.IsAny<string>())).Returns(false);

			sut.Lock(messageBodyMock.Object);

			Assert.That(messageBodyMock.Object.DataFile, Is.TypeOf<AwaitingRetryDataFile>());
		}

		[Test]
		public void LockTransitionsToNewLockedDataFileWhenOperationSucceeds()
		{
			var servantMock = new Mock<DataFileServant>();
			DataFileServant.Instance = servantMock.Object;

			const string filePath = "201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150627111406.locked";
			var sut = new LockedDataFile(filePath);
			var messageBodyMock = new Mock<MessageBody>(sut);
			messageBodyMock.SetupAllProperties();

			servantMock.Setup(s => s.TryMoveFile(filePath, It.IsAny<string>())).Returns(true);

			sut.Lock(messageBodyMock.Object);

			Assert.That(messageBodyMock.Object.DataFile, Is.TypeOf<LockedDataFile>());
			Assert.That(messageBodyMock.Object.DataFile, Is.Not.SameAs(sut));
			Assert.That(sut.Path.Tokenize().LockTime < messageBodyMock.Object.DataFile.Path.Tokenize().LockTime);
		}

		[Test]
		public void ReleasingLockedDataFileIsInvalid()
		{
			var sut = new LockedDataFile("201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150627111406.locked");
			var messageBodyMock = new Mock<MessageBody>(sut);

			Assert.That(
				() => sut.Release(messageBodyMock.Object),
				Throws.InvalidOperationException);
		}

		[Test]
		public void UnlockingLockedDataFileIsInvalid()
		{
			const string filePath = "201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150627111406.locked";
			var sut = new LockedDataFile(filePath);
			var messageBodyMock = new Mock<MessageBody>(sut);

			Assert.That(
				() => sut.Unlock(messageBodyMock.Object),
				Throws.InvalidOperationException);
		}
	}
}

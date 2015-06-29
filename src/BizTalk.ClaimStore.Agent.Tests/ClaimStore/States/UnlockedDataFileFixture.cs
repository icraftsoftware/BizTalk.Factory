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
	public class UnlockedDataFileFixture
	{
		[Test]
		public void GatheringUnlockedDataFileIsInvalid()
		{
			var sut = new UnlockedDataFile("201306158F341A2D6FD7416B87073A0132DD51AE.chk");
			var messageBodyMock = new Mock<MessageBody>(sut);

			Assert.That(
				() => sut.Gather(messageBodyMock.Object, Path.GetTempPath()),
				Throws.InvalidOperationException);
		}

		[Test]
		public void LockRenamesLocalDataFile()
		{
			var servantMock = new Mock<DataFileServant>();
			DataFileServant.Instance = servantMock.Object;

			const string filePath = "201306158F341A2D6FD7416B87073A0132DD51AE.chk";
			var sut = new UnlockedDataFile("201306158F341A2D6FD7416B87073A0132DD51AE.chk");
			var messageBodyMock = new Mock<MessageBody>(sut);

			servantMock.Setup(s => s.TryMoveFile(filePath, It.Is<string>(path => path.Tokenize().State == LockedDataFile.STATE_TOKEN))).Returns(true).Verifiable();

			sut.Lock(messageBodyMock.Object);

			servantMock.VerifyAll();
		}

		[Test]
		public void LockTransitionsToAwaitingRetryDataFileWhenOperationFails()
		{
			var servantMock = new Mock<DataFileServant>();
			DataFileServant.Instance = servantMock.Object;

			const string filePath = "201306158F341A2D6FD7416B87073A0132DD51AE.chk";
			var sut = new UnlockedDataFile(filePath);
			var messageBodyMock = new Mock<MessageBody>(sut);
			messageBodyMock.SetupAllProperties();

			servantMock.Setup(s => s.TryMoveFile(filePath, It.Is<string>(path => path.Tokenize().State == LockedDataFile.STATE_TOKEN))).Returns(false);

			sut.Lock(messageBodyMock.Object);

			Assert.That(messageBodyMock.Object.DataFile, Is.TypeOf<AwaitingRetryDataFile>());
		}

		[Test]
		public void LockTransitionsToLockedDataFileWhenOperationSucceeds()
		{
			var servantMock = new Mock<DataFileServant>();
			DataFileServant.Instance = servantMock.Object;

			const string filePath = "201306158F341A2D6FD7416B87073A0132DD51AE.chk";
			var sut = new UnlockedDataFile(filePath);
			var messageBodyMock = new Mock<MessageBody>(sut);
			messageBodyMock.SetupAllProperties();

			servantMock.Setup(s => s.TryMoveFile(filePath, It.Is<string>(path => path.Tokenize().State == LockedDataFile.STATE_TOKEN))).Returns(true);

			sut.Lock(messageBodyMock.Object);

			Assert.That(messageBodyMock.Object.DataFile, Is.TypeOf<LockedDataFile>());
		}

		[Test]
		public void ReleasingUnlockedDataFileIsInvalid()
		{
			var sut = new UnlockedDataFile("201306158F341A2D6FD7416B87073A0132DD51AE.chk");
			var messageBodyMock = new Mock<MessageBody>(sut);

			Assert.That(
				() => sut.Release(messageBodyMock.Object),
				Throws.InvalidOperationException);
		}

		[Test]
		public void UnlockingAlreadyUnlockedDataFileIsInvalid()
		{
			var sut = new UnlockedDataFile("201306158F341A2D6FD7416B87073A0132DD51AE.chk");
			var messageBodyMock = new Mock<MessageBody>(sut);

			Assert.That(
				() => sut.Unlock(messageBodyMock.Object),
				Throws.InvalidOperationException);
		}
	}
}

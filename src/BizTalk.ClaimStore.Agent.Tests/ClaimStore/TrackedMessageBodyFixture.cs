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

using Be.Stateless.BizTalk.ClaimStore.States;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.ClaimStore
{
	[TestFixture]
	public class TrackedMessageBodyFixture
	{
		[Test]
		public void Collect()
		{
			var dataFileMock = new Mock<DataFile>("201306158F341A2D6FD7416B87073A0132DD51AE.trk") { CallBase = true };

			var sut = new TrackedMessageBody(dataFileMock.Object);
			sut.Collect(@"c:\folder");

			dataFileMock.Verify(df => df.Lock(sut), Times.Once);
			dataFileMock.Verify(df => df.Gather(sut, @"c:\folder"), Times.Once);
			dataFileMock.Verify(df => df.Release(sut), Times.Never);
			dataFileMock.Verify(df => df.Unlock(sut), Times.Once);
		}
	}
}

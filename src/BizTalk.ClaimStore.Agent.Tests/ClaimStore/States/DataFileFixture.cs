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

using NUnit.Framework;

namespace Be.Stateless.BizTalk.ClaimStore.States
{
	[TestFixture]
	public class DataFileFixture
	{
		[Test]
		public void CreateGatheredDataFile()
		{
			Assert.That(DataFile.Create("201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.gathered"), Is.TypeOf<GatheredDataFile>());
		}

		[Test]
		public void CreateLockedDataFile()
		{
			Assert.That(DataFile.Create("201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.locked"), Is.TypeOf<LockedDataFile>());
		}

		[Test]
		public void CreateReleasedDataFile()
		{
			Assert.That(DataFile.Create("201306158F341A2D6FD7416B87073A0132DD51AE.chk.20150626160941.released"), Is.TypeOf<ReleasedDataFile>());
		}

		[Test]
		public void CreateThrowsWhenInvalidMessageBodyDataFileName()
		{
			Assert.That(
				() => DataFile.Create("201306158F341A2D6FD7416B87073A0132DD51AE.thk"),
				Throws.ArgumentException
					.With.Message.StartsWith("Claim Store Agent does not recognized the message body's data file path: '201306158F341A2D6FD7416B87073A0132DD51AE.thk'."));
		}

		[Test]
		public void CreateUnlockedDataFile()
		{
			Assert.That(DataFile.Create("201306158F341A2D6FD7416B87073A0132DD51AE.chk"), Is.TypeOf<UnlockedDataFile>());
		}
	}
}

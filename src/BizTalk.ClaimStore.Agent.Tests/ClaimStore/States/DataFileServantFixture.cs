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
using NUnit.Framework;

namespace Be.Stateless.BizTalk.ClaimStore.States
{
	[TestFixture]
	public class DataFileServantFixture
	{
		[Test]
		public void TryIoOperation()
		{
			var filePath = Path.Combine(Path.GetTempPath(), "20130617E5DF814091FA44478EB41D19EB7CFB1B.trk");
			var dataFileServantMock = new DataFileServant();

			using (File.CreateText(filePath))
			{
				Assert.That(() => dataFileServantMock.TryDeleteFile(filePath), Is.False);
			}
			Assert.That(() => dataFileServantMock.TryDeleteFile(filePath), Is.True);
		}
	}
}

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

using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.IO.Extensions;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Streaming
{
	[TestFixture]
	public class ZipInputStreamFixture
	{
		[Test]
		public void BaseInputStreamIsReadToEnd()
		{
			using (var dataStream = ResourceManager.Load("Data.METERING_REQUEST_20150531.zip"))
			using (var eventingStream = new EventingReadStream(dataStream))
			using (var decompressingStream = new ZipInputStream(eventingStream))
			{
				var eosReached = false;
				eventingStream.AfterLastReadEvent += (sender, args) => { eosReached = true; };
				decompressingStream.GetNextEntry();
				decompressingStream.Drain();
				Assert.That(eosReached, Is.True);
			}
		}

		[Test]
		public void SharpZipLibBaseInputStreamIsNotReadToEnd()
		{
			using (var dataStream = ResourceManager.Load("Data.METERING_REQUEST_20150531.zip"))
			using (var eventingStream = new EventingReadStream(dataStream))
			using (var decompressingStream = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(eventingStream))
			{
				var eosReached = false;
				eventingStream.AfterLastReadEvent += (sender, args) => { eosReached = true; };
				decompressingStream.GetNextEntry();
				decompressingStream.Drain();
				Assert.That(eosReached, Is.False);
			}
		}
	}
}

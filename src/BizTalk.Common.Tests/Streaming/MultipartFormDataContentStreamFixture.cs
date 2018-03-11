#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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
using System.Text;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Streaming
{
	[TestFixture]
	public class MultipartFormDataContentStreamFixture
	{
		[Test]
		public void ClosesWrappedStream()
		{
			var streamMock = new Mock<MemoryStream>(MockBehavior.Default, Encoding.UTF8.GetBytes(CONTENT)) { CallBase = true };

			using (var composite = new MultipartFormDataContentStream(streamMock.Object))
			{
				var buffer = new byte[10];
				composite.Read(buffer, 0, buffer.Length);
			}

			streamMock.Verify(s => s.Close());
		}

		[Test]
		public void ExhaustsWrappedStream()
		{
			using (var stream = new MultipartFormDataContentStream(new MemoryStream(Encoding.UTF8.GetBytes(CONTENT))))
			{
				var buffer = new byte[256];
				var offset = 0;
				int bytesRead;
				while ((bytesRead = stream.Read(buffer, offset, 8)) > 0)
				{
					offset += bytesRead;
				}

				var actual = Encoding.UTF8.GetString(buffer, 0, offset);
				Assert.That(actual, Does.Match(EXPECTED_CONTENT_PATTERN));

				var trail = Encoding.UTF8.GetString(buffer, offset, buffer.Length - offset);
				Assert.That(trail.ToCharArray(), Has.All.EqualTo('\0'));
			}
		}

		[Test]
		public void MultipartEncodesWrappedStream()
		{
			using (var stream = new MultipartFormDataContentStream(new MemoryStream(Encoding.UTF8.GetBytes(CONTENT))))
			using (var reader = new StreamReader(stream))
			{
				var actual = reader.ReadToEnd();

				Assert.That(actual, Does.Match(EXPECTED_CONTENT_PATTERN));
			}
		}

		private const string CONTENT = "<part-one xmlns=\"part-one\"><child-one>one</child-one></part-one>";

		private const string EXPECTED_CONTENT_PATTERN = "^(--[a-f\\d]{8}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{4}-[a-f\\d]{12})\r\nContent-Disposition: form-data\r\n\r\n"
			+ CONTENT
			+ "\r\n\\1--\r\n$";
	}
}

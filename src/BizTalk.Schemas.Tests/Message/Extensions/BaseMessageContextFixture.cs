#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Unit;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	[TestFixture]
	public class BaseMessageContextFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			MessageContextMock = new MessageContextMock();
		}

		#endregion

		[Test]
		public void ReadBoolean()
		{
			MessageContextMock.Setup(c => c.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)).Returns(null);
			Assert.That(MessageContextMock.Object.GetProperty(BtsProperties.AckRequired), Is.Null);
			MessageContextMock.Setup(c => c.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)).Returns("true");
			Assert.That(MessageContextMock.Object.GetProperty(BtsProperties.AckRequired), Is.True);
			MessageContextMock.Setup(c => c.Read(BtsProperties.AckRequired.Name, BtsProperties.AckRequired.Namespace)).Returns("True");
			Assert.That(MessageContextMock.Object.GetProperty(BtsProperties.AckRequired), Is.True);
		}

		[Test]
		public void ReadDateTime()
		{
			MessageContextMock.Setup(c => c.Read(FileProperties.FileCreationTime.Name, FileProperties.FileCreationTime.Namespace)).Returns(null);
			Assert.That(MessageContextMock.Object.GetProperty(FileProperties.FileCreationTime), Is.Null);
			MessageContextMock.Setup(c => c.Read(FileProperties.FileCreationTime.Name, FileProperties.FileCreationTime.Namespace)).Returns("2012-12-01");
			Assert.That(MessageContextMock.Object.GetProperty(FileProperties.FileCreationTime), Is.EqualTo(new DateTime(2012, 12, 1)));
			MessageContextMock.Setup(c => c.Read(FileProperties.FileCreationTime.Name, FileProperties.FileCreationTime.Namespace)).Returns("2017-09-15T10:19:06");
			Assert.That(MessageContextMock.Object.GetProperty(FileProperties.FileCreationTime), Is.EqualTo(new DateTime(2017, 9, 15, 10, 19, 6)));
		}

		[Test]
		public void ReadInteger()
		{
			MessageContextMock.Setup(c => c.Read(BtsProperties.RetryCount.Name, BtsProperties.RetryCount.Namespace)).Returns(null);
			Assert.That(MessageContextMock.Object.GetProperty(BtsProperties.RetryCount), Is.Null);
			MessageContextMock.Setup(c => c.Read(BtsProperties.RetryCount.Name, BtsProperties.RetryCount.Namespace)).Returns("2");
			Assert.That(MessageContextMock.Object.GetProperty(BtsProperties.RetryCount), Is.EqualTo(2));
		}

		private MessageContextMock MessageContextMock { get; set; }
	}
}

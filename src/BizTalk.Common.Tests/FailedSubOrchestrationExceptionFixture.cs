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
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace Be.Stateless.BizTalk
{
	[TestFixture]
	public class FailedSubOrchestrationExceptionFixture
	{
		[Test]
		public void Message()
		{
			Assert.That(
				() => { throw new FailedSubOrchestrationException("process", "Can't proceed."); },
				Throws.TypeOf<FailedSubOrchestrationException>().With.Message.EqualTo("Orchestration 'process' failed. Can't proceed."));
		}

		[Test]
		public void SerializationRoundTrip()
		{
			var stream = new MemoryStream();
			var formatter = new BinaryFormatter();

			var originalException = Assert.Throws<FailedSubOrchestrationException>(() => { throw new FailedSubOrchestrationException("process", "Can't proceed."); });
			formatter.Serialize(stream, originalException);
			stream.Position = 0;
			var deserializedException = (FailedSubOrchestrationException) formatter.Deserialize(stream);

			Assert.That(deserializedException, Is.Not.SameAs(originalException));
			Assert.That(deserializedException.Name, Is.EqualTo(originalException.Name));
			Assert.That(deserializedException.ToString(), Is.EqualTo(originalException.ToString()));
		}

		[Test]
		public void ToStringSerialization()
		{
			var exception = Assert.Throws<FailedSubOrchestrationException>(() => { throw new FailedSubOrchestrationException("process", "Can't proceed."); });
			Assert.That(exception.ToString(), Does.StartWith("Be.Stateless.BizTalk.FailedSubOrchestrationException: Orchestration 'process' failed. Can't proceed."));
		}
	}
}

#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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

using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.BizTalk.Unit.Schema;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Schemas.Xml
{
	[TestFixture]
	public class BatchFixture : SchemaFixture<Batch>
	{
		[Test]
		public void ValidateBatchContent()
		{
			var instance = MessageFactory.CreateMessage<Batch.Content>(ResourceManager.LoadString("Data.BatchContent.xml"));
			Assert.That(() => ValidateInstanceDocument(instance), Throws.Nothing);
		}

		[Test]
		public void ValidateReleaseBatch()
		{
			var instance = MessageFactory.CreateMessage<Batch.Release>(ResourceManager.LoadString("Data.ReleaseBatch.xml"));
			Assert.That(() => ValidateInstanceDocument(instance), Throws.Nothing);
		}
	}
}

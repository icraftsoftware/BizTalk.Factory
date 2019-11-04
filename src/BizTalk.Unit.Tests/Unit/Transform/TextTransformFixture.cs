#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Message;
using Be.Stateless.BizTalk.Unit.Resources;
using Be.Stateless.Xml.Extensions;
using BTF2Schemas;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.Transform
{
	[TestFixture]
	public class TextTransformFixture : ClosedTransformFixture<TextTransform>
	{
		[Test]
		[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
		public void SetupTextTransform()
		{
			using (var stream = MessageFactory.CreateMessage<btf2_services_header>(ResourceManager.LoadString("Data.Message.xml")).AsStream())
			{
				var setup = Given(input => input.Message(stream))
					.Transform
					.OutputsText();

				var result = setup.Validate();

				Assert.That(result.TextContent, Is.EqualTo("services, deliveryReceiptRequest, sendTo, address, sendBy, commitmentReceiptRequest, sendTo, address, sendBy, "));
			}
		}
	}
}

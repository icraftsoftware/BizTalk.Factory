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

using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Message.Interop;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.RuleEngine
{
	[TestFixture]
	public class ContextFixture
	{
		[Test]
		public void ValueWrittenViaContextIsNotReadableViaMessage()
		{
			var message = new Unit.Message.Mock<IBaseMessage>();

			var sut = new Context(message.Object.Context);
			sut.Write(TrackingProperties.ProcessName.QName, "process-name");

			Assert.That(message.Object.GetProperty(TrackingProperties.ProcessName), Is.Null);
		}

		[Test]
		public void ValueWrittenViaContextIsVerifiableViaMessage()
		{
			var message = new Unit.Message.Mock<IBaseMessage>();

			var sut = new Context(message.Object.Context);
			sut.Write(TrackingProperties.ProcessName.QName, "process-name");

			message.Verify(m => m.SetProperty(TrackingProperties.ProcessName, "process-name"));
		}
	}
}

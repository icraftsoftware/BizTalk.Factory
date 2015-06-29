#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Message.Extensions
{
	[TestFixture]
	public class BaseMessageContextFixture
	{
		[Test]
		public void SerializeContextToXml()
		{
			var name = BtsProperties.OutboundTransportLocation.Name;
			var ns = BtsProperties.OutboundTransportLocation.Namespace;

			var context = new Mock<IBaseMessageContext>();
			context
				.Setup(c => c.ReadAt(0, out name, out ns))
				.Returns(@"file://c:\files\ports\out");

			name = BtsProperties.OutboundTransportType.Name;
			ns = BtsProperties.OutboundTransportType.Namespace;
			context
				.Setup(c => c.ReadAt(1, out name, out ns))
				.Returns("FILE");
			context
				.Setup(c => c.IsPromoted(name, ns))
				.Returns(true);

			name = new FILE.Password().Name.Name;
			ns = new FILE.Password().Name.Namespace;
			context
				.Setup(c => c.ReadAt(2, out name, out ns))
				.Returns("p@ssw0rd");

			name = "/*[local-name()='order' and namespace-uri()='urn:schemas.stateless.be:biztalk:system']/*[local-name()='id' and namespace-uri()='urn:schemas.stateless.be:biztalk:system']";
			ns = "http://schemas.microsoft.com/BizTalk/2003/btsDistinguishedFields";
			context
				.Setup(c => c.ReadAt(3, out name, out ns))
				.Returns("123456789");

			context
				.Setup(c => c.CountProperties)
				.Returns(4);

			const string expected = @"<context xmlns:s0=""http://schemas.microsoft.com/BizTalk/2003/system-properties"" xmlns:s1=""http://schemas.microsoft.com/BizTalk/2003/btsDistinguishedFields"">"
				+ @"<s0:p n=""OutboundTransportLocation"">file://c:\files\ports\out</s0:p>"
				+ @"<s0:p n=""OutboundTransportType"" promoted=""true"">FILE</s0:p>"
				+ @"<s1:p n=""/*[local-name()='order' and namespace-uri()='urn:schemas.stateless.be:biztalk:system']/*[local-name()='id' and namespace-uri()='urn:schemas.stateless.be:biztalk:system']"">123456789</s1:p>"
				+ @"</context>";
			Assert.That(context.Object.ToXml(), Is.EqualTo(expected));

			context.VerifyAll();
		}
	}
}
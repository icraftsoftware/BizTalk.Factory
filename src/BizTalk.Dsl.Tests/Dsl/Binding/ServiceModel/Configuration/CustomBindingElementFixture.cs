#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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

using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration
{
	[TestFixture]
	public class CustomBindingElementFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var binding = new CustomBindingElement {
				new MtomMessageEncodingElement { MessageVersion = MessageVersion.Soap11, ReaderQuotas = { MaxStringContentLength = 7340032 } },
				new TcpTransportElement { MaxReceivedMessageSize = 7340032, MaxBufferSize = 7340032 }
			};
			Assert.That(
				binding.GetBindingElementXml("customBinding"),
				Is.EqualTo(
					"<binding name=\"customDslBinding\">" +
						"<mtomMessageEncoding messageVersion=\"Soap11\">" +
						"<readerQuotas maxStringContentLength=\"7340032\" />" +
						"</mtomMessageEncoding>" +
						"<tcpTransport maxReceivedMessageSize=\"7340032\" maxBufferSize=\"7340032\" />" +
						"</binding>"));
		}
	}
}

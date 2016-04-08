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

using System;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class SBMessagingAdapterInboundFixture
	{
		[Test]
		public void SerializeToXml()
		{
			var isma = new SBMessagingAdapter.Inbound(
				a => {
					a.IsSessionful = true;

					a.UseAcsAuthentication = true;
					a.StsUri = new Uri("https://biztalk.factory-sb.accesscontrol.windows.net/");
					a.IssuerName = "issuer_name";
					a.IssuerSecret = "issuer_secret";

					a.CustomBrokeredPropertyNamespace = "urn:schemas.stateless.be:biztalk:service-bus:queue";
					a.PromoteCustomProperties = true;
				});
			var xml = ((IAdapterBindingSerializerFactory) isma).GetAdapterBindingSerializer().Serialize();
			Assert.That(
				xml,
				Is.EqualTo(
					"<CustomProps>" +
						"<ReceiveTimeout vt=\"8\">00:10:00</ReceiveTimeout>" +
						"<IsSessionful vt=\"11\">-1</IsSessionful>" +
						"<PrefetchCount vt=\"3\">-1</PrefetchCount>" +
						"<IssuerName vt=\"8\">issuer_name</IssuerName>" +
						"<IssuerSecret vt=\"8\">issuer_secret</IssuerSecret>" +
						"<StsUri vt=\"8\">https://biztalk.factory-sb.accesscontrol.windows.net/</StsUri>" +
						"<PromoteCustomProperties vt=\"11\">-1</PromoteCustomProperties>" +
						"<CustomBrokeredPropertyNamespace vt=\"8\">urn:schemas.stateless.be:biztalk:service-bus:queue</CustomBrokeredPropertyNamespace>" +
						"<UseAcsAuthentication vt=\"11\">-1</UseAcsAuthentication>" +
						"<UseSasAuthentication vt=\"11\">0</UseSasAuthentication>" +
						"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
						"<SendTimeout vt=\"8\">00:00:00</SendTimeout>" +
						"<CloseTimeout vt=\"8\">00:01:00</CloseTimeout>" +
						"</CustomProps>"));
		}

		[Test]
		public void Validate()
		{
			var isma = new SBMessagingAdapter.Inbound();
			Assert.That(
				() => ((ISupportValidation) isma).Validate(),
				Throws.TypeOf<ArgumentException>().With.Message.EqualTo(@"Required property Address (URI) not specified."));
		}

		[Test]
		public void ValidateAddress()
		{
			var isma = new SBMessagingAdapter.Inbound(a => { a.Address = new Uri("file://biztalf.factory.servicebus.windows.net/batching/"); });
			Assert.That(
				() => ((ISupportValidation) isma).Validate(),
				Throws.TypeOf<ArgumentException>().With.Message.EqualTo(@"The specified address is invalid."));
		}
	}
}

﻿#region Copyright & License

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

using System.ServiceModel;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[TestFixture]
	public class WcfWSHttpAdapterFixture
	{
		[Test]
		public void ProtocolTypeSettingsAreReadFromRegistry()
		{
			var mock = new Mock<WcfWSHttpAdapter<EndpointAddress, WSHttpRLConfig>> { CallBase = true };
			var nta = mock.Object as IAdapter;
			Assert.That(nta.ProtocolType.Name, Is.EqualTo("WCF-WSHttp"));
			Assert.That(nta.ProtocolType.Capabilities, Is.EqualTo(899));
			Assert.That(nta.ProtocolType.ConfigurationClsid, Is.EqualTo("2b219014-ba04-4b70-a66b-a8c418b109fd"));
		}
	}
}

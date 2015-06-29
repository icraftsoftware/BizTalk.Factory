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

using System.ServiceModel;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	[StubServiceHostActivator]
	[TestFixture]
	public class StubServiceHostFixture
	{
		[Test]
		public void DefaultBinding()
		{
			Assert.That(StubServiceHost.DefaultBinding, Is.TypeOf<BasicHttpBinding>());
		}

		[Test]
		public void DefaultEndpointAddress()
		{
			Assert.That(StubServiceHost.DefaultEndpointAddress, Is.EqualTo(new EndpointAddress("http://localhost:8000/stubservice")));
		}

		[Test]
		public void DefaultInstanceRecycling()
		{
			Assert.That(StubServiceHost.DefaultInstance, Is.Not.Null);
			var instance = StubServiceHost.DefaultInstance;
			StubServiceHost.DefaultInstance.Recycle();
			Assert.That(StubServiceHost.DefaultInstance, Is.Not.SameAs(instance));
		}
	}
}

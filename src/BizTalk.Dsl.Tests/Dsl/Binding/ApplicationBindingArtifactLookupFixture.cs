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

using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.MicroPipelines;
using Be.Stateless.BizTalk.Unit.Binding;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[TestFixture]
	public class ApplicationBindingArtifactLookupFixture
	{
		[Test]
		public void ApplicationName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			Assert.That(ab.Name, Is.EqualTo("MyTestApplication"));
		}

		[Test]
		public void ReceiveLocationName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var rl = ab.ReceiveLocation<TestReceiveLocation>();
			Assert.That(rl.Name, Is.EqualTo("MyTestReceiveLocation"));
		}

		[Test]
		public void ReferencedApplicationName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var rab = ab.ReferencedApplication<TestReferencedApplication>();
			Assert.That(rab.Name, Is.EqualTo("MyTestReferencedApplication"));
		}

		[Test]
		public void ReferencedApplicationReceiveLocationName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var rab = ab.ReferencedApplication<TestReferencedApplication>();
			var rl = rab.ReceiveLocation<TestReferencedReceiveLocation>();
			Assert.That(rl.Name, Is.EqualTo("MyTestReferencedReceiveLocation"));
		}

		[Test]
		public void ReferencedApplicationSendPortName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var rab = ab.ReferencedApplication<TestReferencedApplication>();
			var sp = rab.SendPort<TestReferencedSendPort>();
			Assert.That(sp.Name, Is.EqualTo("MyTestReferencedSendPort"));
		}

		[Test]
		public void SendPortName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var sp = ab.SendPort<TestSendPort>();
			Assert.That(sp.Name, Is.EqualTo("MyTestSendPort"));
		}

		private class TestApplication : ApplicationBindingBase<string>
		{
			public TestApplication()
			{
				Name = "MyTestApplication";
				ReferencedApplications.Add(new TestReferencedApplication());
				ReceivePorts.Add(new TestReceivePort());
				SendPorts.Add(new TestSendPort());
			}
		}

		private class TestReceivePort : ReceivePortBase<string>
		{
			public TestReceivePort()
			{
				Name = "MyTestReceivePort";
				ReceiveLocations.Add(new TestReceiveLocation());
			}
		}

		private class TestReceiveLocation : ReceiveLocationBase<string>
		{
			public TestReceiveLocation()
			{
				Name = "MyTestReceiveLocation";
				ReceivePipeline = new ReceivePipeline<XmlReceive>();
				Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files"; });
				Transport.Host = "host";
			}
		}

		private class TestSendPort : SendPortBase<string>
		{
			public TestSendPort()
			{
				Name = "MyTestSendPort";
				SendPipeline = new SendPipeline<XmlTransmit>();
				Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\files"; });
				Transport.Host = "host";
			}
		}

		private class TestReferencedApplication : ApplicationBindingBase<string>
		{
			public TestReferencedApplication()
			{
				Name = "MyTestReferencedApplication";
				ReceivePorts.Add(new TestReferencedReceivePort());
				SendPorts.Add(new TestReferencedSendPort());
			}
		}

		private class TestReferencedReceivePort : ReceivePortBase<string>
		{
			public TestReferencedReceivePort()
			{
				Name = "MyTestReferencedReceivePort";
				ReceiveLocations.Add(new TestReferencedReceiveLocation());
			}
		}

		private class TestReferencedReceiveLocation : ReceiveLocationBase<string>
		{
			public TestReferencedReceiveLocation()
			{
				Name = "MyTestReferencedReceiveLocation";
				ReceivePipeline = new ReceivePipeline<XmlReceive>();
				Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\files"; });
				Transport.Host = "host";
			}
		}

		private class TestReferencedSendPort : SendPortBase<string>
		{
			public TestReferencedSendPort()
			{
				Name = "MyTestReferencedSendPort";
				SendPipeline = new SendPipeline<XmlTransmit>();
				Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\files"; });
				Transport.Host = "host";
			}
		}
	}
}

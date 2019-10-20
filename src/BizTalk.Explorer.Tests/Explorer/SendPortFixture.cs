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

using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Unit.Binding;
using Microsoft.BizTalk.ExplorerOM;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Explorer
{
	[TestFixture]
	public class SendPortFixture
	{
		[Test]
		[Explicit("BizTalk.Factory has to be deployed.")]
		public void Unenlist()
		{
			var application = BizTalkServerGroup.Applications[BizTalkFactorySettings.APPLICATION_NAME];
			var sendPort = application.SendPorts[BizTalkFactoryApplication.SendPort<SinkFailedMessageSendPort>().Name];

			sendPort.Unenlist();
			application.ApplyChanges();
			Assert.That(sendPort.Status, Is.EqualTo(PortStatus.Bound));

			sendPort.Enlist();
			application.ApplyChanges();
			Assert.That(sendPort.Status, Is.EqualTo(PortStatus.Stopped));

			sendPort.Unenlist();
			application.ApplyChanges();
			Assert.That(sendPort.Status, Is.EqualTo(PortStatus.Bound));

			sendPort.Start();
			application.ApplyChanges();
			Assert.That(sendPort.Status, Is.EqualTo(PortStatus.Started));
		}

		private static IApplicationBindingArtifactLookup BizTalkFactoryApplication
		{
			get { return ApplicationBindingArtifactLookupFactory<BizTalkFactoryApplicationBinding>.Create(); }
		}
	}
}

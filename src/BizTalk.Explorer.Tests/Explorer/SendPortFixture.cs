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

using Microsoft.BizTalk.ExplorerOM;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Explorer
{
	[TestFixture]
	public class SendPortFixture
	{
		[Test]
		public void Unenlist()
		{
			var application = BizTalkServerGroup.Applications["BizTalk EDI Application"];
			var sendPort = application.SendPorts["ResendPort"];

			sendPort.Unenlist();
			application.ApplyChanges();
			Assert.That(sendPort.Status, Is.EqualTo(PortStatus.Bound));

			sendPort.Enlist();
			application.ApplyChanges();
			Assert.That(sendPort.Status, Is.EqualTo(PortStatus.Stopped));

			sendPort.Unenlist();
			application.ApplyChanges();
			Assert.That(sendPort.Status, Is.EqualTo(PortStatus.Bound));
		}
	}
}

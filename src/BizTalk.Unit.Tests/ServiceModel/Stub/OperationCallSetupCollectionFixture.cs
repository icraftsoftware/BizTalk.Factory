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

using System.Collections.Generic;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	[TestFixture]
	public class OperationCallSetupCollectionFixture
	{
		[Test]
		public void NoSetupHasBeenPerformed()
		{
			Assert.That(
				() => new OperationCallSetupCollection()["message", "action"],
				Throws.TypeOf<KeyNotFoundException>()
					.With.Message.EqualTo("No operation setup has been performed for neither the message type 'message' nor the SOAP action 'action'."));
		}

		[Test]
		public void OperationCallSetupDefinedForMessageTypeHasPrecedenceOverAction()
		{
			var sut = new OperationCallSetupCollection();
			sut.Add<StubServiceFixture.IWork>("Action");
			var messageSetup = sut.Add<ISolicitResponse>("MessageType");

			Assert.That(sut["MessageType", "Action"], Is.SameAs(messageSetup));
		}
	}
}

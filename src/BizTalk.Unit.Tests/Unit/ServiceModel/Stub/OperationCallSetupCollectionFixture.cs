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

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	[TestFixture]
	public class OperationCallSetupCollectionFixture
	{
		[Test]
		public void AddOrUpdateOperationCallSetup()
		{
			var sut = new OperationCallSetupCollection();
			var operationCallSetup1 = (OperationCallSetup<ISolicitResponse, Stream>) sut.Add<ISolicitResponse, Stream>("MessageType");
			operationCallSetup1.Returns(new MemoryStream());
			var operationCallSetup2 = (OperationCallSetup<ISolicitResponse, Stream>) sut.Add<ISolicitResponse, Stream>("MessageType");
			operationCallSetup2.Returns(new MemoryStream());

			Assert.That(sut["MessageType", "Action"], Is.SameAs(operationCallSetup2));
			Assert.That(operationCallSetup1, Is.Not.SameAs(operationCallSetup2));
			Assert.That(operationCallSetup1.Stream, Is.Not.SameAs(operationCallSetup2.Stream));
		}

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

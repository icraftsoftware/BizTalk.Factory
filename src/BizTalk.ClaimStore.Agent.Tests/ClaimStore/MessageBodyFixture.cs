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

using Be.Stateless.BizTalk.ClaimStore.States;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.ClaimStore
{
	[TestFixture]
	public class MessageBodyFixture
	{
		[Test]
		public void CreateClaimedMessageBody()
		{
			Assert.That(MessageBody.Create(DataFile.Create("201306158F341A2D6FD7416B87073A0132DD51AE.chk")), Is.TypeOf<ClaimedMessageBody>());
		}

		[Test]
		public void CreateTrackedMessageBody()
		{
			Assert.That(MessageBody.Create(DataFile.Create("201306158F341A2D6FD7416B87073A0132DD51AE.trk")), Is.TypeOf<TrackedMessageBody>());
		}
	}
}

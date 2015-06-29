#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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

using Be.Stateless.BizTalk.Component;
using Microsoft.BizTalk.Component;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	[TestFixture]
	public class ComponentListFixture
	{
		[Test]
		public void AddComponentToCompatibleStage()
		{
			var list = new ComponentList(new Stage(StageCategory.Decoder.Id));
			Assert.That(
				() => list.Add(new FailedMessageRoutingEnablerComponent()),
				Throws.Nothing);
		}

		[Test]
		public void AddComponentToIncompatibleStageThrows()
		{
			var list = new ComponentList(new Stage(StageCategory.Decoder.Id));
			Assert.That(
				() => list.Add(new PartyRes()),
				Throws.ArgumentException.With.Message.StartsWith("Party resolution is made for any of the PartyResolver stages and is not compatible with a Decoder stage."));
		}

		[Test]
		public void FetchComponentFromComponentList()
		{
			var component = new FailedMessageRoutingEnablerComponent();
			var list = new ComponentList(new Stage(StageCategory.Decoder.Id)) {
				component
			};

			Assert.That(list.Component<FailedMessageRoutingEnablerComponent>(), Is.SameAs(component));
		}

		[Test]
		public void FetchUnregisteredComponentFromComponentListThrows()
		{
			var list = new ComponentList(new Stage(StageCategory.Decoder.Id)) {
				new FailedMessageRoutingEnablerComponent()
			};

			Assert.That(
				() => list.Component<PartyRes>(),
				Throws.InvalidOperationException.With.Message.EqualTo("Sequence contains no elements"));
		}
	}
}

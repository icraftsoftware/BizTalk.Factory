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

using System;
using NUnit.Framework;
using Stages = Microsoft.BizTalk.PipelineOM.Stage;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	[TestFixture]
	public class StageCategoryFixture
	{
		[Test]
		public void CategoryEqualityIsReflexive()
		{
			Assert.That(StageCategory.Decoder, Is.EqualTo(StageCategory.Decoder));
		}

		[Test]
		public void CategoryNameIsIrrelevant()
		{
			var id = Guid.NewGuid();
			var c1 = new StageCategory("c1", id);
			var c2 = new StageCategory("c2", id);
			Assert.That(c1, Is.EqualTo(c2));
		}

		[Test]
		public void CategoryWithDifferentIdAreNotEqual()
		{
			var c1 = new StageCategory("category", Guid.NewGuid());
			var c2 = new StageCategory("category", Guid.NewGuid());
			Assert.That(c1, Is.Not.EqualTo(c2));
		}

		[Test]
		public void CategoryWithSameIdAreEqual()
		{
			var id = Guid.NewGuid();
			var c1 = new StageCategory("category", id);
			var c2 = new StageCategory("category", id);
			Assert.That(c1, Is.EqualTo(c2));
		}

		[Test]
		public void IsCompatibleWithAny()
		{
			Assert.That(StageCategory.Decoder.IsCompatibleWith(new[] { StageCategory.Encoder, StageCategory.Any }), Is.True);
		}

		[Test]
		public void IsCompatibleWithItself()
		{
			Assert.That(StageCategory.Encoder.IsCompatibleWith(new[] { StageCategory.Any, StageCategory.Encoder }), Is.True);
		}

		[Test]
		public void IsNotCompatibleWith()
		{
			Assert.That(StageCategory.Encoder.IsCompatibleWith(new[] { StageCategory.Decoder, StageCategory.PartyResolver }), Is.False);
		}

		[Test]
		public void SanityCheck()
		{
			Assert.That(StageCategory.Any.Id, Is.EqualTo(Stages.Any));
			Assert.That(StageCategory.AssemblingSerializer.Id, Is.EqualTo(Stages.AssemblingSerializer));
			Assert.That(StageCategory.Decoder.Id, Is.EqualTo(Stages.Decoder));
			Assert.That(StageCategory.DisassemblingParser.Id, Is.EqualTo(Stages.DisassemblingParser));
			Assert.That(StageCategory.Encoder.Id, Is.EqualTo(Stages.Encoder));
			Assert.That(StageCategory.PartyResolver.Id, Is.EqualTo(Stages.PartyResolver));
			Assert.That(StageCategory.Validator.Id, Is.EqualTo(Stages.Validator));
		}
	}
}

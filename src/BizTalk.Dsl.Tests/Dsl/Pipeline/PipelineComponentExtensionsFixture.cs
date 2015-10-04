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

using System;
using Be.Stateless.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	[TestFixture]
	public class PipelineComponentExtensionsFixture
	{
		[Test]
		public void ReflectCategoryFromNonPipelineComponentThrows()
		{
			var component = new Mock<PipelineComponent>();
			Assert.That(
				() => component.Object.GetStageCategories(),
				Throws.ArgumentException.With.Message.EqualTo(
					"PipelineComponentProxy is not categorized as a pipeline component. Apply the ComponentCategoryAttribute with a category of Microsoft.BizTalk.Component.Interop.CategoryTypes.CATID_PipelineComponent."));
		}

		[Test]
		public void ReflectCategoryFromPipelineComponent()
		{
			var component = new FailedMessageRoutingEnablerComponent();
			Assert.That(component.GetStageCategories(), Contains.Item(StageCategory.Any));
		}

		[Test]
		public void ReflectCategoryFromPipelineComponentWithMultipleStageCategoriesDoesNotThrow()
		{
			var component = new MultipleStageComponent();
			Assert.That(
				() => component.GetStageCategories(),
				Throws.Nothing);
		}

		[Test]
		public void ReflectCategoryFromPipelineComponentWithoutStageCategoryThrows()
		{
			var component = new NoStageComponent();
			Assert.That(
				() => component.GetStageCategories(),
				Throws.ArgumentException.With.Message.EqualTo(
					"NoStageComponent has not been associated with a pipeline stage category. Apply the ComponentCategoryAttribute with one of the stage categories available through Microsoft.BizTalk.Component.Interop.CategoryTypes."));
		}

		[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
		private class NoStageComponent : PipelineComponent
		{
			#region Base Class Member Overrides

			public override string Description
			{
				get { throw new NotSupportedException(); }
			}

			protected override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			public override void GetClassID(out Guid classId)
			{
				throw new NotSupportedException();
			}

			protected override void Load(IPropertyBag propertyBag)
			{
				throw new NotSupportedException();
			}

			protected override void Save(IPropertyBag propertyBag)
			{
				throw new NotSupportedException();
			}

			#endregion
		}

		[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
		[ComponentCategory(CategoryTypes.CATID_Decoder)]
		[ComponentCategory(CategoryTypes.CATID_PartyResolver)]
		private class MultipleStageComponent : PipelineComponent
		{
			#region Base Class Member Overrides

			public override string Description
			{
				get { throw new NotSupportedException(); }
			}

			protected override IBaseMessage ExecuteCore(IPipelineContext pipelineContext, IBaseMessage message)
			{
				throw new NotSupportedException();
			}

			public override void GetClassID(out Guid classId)
			{
				throw new NotSupportedException();
			}

			protected override void Load(IPropertyBag propertyBag)
			{
				throw new NotSupportedException();
			}

			protected override void Save(IPropertyBag propertyBag)
			{
				throw new NotSupportedException();
			}

			#endregion
		}
	}
}

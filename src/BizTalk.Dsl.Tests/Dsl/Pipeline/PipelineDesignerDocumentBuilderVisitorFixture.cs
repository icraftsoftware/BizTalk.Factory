#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using System.Linq;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.Linq;
using Microsoft.BizTalk.PipelineEditor;
using Microsoft.BizTalk.PipelineEditor.PipelineFile;
using NUnit.Framework;

namespace Be.Stateless.BizTalk.Dsl.Pipeline
{
	[TestFixture]
	public class PipelineDesignerDocumentBuilderVisitorFixture
	{
		[Test]
		public void CreateComponentInfo()
		{
			var componentDescriptor = new PipelineComponentDescriptor<ActivityTrackerComponent>(new ActivityTrackerComponent());

			var visitor = new Visitor();
			var componentInfo = visitor.CreateComponentInfo(componentDescriptor);

			var expectedProperties = new[] {
				new PropertyContents("Enabled", true),
				new PropertyContents("TrackingContextRetentionDuration", 60),
				new PropertyContents("TrackingModes", "Body"),
			};

			Assert.That(componentInfo.QualifiedNameOrClassId, Is.EqualTo(typeof(ActivityTrackerComponent).FullName));
			Assert.That(componentInfo.ComponentName, Is.EqualTo(typeof(ActivityTrackerComponent).Name));
			Assert.That(
				componentInfo.ComponentProperties
					.Cast<PropertyContents>()
					.SequenceEqual(expectedProperties, new LambdaComparer<PropertyContents>((left, right) => left.Name.Equals(right.Name) && left.Value.Equals(right.Value))));
		}

		[Test]
		public void CreatePipelineDocument()
		{
			var pipeline = new ReceivePipelineImpl();

			var visitor = new Visitor();
			var pipelineDocument = visitor.CreatePipelineDocument(pipeline);

			Assert.That(pipelineDocument.PolicyFilePath, Is.EqualTo("BTSReceivePolicy.xml"));
			Assert.That(pipelineDocument.Description, Is.EqualTo("A receive pipeline."));
			Assert.That(pipelineDocument.MajorVersion, Is.EqualTo(5));
			Assert.That(pipelineDocument.MinorVersion, Is.EqualTo(6));
		}

		[Test]
		public void CreateStageDocument()
		{
			var stage = new Stage(StageCategory.Decoder.Id);

			var visitor = new Visitor();
			var stageDocument = visitor.CreateStageDocument(stage);

			Assert.That(stageDocument.CategoryId, Is.EqualTo(StageCategory.Decoder.Id));
		}

		private class ReceivePipelineImpl : ReceivePipeline
		{
			public ReceivePipelineImpl()
			{
				Description = "A receive pipeline.";
				Version = new Version(5, 6);
			}
		}

		private class Visitor : PipelineDesignerDocumentBuilderVisitor
		{
			public new ComponentInfo CreateComponentInfo(IPipelineComponentDescriptor componentDescriptor)
			{
				return base.CreateComponentInfo(componentDescriptor);
			}

			public new Document CreatePipelineDocument<T>(Pipeline<T> pipeline) where T : IPipelineStageList
			{
				return base.CreatePipelineDocument(pipeline);
			}

			public new Microsoft.BizTalk.PipelineEditor.PipelineFile.Stage CreateStageDocument(IStage stage)
			{
				return base.CreateStageDocument(stage);
			}
		}
	}
}

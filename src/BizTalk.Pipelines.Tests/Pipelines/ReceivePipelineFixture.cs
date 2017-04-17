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

using System;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Be.Stateless.BizTalk.Dsl.Pipeline.Interpreters;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.Unit.Resources;
using Microsoft.BizTalk.Message.Interop;
using NUnit.Framework;
using Winterdom.BizTalk.PipelineTesting;

namespace Be.Stateless.BizTalk.Pipelines
{
	[TestFixture]
	public class ReceivePipelineFixture
	{
		[Test]
		public void DeferredPluginIsAlwaysExecuted()
		{
			using (var stream = ResourceManager.Load("Data.Content.zip"))
			{
				var pipeline = PipelineFactory.CreateReceivePipeline(typeof(ReceivePipelineInterpreter<ZipPassThruReceive>));
				var pluginComponent = (ContextBuilderComponent) pipeline.GetComponent(PipelineStage.Decode, 0);
				pluginComponent.ExecutionMode = PluginExecutionMode.Deferred;
				pluginComponent.Builder = typeof(ContextBuilder);

				var inputMessage = MessageHelper.CreateFromStream(stream);

				var outputMessages = pipeline.Execute(inputMessage);
				Assert.That(outputMessages, Is.Not.Null);
				Assert.That(outputMessages.Count, Is.EqualTo(1));

				Assert.That(outputMessages[0].GetProperty(TrackingProperties.Value1), Is.EqualTo("Plugin has been executed."));
			}
		}

		private class ZipPassThruReceive : ReceivePipeline
		{
			public ZipPassThruReceive()
			{
				Description = "Unit testable receive pipeline with zip-deflating capability.";
				Version = new Version(1, 0);
				VersionDependentGuid = new Guid("f68bbcd2-651e-4c0e-875f-40dce996c6f7");
				Stages.Decode
					.AddComponent(new ContextBuilderComponent { Enabled = true })
					.AddComponent(new ZipDecoderComponent { Enabled = true });
			}
		}

		private class ContextBuilder : IContextBuilder
		{
			#region IContextBuilder Members

			public void Execute(IBaseMessageContext context)
			{
				context.SetProperty(TrackingProperties.Value1, "Plugin has been executed.");
			}

			#endregion
		}
	}
}

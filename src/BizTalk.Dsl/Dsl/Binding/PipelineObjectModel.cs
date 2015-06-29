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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Dsl.Binding.Interop;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.Test.BizTalk.PipelineObjects;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	internal static class PipelineObjectModel<T> where T : Microsoft.BizTalk.PipelineOM.Pipeline, new()
	{
		internal static void CloneStageDefinitions(IPipelineStageList @this)
		{
			var pipeline = new PipelineFactory().CreatePipelineFromType(typeof(T));
			foreach (var stage in pipeline.Stages.Cast<Microsoft.Test.BizTalk.PipelineObjects.Stage>())
			{
				var matchingDslStage = ((StageList) @this).Single(s => s.Category.Id == stage.Id);
				foreach (var component in stage.GetComponentEnumerator().Cast<IBaseComponent>())
				{
					// http://stackoverflow.com/questions/6187470/cast-to-multiple-interfaces
					Expression<Func<PipelineComponent, IStage>> addComponentDelegateExpression = c => matchingDslStage.AddComponent(c);
					var openAddComponentDelegate = ((MethodCallExpression) addComponentDelegateExpression.Body).Method.GetGenericMethodDefinition();
					var closedAddComponentDelegate = openAddComponentDelegate.MakeGenericMethod(component.GetType());
					closedAddComponentDelegate.Invoke(matchingDslStage, new object[] { component });
				}
				// initialize and snapshot component's default property values as set by the pipeline *definition*
				foreach (var component in (List<IPipelineComponentDescriptor>) matchingDslStage.Components)
				{
					var propertyBag = new TrackingPropertyBag();
					component.Save(propertyBag, true, true);
					propertyBag.TrackChanges();
					component.Properties = propertyBag;
				}
			}
		}
	}
}

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
using System.Web.Mvc;

namespace Be.Stateless.Web.Mvc.Filters
{
	// see http://msdn.microsoft.com/en-us/magazine/hh781022.aspx
	public class PropertyFilterModelBinder : IModelBinder
	{
		#region IModelBinder Members

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var propertyName = bindingContext.ModelName;
			var rawValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).RawValue;
			var queryArgument = rawValue is string ? (string) rawValue : ((string[]) rawValue)[0];
			var comparisonPredicate = ComparisonPredicate.Parse(queryArgument);
			var filter = (IPropertyFilterInitializer) Activator.CreateInstance(bindingContext.ModelType);
			filter.Initialize(propertyName, comparisonPredicate);
			return filter;
		}

		#endregion
	}
}
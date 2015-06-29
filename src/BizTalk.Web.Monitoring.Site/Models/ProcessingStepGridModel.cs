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

using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Be.Stateless.BizTalk.Monitoring.Model;
using MvcContrib.UI.Grid;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models
{
	public class ProcessingStepGridModel : GridModel<ProcessingStep>
	{
		public ProcessingStepGridModel(HtmlHelper html)
		{
			Attributes(@class => "processing-step");
			Column.For(
				s => html.ActionLink(
					s.Name,
					"Details",
					"ProcessingStep",
					new RouteValueDictionary(new { id = s.ActivityID }),
					new Dictionary<string, object> { { "data-ajax-grid", "true" } })
				)
				.Named("Step Name");
			Column.For(s => s.BeginTime.ToLocalTime())
				.Named("Begin Time")
				.Attributes(r => new Dictionary<string, object> { { "title", r.Item.BeginTime } });
			Column.For(s => s.EndTime.HasValue ? s.EndTime.Value.ToLocalTime() : s.EndTime)
				.Named("End Time")
				.Attributes(r => r.Item.EndTime.HasValue ? new Dictionary<string, object> { { "title", r.Item.EndTime } } : new Dictionary<string, object>());
			Column.For(s => s.Status);
			RenderUsing(new DynamicGridRenderer<ProcessingStep>());
		}
	}
}

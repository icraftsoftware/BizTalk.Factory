#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.Web.Mvc.UI.Grid;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models
{
	public class ProcessGridModel : MvcContrib.UI.Grid.GridModel<Process>
	{
		public ProcessGridModel(HtmlHelper html)
		{
			Attributes(@class => "process");
			Column.For(p => html.ActionLink(p.FriendlyName, "Details", "Process", new { id = p.ActivityID }, null))
				.Named("Process Name")
				.Sortable(true, p => p.Name)
				.Attributes(r => new Dictionary<string, object> { { "title", r.Item.Name } })
				.Filterable(true, p => p.Name);
			Column.For(p => p.Value1)
				.Named("Value 1")
				.Sortable(true)
				.Filterable(true);
			Column.For(p => p.Value2)
				.Named("Value 2")
				.Sortable(true)
				.Filterable(true);
			Column.For(p => p.Value3)
				.Named("Value 3")
				.Sortable(true)
				.Filterable(true);
			Column.For(p => p.BeginTime.ToLocalTime())
				.Named("Begin Time")
				.Sortable(true, p => p.BeginTime)
				.Attributes(r => new Dictionary<string, object> { { "title", r.Item.BeginTime } });
			Column.For(p => p.EndTime.HasValue ? p.EndTime.Value.ToLocalTime() : p.EndTime)
				.Named("End Time")
				.Sortable(true, p => p.EndTime)
				.Attributes(r => r.Item.EndTime.HasValue ? new Dictionary<string, object> { { "title", r.Item.EndTime } } : new Dictionary<string, object>());
			Column.For(p => p.Status)
				.Named("Status")
				.Sortable(true)
				.Filterable(true);
			RenderUsing(new SortableGridRenderer<Process>());
		}
	}
}

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
using System.Web.Routing;
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.BizTalk.Web.Monitoring.Site.Helpers;
using Be.Stateless.Web.Mvc.UI.Grid;
using MvcContrib.UI.Grid;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models
{
	public class MessagingStepGridModel : GridModel<MessagingStep>
	{
		public MessagingStepGridModel(HtmlHelper html) : this(html, Filiation.Any, GridFilteringModes.StepInto) { }

		public MessagingStepGridModel(HtmlHelper html, Filiation filiation, GridFilteringModes modes)
		{
			Attributes(@class => "messaging-step");
			Column.For(
				s => html.ActionLink(
					s.Name ?? Resource.Unknown,
					"Details",
					"MessagingStep",
					new RouteValueDictionary(new { id = s.ActivityID }),
					new Dictionary<string, object> { { "data-ajax-grid", "true" } })
				)
				.Named("Port Name")
				.Sortable(modes.Sortable(), s => s.Name)
				.Filterable(modes.Steppable(), s => s.Name);
			// TODO Column.For(s => s.MessageBody.IsNullOrEmpty() ? null : Html.ActionButton("Save", "Save", "MessagingStep", new { id = s.ActivityID }))
			Column.For(s => html.ActionButton("Download", "Save", "MessagingStep", new { id = s.ActivityID }))
				.Named("Body Action")
				.Encode(false)
				.Sortable(false);
			if (modes.Sortable() && filiation != Filiation.Orphan)
				Column.For(s => html.ActionLink("Process", "Process", "MessagingStep", new { id = s.ActivityID }, null))
					.Named("Go to...")
					.Encode(false)
					.Sortable(false);
			Column.For(s => s.Value1)
				.Named("Value 1")
				.Sortable(modes.Sortable())
				.Filterable(modes.Sortable());
			Column.For(s => s.Value2)
				.Named("Value 2")
				.Sortable(modes.Sortable())
				.Filterable(modes.Sortable());
			Column.For(s => s.Value3)
				.Named("Value 3")
				.Sortable(modes.Sortable())
				.Filterable(modes.Sortable());
			Column.For(s => s.BeginTime.ToLocalTime())
				.Named("Time")
				.Sortable(modes.Sortable(), s => s.BeginTime)
				.Attributes(s => new Dictionary<string, object> { { "title", s.Item.BeginTime } });
			Column.For(s => s.Status)
				.Sortable(modes.Sortable())
				.Filterable(modes.Sortable());
			Column.For(s => s.MessageSize)
				.Sortable(modes.Sortable());
			Column.For(s => s.FriendlyMessageType)
				.Named("Message Type")
				.Sortable(modes.Sortable(), s => s.MessageType)
				.Attributes(s => new Dictionary<string, object> { { "title", s.Item.MessageType } })
				.Filterable(modes.Sortable(), s => s.MessageType);
			Column.For(s => s.TransportType)
				.Sortable(modes.Sortable())
				.Filterable(modes.Sortable());
			RenderUsing(new DynamicGridRenderer<MessagingStep>());
		}
	}
}

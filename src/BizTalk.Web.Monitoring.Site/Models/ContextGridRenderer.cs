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

using System.Web.Mvc;
using MvcContrib.UI.Grid;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models
{
	public class ContextGridRenderer<T> : HtmlTableGridRenderer<T> where T : class
	{
		protected override void RenderGridStart()
		{
			if (!GridModel.Attributes.ContainsKey("class")) GridModel.Attributes["class"] = "context";
			else GridModel.Attributes["class"] = "context " + GridModel.Attributes["class"];
			base.RenderGridStart();
		}

		protected override void RenderHeadStart()
		{
			// only way found to have the 1st column's width render correctly in both chrome and ie
			// see http://www.w3.org/TR/html4/struct/tables.html#h-11.2.4.4
			var promoted = new TagBuilder("col");
			promoted.AddCssClass("promoted");

			var property = new TagBuilder("col");
			property.AddCssClass("property");

			var value = new TagBuilder("col");
			value.AddCssClass("value");

			var colgroup = new TagBuilder("colgroup") {
				InnerHtml = promoted.ToString() + property + value
			};
			RenderText(colgroup.ToString());

			base.RenderHeadStart();
		}
	}
}
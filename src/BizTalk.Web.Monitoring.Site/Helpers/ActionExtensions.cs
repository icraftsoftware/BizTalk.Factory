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

using System.Web;
using System.Web.Mvc;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Helpers
{
	public static class ActionExtensions
	{
		public static IHtmlString ActionButton(this HtmlHelper helper, string text, string actionName, string controllerName)
		{
			return helper.ActionButton(text, actionName, controllerName, null);
		}

		public static IHtmlString ActionButton(this HtmlHelper helper, string text, string actionName, string controllerName, object routeValues)
		{
			var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
			var url = urlHelper.Action(actionName, controllerName, routeValues);
			return new HtmlString(string.Format(@"<input type='button' onclick=""javacript: window.location='{1}';"" value='{0}' />", text, url));
		}
	}
}
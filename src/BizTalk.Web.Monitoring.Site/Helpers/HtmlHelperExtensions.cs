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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Helpers
{
	public static class HtmlHelperExtensions
	{
		public static IHtmlString AsXml(this HtmlHelper helper, string text)
		{
			return helper.Raw(
				text.IsNullOrEmpty()
					? Resource.NoData
					: string.Format(
						"<pre>{0}</pre>",
						helper.Encode(
							(text[0] == '<')
								? XDocument.Parse(text).ToString(SaveOptions.OmitDuplicateNamespaces)
								: text)));
		}

		/// <summary>
		/// Returns a check box input element by using the specified HTML helper,
		/// the name of the form field, and a value to indicate whether the check
		/// box is selected. Notice the check box can be made read-only without
		/// dimming its visual and that it has no hidden affiliated input.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
		/// <param name="name">The name of the form field.</param>
		/// <param name="isChecked">true to select the check box; otherwise, false.</param>
		/// <param name="isReadOnly">true to make the check box read-only; otherwise, false.</param>
		/// <returns>An input element whose type attribute is set to "checkbox".</returns>
		public static IHtmlString CheckBox(this HtmlHelper htmlHelper, string name, bool isChecked, bool isReadOnly)
		{
			var tagBuilder = new TagBuilder("input");
			tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.CheckBox));
			tagBuilder.MergeAttribute("name", name);
			if (isChecked) tagBuilder.MergeAttribute("checked", "checked");
			if (isReadOnly) tagBuilder.MergeAttribute("data-readonly", "true");
			return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString DisplayFor(this HtmlHelper helper, MessageBody message)
		{
			return message == null || !message.HasContent
				? helper.Raw(Resource.NoData)
				: message.HasBeenClaimed
					? message.ClaimAvailabe
						? helper.Raw(
							string.Format(
								"<pre>{0} character preview of large message body.<br/><br/>{1}</pre>",
								message.Body.Length,
								helper.Encode(Regex.Replace(message.Body, @"\p{Cc}", ""))))
						: helper.Raw(message.Body)
					: (message.MimeType.Equals("text/html", StringComparison.OrdinalIgnoreCase)
						? helper.Raw(helper.Encode(message.Body))
						: message.MimeType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
							? helper.AsXml(message.Body)
							// remove any control character (matched by \p{Cc}) from message body before displaying it
							: helper.Raw(string.Format("<pre>{0}</pre>", helper.Encode(Regex.Replace(message.Body, @"\p{Cc}", "")))));
		}

		public static IEnumerable<IGrouping<string, MessageContext.Property>> MessageContextPropertiesGroupedByNamespace(
			this HtmlHelper helper,
			IEnumerable<MessageContext.Property> properties)
		{
			return properties
				.Where(p => !p.IsPipelineConfigurationData())
				.OrderBy(p => p.Name) // specify the ordering of the properties within each group
				.GroupBy(p => p.Namespace)
				.OrderBy(g => g.Key);
		}

		public static IEnumerable<IGrouping<string, MessageContext.Property>> MessageContextPipelineDataByNamespace(
			this HtmlHelper helper,
			IEnumerable<MessageContext.Property> properties)
		{
			// TODO return nothing when not BizTalk Administrator role
			return properties
				.Where(p => p.IsPipelineConfigurationData())
				.OrderBy(p => p.Name)
				.GroupBy(p => p.Namespace)
				.OrderBy(g => g.Key);
		}

		private static bool IsPipelineConfigurationData(this MessageContext.Property property)
		{
			return property.Name.Contains("Config") || (property.Name.Contains("Pipeline") && property.Namespace.Equals(Resource.BiztalkSystemPropertiesNamespace));
		}

		public static IHtmlString SpanFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
		{
			return helper.SpanFor(expression, null);
		}

		public static IHtmlString SpanFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
		{
			var getter = expression.Compile();
			var value = Convert.ToString(getter(helper.ViewData.Model));
			var span = new TagBuilder("span");
			span.MergeAttributes(new RouteValueDictionary(htmlAttributes));
			span.SetInnerText(value);
			return MvcHtmlString.Create(span.ToString());
		}
	}
}

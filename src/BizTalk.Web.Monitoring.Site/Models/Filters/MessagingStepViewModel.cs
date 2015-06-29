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
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.Web.Mvc.Filters;
using MvcContrib.Pagination;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models.Filters
{
	public class MessagingStepViewModel : MonitoringGridFilterOptions
	{
		public FiliationPropertyFilter Filiation
		{
			// TODO simpler way to set up a default value
			// ensure default value
			get { return _filiation ?? new FiliationPropertyFilter(); }
			set { _filiation = value; }
		}

		public IPagination<MessagingStep> MessagingSteps { get; set; }

		public PropertyFilter<string> MessageType { get; set; }

		public PropertyFilter<string> TransportType { get; set; }
		public static readonly IEnumerable<SelectListItem> FiliationOptions = FiliationPropertyFilter.Options;

		// ReSharper disable LocalizableElement
		public static readonly IEnumerable<SelectListItem> StatusOptions = new[] {
			new SelectListItem { Text = "any", Value = "*" },
			new SelectListItem { Text = "a failed", Value = "f" },
			new SelectListItem { Text = "a received", Value = "r" },
			new SelectListItem { Text = "a sent", Value = "s" }
		};
		// ReSharper restore LocalizableElement

		private FiliationPropertyFilter _filiation;
	}
}

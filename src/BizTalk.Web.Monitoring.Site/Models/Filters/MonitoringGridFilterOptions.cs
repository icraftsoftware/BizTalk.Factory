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
using Be.Stateless.Web.Mvc.Filters;
using Be.Stateless.Web.Mvc.GridFilters;
using MvcContrib.Sorting;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models.Filters
{
	public abstract class MonitoringGridFilterOptions : GridFilter
	{
		// TODO simpler way to set up a default value
		protected MonitoringGridFilterOptions()
		{
			// default sort order
			Column = "BeginTime";
			Direction = SortDirection.Descending;
			// default pagination
			Page = 1;
			Size = 40;
		}

		// TODO ctxt menu earlier or later
		//public string BeginTime { get; set; }

		// TODO ctxt menu earlier or later
		//public string EndTime { get; set; }

		public PropertyFilter<string> Name { get; set; }

		public StatusPropertyFilter Status { get; set; }

		public KeyPropertyFilter Value { get; set; }

		public PropertyFilter<string> Value1 { get; set; }

		public PropertyFilter<string> Value2 { get; set; }

		public PropertyFilter<string> Value3 { get; set; }

		public WithinPropertyFilter Within
		{
			// TODO simpler way to set up a default value
			// ensure default lower time bound
			get { return _within ?? new WithinPropertyFilter(); }
			set { _within = value; }
		}

		public static readonly IEnumerable<SelectListItem> WithinOptions = WithinPropertyFilter.Options;
		private WithinPropertyFilter _within;
	}
}

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
using System.Linq;
using System.Web.Mvc;
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.Extensions;
using MvcContrib.Pagination;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models.Filters
{
	public class ProcessViewModel : MonitoringGridFilterOptions
	{
		#region Base Class Member Overrides

		public override IQueryable<TQ> Filter<TQ>(IQueryable<TQ> queryable)
		{
			return MessagingStepActivityID.IsNullOrEmpty()
				? base.Filter(queryable)
				// when navigating to its process from a messaging step, no other filter should have been passed in query string
				: (IQueryable<TQ>) ((IQueryable<Process>) queryable).Where(p => p.MessagingSteps.Any(ms => ms.ActivityID == MessagingStepActivityID));
		}

		#endregion

		// only used when redirecting from messaging step controller to process controller to navigate from a messaging step to its process
		public string MessagingStepActivityID { get; set; }

		public IPagination<Process> Processes { get; set; }

		public static readonly IEnumerable<SelectListItem> ProcessOptions = (
			new[] { new SelectListItem { Text = "Any", Value = "*" } }
				.Union(
					ActivityContext.ProcessDescriptors.Select(pd => new SelectListItem { Text = pd.FriendlyName, Value = pd.Name })
				)
			).ToArray();

		// ReSharper disable LocalizableElement
		public static readonly IEnumerable<SelectListItem> StatusOptions = new[] {
			new SelectListItem { Text = "any status", Value = "*" },
			new SelectListItem { Text = "a completed status", Value = "c" },
			new SelectListItem { Text = "a pending status", Value = "p" },
			new SelectListItem { Text = "a failed status", Value = "f" },
			new SelectListItem { Text = "failed steps", Value = "fs" },
			new SelectListItem { Text = "no failed steps", Value = "nfs" }
		};
		// ReSharper restore LocalizableElement
	}
}

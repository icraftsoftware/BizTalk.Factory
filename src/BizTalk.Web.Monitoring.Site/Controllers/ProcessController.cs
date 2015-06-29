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

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.BizTalk.Web.Monitoring.Site.Models.Filters;
using Be.Stateless.Extensions;
using Be.Stateless.Web.Mvc.GridFilters;
using MvcContrib.Pagination;
using MvcContrib.Sorting;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Controllers
{
	public class ProcessController : Controller
	{
		#region Base Class Member Overrides

		protected override void Dispose(bool disposing)
		{
			_repository.Dispose();
			base.Dispose(disposing);
		}

		#endregion

		//
		// GET: /Process/

		public ViewResult Index(ProcessViewModel model)
		{
			// TODO simpler way ??
			// cache lower time bound in TempData for when we come back to this or an other page
			TempData["Within"] = model.Within.IfNotNull(f => f.RawValue);

			//if ("fs".Contains(filter.Status) || !filter.Value.IsNullOrEmpty()) _repository.CommandTimeout = 90;

			model.Processes = _repository.Processes
				.FilterBy(model)
				.OrderBy(model.Column, model.Direction)
				.AsPagination(model.Page, model.Size);
			return View(model);
		}

		//
		// GET: /Process/Details/9d783ae8-d214-46e6-9015-74f6b353b85f

		public ViewResult Details(string id)
		{
			var process = _repository.Processes
				.Include(p => p.ProcessingSteps)
				.Include(p => p.MessagingSteps)
				.Single(p => p.ActivityID == id);
			return View(process);
		}

		//
		// GET: /Process/Reset

		public RedirectToRouteResult Reset()
		{
			return RedirectToAction("Index");
		}

		private readonly ActivityContext _repository = new ActivityContext();
	}
}

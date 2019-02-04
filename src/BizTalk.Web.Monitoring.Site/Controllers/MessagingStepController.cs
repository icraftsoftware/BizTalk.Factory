#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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
	public class MessagingStepController : Controller
	{
		#region Base Class Member Overrides

		protected override void Dispose(bool disposing)
		{
			_repository.Dispose();
			base.Dispose(disposing);
		}

		#endregion

		//
		// GET: /MessagingStep/

		public ViewResult Index(MessagingStepViewModel model)
		{
			// TODO simpler way ??
			// cache lower time bound in TempData for when we come back to this or an other page
			TempData["Within"] = model.Within.IfNotNull(f => f.RawValue);

			//if (filter.Filiation != Filiation.Any) _repository.CommandTimeout = 90;

			model.MessagingSteps = _repository.MessagingSteps
				.FilterBy(model)
				.OrderBy(model.Column, model.Direction)
				.AsPagination(model.Page, model.Size);
			return View(model);
		}

		//
		// GET: /MessagingStep/Details/9d783ae8-d214-46e6-9015-74f6b353b85f

		public ActionResult Details(string id)
		{
			var messagingStep = _repository.MessagingSteps
				.Include(ms => ms.Context)
				.Include(ms => ms.Message)
				.Include(ms => ms.Processes)
				.Single(ms => ms.ActivityID == id);

			ViewBag.Filiation = messagingStep.Processes.Any() ? Filiation.Filiated : Filiation.Orphan;
			return Request.IsAjaxRequest()
				? (ActionResult) PartialView("_Details", messagingStep)
				: View(Enumerable.Repeat(messagingStep, 1));
		}

		//
		// GET: /MessagingStep/Process/9d783ae8-d214-46e6-9015-74f6b353b85f

		public RedirectToRouteResult Process(string id)
		{
			var messagingStep = _repository.MessagingSteps
				.Include(ms => ms.Processes)
				.Single(ms => ms.ActivityID == id);

			if (messagingStep.Processes.Count() != 1) return RedirectToAction("Index", "Process", new { MessagingStepActivityID = id });

			var processActivityId = messagingStep.Processes
				.SingleOrDefault()
				.IfNotNull(p => p.ActivityID);
			return RedirectToAction("Details", "Process", new { id = processActivityId });
		}

		//
		// GET: /MessagingStep/Save/9d783ae8-d214-46e6-9015-74f6b353b85f

		public ActionResult Save(string id)
		{
			var message = _repository.MessagingSteps
				.Single(m => m.ActivityID == id)
				.Message;
			return message != null && message.HasContent
				? (ActionResult) File(message.Stream, message.MimeType, message.ReceivedFileName)
				// HACK workaround to avoid NullReferenceException but should Disable Download button instead, see MessagingStepGridModel
				: new EmptyResult();
		}

		//
		// GET: /MessagingStep/Reset

		public RedirectToRouteResult Reset()
		{
			return RedirectToAction("Index");
		}

		private readonly ActivityContext _repository = new ActivityContext();
	}
}

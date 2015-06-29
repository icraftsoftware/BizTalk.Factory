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

using System.Linq;
using System.Web.Mvc;
using Be.Stateless.BizTalk.Monitoring.Model;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Controllers
{
	public class ProcessingStepController : Controller
	{
		#region Base Class Member Overrides

		protected override void Dispose(bool disposing)
		{
			_repository.Dispose();
			base.Dispose(disposing);
		}

		#endregion

		//
		// GET: /ProcessingStep/Details/9d783ae8-d214-46e6-9015-74f6b353b85f

		public PartialViewResult Details(string id)
		{
			var processingStep = _repository.ProcessingSteps
				.Single(p => p.ActivityID == id);
			return PartialView("_Details", processingStep);
		}

		private readonly ActivityContext _repository = new ActivityContext();
	}
}

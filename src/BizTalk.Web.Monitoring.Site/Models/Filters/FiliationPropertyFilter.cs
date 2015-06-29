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
using System.Web.Mvc;
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.Web.Mvc.Filters;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models.Filters
{
	public class FiliationPropertyFilter : PropertyFilter<Filiation>
	{
		// TODO simpler way to set up a default value
		public FiliationPropertyFilter()
		{
			RawValue = Options.First().Value;
			Value = (Filiation) Enum.Parse(typeof(Filiation), RawValue);
		}

		#region Base Class Member Overrides

		public override IQueryable<TQ> Filter<TQ>(IQueryable<TQ> queryable)
		{
			// call the TQ type-specific overload, i.e. either MessagingStep or Process
			return ((dynamic) this).Filter(queryable);
		}

		#endregion

		public IQueryable<MessagingStep> Filter(IQueryable<MessagingStep> messagingSteps)
		{
			switch (Value)
			{
				case Filiation.Filiated:
					return messagingSteps.Where(s => s.Processes.Any());
				case Filiation.Orphan:
					return messagingSteps.Where(s => !s.Processes.Any());
				default:
					return messagingSteps;
			}
		}

		// ReSharper disable LocalizableElement
		public static readonly IEnumerable<SelectListItem> Options = new[] {
			new SelectListItem { Text = "Any", Value = Filiation.Any.ToString() },
			new SelectListItem { Text = "Filiated", Value = Filiation.Filiated.ToString() },
			new SelectListItem { Text = "Orphan", Value = Filiation.Orphan.ToString() }
		};
		// ReSharper restore LocalizableElement
	}
}

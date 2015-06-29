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
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.Web.Mvc.Filters;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models.Filters
{
	public class StatusPropertyFilter : PropertyFilter<Status>
	{
		#region Base Class Member Overrides

		public override IQueryable<TQ> Filter<TQ>(IQueryable<TQ> queryable)
		{
			// call the TQ type-specific overload, i.e. either MessagingStep or Process
			return ((dynamic) this).Filter(queryable);
		}

		#endregion

		public IQueryable<MessagingStep> Filter(IQueryable<MessagingStep> messagingSteps)
		{
			if (Value == Status.Failed || Value == Status.FailedStep) Value = Status.FailedMessage;
			return base.Filter(messagingSteps);
		}

		public IQueryable<Process> Filter(IQueryable<Process> processes)
		{
			var f = Status.Failed.ToString();
			var fm = Status.FailedMessage.ToString();

			if (Value == Status.FailedStep && Operator == ComparisonOperator.Equal)
			{
				return processes
					.Where(p => p.MessagingSteps.Any(ms => ms.Status == fm) || p.ProcessingSteps.Any(ps => ps.Status == f));
			}

			if (Value == Status.FailedStep && Operator == ComparisonOperator.NotEqual)
			{
				return processes
					.Where(p => p.MessagingSteps.All(ms => ms.Status != fm) && p.ProcessingSteps.All(ps => ps.Status != f));
			}

			if (Value == Status.FailedMessage && Operator == ComparisonOperator.Equal)
			{
				// ReSharper disable ImplicitlyCapturedClosure
				return processes
					.Where(p => p.MessagingSteps.Any(ms => ms.Status == fm));
				// ReSharper restore ImplicitlyCapturedClosure
			}

			if (Value == Status.FailedMessage && Operator == ComparisonOperator.NotEqual)
			{
				// ReSharper disable ImplicitlyCapturedClosure
				return processes
					.Where(p => p.MessagingSteps.All(ms => ms.Status != fm));
				// ReSharper restore ImplicitlyCapturedClosure
			}

			return base.Filter(processes);
		}
	}
}

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
using System.Linq;
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.Web.Mvc.Filters;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models.Filters
{
	public class KeyPropertyFilter : PropertyFilter<string>
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
			switch (Operator)
			{
				case ComparisonOperator.Any:
					return messagingSteps;

				case ComparisonOperator.Equal:
					return messagingSteps.Where(s => s.Value1 == Value || s.Value2 == Value || s.Value3 == Value);

				case ComparisonOperator.NotEqual:
					return messagingSteps.Where(s => s.Value1 != Value && s.Value2 != Value && s.Value3 != Value);

				case ComparisonOperator.Like:
					return messagingSteps.Where(s => s.Value1.Contains(Value) || s.Value2.Contains(Value) || s.Value3.Contains(Value));

				case ComparisonOperator.Unlike:
					return messagingSteps.Where(s => !s.Value1.Contains(Value) && !s.Value2.Contains(Value) && !s.Value3.Contains(Value));

				default:
					throw new InvalidOperationException(string.Format("{0} comparison operator.", Operator));
			}
		}

		public IQueryable<Process> Filter(IQueryable<Process> processes)
		{
			switch (Operator)
			{
				case ComparisonOperator.Any:
					return processes;

				case ComparisonOperator.Equal:
					return processes.Where(p => p.Value1 == Value || p.Value2 == Value || p.Value3 == Value);
					// TODO provide visual for any process' step containing the passed business value
					//return processes.Where(p => p.RelatedMessagingSteps
					//   .Any(ms => ms.MessagingStep.Value1 == Value || ms.MessagingStep.Value2 == Value || ms.MessagingStep.Value3 == Value));

				case ComparisonOperator.NotEqual:
					return processes.Where(p => p.Value1 != Value && p.Value2 != Value && p.Value3 != Value);
					// TODO provide visual for any process' step containing the passed business value
					//return processes.Where(p => p.RelatedMessagingSteps
					//   .All(ms => ms.MessagingStep.Value1 != Value && ms.MessagingStep.Value2 != Value && ms.MessagingStep.Value3 != Value));

				case ComparisonOperator.Like:
					return processes.Where(p => p.Value1.Contains(Value) || p.Value2.Contains(Value) || p.Value3.Contains(Value));
					// TODO provide visual for any process' step containing the passed business value
					//return processes.Where(p => p.RelatedMessagingSteps
					//   .Any(ms => ms.MessagingStep.Value1.Contains(Value) || ms.MessagingStep.Value2.Contains(Value) || ms.MessagingStep.Value3.Contains(Value)));

				case ComparisonOperator.Unlike:
					return processes.Where(p => !p.Value1.Contains(Value) && !p.Value2.Contains(Value) && !p.Value3.Contains(Value));
					// TODO provide visual for any process' step containing the passed business value
					//return processes.Where(p => p.RelatedMessagingSteps
					//   .All(ms => !ms.MessagingStep.Value1.Contains(Value) && !ms.MessagingStep.Value2.Contains(Value) && !ms.MessagingStep.Value3.Contains(Value)));

				default:
					throw new InvalidOperationException(string.Format("{0} comparison operator.", Operator));
			}
		}
	}
}

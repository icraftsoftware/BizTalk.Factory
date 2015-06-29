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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Be.Stateless.BizTalk.Monitoring.Model;
using Be.Stateless.Extensions;
using Be.Stateless.Web.Mvc.Filters;

namespace Be.Stateless.BizTalk.Web.Monitoring.Site.Models.Filters
{
	public class WithinPropertyFilter : PropertyFilter<DateTime>
	{
		#region Nested type: DateTimeConverter

		public class DateTimeConverter : TypeConverter
		{
			internal static DateTime ToDateTime(string text)
			{
				TimeSpan offset;
				var beginTime = text.IfNotNull(s => s.Equals("*", StringComparison.OrdinalIgnoreCase))
					? DateTime.MinValue
					: TimeSpan.TryParse("-" + text + ":00:00", out offset)
						? DateTime.Now.Add(offset)
						: DateTime.Now.AddHours(-1);
				return beginTime.ToUniversalTime();
			}

			#region Base Class Member Overrides

			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value == null) throw new ArgumentNullException("value");

				return value is string
					? ToDateTime((string) value)
					: base.ConvertFrom(context, culture, value);
			}

			#endregion
		}

		#endregion

		static WithinPropertyFilter()
		{
			// ensure custom conversion from string to DateTime
			// TODO should use a converter specified property-wise
			// see http://stackoverflow.com/questions/458935/extend-a-typeconverter
			TypeDescriptor.AddAttributes(typeof(DateTime), new TypeConverterAttribute(typeof(DateTimeConverter)));
		}

		// TODO simpler way to set up a default value
		public WithinPropertyFilter()
		{
			// default lower time bound
			Operator = ComparisonOperator.Greater;
			RawValue = Options.First().Value;
			Value = DateTimeConverter.ToDateTime(RawValue);
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
			return messagingSteps.Where(s => s.BeginTime >= Value || s.BeginTime == null);
		}

		public IQueryable<Process> Filter(IQueryable<Process> processes)
		{
			return processes.Where(p => p.BeginTime >= Value);
		}

		// ReSharper disable LocalizableElement
		public static readonly IEnumerable<SelectListItem> Options = new[] {
			new SelectListItem { Text = "last hour", Value = "1" },
			new SelectListItem { Text = "last 2 hours", Value = "2" },
			new SelectListItem { Text = "last day", Value = "1.0" },
			new SelectListItem { Text = "last 2 days", Value = "2.0" },
			new SelectListItem { Text = "last week", Value = "7.0" },
			new SelectListItem { Text = "any time", Value = "*" }
		};
		// ReSharper restore LocalizableElement
	}
}

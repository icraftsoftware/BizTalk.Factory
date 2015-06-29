#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
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
using System.Reflection;
using Be.Stateless.Web.Mvc.Filters;
using MvcContrib.UI.Grid;

namespace Be.Stateless.Web.Mvc.GridFilters
{
	public abstract class GridFilter : GridSortOptions
	{
		#region Pagination Options

		public int Page { get; set; }

		public int Size { get; set; }

		#endregion

		public virtual IQueryable<TQ> Filter<TQ>(IQueryable<TQ> queryable)
		{
			return GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public)
				// avoid to fetch value from other properties than IFilterOptions ones
				.Where(pi => typeof(IPropertyFilter).IsAssignableFrom(pi.PropertyType))
				.Select(pi => pi.GetValue(this, null))
				// returns a collection of IFilterOptions and skip null instances as well
				.OfType<IPropertyFilter>()
				// avoid calling Filter() on filterOption when it will not setup any filter
				.Where(filterOption => filterOption.Operator != ComparisonOperator.Any)
				.Aggregate(queryable, (currentQueryable, filterOption) => filterOption.Filter(currentQueryable));
		}
	}
}
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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MvcContrib.UI.Grid;

namespace Be.Stateless.Web.Mvc.UI.Grid
{
	public static class GridColumnExtensions
	{
		public static IGridColumn<T> Filterable<T>(this IGridColumn<T> column, bool isColumnFilterable) where T : class
		{
			return column.Filterable(isColumnFilterable, ((GridColumn<T>) column).Name);
		}

		public static IGridColumn<T> Filterable<T, TQ>(this IGridColumn<T> column, bool isColumnFilterable, Expression<Func<T, TQ>> filterProperty) where T : class
		{
			if (filterProperty.Body.NodeType != ExpressionType.MemberAccess) throw new ArgumentException("filterProperty");
			var propertyName = ((MemberExpression) filterProperty.Body).Member.Name;
			return column.Filterable(isColumnFilterable, propertyName);
		}

		private static IGridColumn<T> Filterable<T>(this IGridColumn<T> column, bool isColumnFilterable, string filterName) where T : class
		{
			return !isColumnFilterable
				? column
				: column.HeaderAttributes(
					new Dictionary<string, object> {
						{ "data-filter", "true" },
						{ "data-filter-name", filterName }
					});
		}

		public static IGridColumn<T> Sortable<T, TQ>(this IGridColumn<T> column, bool isColumnSortable, Expression<Func<T, TQ>> sortProperty) where T : class
		{
			if (sortProperty.Body.NodeType != ExpressionType.MemberAccess) throw new ArgumentException("sortProperty");
			var propertyName = ((MemberExpression) sortProperty.Body).Member.Name;
			return !isColumnSortable
				? column
				: column.Sortable(true).SortColumnName(propertyName);
		}
	}
}
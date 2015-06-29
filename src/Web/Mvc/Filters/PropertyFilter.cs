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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Be.Stateless.Web.Mvc.Filters
{
	[PropertyFilterModelBinder]
	public class PropertyFilter<T> : IPropertyFilter, IPropertyFilterInitializer
	{
		#region IPropertyFilter Members

		public ComparisonOperator Operator { get; protected set; }

		public virtual IQueryable<TQ> Filter<TQ>(IQueryable<TQ> queryable)
		{
			// see http://msdn.microsoft.com/en-us/library/bb882637.aspx
			var parameterExpression = Expression.Parameter(typeof(TQ), "p");
			var filteredProperty = Expression.Property(parameterExpression, Name);
			// use converter if exists to give a chance to types (like enum) to provide a custom conversion
			var converter = TypeDescriptor.GetConverter(typeof(T));
			var value = converter != null && converter.CanConvertTo(filteredProperty.Type)
				? converter.ConvertTo(Value, filteredProperty.Type)
				: Convert.ChangeType(Value, filteredProperty.Type);
			var filterValue = Expression.Constant(value, filteredProperty.Type);

			Expression predicateBody;
			switch (Operator)
			{
				case ComparisonOperator.Any:
					return queryable;

				case ComparisonOperator.Equal:
					predicateBody = Expression.Equal(filteredProperty, filterValue);
					break;

				case ComparisonOperator.NotEqual:
					predicateBody = Expression.NotEqual(filteredProperty, filterValue);
					break;

				case ComparisonOperator.Like:
					if (typeof(T) != typeof(string))
						throw new InvalidOperationException(
							string.Format(
								"{0} operator is only supported for value of type string, and not {1}.",
								Operator,
								typeof(T)));
					Expression<Func<string, bool>> like = s => s.Contains((string) (object) Value);
					predicateBody = ((MethodCallExpression) like.Body).Update(filteredProperty, new[] { filterValue });
					break;

				case ComparisonOperator.Unlike:
					if (typeof(T) != typeof(string))
						throw new InvalidOperationException(
							string.Format(
								"{0} operator is only supported for value of type string, and not {1}.",
								Operator,
								typeof(T)));
					Expression<Func<string, bool>> unlike = s => s.Contains((string) (object) Value);
					predicateBody = Expression.Not(((MethodCallExpression) unlike.Body).Update(filteredProperty, new[] { filterValue }));
					break;

				default:
					throw new InvalidOperationException(string.Format("{0} comparison operator.", Operator));
			}

			return queryable.Where(Expression.Lambda<Func<TQ, bool>>(predicateBody, new[] { parameterExpression }));
		}

		#endregion

		#region IPropertyFilterInitializer Members

		void IPropertyFilterInitializer.Initialize(string propertyName, ComparisonPredicate predicate)
		{
			Name = propertyName;
			Operator = predicate.Operator;
			RawValue = predicate.RawValue;
			if (Operator != ComparisonOperator.Any)
			{
				// use converter if exists to give a chance to types (like enum) to provide a custom conversion
				// TODO should use a converter specified property-wise, see http://stackoverflow.com/questions/458935/extend-a-typeconverter
				var converter = TypeDescriptor.GetConverter(typeof(T));
				Value = (T) (converter != null && converter.CanConvertFrom(typeof(string))
					? converter.ConvertFrom(predicate.Value)
					: Convert.ChangeType(predicate.Value, typeof(T)));
			}
		}

		#endregion

		public string Name { get; protected set; }

		public string RawValue { get; protected set; }

		public T Value { get; protected set; }

		public override string ToString()
		{
			return RawValue;
		}
	}
}
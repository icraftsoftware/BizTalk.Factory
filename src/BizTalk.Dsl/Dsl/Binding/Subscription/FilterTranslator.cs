﻿#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using System.Linq.Expressions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.Extensions;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.B2B.PartnerManagement;

namespace Be.Stateless.BizTalk.Dsl.Binding.Subscription
{
	internal static class FilterTranslator
	{
		public static FilterPredicate Translate(Expression<Func<bool>> expression)
		{
			try
			{
				return TranslateFilterPredicate(expression.Body);
			}
			catch (TpmException exception)
			{
				throw new NotSupportedException(
					string.Format(
						"Cannot translate FilterPredicate \"{0}\" because {1}",
						expression,
						exception.Message),
					exception);
			}
		}

		private static FilterPredicate TranslateFilterPredicate(Expression expression)
		{
			var binaryExpression = expression as BinaryExpression;
			if (binaryExpression != null)
			{
				var filterPredicate = new FilterPredicate();
				var groups = TranslateFilterGroup(binaryExpression);
				groups.Each(g => filterPredicate.Groups.Add(g));
				return filterPredicate;
			}

			throw new NotSupportedException(
				string.Format(
					"Cannot translate FilterPredicate \"{0}\" because {1} node is not supported.",
					expression,
					expression.NodeType));
		}

		private static IEnumerable<FilterGroup> TranslateFilterGroup(Expression expression)
		{
			var binaryExpression = expression as BinaryExpression;
			if (binaryExpression != null) return TranslateFilterGroup(binaryExpression);

			throw new NotSupportedException(
				string.Format(
					"Cannot translate FilterPredicate \"{0}\" because {1} node is not supported.",
					expression,
					expression.NodeType));
		}

		private static IEnumerable<FilterGroup> TranslateFilterGroup(BinaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.ExclusiveOr:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					return TranslateFilterGroup(expression.Left)
						.Concat(TranslateFilterGroup(expression.Right));
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					return new[] {
						BuildFilterGroup(TranslateFilterStatement(expression))
					};
				default:
					return new[] {
						BuildFilterGroup(TranslateFilterStatement(expression))
					};
			}
		}

		private static FilterGroup BuildFilterGroup(IEnumerable<FilterStatement> statements)
		{
			var group = new FilterGroup();
			statements.Each(s => group.Statements.Add(s));
			return group;
		}

		private static IEnumerable<FilterStatement> TranslateFilterStatement(Expression expression)
		{
			var binaryExpression = expression as BinaryExpression;
			if (binaryExpression != null) return TranslateFilterStatement(binaryExpression);

			throw new NotSupportedException(
				string.Format(
					"Cannot translate FilterPredicate \"{0}\" because {1} node is not supported.",
					expression,
					expression.NodeType));
		}

		private static IEnumerable<FilterStatement> TranslateFilterStatement(BinaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					foreach (var filterStatement in TranslateFilterStatement(expression.Left).Concat(TranslateFilterStatement(expression.Right)))
					{
						yield return filterStatement;
					}
					yield break;
				case ExpressionType.Equal:
					yield return new FilterStatement(
						TranslateFilterExpression(expression.Left),
						FilterOperator.Equals,
						TranslateFilterExpression(expression.Right));
					yield break;
				case ExpressionType.GreaterThan:
					yield return new FilterStatement(
						TranslateFilterExpression(expression.Left),
						FilterOperator.GreaterThan,
						TranslateFilterExpression(expression.Right));
					yield break;
				case ExpressionType.GreaterThanOrEqual:
					yield return new FilterStatement(
						TranslateFilterExpression(expression.Left),
						FilterOperator.GreaterThanOrEquals,
						TranslateFilterExpression(expression.Right));
					yield break;
				case ExpressionType.LessThan:
					yield return new FilterStatement(
						TranslateFilterExpression(expression.Left),
						FilterOperator.LessThan,
						TranslateFilterExpression(expression.Right));
					yield break;
				case ExpressionType.LessThanOrEqual:
					yield return new FilterStatement(
						TranslateFilterExpression(expression.Left),
						FilterOperator.LessThanOrEquals,
						TranslateFilterExpression(expression.Right));
					yield break;
				case ExpressionType.NotEqual:
					// != null is rewritten as Exists operator
					var value = TranslateFilterExpression(expression.Right);
					yield return new FilterStatement(
						TranslateFilterExpression(expression.Left),
						value == null ? FilterOperator.Exists : FilterOperator.NotEqual,
						value);
					yield break;
				default:
					throw new NotSupportedException(
						string.Format(
							"Cannot translate FilterStatement \"{0}\" because {1} node is not supported.",
							expression,
							expression.NodeType));
			}
		}

		private static string TranslateFilterExpression(Expression expression)
		{
			var constantExpression = expression as ConstantExpression;
			if (constantExpression != null) return TranslateFilterExpression(constantExpression);

			var memberExpression = expression as MemberExpression;
			if (memberExpression != null) return TranslateFilterExpression(memberExpression);

			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null) return TranslateFilterExpression(unaryExpression);

			throw new NotSupportedException(
				string.Format(
					"Cannot translate FilterExpression \"{0}\" because {1} node is not supported.",
					expression,
					expression.NodeType));
		}

		private static string TranslateFilterExpression(ConstantExpression expression)
		{
			if (expression.Type.IsEnum || expression.Type.IsPrimitive || expression.Type == typeof(string))
			{
				return expression.Value.IfNotNull(v => v.ToString());
			}

			throw new NotSupportedException(
				string.Format(
					"Cannot translate ConstantExpression \"{0}\" because {1} constant type is not supported.",
					expression,
					expression.Type));
		}

		private static string TranslateFilterExpression(MemberExpression expression)
		{
			// handle MessageContextProperty<T, TR>
			var type = expression.Type;
			if (type.IsSubclassOfOpenGenericType(typeof(MessageContextProperty<,>)))
			{
				var property = Expression.Lambda(expression).Compile().DynamicInvoke() as IMessageContextProperty;
				if (property == null)
					throw new NotSupportedException(
						string.Format(
							"Cannot translate MemberExpression \"{0}\" because it evaluates to null.",
							expression));
				return property.Type.FullName;
			}

			// handle Schema<T>
			type = expression.Member.ReflectedType;
			if (type != null && type.IsSubclassOfOpenGenericType(typeof(Schema<>)))
			{
				var value = Expression.Lambda(expression).Compile().DynamicInvoke();
				return value.ToString();
			}

			// handle IReceivePort<TNamingConvention>.Name and ISendPort<TNamingConvention>.Name
			var containingObjectType = expression.Expression.Type;
			if (containingObjectType.IsSubclassOfOpenGenericType(typeof(IReceivePort<>)) || containingObjectType.IsSubclassOfOpenGenericType(typeof(ISendPort<>)))
			{
				var port = (ISupportNamingConvention) Expression.Lambda(expression.Expression).Compile().DynamicInvoke();
				return port.Name;
			}

			throw new NotSupportedException(
				string.Format(
					"Cannot translate MemberExpression \"{0}\" because {1} node is not supported.",
					expression,
					expression.NodeType));
		}

		private static string TranslateFilterExpression(UnaryExpression expression)
		{
			if (expression.NodeType == ExpressionType.Convert)
			{
				// handle cast operator to INamingConvention<TNamingConvention>
				var declaringType = expression.Method.DeclaringType;
				if (declaringType != null && declaringType.IsSubclassOfOpenGenericType(typeof(INamingConvention<>)))
				{
					var memberExpression = expression.Operand as MemberExpression;
					if (memberExpression != null) return TranslateFilterExpression(memberExpression);
				}
			}

			throw new NotSupportedException(
				string.Format(
					"Cannot translate UnaryExpression \"{0}\" because {1} node is not supported.",
					expression,
					expression.NodeType));
		}
	}
}

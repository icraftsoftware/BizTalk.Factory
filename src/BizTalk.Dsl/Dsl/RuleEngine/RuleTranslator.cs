#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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
using System.Reflection;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.RuleEngine;
using Be.Stateless.BizTalk.Tracking.Messaging;
using Be.Stateless.Linq.Extensions;
using Microsoft.RuleEngine;

namespace Be.Stateless.BizTalk.Dsl.RuleEngine
{
	/// <summary>
	/// Translates a business rule from its DSL expression into its BRL equivalent.
	/// </summary>
	/// <remarks>
	/// Designed according to the visitor pattern.
	/// </remarks>
	/// <seealso href="http://msdn.microsoft.com/en-us/library/system.linq.expressions.expressionvisitor.aspx">ExpressionVisitor Class</seealso>
	/// <seealso href="http://blog.mikeobrien.net/2011/01/building-better-expression-visitor.html">Building a Better Expression Visitor</seealso>
	/// <seealso href="http://msdn.microsoft.com/en-us/library/bb882521(v=vs.90).aspx">How to: Implement an Expression Tree Visitor</seealso>
	internal static class RuleTranslator
	{
		public static Function Translate(Expression<Action> expression)
		{
			var body = expression.Body;
			return (Function) TranslateTerm(body);
		}

		public static LogicalExpression Translate(Expression<Func<bool>> expression)
		{
			var body = expression.Body;

			var constantExpression = body as ConstantExpression;
			if (constantExpression != null)
			{
				// translate constant true antecedent (which is not natively supported by BRE grammar)
				if (constantExpression.Type == typeof(bool) && (bool) constantExpression.Value) return new Equal(new Constant(true), new Constant(true));

				throw new NotSupportedException(
					string.Format(
						"Cannot translate ConstantExpression \"{0}\" because {1} {2} constant is not supported as rule predicate, only Boolean true is.",
						constantExpression,
						constantExpression.NodeType,
						constantExpression.Value));
			}

			return TranslateLogicalExpression(body);
		}

		private static LogicalExpression TranslateLogicalExpression(Expression expression)
		{
			var binaryExpression = expression as BinaryExpression;
			if (binaryExpression != null) return TranslateLogicalExpression(binaryExpression);

			var methodCallExpression = expression as MethodCallExpression;
			if (methodCallExpression != null) return TranslateLogicalExpression(methodCallExpression);

			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null) return TranslateLogicalExpression(unaryExpression);

			throw new NotSupportedException(
				string.Format(
					"Cannot translate Expression \"{0}\" because {1} node is not supported.",
					expression,
					expression.NodeType));
		}

		private static LogicalExpression TranslateLogicalExpression(BinaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					return new LogicalAnd(
						new LogicalExpressionCollection {
							TranslateLogicalExpression(expression.Left),
							TranslateLogicalExpression(expression.Right)
						});
				case ExpressionType.ExclusiveOr:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					return new LogicalOr(
						new LogicalExpressionCollection {
							TranslateLogicalExpression(expression.Left),
							TranslateLogicalExpression(expression.Right)
						});
				case ExpressionType.Equal:
					return new Equal(
						TranslateTerm(expression.Left),
						TranslateTerm(expression.Right));
				case ExpressionType.GreaterThan:
					return new GreaterThan(
						TranslateTerm(expression.Left),
						TranslateTerm(expression.Right));
				case ExpressionType.GreaterThanOrEqual:
					return new GreaterThanEqual(
						TranslateTerm(expression.Left),
						TranslateTerm(expression.Right));
				case ExpressionType.LessThan:
					return new LessThan(
						TranslateTerm(expression.Left),
						TranslateTerm(expression.Right));
				case ExpressionType.LessThanOrEqual:
					return new LessThanEqual(
						TranslateTerm(expression.Left),
						TranslateTerm(expression.Right));
				case ExpressionType.NotEqual:
					return new NotEqual(
						TranslateTerm(expression.Left),
						TranslateTerm(expression.Right));
				default:
					throw new NotSupportedException(
						string.Format(
							"Cannot translate BinaryExpression \"{0}\" because {1} node is not supported.",
							expression,
							expression.NodeType));
			}
		}

		private static LogicalExpression TranslateLogicalExpression(MethodCallExpression expression)
		{
			return new UserPredicate(TranslateBinding(expression));
		}

		private static LogicalExpression TranslateLogicalExpression(UnaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Not:
					return new LogicalNot(TranslateLogicalExpression(expression.Operand));
				default:
					throw new NotSupportedException(
						string.Format(
							"Cannot translate UnaryExpression \"{0}\" because {1} node is not supported.",
							expression,
							expression.NodeType));
			}
		}

		private static Term TranslateTerm(Expression expression)
		{
			var constantExpression = expression as ConstantExpression;
			if (constantExpression != null) return TranslateTerm(constantExpression);

			var memberExpression = expression as MemberExpression;
			if (memberExpression != null) return TranslateTerm(memberExpression);

			var methodCallExpression = expression as MethodCallExpression;
			if (methodCallExpression != null) return TranslateTerm(methodCallExpression);

			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null) return TranslateTerm(unaryExpression);

			throw new NotSupportedException(
				string.Format(
					"Cannot translate Expression \"{0}\" because {1} node is not supported.",
					expression,
					expression.NodeType));
		}

		private static Constant TranslateTerm(ConstantExpression expression)
		{
			if (expression.Type.IsEnum) return new Constant(expression.Type, expression.Value);

			if (expression.Type == typeof(string) || expression.Type.IsPrimitive) return new Constant(expression.Value);

			throw new NotSupportedException(
				string.Format(
					"Cannot translate ConstantExpression \"{0}\" because {1} constant type is not supported.",
					expression,
					expression.Type));
		}

		private static Term TranslateTerm(MemberExpression expression)
		{
			var member = expression.Member;

			var reflectedType = member.ReflectedType;
			if (reflectedType != null)
			{
				// handle RuleSet.Schema<T>
				if (reflectedType.IsGenericType && reflectedType.GetGenericTypeDefinition() == typeof(Schema<>))
				{
					var value = Expression.Lambda(expression).Compile().DynamicInvoke();
					return new Constant(value);
				}

				// handle RuleSet.Transform<T>
				if (reflectedType.IsGenericType && reflectedType.GetGenericTypeDefinition() == typeof(Transform<>))
				{
					var value = Expression.Lambda(expression).Compile().DynamicInvoke();
					return new Constant(value);
				}

				var reflectedBaseType = reflectedType.BaseType;
				// handle ProcessNames<T>-derived types' members
				if (reflectedBaseType != null && reflectedBaseType.IsGenericType && reflectedBaseType.GetGenericTypeDefinition() == typeof(ProcessNames<>))
				{
					var value = Expression.Lambda(expression).Compile().DynamicInvoke();
					return new Constant(value);
				}
			}

			// handle MessageContextProperty<TP, TV>
			if (expression.Type.IsGenericType && expression.Type.GetGenericTypeDefinition() == typeof(MessageContextProperty<,>))
			{
				throw new InvalidOperationException(
					string.Format(
						"Cannot translate MemberExpression \"{0}\" of type {1} because expressions of type MessageContextProperty<,> are not expected at this stage.",
						expression,
						expression.Type));
			}

			// handle property / field
			if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
			{
				// handle static property / field
				if (expression.Expression == null) return new UserFunction(new ClassMemberBinding(member.Name, new ClassBinding(reflectedType)));

				// handle Type's instance property
				if (expression.Expression.Type == typeof(Type)) return new Constant(Expression.Lambda(expression).Compile().DynamicInvoke());

				// handle any constant instance property
				if (expression.Expression.NodeType == ExpressionType.Constant) return new Constant(Expression.Lambda(expression).Compile().DynamicInvoke());

				throw new NotSupportedException(
					string.Format(
						"Cannot translate MemberExpression \"{0}\" because \"{1}\" is neither a static/constant property/field, nor a property/field member of a Type instance. " +
							"{2} must either be null for a static/constant property/field, or either a Type instance for a property/field instance member.",
						expression,
						member.Name,
						expression.Expression));
			}

			throw new NotSupportedException(
				string.Format(
					"Cannot translate MemberExpression \"{0}\" because {1} {2} is not supported.",
					expression,
					expression.Type,
					member.MemberType));
		}

		private static IEnumerable<Term> TranslateMessageContextPropertyTerm(MemberExpression expression)
		{
			if (!expression.Type.IsGenericType || expression.Type.GetGenericTypeDefinition() != typeof(MessageContextProperty<,>))
			{
				throw new InvalidOperationException(
					string.Format(
						"Cannot translate MemberExpression \"{0}\" of type {1}; only expressions of type MessageContextProperty<,> are expected at this stage.",
						expression,
						expression.Type));
			}

			// handle MessageContextProperty<TP, TV>
			var member = expression.Member;
			var reflectedType = member.ReflectedType;

			if (expression.Expression != null)
			{
				var property = Expression.Lambda(expression).Compile().DynamicInvoke() as IMessageContextProperty;
				if (property == null)
					throw new NotSupportedException(
						string.Format(
							"Cannot translate MemberExpression \"{0}\" because {1} is not null: MessageContextProperty<,> properties are static and a call site is not expected.",
							expression,
							expression.Expression));
				return new Term[] { new Constant(property.Name), new Constant(property.Namespace) };
			}

			// qnameGetter is just a way to get 'QName' property's name in a refactoring-safe way should its name change;
			// besides, performance is irrelevant as this code is never executed at runtime but only during deployment.
			Expression<Func<IMessageContextProperty, XmlQualifiedName>> qnameGetter = c => c.QName;
			var classBinding = new ClassBinding(reflectedType);
			var classMemberBinding = new ClassMemberBinding(member.Name, classBinding);
			return new Term[] { new UserFunction(new ClassMemberBinding(((MemberExpression) qnameGetter.Body).Member.Name, classMemberBinding)) };
		}

		private static Term TranslateTerm(MethodCallExpression expression)
		{
			return new UserFunction(TranslateBinding(expression));
		}

		private static Term TranslateTerm(UnaryExpression expression)
		{
			if (expression.NodeType == ExpressionType.Convert)
			{
				// for value types, skip implicit boxing, or cast to object, injected by compiler when building Expression
				if (expression.Type == typeof(object) && expression.Operand.Type.IsValueType) return TranslateTerm(expression.Operand);

				throw new NotSupportedException(
					string.Format(
						"Cannot translate UnaryExpression Convert expression \"{0}\" because cast from {1} to {2} is not supported.",
						expression,
						expression.Operand.Type,
						expression.Type));
			}

			throw new NotSupportedException(
				string.Format(
					"Cannot translate UnaryExpression \"{0}\" because {1} node is not supported.",
					expression,
					expression.NodeType));
		}

		private static ArgumentCollection TranslateArguments(IEnumerable<Expression> argumentExpressions)
		{
			var arguments = new ArgumentCollection();
			argumentExpressions.Each(arg => arguments.Add(TranslateTerm(arg)));
			return arguments;
		}

		private static ArgumentCollection TranslateMessageContextPropertyArgument(IEnumerable<Expression> argumentExpressions)
		{
			var expressions = argumentExpressions as Expression[] ?? argumentExpressions.ToArray();
			var arguments = new ArgumentCollection();
			TranslateMessageContextPropertyTerm((MemberExpression) expressions.First()).Each(arg => arguments.Add(arg));
			expressions.Skip(1).Each(arg => arguments.Add(TranslateTerm(arg)));
			return arguments;
		}

		private static ClassMemberBinding TranslateBinding(MethodCallExpression expression)
		{
			if (expression.Object == null)
			{
				var methodInfo = expression.Method;
				var classType = methodInfo.ReflectedType;
				// fluent-syntactic-sugar Dsl.RuleSet.Context class is replaced by actual runtime RuleEngine.Context class
				if (classType != typeof(RuleSet.Context))
				{
					var @class = new ClassBinding(classType);
					var arguments = TranslateArguments(expression.Arguments);
					return new ClassMemberBinding(methodInfo.Name, @class, arguments);
				}

				var contextClass = new ClassBinding(typeof(Context));
				var contextArguments = TranslateMessageContextPropertyArgument(expression.Arguments);
				var classMemberBinding = new ClassMemberBinding(methodInfo.Name, contextClass, contextArguments);

				// contextRead is just a way to get 'Read' method's name in a refactoring-safe way should its name change;
				// besides, performance is irrelevant as this code is never executed at runtime but only during deployment.
				Expression<Func<Context, object>> contextRead = c => c.Read(null);
				if (methodInfo.Name == ((MethodCallExpression) contextRead.Body).Method.Name)
				{
					// RuleEngine.Context.Read() will actually return an object, whereas Dsl.RuleSet.Context would return a
					// strong-typed value. Because one has written DSL expressions involving the strong-typed value, it has
					// to be casted into the expected type, or else RuleEngine will throw a type mismatch exception. It is
					// important to call the respective .To<TypeCode>() conversion methods and not the .ChangeType() one as
					// the latter throws when Context.Read() returns null.
					classMemberBinding = new ClassMemberBinding(
						"To" + Type.GetTypeCode(expression.Type),
						new ClassBinding(typeof(Convert)),
						new ArgumentCollection { new UserFunction(classMemberBinding) });
					classMemberBinding.SideEffects = false;
				}

				return classMemberBinding;
			}

			//var methodCallExpression = expression.Object as MethodCallExpression;
			//if (methodCallExpression != null)
			//{
			//   var @class = TranslateBinding(methodCallExpression);
			//   var arguments = TranslateArguments(expression.Arguments);
			//   return new ClassMemberBinding(expression.Method.Name, @class, arguments);
			//}

			throw new NotSupportedException(
				string.Format(
					"Cannot translate MethodCallExpression \"{0}\" because {1} is neither null nor a static: " +
						"writing rules against a specific object/instance is not supported.",
					expression,
					expression.Object));
		}
	}
}

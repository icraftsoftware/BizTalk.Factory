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
using System.Linq.Expressions;
using System.Reflection;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.RuleEngine.Dsl.Extensions;
using Microsoft.RuleEngine;

namespace Be.Stateless.BizTalk.RuleEngine.Dsl
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
			return TranslateExpression(body);
		}

		internal static LogicalExpression TranslateExpression(Expression expression)
		{
			var binaryExpression = expression as BinaryExpression;
			if (binaryExpression != null)
				return Translate(binaryExpression);

			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
				return Translate(unaryExpression);

			var methodCallExpression = expression as MethodCallExpression;
			if (methodCallExpression != null)
				return new UserPredicate(Translate(methodCallExpression));

			// translation of true antecedent (which is not natively supported by BRE grammar)
			if (expression is ConstantExpression && (bool) ((ConstantExpression) expression).Value)
				return new Equal(new Constant(true), new Constant(true));

			throw new NotSupportedException(string.Format("{0} expression.", expression.NodeType));
		}

		internal static Term TranslateTerm(Expression expression)
		{
			var constantExpression = expression as ConstantExpression;
			if (constantExpression != null)
				return Translate(constantExpression);

			var memberExpression = expression as MemberExpression;
			if (memberExpression != null)
				return Translate(memberExpression);

			var methodCallExpression = expression as MethodCallExpression;
			if (methodCallExpression != null)
				return new UserFunction(Translate(methodCallExpression));

			throw new NotSupportedException(string.Format("{0} expression.", expression.NodeType));
		}

		internal static LogicalExpression Translate(BinaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					return new LogicalAnd(
						new LogicalExpressionCollection {
							TranslateExpression(expression.Left),
							TranslateExpression(expression.Right)
						});
				case ExpressionType.ExclusiveOr:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					return new LogicalOr(
						new LogicalExpressionCollection {
							TranslateExpression(expression.Left),
							TranslateExpression(expression.Right)
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
					throw new NotSupportedException(string.Format("{0} expression type not supported.", expression.NodeType));
			}
		}

		internal static Constant Translate(ConstantExpression expression)
		{
			if (expression.Type == typeof(string) || expression.Type.IsPrimitive)
				return new Constant(expression.Value);
			throw new NotSupportedException(string.Format("{0} constant type is not supported.", expression.Type));
		}

		internal static Term Translate(MemberExpression expression)
		{
			var member = expression.Member;

			// handle MessageContextProperty<TP, TV>
			if (expression.Type.IsGenericType && expression.Type.GetGenericTypeDefinition() == typeof(MessageContextProperty<,>))
			{
				if (expression.Expression != null)
					throw new NotSupportedException("MemberExpression.Expression is not null: MessageContextProperty<,> properties are static and a call site is not expected.");
				var classBinding = new ClassBinding(member.ReflectedType);
				var classMemberBinding = new ClassMemberBinding(member.Name, classBinding);
				return new UserFunction(new ClassMemberBinding("QName", classMemberBinding));
			}

			// handle RuleSet.Schema<T>
			if (member.ReflectedType.IsGenericType && member.ReflectedType.GetGenericTypeDefinition() == typeof(RuleSetBase.Schema<>))
			{
				var value = Expression.Lambda(expression).Compile().DynamicInvoke();
				return new Constant(value);
			}

			// handle RuleSet.Transform<T>
			if (member.ReflectedType.IsGenericType && member.ReflectedType.GetGenericTypeDefinition() == typeof(RuleSetBase.Transform<>))
			{
				var value = Expression.Lambda(expression).Compile().DynamicInvoke();
				return new Constant(value);
			}

			// handle ProcessNameAttribute-qualified fields or properties
			if (member.IsProcessName())
			{
				var value = member.GetProcessName();
				return new Constant(value);
			}

			// handle static property / field
			if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
			{
				if (expression.Expression != null)
					throw new NotSupportedException("MemberExpression.Expression is not null: only static properties are supported and a call site is not expected.");
				var classBinding = new ClassBinding(member.ReflectedType);
				return new UserFunction(new ClassMemberBinding(member.Name, classBinding));
			}

			throw new NotSupportedException(string.Format("{0} {1} is not supported.", expression.Type, member.MemberType));
		}

		internal static ClassMemberBinding Translate(MethodCallExpression expression)
		{
			if (expression.Object != null)
				throw new NotSupportedException("MethodCallExpression.Object is not null: writing rules against a specific object/instance is not supported.");

			var methodInfo = expression.Method;
			var classType = methodInfo.ReflectedType;
			// methods of the fluent-syntactic-sugar RuleSet.Context class are replaced by actual runtime RuleEngine.Context type
			if (classType == typeof(RuleSet.Context))
				classType = typeof(Context);
			var arguments = new ArgumentCollection();
			foreach (var arg in expression.Arguments)
			{
				arguments.Add(TranslateTerm(arg));
			}
			var classMemberBinding = new ClassMemberBinding(methodInfo.Name, new ClassBinding(classType), arguments);

			// contextReadExpression is a just a trick to get the 'Read' method name in a refactoring-safe way,
			// should the method name change; performance is moreover irrelevant as this code is executed only
			// at deployment time and never at runtime.
			Expression<Func<Context, object>> contextReadExpression = c => c.Read(null);
			if (classType == typeof(Context) && methodInfo.Name == ((MethodCallExpression) contextReadExpression.Body).Method.Name)
			{
				// RuleEngine.Context.Read() will actually return an object, while Dsl.RuleSet.Context would return
				// a strong-typed value. Because one has written DSL expressions involving the strong-typed value,
				// we have to cast it into the expected type, or RuleEngine will throw a type mismatch exception.
				// It is important to call the respective .To<TypeCode>() conversion methods and not the .ChangeType()
				// one as the latter throws when Context.Read() returns null.
				classMemberBinding = new ClassMemberBinding(
					"To" + Type.GetTypeCode(expression.Type),
					new ClassBinding(typeof(Convert)),
					new ArgumentCollection { new UserFunction(classMemberBinding) });
				classMemberBinding.SideEffects = false;
			}

			return classMemberBinding;
		}

		internal static LogicalExpression Translate(UnaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Not:
					return new LogicalNot(TranslateExpression(expression.Operand));
				default:
					throw new NotSupportedException(string.Format("{0} expression type not supported.", expression.NodeType));
			}
		}
	}
}
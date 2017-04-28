#region Copyright & License

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
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Xml;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Moq.Language.Flow;

namespace Be.Stateless.BizTalk.Unit.Message.Context
{
	/// <summary>
	/// <see cref="Moq.Mock"/> overloads to support the direct setup of the <see cref="BaseMessageContextExtensions"/>'s
	/// extension methods to read, write and promote <see cref="IBaseMessageContext"/> properties in a shorter and
	/// <b>type-safe</b> way.
	/// </summary>
	/// <typeparam name="T">
	/// Type to mock, which can be an interface or a class; in this case, <see cref="IBaseMessageContext"/>.
	/// </typeparam>
	/// <seealso cref="BaseMessageContextExtensions.GetProperty{T}(IBaseMessageContext,MessageContextProperty{T,string})"/>
	/// <seealso cref="BaseMessageContextExtensions.GetProperty{T,TResult}(IBaseMessageContext,MessageContextProperty{T,TResult})"/>
	/// <seealso cref="BaseMessageContextExtensions.SetProperty{T}(IBaseMessageContext,ContextProperties.MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessageContextExtensions.SetProperty{T,TV}(IBaseMessageContext,ContextProperties.MessageContextProperty{T,TV},TV)"/>
	/// <seealso cref="BaseMessageContextExtensions.Promote{T}(IBaseMessageContext,ContextProperties.MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessageContextExtensions.Promote{T,TV}(IBaseMessageContext,ContextProperties.MessageContextProperty{T,TV},TV)"/>
	[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
	public class Mock<T> : Moq.Mock<T> where T : class, IBaseMessageContext
	{
		public Mock() : this(MockBehavior.Default) { }

		public Mock(MockBehavior behavior) : this(behavior, new object[0]) { }

		public Mock(params object[] args) : this(MockBehavior.Default, args) { }

		public Mock(MockBehavior behavior, params object[] args) : base(behavior, args) { }

		public ISetup<T> Setup(Expression<Action<IBaseMessageContext>> expression)
		{
			// intercept Setups
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContextExtensions))
			{
				// rewrite Setups
				var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
				switch (methodCallExpression.Method.Name)
				{
					case "DeleteProperty":
						return Setup(ctxt => ctxt.Write(qualifiedName.Name, qualifiedName.Namespace, null));
					case "SetProperty":
						var writtenValue = Expression.Lambda(methodCallExpression.Arguments[2]).Compile().DynamicInvoke();
						return Setup(ctxt => ctxt.Write(qualifiedName.Name, qualifiedName.Namespace, writtenValue));
					case "Promote":
						var promotedValue = Expression.Lambda(methodCallExpression.Arguments[2]).Compile().DynamicInvoke();
						// setup IsPromoted as well, but to mock how a promoted property actually behaves it must also have a value to be read
						Setup(ctxt => ctxt.Read(qualifiedName.Name, qualifiedName.Namespace)).Returns(promotedValue);
						Setup(ctxt => ctxt.IsPromoted(qualifiedName.Name, qualifiedName.Namespace)).Returns(true);
						return Setup(ctxt => ctxt.Promote(qualifiedName.Name, qualifiedName.Namespace, promotedValue));
					default:
						throw new NotSupportedException(string.Format("Unexpected IBaseMessageContext extension method: '{0}'.", methodCallExpression.Method.Name));
				}
			}

			// let base class handle all other Setups
			return Setup((Expression<Action<T>>) (Expression) expression);
		}

		public ISetup<T, object> Setup(Expression<Func<IBaseMessageContext, string>> expression)
		{
			// intercept Setups
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContextExtensions)
				&& methodCallExpression.Method.Name == "GetProperty")
			{
				// rewrite Setups
				var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
				return Setup(ctxt => ctxt.Read(qualifiedName.Name, qualifiedName.Namespace));
			}

			// let base class handle all other Setups
			return Setup((Expression<Func<T, object>>) (Expression) expression);
		}

		public ISetup<T, bool> Setup(Expression<Func<IBaseMessageContext, bool>> expression)
		{
			// intercept Setups
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContextExtensions)
				&& methodCallExpression.Method.Name == "IsPromoted")
			{
				// rewrite Setups
				var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
				return Setup(ctxt => ctxt.IsPromoted(qualifiedName.Name, qualifiedName.Namespace));
			}

			// let base class handle all other Setups
			return Setup((Expression<Func<T, bool>>) (Expression) expression);
		}

		public ISetup<T, object> Setup<TResult>(Expression<Func<IBaseMessageContext, TResult?>> expression) where TResult : struct
		{
			// intercept Setups
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContextExtensions)
				&& methodCallExpression.Method.Name == "GetProperty")
			{
				// rewrite Setups
				var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
				return Setup(ctxt => ctxt.Read(qualifiedName.Name, qualifiedName.Namespace));
			}

			// let base class handle all other Setups
			return Setup((Expression<Func<T, object>>) (Expression) expression);
		}

		public ISetup<T, TResult> Setup<TResult>(Expression<Func<IBaseMessageContext, TResult>> expression)
		{
			// intercept Setups
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContextExtensions))
				throw new NotSupportedException(
					string.Format(
						"Unexpected IBaseMessageContext extension method: '{0}'.",
						methodCallExpression.Method.Name));

			// let base class handle all other Setups
			return Setup((Expression<Func<T, TResult>>) (Expression) expression);
		}

		public void Verify(Expression<Action<IBaseMessageContext>> expression)
		{
			Verify(expression, Times.AtLeastOnce(), null);
		}

		public void Verify(Expression<Action<IBaseMessageContext>> expression, Func<Times> times)
		{
			Verify(expression, times(), null);
		}

		public void Verify(Expression<Action<IBaseMessageContext>> expression, Times times)
		{
			Verify(expression, times, null);
		}

		public void Verify(Expression<Action<IBaseMessageContext>> expression, string failMessage)
		{
			Verify(expression, Times.AtLeastOnce(), failMessage);
		}

		public void Verify(Expression<Action<IBaseMessageContext>> expression, Func<Times> times, string failMessage)
		{
			Verify(expression, times(), failMessage);
		}

		public void Verify(Expression<Action<IBaseMessageContext>> expression, Times times, string failMessage)
		{
			// intercept and rewrite IBaseMessage Verify calls against IBaseMessageContext
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(BaseMessageContextExtensions))
			{
				// rewrite expression to let base Moq class handle It.Is<> and It.IsAny<> expressions should there be any
				var rewrittenExpression = RewriteExpression(methodCallExpression);
				var mock = Get(Object);
				mock.Verify(rewrittenExpression, times, failMessage);
			}
			else
			{
				// let base Moq class handle all other Verify calls
				Verify((Expression<Action<T>>) (Expression) expression, times, failMessage);
			}
		}

		internal XmlQualifiedName GetContextPropertyXmlQualifiedName(MethodCallExpression methodCallExpression)
		{
			var propertyArgument = methodCallExpression.Arguments[1];
			if (!propertyArgument.Type.IsGenericType || propertyArgument.Type.GetGenericTypeDefinition() != typeof(MessageContextProperty<,>)) throw new NotSupportedException();
			dynamic contextProperty = Expression.Lambda(propertyArgument).Compile().DynamicInvoke();
			var qualifiedName = new XmlQualifiedName(contextProperty.Name, contextProperty.Namespace);
			return qualifiedName;
		}

		internal Expression<Action<T>> RewriteExpression(MethodCallExpression methodCallExpression)
		{
			var qname = GetContextPropertyXmlQualifiedName(methodCallExpression);
			switch (methodCallExpression.Method.Name)
			{
				case "DeleteProperty":
					return RewriteExpression(_writeExpressionTemplate, qname, Expression.Constant(null));
				case "GetProperty":
					//private static readonly Expression<Action<T>> _readExpressionTemplate =
					//	c => c.Read(BizTalkFactoryProperties.EnvironmentTag.Name, BizTalkFactoryProperties.EnvironmentTag.Namespace);
					//var mce = (MethodCallExpression)_readExpressionTemplate.Body;
					//var ec = Expression.Call(
					//	mce.Object,
					//	mce.Method,
					//	Expression.Constant(qname.Name),
					//	Expression.Constant(qname.Namespace));
					//return Expression.Lambda<Action<T>>(ec, _readExpressionTemplate.Parameters);
					return c => c.Read(qname.Name, qname.Namespace);
				case "SetProperty":
					return RewriteExpression(_writeExpressionTemplate, qname, methodCallExpression.Arguments[2]);
				case "Promote":
					return RewriteExpression(_promoteExpressionTemplate, qname, methodCallExpression.Arguments[2]);
				default:
					throw new NotSupportedException(string.Format("Unexpected call of extension method: '{0}'.", methodCallExpression.Method.Name));
			}
		}

		private Expression<Action<T>> RewriteExpression(Expression<Action<T>> expressionTemplate, XmlQualifiedName qname, Expression valueExpression)
		{
			var mce = (MethodCallExpression) expressionTemplate.Body;
			var ec = Expression.Call(
				mce.Object,
				mce.Method,
				Expression.Constant(qname.Name),
				Expression.Constant(qname.Namespace),
				Expression.Convert(valueExpression, typeof(object)));
			return Expression.Lambda<Action<T>>(ec, expressionTemplate.Parameters);
		}

		private static readonly Expression<Action<T>> _promoteExpressionTemplate =
			c => c.Promote(BizTalkFactoryProperties.EnvironmentTag.Name, BizTalkFactoryProperties.EnvironmentTag.Namespace, null);

		private static readonly Expression<Action<T>> _writeExpressionTemplate =
			c => c.Write(BizTalkFactoryProperties.EnvironmentTag.Name, BizTalkFactoryProperties.EnvironmentTag.Namespace, null);
	}
}

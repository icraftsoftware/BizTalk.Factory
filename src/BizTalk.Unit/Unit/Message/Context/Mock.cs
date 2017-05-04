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
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Moq.Language.Flow;

namespace Be.Stateless.BizTalk.Unit.Message.Context
{
	/// <summary>
	/// <see cref="Moq.Mock"/> overloads to support the direct setup of the <see cref="BaseMessageContext"/>'s extension
	/// methods to read, write and promote <see cref="IBaseMessageContext"/> properties in a shorter and <b>type-safe</b>
	/// way.
	/// </summary>
	/// <typeparam name="TMock">
	/// Type to mock, which can be an interface or a class; in this case, <see cref="IBaseMessageContext"/>.
	/// </typeparam>
	/// <seealso cref="BaseMessageContext.GetProperty{T}(IBaseMessageContext,MessageContextProperty{T,string})"/>
	/// <seealso cref="BaseMessageContext.GetProperty{T,TResult}(IBaseMessageContext,MessageContextProperty{T,TResult})"/>
	/// <seealso cref="BaseMessageContext.SetProperty{T}(IBaseMessageContext,ContextProperties.MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessageContext.SetProperty{T,TV}(IBaseMessageContext,ContextProperties.MessageContextProperty{T,TV},TV)"/>
	/// <seealso cref="BaseMessageContext.Promote{T}(IBaseMessageContext,ContextProperties.MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessageContext.Promote{T,TV}(IBaseMessageContext,ContextProperties.MessageContextProperty{T,TV},TV)"/>
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
	public class Mock<TMock> : Moq.Mock<TMock> where TMock : class, IBaseMessageContext
	{
		public Mock() : this(MockBehavior.Default) { }

		public Mock(MockBehavior behavior) : base(behavior) { }

		internal Mock(MockBehavior behavior, Moq.Mock<TMock> baseMessageContext) : base(behavior)
		{
			if (baseMessageContext == null) throw new ArgumentNullException("baseMessageContext");
			_baseMessageContext = baseMessageContext;
		}

		private Moq.Mock<TMock> BaseMock
		{
			get { return _baseMessageContext ?? this; }
		}

		public ISetup<TMock> Setup(Expression<Action<IBaseMessageContext>> expression)
		{
			// intercept setup
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessageContext)))
			{
				return SetupCoreMethodCallExpression(methodCallExpression);
			}

			// let base class handle all other setup
			return BaseMock.Setup((Expression<Action<TMock>>) (Expression) expression);
		}

		internal ISetup<TMock> SetupCoreMethodCallExpression(MethodCallExpression methodCallExpression)
		{
			// rewrite setup
			var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
			switch (methodCallExpression.Method.Name)
			{
				case "DeleteProperty":
					return BaseMock.Setup(ctxt => ctxt.Write(qualifiedName.Name, qualifiedName.Namespace, null));
				case "SetProperty":
					// rewrite expression to let base Moq class handle It.Is<> and It.IsAny<> expressions should there be any
					return BaseMock.Setup(RewriteExpression(methodCallExpression));
				case "Promote":
					// setup IsPromoted() as well
					BaseMock.Setup(ctxt => ctxt.IsPromoted(qualifiedName.Name, qualifiedName.Namespace)).Returns(true);
					// to mock how a promoted property actually behaves Read() must also be setup
					// pass expression's argument verbatim should it be It.IsAny<>
					var promotedValue = Expression.Lambda(methodCallExpression.Arguments[2]).Compile().DynamicInvoke();
					BaseMock.Setup(ctxt => ctxt.Read(qualifiedName.Name, qualifiedName.Namespace)).Returns(promotedValue);
					// rewrite expression to let base Moq class handle It.Is<> and It.IsAny<> expressions should there be any
					return BaseMock.Setup(RewriteExpression(methodCallExpression));
				default:
					throw new InvalidOperationException(string.Format("Unexpected call of extension method: '{0}'.", methodCallExpression.Method.Name));
			}
		}

		public ISetup<TMock, object> Setup(Expression<Func<IBaseMessageContext, string>> expression)
		{
			// intercept setup
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessageContext))
				&& methodCallExpression.Method.Name == "GetProperty")
			{
				// rewrite setup
				var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
				return BaseMock.Setup(ctxt => ctxt.Read(qualifiedName.Name, qualifiedName.Namespace));
			}

			// let base class handle all other setup
			return BaseMock.Setup((Expression<Func<TMock, object>>) (Expression) expression);
		}

		public ISetup<TMock, bool> Setup(Expression<Func<IBaseMessageContext, bool>> expression)
		{
			// intercept setup
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessageContext))
				&& methodCallExpression.Method.Name == "IsPromoted")
			{
				// rewrite setup
				var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
				return BaseMock.Setup(ctxt => ctxt.IsPromoted(qualifiedName.Name, qualifiedName.Namespace));
			}

			// let base class handle all other setup
			return BaseMock.Setup((Expression<Func<TMock, bool>>) (Expression) expression);
		}

		public ISetup<TMock, object> Setup<TResult>(Expression<Func<IBaseMessageContext, TResult?>> expression) where TResult : struct
		{
			// intercept setup
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessageContext))
				&& methodCallExpression.Method.Name == "GetProperty")
			{
				// rewrite setup
				var qualifiedName = GetContextPropertyXmlQualifiedName(methodCallExpression);
				return BaseMock.Setup(ctxt => ctxt.Read(qualifiedName.Name, qualifiedName.Namespace));
			}

			// let base class handle all other setup
			return BaseMock.Setup((Expression<Func<TMock, object>>) (Expression) expression);
		}

		public ISetup<TMock, TResult> Setup<TResult>(Expression<Func<IBaseMessageContext, TResult>> expression)
		{
			// intercept setup
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessageContext)))
				throw new InvalidOperationException(
					string.Format(
						"Unexpected call of extension method: '{0}'.",
						methodCallExpression.Method.Name));

			// let base class handle all other setup
			return BaseMock.Setup((Expression<Func<TMock, TResult>>) (Expression) expression);
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
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessageContext)))
			{
				// rewrite expression to let base Moq class handle It.Is<> and It.IsAny<> expressions should there be any
				var rewrittenExpression = RewriteExpression(methodCallExpression);
				//var mock = Get(Object);
				//mock.Verify(rewrittenExpression, times, failMessage);
				BaseMock.Verify(rewrittenExpression, times, failMessage);
			}
			else
			{
				// let base Moq class handle all other Verify calls
				//Verify((Expression<Action<TMock>>)(Expression)expression, times, failMessage);
				BaseMock.Verify((Expression<Action<TMock>>) (Expression) expression, times, failMessage);
			}
		}

		internal XmlQualifiedName GetContextPropertyXmlQualifiedName(MethodCallExpression methodCallExpression)
		{
			var propertyArgument = methodCallExpression.Arguments[1];
			if (!propertyArgument.Type.IsSubclassOfOpenGenericType(typeof(MessageContextProperty<,>))) throw new InvalidOperationException();
			dynamic contextProperty = Expression.Lambda(propertyArgument).Compile().DynamicInvoke();
			var qualifiedName = new XmlQualifiedName(contextProperty.Name, contextProperty.Namespace);
			return qualifiedName;
		}

		internal Expression<Action<TMock>> RewriteExpression(MethodCallExpression methodCallExpression)
		{
			var qname = GetContextPropertyXmlQualifiedName(methodCallExpression);
			switch (methodCallExpression.Method.Name)
			{
				case "DeleteProperty":
					return RewriteExpression(_writeExpressionTemplate, qname, Expression.Constant(null));
				case "GetProperty":
					return ctxt => ctxt.Read(qname.Name, qname.Namespace);
				case "IsPromoted":
					// TODO IsPromoted extension method ensures both that there is a value and that it is promoted
					return ctxt => ctxt.IsPromoted(qname.Name, qname.Namespace);
				case "SetProperty":
					return RewriteExpression(_writeExpressionTemplate, qname, methodCallExpression.Arguments[2]);
				case "Promote":
					return RewriteExpression(_promoteExpressionTemplate, qname, methodCallExpression.Arguments[2]);
				default:
					throw new InvalidOperationException(string.Format("Unexpected call of extension method: '{0}'.", methodCallExpression.Method.Name));
			}
		}

		private Expression<Action<TMock>> RewriteExpression(Expression<Action<TMock>> expressionTemplate, XmlQualifiedName qname, Expression valueExpression)
		{
			var mce = (MethodCallExpression) expressionTemplate.Body;
			var ec = Expression.Call(
				mce.Object,
				mce.Method,
				Expression.Constant(qname.Name),
				Expression.Constant(qname.Namespace),
				Expression.Convert(valueExpression, typeof(object)));
			return Expression.Lambda<Action<TMock>>(ec, expressionTemplate.Parameters);
		}

		private static readonly Expression<Action<TMock>> _promoteExpressionTemplate =
			ctxt => ctxt.Promote(BizTalkFactoryProperties.EnvironmentTag.Name, BizTalkFactoryProperties.EnvironmentTag.Namespace, null);

		private static readonly Expression<Action<TMock>> _writeExpressionTemplate =
			ctxt => ctxt.Write(BizTalkFactoryProperties.EnvironmentTag.Name, BizTalkFactoryProperties.EnvironmentTag.Namespace, null);

		private readonly Moq.Mock<TMock> _baseMessageContext;
	}
}

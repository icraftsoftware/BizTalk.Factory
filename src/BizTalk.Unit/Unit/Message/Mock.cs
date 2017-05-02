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
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.BizTalk.Unit.Message.Language.Flow;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Message.Interop;
using Moq;
using Moq.Language.Flow;

namespace Be.Stateless.BizTalk.Unit.Message
{
	/// <summary>
	/// <see cref="Moq.Mock"/> overloads to support the direct setup of the <see cref="BaseMessage"/>'s extension methods
	/// to read, write and promote <see cref="IBaseMessageContext"/> properties in a shorter and <b>type-safe</b> way.
	/// </summary>
	/// <typeparam name="TMock">
	/// Type to mock, which can be an interface or a class; in this case, <see cref="IBaseMessage"/>.
	/// </typeparam>
	/// <seealso cref="BaseMessage.GetProperty{T}(IBaseMessage,MessageContextProperty{T,string})"/>
	/// <seealso cref="BaseMessage.GetProperty{T,TResult}(IBaseMessage,MessageContextProperty{T,TResult})"/>
	/// <seealso cref="BaseMessage.SetProperty{T}(IBaseMessage,ContextProperties.MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessage.SetProperty{T,TV}(IBaseMessage,ContextProperties.MessageContextProperty{T,TV},TV)"/>
	/// <seealso cref="BaseMessage.Promote{T}(IBaseMessage,ContextProperties.MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessage.Promote{T,TV}(IBaseMessage,ContextProperties.MessageContextProperty{T,TV},TV)"/>
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
	public class Mock<TMock> : Moq.Mock<TMock> where TMock : class, IBaseMessage
	{
		public Mock() : this(MockBehavior.Default) { }

		public Mock(MockBehavior behavior) : base(behavior)
		{
			// setup a default context so that .GetProperty() extension on IBaseMessage mock can be called for property without setup

			// HACK Don't setup a new Context.Mock<> of our own as it would be overwritten by Moq recursive mocking
			// feature. Hence we perform a mock setup that will kick the recursive mocking to instantiate a context mock
			// that will be wrapped and delegated to by our own Context.Mock<>. This must be considered as a bug in Moq.
			base.Setup(msg => msg.Context.AddPredicate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()));
			// ReSharper disable once VirtualMemberCallInConstructor
			_contextMock = new Context.Mock<IBaseMessageContext>(behavior, Get(Object.Context));

			// the following two lines should have been used instead
			//_contextMock = new Context.Mock<IBaseMessageContext>(behavior);
			//base.Setup(msg => msg.Context).Returns(_contextMock.Object);

			// hook GetOriginalDataStream() onto BodyPart.Data, so that it does not fail when BodyPart has a Data stream
			base.Setup(msg => msg.BodyPart.GetOriginalDataStream()).Returns(() => Object.BodyPart.Data);
		}

		public ISetup<TMock> Setup(Expression<Action<IBaseMessage>> expression)
		{
			// intercept setup
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContextExtensions)))
			{
				return new ContextMethodCallSetup<TMock>(_contextMock.SetupCoreMethodCallExpression(methodCallExpression));
			}

			// let base class handle all other setup
			return base.Setup((Expression<Action<TMock>>) (Expression) expression);
		}

		public ISetup<TMock, object> Setup(Expression<Func<IBaseMessage, string>> expression)
		{
			// intercept setup
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContextExtensions))
				&& methodCallExpression.Method.Name == "GetProperty")
			{
				// rewrite setup to delegate it to context mock
				var qualifiedName = _contextMock.GetContextPropertyXmlQualifiedName(methodCallExpression);
				return new ContextFunctionCallSetup<TMock, object>(_contextMock.Setup(ctxt => ctxt.Read(qualifiedName.Name, qualifiedName.Namespace)));
			}

			// let base class handle all other setup
			return base.Setup((Expression<Func<TMock, object>>) (Expression) expression);
		}

		public ISetup<TMock, bool> Setup(Expression<Func<IBaseMessage, bool>> expression)
		{
			// intercept setup
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContextExtensions))
				&& methodCallExpression.Method.Name == "IsPromoted")
			{
				// rewrite setup to delegate it to context mock
				var qualifiedName = _contextMock.GetContextPropertyXmlQualifiedName(methodCallExpression);
				return new ContextFunctionCallSetup<TMock, bool>(_contextMock.Setup(ctxt => ctxt.IsPromoted(qualifiedName.Name, qualifiedName.Namespace)));
			}

			// let base class handle all other setup
			return base.Setup((Expression<Func<TMock, bool>>) (Expression) expression);
		}

		public ISetup<TMock, object> Setup<TResult>(Expression<Func<IBaseMessage, TResult?>> expression) where TResult : struct
		{
			// intercept and rewrite IBaseMessage setup against IBaseMessageContext
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContextExtensions))
				&& methodCallExpression.Method.Name == "GetProperty")
			{
				// rewrite setup to delegate it to context mock
				var qualifiedName = _contextMock.GetContextPropertyXmlQualifiedName(methodCallExpression);
				return new ContextFunctionCallSetup<TMock, object>(_contextMock.Setup(ctxt => ctxt.Read(qualifiedName.Name, qualifiedName.Namespace)));
			}

			// let base class handle all other setup
			return base.Setup((Expression<Func<TMock, object>>) (Expression) expression);
		}

		public ISetup<TMock, TResult> Setup<TResult>(Expression<Func<IBaseMessage, TResult>> expression)
		{
			// intercept setup
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContextExtensions)))
				throw new InvalidOperationException(
					string.Format(
						"Unexpected call of extension method: '{0}'.",
						methodCallExpression.Method.Name));

			// let base class handle all other setup
			return base.Setup((Expression<Func<TMock, TResult>>) (Expression) expression);
		}

		public new void Verify()
		{
			_contextMock.Verify();
			base.Verify();
		}

		public new void VerifyAll()
		{
			_contextMock.VerifyAll();
			base.VerifyAll();
		}

		public void Verify(Expression<Action<IBaseMessage>> expression)
		{
			Verify(expression, Times.AtLeastOnce(), null);
		}

		public void Verify(Expression<Action<IBaseMessage>> expression, Func<Times> times)
		{
			Verify(expression, times(), null);
		}

		public void Verify(Expression<Action<IBaseMessage>> expression, Times times)
		{
			Verify(expression, times, null);
		}

		public void Verify(Expression<Action<IBaseMessage>> expression, string failMessage)
		{
			Verify(expression, Times.AtLeastOnce(), failMessage);
		}

		public void Verify(Expression<Action<IBaseMessage>> expression, Func<Times> times, string failMessage)
		{
			Verify(expression, times(), failMessage);
		}

		public void Verify(Expression<Action<IBaseMessage>> expression, Times times, string failMessage)
		{
			// intercept and rewrite IBaseMessage Verify calls against IBaseMessageContext
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf(typeof(BaseMessage), typeof(BaseMessageContextExtensions)))
			{
				// rewrite expression to let base Moq class handle It.Is<> and It.IsAny<> expressions should there be any
				var rewrittenExpression = _contextMock.RewriteExpression(methodCallExpression);
				_contextMock.Verify(rewrittenExpression, times, failMessage);
			}
			else if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IsTypeOf<IBaseMessageContext>())
			{
				var parameter = Expression.Parameter(typeof(IBaseMessageContext), "ctxt");
				var ec = Expression.Call(parameter, methodCallExpression.Method, methodCallExpression.Arguments);
				var rewrittenExpression = Expression.Lambda<Action<IBaseMessageContext>>(ec, parameter);
				_contextMock.Verify(rewrittenExpression, times, failMessage);
			}
			else
			{
				// let base class handle all other Verify calls
				Verify((Expression<Action<TMock>>) (Expression) expression, times, failMessage);
			}
		}

		private readonly Context.Mock<IBaseMessageContext> _contextMock;
	}
}

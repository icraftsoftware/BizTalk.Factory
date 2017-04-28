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
	/// <typeparam name="T">
	/// Type to mock, which can be an interface or a class; in this case, <see cref="IBaseMessage"/>.
	/// </typeparam>
	/// <seealso cref="BaseMessage.GetProperty{T}(IBaseMessage,MessageContextProperty{T,string})"/>
	/// <seealso cref="BaseMessage.GetProperty{T,TResult}(IBaseMessage,MessageContextProperty{T,TResult})"/>
	/// <seealso cref="BaseMessage.SetProperty{T}(IBaseMessage,ContextProperties.MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessage.SetProperty{T,TV}(IBaseMessage,ContextProperties.MessageContextProperty{T,TV},TV)"/>
	/// <seealso cref="BaseMessage.Promote{T}(IBaseMessage,ContextProperties.MessageContextProperty{T,string},string)"/>
	/// <seealso cref="BaseMessage.Promote{T,TV}(IBaseMessage,ContextProperties.MessageContextProperty{T,TV},TV)"/>
	[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
	public class Mock<T> : Moq.Mock<T> where T : class, IBaseMessage
	{
		public Mock() : this(MockBehavior.Default) { }

		public Mock(MockBehavior behavior) : this(behavior, new object[0]) { }

		public Mock(params object[] args) : this(MockBehavior.Default, args) { }

		public Mock(MockBehavior behavior, params object[] args) : base(behavior, args)
		{
			// avoid NullReferenceException when calling .GetProperty() extension on IBaseMessage mock and no property setup has been performed
			_contextMock = new Context.Mock<IBaseMessageContext>(behavior);
			Setup(m => m.Context).Returns(_contextMock.Object);

			// hook GetOriginalDataStream() onto BodyPart.Data, so that it does not fail when BodyPart has a Data stream
			Setup(m => m.BodyPart.GetOriginalDataStream()).Returns(() => Object.BodyPart.Data);
		}

		public ISetup<T> Setup(Expression<Action<IBaseMessage>> expression)
		{
			// intercept Setups
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(BaseMessage))
			{
				// rewrite Setups
				var qualifiedName = _contextMock.GetContextPropertyXmlQualifiedName(methodCallExpression);
				switch (methodCallExpression.Method.Name)
				{
					case "DeleteProperty":
						return base.Setup(m => m.Context.Write(qualifiedName.Name, qualifiedName.Namespace, null));
					//return SetupContext(m => m.Context.Write(qualifiedName.Name, qualifiedName.Namespace, null));
					case "SetProperty":
						var writtenValue = Expression.Lambda(methodCallExpression.Arguments[2]).Compile().DynamicInvoke();
						return base.Setup(m => m.Context.Write(qualifiedName.Name, qualifiedName.Namespace, writtenValue));
					//return SetupContext(m => m.Context.Write(qualifiedName.Name, qualifiedName.Namespace, writtenValue));
					case "Promote":
						var promotedValue = Expression.Lambda(methodCallExpression.Arguments[2]).Compile().DynamicInvoke();
						// setup IsPromoted as well, but to mock how a promoted property actually behaves it must also have a value to be read
						base.Setup(m => m.Context.Read(qualifiedName.Name, qualifiedName.Namespace)).Returns(promotedValue);
						base.Setup(m => m.Context.IsPromoted(qualifiedName.Name, qualifiedName.Namespace)).Returns(true);
						return base.Setup(m => m.Context.Promote(qualifiedName.Name, qualifiedName.Namespace, promotedValue));
					default:
						throw new NotSupportedException(string.Format("Unexpected IBaseMessage extension method: '{0}'.", methodCallExpression.Method.Name));
				}
			}

			// let base class handle all other Setups
			return Setup((Expression<Action<T>>) (Expression) expression);
		}

		public ISetup<T, object> Setup(Expression<Func<IBaseMessage, string>> expression)
		{
			// intercept Setups
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(BaseMessage) && methodCallExpression.Method.Name == "GetProperty")
			{
				// rewrite Setups
				var qualifiedName = _contextMock.GetContextPropertyXmlQualifiedName(methodCallExpression);
				return base.Setup(m => m.Context.Read(qualifiedName.Name, qualifiedName.Namespace));
			}

			// let base class handle all other Setups
			return Setup((Expression<Func<T, object>>) (Expression) expression);
		}

		public ISetup<T, bool> Setup(Expression<Func<IBaseMessage, bool>> expression)
		{
			// intercept Setups
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(BaseMessage) && methodCallExpression.Method.Name == "IsPromoted")
			{
				// rewrite Setups
				var qualifiedName = _contextMock.GetContextPropertyXmlQualifiedName(methodCallExpression);
				return base.Setup(m => m.Context.IsPromoted(qualifiedName.Name, qualifiedName.Namespace));
			}

			// let base class handle all other Setups
			return Setup((Expression<Func<T, bool>>) (Expression) expression);
		}

		public ISetup<T, object> Setup<TResult>(Expression<Func<IBaseMessage, TResult?>> expression) where TResult : struct
		{
			// intercept and rewrite IBaseMessage Setups against IBaseMessageContext
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(BaseMessage) && methodCallExpression.Method.Name == "GetProperty")
			{
				// rewrite Setups
				var qualifiedName = _contextMock.GetContextPropertyXmlQualifiedName(methodCallExpression);
				return base.Setup(m => m.Context.Read(qualifiedName.Name, qualifiedName.Namespace));
			}

			// let base class handle all other Setups
			return Setup((Expression<Func<T, object>>) (Expression) expression);
		}

		public ISetup<T, TResult> Setup<TResult>(Expression<Func<IBaseMessage, TResult>> expression)
		{
			// intercept Setups
			var methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(BaseMessage))
				throw new NotSupportedException(
					string.Format(
						"Unexpected IBaseMessage extension method: '{0}'.",
						methodCallExpression.Method.Name));

			// let base class handle all other Setups
			return Setup((Expression<Func<T, TResult>>) (Expression) expression);
		}

		public new void Verify()
		{
			_contextMock.Verify();
			base.Verify();
		}

		public new void VerifyAll()
		{
			// TODO var c = Object.Context; <-- can't have this setup to be verified
#pragma warning disable 168
			// TODO ? why thise line ?
			// ReSharper disable once UnusedVariable
			var p = Object.BodyPart;
#pragma warning restore 168
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
			if (methodCallExpression != null && methodCallExpression.Method.DeclaringType.IfNotNull(t => t == typeof(BaseMessage) || t == typeof(BaseMessageContextExtensions)))
			{
				// rewrite expression to let base Moq class handle It.Is<> and It.IsAny<> expressions should there be any
				var rewrittenExpression = _contextMock.RewriteExpression(methodCallExpression);
				_contextMock.Verify(rewrittenExpression, times, failMessage);
			}
			else if (methodCallExpression != null && methodCallExpression.Method.DeclaringType == typeof(IBaseMessageContext))
			{
				var parameter = Expression.Parameter(typeof(IBaseMessageContext), "c");
				var ec = Expression.Call(parameter, methodCallExpression.Method, methodCallExpression.Arguments);
				var rewrittenExpression = Expression.Lambda<Action<IBaseMessageContext>>(ec, parameter);
				_contextMock.Verify(rewrittenExpression, times, failMessage);
			}
			else
			{
				// let base class handle all other Verify calls
				Verify((Expression<Action<T>>) (Expression) expression, times, failMessage);
			}
		}

		private readonly Context.Mock<IBaseMessageContext> _contextMock;
	}
}

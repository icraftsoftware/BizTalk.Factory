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
using Microsoft.BizTalk.Message.Interop;
using Moq.Language.Flow;

namespace Be.Stateless.BizTalk.Unit.Message.Language.Flow
{
	internal class ContextFunctionCallReturn<TMock, TResult> : IReturnsThrows<TMock, TResult> // IReturnsResult<TMock> //
		where TMock : class, IBaseMessage
	{
		public ContextFunctionCallReturn(IReturnsThrows<IBaseMessageContext, TResult> contextFunctionCallReturnImplementation)
		{
			if (contextFunctionCallReturnImplementation == null) throw new ArgumentNullException("contextFunctionCallReturnImplementation");
			_contextFunctionCallReturnImplementation = contextFunctionCallReturnImplementation;
		}

		#region IReturnsThrows<TMock,TResult> Members

		public IThrowsResult Throws(Exception exception)
		{
			return _contextFunctionCallReturnImplementation.Throws(exception);
		}

		public IThrowsResult Throws<TException>() where TException : Exception, new()
		{
			return _contextFunctionCallReturnImplementation.Throws<TException>();
		}

		public IReturnsResult<TMock> Returns(TResult value)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(value));
		}

		public IReturnsResult<TMock> Returns(Func<TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T>(Func<T, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> CallBase()
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.CallBase());
		}

		public IReturnsResult<TMock> Returns<T1, T2>(Func<T1, T2, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallReturnImplementation.Returns(valueFunction));
		}

		#endregion

		private readonly IReturnsThrows<IBaseMessageContext, TResult> _contextFunctionCallReturnImplementation;
	}
}

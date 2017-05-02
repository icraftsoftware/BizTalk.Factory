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
	internal class ContextFunctionCallSetup<TMock, TResult> : ISetup<TMock, TResult>
		where TMock : class, IBaseMessage
	{
		public ContextFunctionCallSetup(ISetup<IBaseMessageContext, TResult> contextFunctionCallSetupImplementation)
		{
			if (contextFunctionCallSetupImplementation == null) throw new ArgumentNullException("contextFunctionCallSetupImplementation");
			_contextFunctionCallSetupImplementation = contextFunctionCallSetupImplementation;
		}

		#region ISetup<TMock,TResult> Members

		public IReturnsResult<TMock> Returns(TResult value)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(value));
		}

		public IReturnsResult<TMock> Returns(Func<TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T>(Func<T, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> CallBase()
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.CallBase());
		}

		public IReturnsResult<TMock> Returns<T1, T2>(Func<T1, T2, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IReturnsResult<TMock> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> valueFunction)
		{
			return new ContextMethodCallReturn<TMock>(_contextFunctionCallSetupImplementation.Returns(valueFunction));
		}

		public IThrowsResult Throws(Exception exception)
		{
			return _contextFunctionCallSetupImplementation.Throws(exception);
		}

		public IThrowsResult Throws<TException>() where TException : Exception, new()
		{
			return _contextFunctionCallSetupImplementation.Throws<TException>();
		}

		public void Verifiable()
		{
			_contextFunctionCallSetupImplementation.Verifiable();
		}

		public void Verifiable(string failMessage)
		{
			_contextFunctionCallSetupImplementation.Verifiable(failMessage);
		}

		public IReturnsThrows<TMock, TResult> Callback(Action action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T>(Action<T> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2>(Action<T1, T2> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3>(Action<T1, T2, T3> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		public IReturnsThrows<TMock, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
			Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action)
		{
			return new ContextFunctionCallReturn<TMock, TResult>(_contextFunctionCallSetupImplementation.Callback(action));
		}

		#endregion

		private readonly ISetup<IBaseMessageContext, TResult> _contextFunctionCallSetupImplementation;
	}
}

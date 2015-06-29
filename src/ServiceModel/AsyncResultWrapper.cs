#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using System.Threading;
using Be.Stateless.ServiceModel;

namespace Be.Stateless
{
	/// <summary>
	/// <see cref="IAsyncResult"/> wrapper to be used to wrap around another <see cref="IAsyncResult"/> when one wants to
	/// intercept an asynchronous call and provides his own user-defined <see cref="AsyncState"/> object.
	/// </summary>
	/// <remarks>
	/// See <see cref="ServiceRelay.BeginRelayRequest{TRequest}(TRequest,System.AsyncCallback,object)"/> for sample
	/// usages.
	/// </remarks>
	/// <seealso cref="IAsyncResult"/>
	public class AsyncResultWrapper : IAsyncResult
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncResultWrapper"/> around another specified <paramref
		/// name="asyncResult"/> and using the specified <paramref name="asyncState"/>.
		/// </summary>
		/// <param name="asyncResult">
		/// The <see cref="IAsyncResult"/> to wrap.
		/// </param>
		/// <param name="asyncState">
		/// A user-defined <see cref="object"/> that qualifies or contains information about an asynchronous operation.
		/// </param>
		public AsyncResultWrapper(IAsyncResult asyncResult, object asyncState)
		{
			AsyncState = asyncState;
			WrappedAsyncResult = asyncResult;
		}

		#region IAsyncResult Members

		/// <summary>
		/// Gets a value that indicates whether the asynchronous operation has completed.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the operation is complete; <c>false</c> otherwise.
		/// </returns>
		public bool IsCompleted
		{
			get { return WrappedAsyncResult.IsCompleted; }
		}

		/// <summary>
		/// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to
		/// complete.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
		/// </returns>
		public WaitHandle AsyncWaitHandle
		{
			get { return WrappedAsyncResult.AsyncWaitHandle; }
		}

		/// <summary>
		/// Gets a user-defined <see cref="object"/> that qualifies or contains information about an asynchronous
		/// operation.
		/// </summary>
		/// <returns>
		/// A user-defined <see cref="object"/> that qualifies or contains information about an asynchronous operation.
		/// </returns>
		public object AsyncState { get; private set; }

		/// <summary>
		/// Gets a value that indicates whether the asynchronous operation completed synchronously.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the asynchronous operation completed synchronously; <c>false</c> otherwise.
		/// </returns>
		public bool CompletedSynchronously
		{
			get { return WrappedAsyncResult.CompletedSynchronously; }
		}

		#endregion

		/// <summary>
		/// The wrapped <see cref="IAsyncResult"/>.
		/// </summary>
		public IAsyncResult WrappedAsyncResult { get; private set; }
	}
}

#region Copyright & License

// Copyright © 2012 - 2014 François Chabot, Yves Dierick
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using Be.Stateless.BizTalk.Unit.ServiceModel.Stub.Language;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	/// <summary>
	/// Collection of all the <see cref="IOperationCallSetup{TContract}"/> objects that have been defined via <see
	/// cref="ISetupOperation{TContract}"/> in terms of either SOAP actions or in terms of <see cref="DocumentSpec"/>
	/// message types.
	/// </summary>
	internal class OperationCallSetupCollection
	{
		/// <summary>
		/// Gets the <see cref="IOperationCallSetup{TContract,TResult}"/> object associated to either a <paramref
		/// name="messageType"/> message type or a SOAP <paramref name="action"/>.
		/// </summary>
		/// <param name="messageType">
		/// A <see cref="DocumentSpec"/> message type.
		/// </param>
		/// <param name="action">
		/// A SOAP action.
		/// </param>
		/// <returns>
		/// The <see cref="IOperationCallSetup{TContract,TResult}"/> setup.
		/// </returns>
		/// <remarks>
		/// Notice that if <see cref="IOperationCallSetup{TContract,TResult}"/> setups have been defined for both a <see
		/// cref="DocumentSpec"/> message type and a SOAP action, <paramref name="messageType"/> will have precedence over
		/// <paramref name="action"/>.
		/// </remarks>
		internal OperationCallSetup this[string messageType, string action]
		{
			get
			{
				OperationCallSetup setup;
				if (_setups.TryGetValue(messageType, out setup)) return setup;
				if (_setups.TryGetValue(action, out setup)) return setup;
				throw new KeyNotFoundException(
					string.Format(
						"No operation setup has been performed for neither the message type '{0}' nor the SOAP action '{1}'.",
						messageType,
						action));
			}
		}

		/// <summary>
		/// Adds a <see cref="IOperationCallbackSetup{TContract,TResult}"/> object to the <see
		/// cref="OperationCallSetupCollection"/> if the key does not already exist; returns the <see
		/// cref="IOperationCallbackSetup{TContract,TResult}"/> object otherwise.
		/// </summary>
		/// <param name="key">
		/// The key to use, which is either a <see cref="DocumentSpec"/> message type or a SOAP action.
		/// </param>
		/// <returns>
		/// The <see cref="IOperationCallbackSetup{TContract,TResult}"/> object associated to the <paramref name="key"/>.
		/// </returns>
		internal IOperationCallSetup<TContract, TResult> Add<TContract, TResult>(string key)
			where TContract : class
		{
			return (IOperationCallSetup<TContract, TResult>) Add(key, () => new OperationCallSetup<TContract, TResult>());
		}

		internal IOperationCallSetup<TContract> Add<TContract>(string key)
			where TContract : class
		{
			return (IOperationCallSetup<TContract>) Add(key, () => new OperationCallSetup<TContract>());
		}

		private OperationCallSetup Add(string key, Func<OperationCallSetup> operationCallSetupFactory)
		{
			return _setups.GetOrAdd(key, k => operationCallSetupFactory());
		}

		internal void Clear()
		{
			_setups.Clear();
		}

		private static readonly ConcurrentDictionary<string, OperationCallSetup> _setups = new ConcurrentDictionary<string, OperationCallSetup>();
	}
}

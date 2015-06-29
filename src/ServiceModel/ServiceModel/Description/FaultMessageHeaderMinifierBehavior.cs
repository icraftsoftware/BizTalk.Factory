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

using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Be.Stateless.ServiceModel.Dispatcher;

namespace Be.Stateless.ServiceModel.Description
{
	/// <summary>
	/// Extends run-time behavior for an endpoint in client application.
	/// </summary>
	public class FaultMessageHeaderMinifierBehavior : IEndpointBehavior
	{
		#region IEndpointBehavior Members

		/// <summary>
		/// Passes data at runtime to bindings to support custom behavior.
		/// </summary>
		/// <param name="endpoint">
		/// The endpoint to modify.
		/// </param>
		/// <param name="bindingParameters">
		/// The objects that binding elements require to support the behavior.
		/// </param>
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

		/// <summary>
		/// Implements a modification or extension of the client across an endpoint.
		/// </summary>
		/// <param name="endpoint">
		/// The endpoint that is to be customized.
		/// </param>
		/// <param name="clientRuntime">
		/// The client runtime to be customized.
		/// </param>
		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			var fmm = new FaultMessageHeaderMinifier();
			clientRuntime.MessageInspectors.Add(fmm);
		}

		/// <summary>
		/// Implements a modification or extension of the service across an endpoint.
		/// </summary>
		/// <param name="endpoint">
		/// The endpoint that exposes the contract.
		/// </param>
		/// <param name="endpointDispatcher">
		/// The endpoint dispatcher to be modified or extended.
		/// </param>
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

		/// <summary>
		/// Implement to confirm that the endpoint meets some intended criteria.
		/// </summary>
		/// <param name="endpoint">
		/// The endpoint to validate.
		/// </param>
		public void Validate(ServiceEndpoint endpoint) { }

		#endregion
	}
}

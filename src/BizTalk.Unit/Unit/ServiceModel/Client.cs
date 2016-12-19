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

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	/// <summary>
	/// Provides a basic service proxy client implementation that can exchange messages through the <typeparamref
	/// name="TChannel"/>-shaped channel.
	/// </summary>
	/// <typeparam name="TChannel">
	/// The shape of the channel to be used to connect to the service's endpoint.
	/// </typeparam>
	public class Client<TChannel> : ClientBase<TChannel>
		where TChannel : class
	{
		/// <summary>
		/// Creates a new instance of the <see cref="Client{TChannel}"/> class using the specified service <paramref
		/// name="endpoint"/>.
		/// </summary>
		/// <param name="endpoint">
		/// The service endpoint.
		/// </param>
		/// <returns>
		/// The <typeparamref name="TChannel"/> channel to be used to connect to the service.
		/// </returns>
		/// <remarks>
		/// Notice that only the <see cref="Binding"/> and <see cref="EndpointAddress"/> pieces of the <paramref
		/// name="endpoint"/> are used to create the client. This method is therefore essentially the same as its <see
		/// cref="Create(System.ServiceModel.Channels.Binding,System.ServiceModel.EndpointAddress)"/> overload.
		/// </remarks>
		public static TChannel Create(ServiceEndpoint endpoint)
		{
			return new Client<TChannel>(endpoint.Binding, endpoint.Address).Channel;
		}

		/// <summary>
		/// Creates a new instance of the <see cref="Client{TChannel}"/> class using the specified <paramref
		/// name="binding"/> and target <paramref name="address"/>.
		/// </summary>
		/// <param name="binding">
		/// The binding with which to make calls to the service.
		/// </param>
		/// <param name="address">
		/// The address of the service endpoint.
		/// </param>
		/// <returns>
		/// The <typeparamref name="TChannel"/> channel to be used to connect to the service.
		/// </returns>
		public static TChannel Create(Binding binding, EndpointAddress address)
		{
			return new Client<TChannel>(binding, address).Channel;
		}

		protected Client(Binding binding, EndpointAddress address) : base(binding, address) { }
	}
}

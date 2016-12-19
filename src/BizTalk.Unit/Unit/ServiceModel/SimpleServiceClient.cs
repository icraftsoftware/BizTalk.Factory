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

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	/// <summary>
	/// Provides a basic service proxy client implementation that can call the <seealso
	/// cref="SimpleServiceHost{TService,TChannel}"/>'s endpoint to exchange messages through the <typeparamref
	/// name="TChannel"/>-shaped channel.
	/// </summary>
	/// <typeparam name="TService">
	/// The service class that implements the <typeparamref name="TChannel"/> service contract.
	/// </typeparam>
	/// <typeparam name="TChannel">
	/// The shape of the channel to be used to connect to the service's endpoint.
	/// </typeparam>
	public class SimpleServiceClient<TService, TChannel> : Client<TChannel>
		where TService : TChannel
		where TChannel : class
	{
		/// <summary>
		/// Creates a new instance of the <see cref="SimpleServiceHost{TService,TChannel}"/> class that can exchange
		/// messages with the default <see cref="SimpleServiceHost{TService,TChannel}"/> host.
		/// </summary>
		/// <returns>
		/// The <typeparamref name="TChannel"/> channel to be used to connect to the service.
		/// </returns>
		public static TChannel Create()
		{
			return new SimpleServiceClient<TService, TChannel>().Channel;
		}

		private SimpleServiceClient() : base(SimpleServiceHost<TService, TChannel>.Endpoint.Binding, SimpleServiceHost<TService, TChannel>.Endpoint.Address) { }
	}
}

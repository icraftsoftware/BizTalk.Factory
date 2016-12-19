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

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	/// <summary>
	/// Provides a basic service proxy client implementation that can call the <see cref="IStubService"/> endpoint to
	/// perform solicit-response-only message exchanges.
	/// </summary>
	internal static class StubServiceClient
	{
		public static IRequestChannel Create()
		{
			return StubServiceClient<IRequestChannel>.Create();
		}
	}

	/// <summary>
	/// Provides a basic service proxy client implementation that can call the <see cref="IStubService"/> endpoint to
	/// exchange messages through the <typeparamref name="TChannel"/>-shaped channel.
	/// </summary>
	/// <typeparam name="TChannel">
	/// The shape of the channel to be used to connect to the service's endpoint.
	/// </typeparam>
	internal class StubServiceClient<TChannel> : ClientBase<TChannel>
		where TChannel : class
	{
		public static TChannel Create()
		{
			return new StubServiceClient<TChannel>().Channel;
		}

		private StubServiceClient() : base(StubServiceHost.DefaultBinding, StubServiceHost.DefaultEndpointAddress) { }
	}
}

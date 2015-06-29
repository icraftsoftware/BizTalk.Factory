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

namespace Be.Stateless.BizTalk.Unit.ServiceModel.Stub
{
	/// <summary>
	/// Defines a most-general contract that the <see cref="IStubService"/> implements to be on the responding side of a
	/// request-reply communication between messaging endpoints.
	/// </summary>
	[ServiceContract(Namespace = "urn:services.stateless.be:unit:2013:01:process-message")]
	internal interface IStubService
	{
		/// <summary>
		/// Sends a <see cref="System.ServiceModel.Channels.Message"/>-based request and returns the correlated <see
		/// cref="System.ServiceModel.Channels.Message"/>-based response.
		/// </summary>
		/// <param name="request">
		/// The request <see cref="T:System.ServiceModel.Channels.Message"/> to be transmitted.
		/// </param>
		/// <returns>
		/// The <see cref="T:System.ServiceModel.Channels.Message"/> received in response to the request.
		/// </returns>
		[OperationContract(Action = "*", ReplyAction = "*")]
		System.ServiceModel.Channels.Message Request(System.ServiceModel.Channels.Message request);
	}
}

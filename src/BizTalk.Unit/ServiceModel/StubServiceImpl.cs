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
using System.ServiceModel;
using Be.Stateless.BizTalk.Unit.ServiceModel.Channels.Extensions;

namespace Be.Stateless.BizTalk.Unit.ServiceModel
{
	[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any,
		ConcurrencyMode = ConcurrencyMode.Multiple,
		InstanceContextMode = InstanceContextMode.Single,
		ValidateMustUnderstand = false)]
	internal class StubServiceImpl : IStubService
	{
		#region IStubService Members

		System.ServiceModel.Channels.Message IStubService.Request(System.ServiceModel.Channels.Message request)
		{
			var action = request.Headers.Action;
			var messageType = request.GetMessageType();
			var response = ResponseSetupCollection.Instance[messageType, action];
			if (response == null)
				throw new InvalidOperationException(
					string.Format(
						"No response stream has been setup for the message type '{0}' or the SOAP action '{1}'.",
						messageType,
						action));

			if (response.CallbackAction != null)
			{
				response.CallbackAction();
			}

			if (response.MustAbort)
			{
				OperationContext.Current.Channel.Abort();
				return null;
			}

			return System.ServiceModel.Channels.Message.CreateMessage(
				request.Version,
				action + "/response",
				response.Body);
		}

		#endregion
	}
}

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
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Be.Stateless.BizTalk.Tracking.Messaging;

namespace Be.Stateless.ServiceModel.Dispatcher
{
	/// <summary>
	/// Remove security header from fault message so that, when a fault message is serialized by <see
	/// cref="MessagingStep" />, the actual fault reason is not truncated away due to the lengthy security headers.
	/// </summary>
	/// <seealso href="http://weblogs.asp.net/paolopia/archive/2008/02/25/handling-custom-soap-headers-via-wcf-behaviors.aspx"/>
	/// <seealso href="http://msdn.microsoft.com/en-us/library/aa717047.aspx"/>
	public class FaultMessageHeaderMinifier : IClientMessageInspector
	{
		#region IClientMessageInspector Members

		/// <summary>
		/// Enables inspection or modification of a message before a request message is sent to a service.
		/// </summary>
		/// <returns>
		/// The object that is returned as the <i>correlationState</i> argument of the <see
		/// cref="IClientMessageInspector.AfterReceiveReply"/> method. This is null if no correlation state is used. The
		/// best practice is to make this a <see cref="Guid"/> to ensure that no two <i>correlationState</i> objects are
		/// the same.
		/// </returns>
		/// <param name="request">
		/// The message to be sent to the service.
		/// </param>
		/// <param name="channel">
		/// The  client object channel.
		/// </param>
		/// <returns>
		/// </returns>
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			return null;
		}

		/// <summary>
		/// Enables inspection or modification of a message after a reply message is received but prior to passing it back
		/// to the client application.
		/// </summary>
		/// <param name="reply">
		/// The message to be transformed into types and handed back to the client application.
		/// </param>
		/// <param name="correlationState">
		/// Correlation state data.
		/// </param>
		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			if (reply.IsFault) reply.Headers.RemoveAll("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
		}

		#endregion
	}
}

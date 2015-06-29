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

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Be.Stateless.ServiceModel.Dispatcher
{
	/// <summary>
	/// Remove security header from fault message so that when a fault message is
	/// serialized by <see
	/// cref="Be.Stateless.BizTalk.Tracking.Messaging.MessagingStepTracking" />
	/// the actual fault reason is not truncated away due to the lengthy security
	/// headers.
	/// </summary>
	/// <seealso href="http://weblogs.asp.net/paolopia/archive/2008/02/25/handling-custom-soap-headers-via-wcf-behaviors.aspx"/>
	/// for custom config, <seealso
	/// href="http://msdn.microsoft.com/en-us/library/aa717047.aspx"/>
	public class FaultMessageHeaderMinifier : IClientMessageInspector
	{
		#region IClientMessageInspector Members

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			return null;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			if (reply.IsFault) reply.Headers.RemoveAll("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
		}

		#endregion
	}
}
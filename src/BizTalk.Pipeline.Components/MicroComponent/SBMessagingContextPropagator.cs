#region Copyright & License

// Copyright © 2012 - 2018 François Chabot
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

using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.ContextProperties.Extensions;
using Be.Stateless.BizTalk.Message.Extensions;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent
{
	/// <summary>
	/// Propagates message type and correlation token inwards or outwards.
	/// </summary>
	/// <remarks>
	/// <para>
	/// For inbound messages, <see cref="SBMessagingProperties.ContentType"/> and <see
	/// cref="SBMessagingProperties.CorrelationId"/>, if any, are respectively propagated as <see
	/// cref="BtsProperties.MessageType"/> and <see cref="BizTalkFactoryProperties.CorrelationToken"/>. Notice that <see
	/// cref="BizTalkFactoryProperties.CorrelationToken"/> is moreover promoted.
	/// </para>
	/// <para>
	/// For outbound messages, <see cref="BtsProperties.MessageType"/> and <see
	/// cref="BizTalkFactoryProperties.CorrelationToken"/>, if any, are respectively propagated as <see
	/// cref="SBMessagingProperties.ContentType"/> and <see cref="SBMessagingProperties.CorrelationId"/>.
	/// </para>
	/// </remarks>
	public class SBMessagingContextPropagator : IMicroPipelineComponent
	{
		#region IMicroPipelineComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			if (message.Direction().IsInbound())
			{
				var correlationId = message.GetProperty(SBMessagingProperties.CorrelationId);
				if (!correlationId.IsNullOrEmpty()) message.PromoteCorrelationToken(correlationId);
				var contentType = message.GetProperty(SBMessagingProperties.ContentType);
				if (!contentType.IsNullOrEmpty()) message.SetProperty(BtsProperties.MessageType, contentType);
			}
			else
			{
				var correlationToken = message.GetProperty(BizTalkFactoryProperties.CorrelationToken);
				if (!correlationToken.IsNullOrEmpty()) message.SetProperty(SBMessagingProperties.CorrelationId, correlationToken);
				var messageType = message.GetProperty(BtsProperties.MessageType);
				if (!messageType.IsNullOrEmpty()) message.SetProperty(SBMessagingProperties.ContentType, messageType);
			}
			return message;
		}

		#endregion
	}
}

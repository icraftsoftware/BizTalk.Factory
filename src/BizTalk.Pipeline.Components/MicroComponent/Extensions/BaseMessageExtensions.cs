﻿#region Copyright & License

// Copyright © 2012 - 2019 François Chabot
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

using Be.Stateless.BizTalk.Component.Extensions;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Message.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.MicroComponent.Extensions
{
	public static class BaseMessageExtensions
	{
		public static string GetOrProbeMessageType(this IBaseMessage message, IPipelineContext pipelineContext)
		{
			return message.GetOrProbeMessageType(pipelineContext.ResourceTracker);
		}

		public static string ProbeMessageType(this IBaseMessage message, IPipelineContext pipelineContext)
		{
			return message.ProbeMessageType(pipelineContext.ResourceTracker);
		}

		public static void ProbeAndPromoteMessageType(this IBaseMessage message, IPipelineContext pipelineContext)
		{
			var messageType = message.ProbeMessageType(pipelineContext);
			var docSpec = pipelineContext.GetDocumentSpecByType(messageType);
			message.Promote(BtsProperties.MessageType, docSpec.DocType);
			message.Promote(BtsProperties.SchemaStrongName, docSpec.DocSpecStrongName);
		}

		public static void TryProbeAndPromoteMessageType(this IBaseMessage message, IPipelineContext pipelineContext)
		{
			var messageType = message.ProbeMessageType(pipelineContext);
			IDocumentSpec docSpec;
			if (pipelineContext.TryGetDocumentSpecByType(messageType, out docSpec))
			{
				message.Promote(BtsProperties.MessageType, docSpec.DocType);
				message.Promote(BtsProperties.SchemaStrongName, docSpec.DocSpecStrongName);
			}
		}
	}
}

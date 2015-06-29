#region Copyright & License

// Copyright © 2012 - 2015 François Chabot, Yves Dierick
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

using Be.Stateless.BizTalk.Schemas.BizTalkFactory;

namespace Be.Stateless.BizTalk.ContextProperties
{
	public static class BizTalkFactoryProperties
	{
		public static readonly MessageContextProperty<ArchiveTargetLocation, string> ArchiveTargetLocation
			= new MessageContextProperty<ArchiveTargetLocation, string>();

		public static readonly MessageContextProperty<ClaimedMessageType, string> ClaimedMessageType
			= new MessageContextProperty<ClaimedMessageType, string>();

		public static readonly MessageContextProperty<ContextBuilderTypeName, string> ContextBuilderTypeName
			= new MessageContextProperty<ContextBuilderTypeName, string>();

		public static readonly MessageContextProperty<CorrelationToken, string> CorrelationToken
			= new MessageContextProperty<CorrelationToken, string>();

		public static readonly MessageContextProperty<EnvelopePartition, string> EnvelopePartition
			= new MessageContextProperty<EnvelopePartition, string>();

		public static readonly MessageContextProperty<EnvelopeSpecName, string> EnvelopeSpecName
			= new MessageContextProperty<EnvelopeSpecName, string>();

		public static readonly MessageContextProperty<MapTypeName, string> MapTypeName
			= new MessageContextProperty<MapTypeName, string>();

		public static readonly MessageContextProperty<MessageFactoryTypeName, string> MessageFactoryTypeName
			= new MessageContextProperty<MessageFactoryTypeName, string>();

		public static readonly MessageContextProperty<OutboundTransportLocation, string> OutboundTransportLocation
			= new MessageContextProperty<OutboundTransportLocation, string>();

		public static readonly MessageContextProperty<ReceiverName, string> ReceiverName
			= new MessageContextProperty<ReceiverName, string>();

		public static readonly MessageContextProperty<SenderName, string> SenderName
			= new MessageContextProperty<SenderName, string>();

		public static readonly MessageContextProperty<XmlTranslations, string> XmlTranslations
			= new MessageContextProperty<XmlTranslations, string>();
	}
}

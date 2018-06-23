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

using System.Diagnostics.CodeAnalysis;
using WCF;

namespace Be.Stateless.BizTalk.ContextProperties
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class WcfProperties
	{
		public static readonly MessageContextProperty<Action, string> Action
			= new MessageContextProperty<Action, string>();

		public static readonly MessageContextProperty<HttpHeaders, string> HttpHeaders
			= new MessageContextProperty<HttpHeaders, string>();

		public static readonly MessageContextProperty<InboundBodyLocation, string> InboundBodyLocation
			= new MessageContextProperty<InboundBodyLocation, string>();

		public static readonly MessageContextProperty<InboundBodyPathExpression, string> InboundBodyPathExpression
			= new MessageContextProperty<InboundBodyPathExpression, string>();

		public static readonly MessageContextProperty<OutboundBodyLocation, string> OutboundBodyLocation
			= new MessageContextProperty<OutboundBodyLocation, string>();

		public static readonly MessageContextProperty<OutboundXmlTemplate, string> OutboundXmlTemplate
			= new MessageContextProperty<OutboundXmlTemplate, string>();

		public static readonly MessageContextProperty<SendTimeout, string> SendTimeout
			= new MessageContextProperty<SendTimeout, string>();
	}
}

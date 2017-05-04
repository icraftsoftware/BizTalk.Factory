#region Copyright & License

// Copyright © 2012 - 2017 François Chabot, Yves Dierick
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
using EDI;

namespace Be.Stateless.BizTalk.ContextProperties
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class EdiProperties
	{
		public static readonly MessageContextProperty<BGM1_1, string> BGM1_1
			= new MessageContextProperty<BGM1_1, string>();

		public static readonly MessageContextProperty<DestinationPartyReceiverIdentifier, string> DestinationPartyReceiverIdentifier
			= new MessageContextProperty<DestinationPartyReceiverIdentifier, string>();

		public static readonly MessageContextProperty<DestinationPartyReceiverQualifier, string> DestinationPartyReceiverQualifier
			= new MessageContextProperty<DestinationPartyReceiverQualifier, string>();

		public static readonly MessageContextProperty<DestinationPartySenderIdentifier, string> DestinationPartySenderIdentifier
			= new MessageContextProperty<DestinationPartySenderIdentifier, string>();

		public static readonly MessageContextProperty<DestinationPartySenderQualifier, string> DestinationPartySenderQualifier
			= new MessageContextProperty<DestinationPartySenderQualifier, string>();

		public static readonly MessageContextProperty<MKS1, string> MKS1
			= new MessageContextProperty<MKS1, string>();

		public static readonly MessageContextProperty<Unb21, string> UNB2_1
			= new MessageContextProperty<Unb21, string>();

		public static readonly MessageContextProperty<Unb31, string> UNB3_1
			= new MessageContextProperty<Unb31, string>();
	}
}

#region Copyright & License

// Copyright © 2012 - 2016 François Chabot, Yves Dierick
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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using Microsoft.BizTalk.Adapter.POP3;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public abstract partial class Pop3Adapter : LegacyAdapterBase<POP3AdapterManagement>
	{
		#region AuthenticationScheme Enum

		public enum AuthenticationScheme
		{
			/// <summary>
			/// Password will be sent to POP3 server in clear text.
			/// </summary>
			[XmlEnum("Basic")]
			Basic,

			/// <summary>
			/// Password hash will be sent to POP3 server.
			/// </summary>
			[XmlEnum("Digest")]
			Digest,

			/// <summary>
			/// NTLM will be used for authentication.
			/// </summary>
			[XmlEnum("SPA")]
			SecurePasswordAuthentication
		}

		#endregion

		static Pop3Adapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("1787fcc1-9aaa-4bbd-9096-7eb77e3d9d9b"));
		}

		protected Pop3Adapter() : base(_protocolType) { }

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}

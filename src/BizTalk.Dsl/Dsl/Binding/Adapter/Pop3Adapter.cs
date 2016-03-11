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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API.")]
	public abstract partial class Pop3Adapter : AdapterBase
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

		#region PollingUnitOfMeasure Enum

		public enum PollingUnitOfMeasure
		{
			Seconds,

			Minutes,

			Hours,

			Days
		}

		#endregion

		static Pop3Adapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("1787fcc1-9aaa-4bbd-9096-7eb77e3d9d9b"));
		}

		#region XML Serialization

		private string Serialize()
		{
			var builder = new StringBuilder();
			using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
			{
				// http://stackoverflow.com/questions/625927/omitting-all-xsi-and-xsd-namespaces-when-serializing-an-object-in-net
				var ns = new XmlSerializerNamespaces();
				ns.Add(string.Empty, string.Empty);
				var serializer = new XmlSerializer(GetType());
				serializer.Serialize(writer, this, ns);
			}
			return builder.ToString();
		}

		#endregion


		protected Pop3Adapter() : base(_protocolType) { }


		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}

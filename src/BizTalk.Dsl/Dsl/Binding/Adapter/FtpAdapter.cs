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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.BizTalk.Adapter.FtpAdapter;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class FtpAdapter : LegacyAdapterBase<FtpAdapterManagement>
	{
		#region Nested Type: Boolean

		/// <summary>
		/// Hack to serialize <see cref="bool"/> back and forth to Pascal-cased strings.
		/// </summary>
		/// <remarks>
		/// This class is intended to be used only with <see cref="FtpAdapter"/>-derived classes.
		/// </remarks>
		public class Boolean : IXmlSerializable
		{
			#region Operators

			public static implicit operator Boolean(bool @bool)
			{
				return @bool ? True : False;
			}

			public static implicit operator bool(Boolean boolean)
			{
				return Convert.ToBoolean(boolean._value);
			}

			#endregion

			public static Boolean False
			{
				get { return new Boolean("False"); }
			}

			public static Boolean True
			{
				get { return new Boolean("True"); }
			}

			public Boolean()
			{
				_value = False._value;
			}

			private Boolean(string value)
			{
				_value = value;
			}

			#region IXmlSerializable Members

			public XmlSchema GetSchema()
			{
				return null;
			}

			public void ReadXml(XmlReader reader)
			{
				throw new NotSupportedException();
			}

			public void WriteXml(XmlWriter writer)
			{
				writer.WriteString(_value);
			}

			#endregion

			private readonly string _value;
		}

		#endregion

		#region FirewallType Enum

		public enum FirewallType
		{
			[XmlEnum("NoFirewall")]
			None,
			Socks4,
			Socks5
		}

		#endregion

		#region FtpsConnectionMode Enum

		public enum FtpsConnectionMode
		{
			Explicit,
			Implicit
		}

		#endregion

		#region Mode Enum

		public enum Mode
		{
			Active,
			Passive
		}

		#endregion

		#region RepresentationType Enum

		public enum RepresentationType
		{
			[XmlEnum("binary")]
			Binary,

			[XmlEnum("ASCII")]
			Ascii
		}

		#endregion

		static FtpAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("3979ffed-0067-4cc6-9f5a-859a5db6e9bb"));
		}

		protected FtpAdapter() : base(_protocolType) { }

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}

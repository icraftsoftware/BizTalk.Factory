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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Be.Stateless.BizTalk.Xml
{
	/// <summary>
	/// XML serializer surrogate that supports the serialization of <see cref="Type"/>.
	/// </summary>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Required by XML serialization")]
	public class RuntimeTypeXmlSerializer : IXmlSerializable
	{
		#region Operators

		public static implicit operator Type(RuntimeTypeXmlSerializer serializer)
		{
			return Type.GetType(serializer._assemblyQualifiedName, true);
		}

		public static implicit operator RuntimeTypeXmlSerializer(Type type)
		{
			return new RuntimeTypeXmlSerializer(type.AssemblyQualifiedName);
		}

		#endregion

		public RuntimeTypeXmlSerializer() { }

		private RuntimeTypeXmlSerializer(string assemblyQualifiedName)
		{
			_assemblyQualifiedName = assemblyQualifiedName;
		}

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			_assemblyQualifiedName = reader.ReadElementContentAsString();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteString(_assemblyQualifiedName);
		}

		#endregion

		private string _assemblyQualifiedName;
	}
}

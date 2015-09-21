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

using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.RuleEngine;

namespace Be.Stateless.BizTalk.Xml
{
	/// <summary>
	/// XML serializer surrogate that supports the serialization of <see cref="PolicyName"/>.
	/// </summary>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Required by XML serialization")]
	public class PolicyNameXmlSerializer : IXmlSerializable
	{
		#region Operators

		public static implicit operator PolicyName(PolicyNameXmlSerializer serializer)
		{
			return PolicyName.Parse(serializer._serializedPolicyName);
		}

		public static implicit operator PolicyNameXmlSerializer(PolicyName policyName)
		{
			return new PolicyNameXmlSerializer(policyName.ToString());
		}

		#endregion

		public PolicyNameXmlSerializer() { }

		private PolicyNameXmlSerializer(string serializedPolicyName)
		{
			_serializedPolicyName = serializedPolicyName;
		}

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			_serializedPolicyName = reader.ReadElementContentAsString();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteString(_serializedPolicyName);
		}

		#endregion

		private string _serializedPolicyName;
	}
}

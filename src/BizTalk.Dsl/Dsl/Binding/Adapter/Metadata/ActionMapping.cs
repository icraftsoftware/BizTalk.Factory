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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.BizTalk.Adapter.Wcf.Metadata;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter.Metadata
{
	/// <summary>
	/// Describes the BizTalk Server action mapping operations.
	/// </summary>
	public class ActionMapping : List<ActionMappingOperation>
	{
		#region Operators

		public static implicit operator string(ActionMapping actionMapping)
		{
			var btsActionMapping = new BtsActionMapping {
				Operation = actionMapping.Select(am => new BtsActionMappingOperation { Action = am.Action, Name = am.Name }).ToArray()
			};
			using (var stringWriter = new StringWriter())
			using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8, OmitXmlDeclaration = true }))
			{
				var serializer = new XmlSerializer(typeof(BtsActionMapping));
				var absorbXsdAndXsiXmlns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });
				serializer.Serialize(writer, btsActionMapping, absorbXsdAndXsiXmlns);
				return stringWriter.ToString();
			}
		}

		#endregion
	}
}

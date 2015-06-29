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

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Dsl.Binding.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	internal class AdapterBindingSerializer : IDslSerializer
	{
		public AdapterBindingSerializer(IAdapter adapter)
		{
			_adapter = adapter;
		}

		#region IDslSerializer Members

		public string Serialize()
		{
			var bindingProperties = GetBindingProperties();
			using (var stringWriter = new StringWriter())
			using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8, OmitXmlDeclaration = true }))
			{
				var serializer = new XmlSerializer(typeof(PropertyBag), new XmlRootAttribute("CustomProps"));
				var absorbXsdAndXsiXmlns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });
				serializer.Serialize(writer, bindingProperties, absorbXsdAndXsiXmlns);
				return stringWriter.ToString();
			}
		}

		public void Save(string filePath)
		{
			var bindingProperties = GetBindingProperties();
			using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
			using (var writer = XmlWriter.Create(file))
			{
				var serializer = new XmlSerializer(typeof(PropertyBag), new XmlRootAttribute("CustomProps"));
				serializer.Serialize(writer, bindingProperties);
			}
		}

		public void Write(Stream stream)
		{
			var bindingProperties = GetBindingProperties();
			using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8, OmitXmlDeclaration = true }))
			{
				var serializer = new XmlSerializer(typeof(PropertyBag), new XmlRootAttribute("CustomProps"));
				serializer.Serialize(writer, bindingProperties);
			}
		}

		#endregion

		private PropertyBag GetBindingProperties()
		{
			var bag = new PropertyBag();
			_adapter.Save(bag);
			return bag;
		}

		private readonly IAdapter _adapter;
	}
}

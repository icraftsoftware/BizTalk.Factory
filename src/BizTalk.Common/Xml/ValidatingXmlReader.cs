#region Copyright & License

// Copyright © 2012 - 2013 François Chabot, Yves Dierick
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
using System.Xml;
using System.Xml.Schema;
using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Xml
{
	public static class ValidatingXmlReader
	{
		public static XmlReader Create<T>(Stream input)
			where T : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T>());
		}

		public static XmlReader Create<T>(Stream input, XmlSchemaContentProcessing contentProcessing)
			where T : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T>(contentProcessing));
		}

		public static XmlReader Create<T>(string inputUri)
			where T : SchemaBase, new()
		{
			return XmlReader.Create(inputUri, ValidatingXmlReaderSettings.Create<T>());
		}

		public static XmlReader Create<T>(string inputUri, XmlSchemaContentProcessing contentProcessing)
			where T : SchemaBase, new()
		{
			return XmlReader.Create(inputUri, ValidatingXmlReaderSettings.Create<T>(contentProcessing));
		}

		public static XmlReader Create<T>(TextReader input)
			where T : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T>());
		}

		public static XmlReader Create<T>(TextReader input, XmlSchemaContentProcessing contentProcessing)
			where T : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T>(contentProcessing));
		}

		public static XmlReader Create<T>(XmlReader reader)
			where T : SchemaBase, new()
		{
			return XmlReader.Create(reader, ValidatingXmlReaderSettings.Create<T>());
		}

		public static XmlReader Create<T>(XmlReader reader, XmlSchemaContentProcessing contentProcessing)
			where T : SchemaBase, new()
		{
			return XmlReader.Create(reader, ValidatingXmlReaderSettings.Create<T>(contentProcessing));
		}

		public static XmlReader Create<T1, T2>(Stream input)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T1, T2>());
		}

		public static XmlReader Create<T1, T2>(Stream input, XmlSchemaContentProcessing contentProcessing)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T1, T2>(contentProcessing));
		}

		public static XmlReader Create<T1, T2>(string inputUri)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
		{
			return XmlReader.Create(inputUri, ValidatingXmlReaderSettings.Create<T1, T2>());
		}

		public static XmlReader Create<T1, T2>(string inputUri, XmlSchemaContentProcessing contentProcessing)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
		{
			return XmlReader.Create(inputUri, ValidatingXmlReaderSettings.Create<T1, T2>(contentProcessing));
		}

		public static XmlReader Create<T1, T2>(TextReader input)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T1, T2>());
		}

		public static XmlReader Create<T1, T2>(TextReader input, XmlSchemaContentProcessing contentProcessing)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T1, T2>(contentProcessing));
		}

		public static XmlReader Create<T1, T2>(XmlReader reader)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
		{
			return XmlReader.Create(reader, ValidatingXmlReaderSettings.Create<T1, T2>());
		}

		public static XmlReader Create<T1, T2>(XmlReader reader, XmlSchemaContentProcessing contentProcessing)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
		{
			return XmlReader.Create(reader, ValidatingXmlReaderSettings.Create<T1, T2>(contentProcessing));
		}

		public static XmlReader Create<T1, T2, T3>(Stream input)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T1, T2, T3>());
		}

		public static XmlReader Create<T1, T2, T3>(Stream input, XmlSchemaContentProcessing contentProcessing)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T1, T2, T3>(contentProcessing));
		}

		public static XmlReader Create<T1, T2, T3>(string inputUri)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			return XmlReader.Create(inputUri, ValidatingXmlReaderSettings.Create<T1, T2, T3>());
		}

		public static XmlReader Create<T1, T2, T3>(string inputUri, XmlSchemaContentProcessing contentProcessing)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			return XmlReader.Create(inputUri, ValidatingXmlReaderSettings.Create<T1, T2, T3>(contentProcessing));
		}

		public static XmlReader Create<T1, T2, T3>(TextReader input)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T1, T2, T3>());
		}

		public static XmlReader Create<T1, T2, T3>(TextReader input, XmlSchemaContentProcessing contentProcessing)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			return XmlReader.Create(input, ValidatingXmlReaderSettings.Create<T1, T2, T3>(contentProcessing));
		}

		public static XmlReader Create<T1, T2, T3>(XmlReader reader)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			return XmlReader.Create(reader, ValidatingXmlReaderSettings.Create<T1, T2, T3>());
		}

		public static XmlReader Create<T1, T2, T3>(XmlReader reader, XmlSchemaContentProcessing contentProcessing)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			return XmlReader.Create(reader, ValidatingXmlReaderSettings.Create<T1, T2, T3>(contentProcessing));
		}
	}
}

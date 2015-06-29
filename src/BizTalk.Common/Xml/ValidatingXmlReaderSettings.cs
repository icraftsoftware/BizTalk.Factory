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

using System.Xml;
using System.Xml.Schema;
using Microsoft.XLANGs.BaseTypes;
using ValidatingXmlReaderSettingsBase = Be.Stateless.Xml.ValidatingXmlReaderSettings;

namespace Be.Stateless.BizTalk.Xml
{
	public static class ValidatingXmlReaderSettings
	{
		public static XmlReaderSettings Create<T>() where T : SchemaBase, new()
		{
			return Create<T>(XmlSchemaContentProcessing.Strict);
		}

		public static XmlReaderSettings Create<T>(XmlSchemaContentProcessing contentProcessing) where T : SchemaBase, new()
		{
			return ValidatingXmlReaderSettingsBase.Create(contentProcessing, new T().CreateResolvedSchema());
		}

		public static XmlReaderSettings Create<T1, T2>()
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
		{
			return Create<T1, T2>(XmlSchemaContentProcessing.Strict);
		}

		public static XmlReaderSettings Create<T1, T2>(XmlSchemaContentProcessing contentProcessing)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
		{
			return ValidatingXmlReaderSettingsBase.Create(contentProcessing, new T1().CreateResolvedSchema(), new T2().CreateResolvedSchema());
		}

		public static XmlReaderSettings Create<T1, T2, T3>()
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			return Create<T1, T2, T3>(XmlSchemaContentProcessing.Strict);
		}

		public static XmlReaderSettings Create<T1, T2, T3>(XmlSchemaContentProcessing contentProcessing)
			where T1 : SchemaBase, new()
			where T2 : SchemaBase, new()
			where T3 : SchemaBase, new()
		{
			return ValidatingXmlReaderSettingsBase.Create(contentProcessing, new T1().CreateResolvedSchema(), new T2().CreateResolvedSchema(), new T3().CreateResolvedSchema());
		}
	}
}

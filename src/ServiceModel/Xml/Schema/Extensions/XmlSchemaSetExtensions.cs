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

using System.Linq;
using System.Xml.Schema;

namespace Be.Stateless.Xml.Schema.Extensions
{
	public static class XmlSchemaSetExtensions
	{
		/// <summary>
		/// Merge an <see cref="XmlSchema"/> into an <see cref="XmlSchemaSet"/>.
		/// </summary>
		/// <param name="schemaSet">
		/// The <see cref="XmlSchemaSet"/> into which the <paramref name="schema"/> will be merged.
		/// </param>
		/// <param name="schema">
		/// The <see cref="XmlSchema"/> to merge into the <paramref name="schemaSet"/>.
		/// </param>
		/// <returns>
		/// The <see cref="XmlSchema"/> to merge into.
		/// </returns>
		public static XmlSchema Merge(this XmlSchemaSet schemaSet, XmlSchema schema)
		{
			// only support XmlSchemaImport so far but Cast<XmlSchemaImport>() will throw otherwise
			var knownSchemaImports = schema.Includes.Cast<XmlSchemaImport>()
				.Where(xsi => schemaSet.Contains(xsi.Namespace));
			foreach (var xmlSchemaImport in knownSchemaImports)
			{
				var existingSchema = schemaSet.Schemas(xmlSchemaImport.Namespace).Cast<XmlSchema>().Single();
				existingSchema.Merge(xmlSchemaImport.Schema);
				schemaSet.Reprocess(existingSchema);
			}

			// only support XmlSchemaImport so far but Cast<XmlSchemaImport>() will throw otherwise
			var unknownSchemaImports = schema.Includes.Cast<XmlSchemaImport>()
				.Where(xsi => !schemaSet.Contains(xsi.Namespace));
			foreach (var xmlSchemaImport in unknownSchemaImports)
			{
				schemaSet.Add(xmlSchemaImport.Schema);
			}

			if (schemaSet.Contains(schema.TargetNamespace))
			{
				var existingSchema = schemaSet.Schemas(schema.TargetNamespace).Cast<XmlSchema>().Single();
				existingSchema.Merge(schema);
				schemaSet.Reprocess(existingSchema);
			}
			else
			{
				schemaSet.Add(schema);
			}

			return schema;
		}
	}
}

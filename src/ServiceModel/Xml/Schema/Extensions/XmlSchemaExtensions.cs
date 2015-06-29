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

using System;
using System.Xml.Schema;

namespace Be.Stateless.Xml.Schema.Extensions
{
	public static class XmlSchemaExtensions
	{
		/// <summary>
		/// Merge an <see cref="XmlSchema"/> into another <see cref="XmlSchema"/>.
		/// </summary>
		/// <param name="existingSchema">
		/// The <see cref="XmlSchema"/> into which the other <see cref="XmlSchema"/> will be merged.
		/// </param>
		/// <param name="schema">
		/// The <see cref="XmlSchema"/> to be merged into the other <see cref="XmlSchema"/>.
		/// </param>
		/// <returns>
		/// The resulting <see cref="XmlSchema"/>; i.e. the <paramref name="existingSchema"/> into which the other
		/// <paramref name="schema"/> has been merged.
		/// </returns>
		/// <see href="http://stackoverflow.com/questions/6312154/xmlschema-removing-duplicate-types" />
		public static XmlSchema Merge(this XmlSchema existingSchema, XmlSchema schema)
		{
			foreach (var item in schema.Items)
			{
				if (item is XmlSchemaType)
				{
					var type = item as XmlSchemaType;
					if (!existingSchema.SchemaTypes.Contains(type.QualifiedName)) existingSchema.Items.Add(item);
				}
				else if (item is XmlSchemaElement)
				{
					var element = item as XmlSchemaElement;
					if (!existingSchema.Elements.Contains(element.QualifiedName)) existingSchema.Items.Add(item);
				}
				else if (item is XmlSchemaAnnotation)
				{
					// ignore XmlSchemaAnnotation for merge operations
				}
				else
				{
					throw new InvalidOperationException(item.GetType().Name);
				}
			}
			return existingSchema;
		}
	}
}

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

using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.ServiceModel.Channels;

namespace Be.Stateless.Xml.Schema
{
	/// <summary>
	/// Scaffolding interface meant to help an <see cref="XmlMessage{TSchemaProvider}"/>-derived class to either get the
	/// <see cref="XmlSchema"/> against which to validate its XML representation, or fulfill its contract with respect to
	/// the <see cref="XmlSchemaProviderAttribute"/>.
	/// </summary>
	/// <remarks>
	/// Provides the <see cref="XmlSchema"/> to the <see cref="XmlSchemaSet"/> and the <see cref="XmlSchemaType"/> that
	/// controls the serialization of the type to the .
	/// </remarks>
	public interface IXmlSchemaProvider
	{
		/// <summary>
		/// The <see cref="XmlSchema"/> against which to validate its XML representation.
		/// </summary>
		XmlSchema Schema { get; }

		/// <summary>
		/// Provides the <see cref="XmlSchema"/> to the <paramref name="schemaSet"/> and the <see cref="XmlSchemaType"/>
		/// that controls the serialization of the type.
		/// </summary>
		/// <param name="schemaSet">
		/// The <see cref="XmlSchemaSet"/> that will be populated with the <see cref="XmlSchema"/>.
		/// </param>
		/// <returns>
		/// The <see cref="XmlSchemaType"/> that defines its data type.
		/// </returns>
		/// <remarks>
		/// This is a scaffolding method that is meant to be called from within the derived classes' static method
		/// identified by the <see cref="XmlSchemaProviderAttribute"/>.
		/// </remarks>
		XmlSchemaType ProvideSchema(XmlSchemaSet schemaSet);
	}
}

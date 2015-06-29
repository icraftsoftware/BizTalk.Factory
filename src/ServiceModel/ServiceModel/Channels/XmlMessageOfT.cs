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

using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.Xml;
using Be.Stateless.Xml.Schema;

namespace Be.Stateless.ServiceModel.Channels
{
	/// <summary>
	/// Represents an <see cref="IXmlSerializable"/> unit of communication between endpoints in a distributed environment
	/// whose content is defined by an <see cref="XmlSchema"/> that a <typeparamref name="TSchemaProvider"/> can provide.
	/// </summary>
	/// <remarks>
	/// This class helps its derived classes to fulfill the <see cref="XmlSchemaProviderAttribute"/>'s contract, see <see
	/// cref="ProvideSchema"/>.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// [XmlSchemaProvider("GetSchema")]
	/// public class CalculatorArguments : XmlMessage<CalculatorMessageSchema.Arguments>
	/// {
	///    public new static XmlSchemaType GetSchema(XmlSchemaSet schemaSet)
	///    {
	///       return ProvideSchema(schemaSet);
	///    }
	/// 
	///    ...
	/// 
	/// }
	/// ]]></code>
	/// </example>
	public abstract class XmlMessage<TSchemaProvider> : XmlMessage
		where TSchemaProvider : IXmlSchemaProvider, new()
	{
		/// <summary>
		/// Scaffolding method that helps a derived class to fulfill its <see cref="XmlSchemaProviderAttribute"/>'s
		/// contract in providing the <see cref="XmlSchema"/> to the <paramref name="schemaSet"/> and the <see
		/// cref="XmlSchemaType"/> that controls the serialization of the type.
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
		/// <example>
		/// <code><![CDATA[
		/// [XmlSchemaProvider("GetSchema")]
		/// public class CalculatorArguments : XmlMessage<CalculatorMessageSchema.Arguments>
		/// {
		///    public new static XmlSchemaType GetSchema(XmlSchemaSet schemaSet)
		///    {
		///       return ProvideSchema(schemaSet);
		///    }
		/// 
		///    ...
		/// 
		/// }
		/// ]]></code>
		/// </example>
		protected static XmlSchemaType ProvideSchema(XmlSchemaSet schemaSet)
		{
			var schemaProvider = new TSchemaProvider();
			return schemaProvider.ProvideSchema(schemaSet);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlMessage{TSchemaProvider}"/> class that will skip XML
		/// validation.
		/// </summary>
		protected XmlMessage() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlMessage{TSchemaProvider}"/> class that will validate its XML
		/// representation.
		/// </summary>
		/// <param name="schemaContentProcessing">
		/// The <see cref="XmlSchemaContentProcessing"/>, or validation strictness, that will be applied when validating
		/// the XML representation.
		/// </param>
		/// <seealso cref="ValidatingXmlReaderSettings.Create"/>
		/// <seealso cref="XmlSchemaContentProcessing"/>
		protected XmlMessage(XmlSchemaContentProcessing schemaContentProcessing)
		{
			if (_validatingSchema == null) Interlocked.CompareExchange(ref _validatingSchema, new TSchemaProvider().Schema, null);
			_schemaContentProcessing = schemaContentProcessing;
		}

		#region Base Class Member Overrides

		/// <summary>
		/// Generates an object from its XML representation that will be validated while being read if validation has been
		/// enforced at construction time.
		/// </summary>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> stream from which the object is deserialized.
		/// </param>
		public override void ReadXml(XmlReader reader)
		{
			if (_validatingSchema != null) reader = CreateValidatingXmlReader(reader);
			base.ReadXml(reader);
		}

		#endregion

		#region Helpers

		private XmlReader CreateValidatingXmlReader(XmlReader reader)
		{
			var validatingXmlReader = XmlReader.Create(reader.ReadSubtree(), ValidatingXmlReaderSettings.Create(_schemaContentProcessing, _validatingSchema));
			validatingXmlReader.MoveToContent();
			return validatingXmlReader;
		}

		#endregion

		// ReSharper disable StaticFieldInGenericType
		private static XmlSchema _validatingSchema;
		// ReSharper restore StaticFieldInGenericType
		private readonly XmlSchemaContentProcessing _schemaContentProcessing;
	}
}

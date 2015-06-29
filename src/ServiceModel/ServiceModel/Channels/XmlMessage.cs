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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Be.Stateless.ServiceModel.Channels
{
	/// <summary>
	/// Represents an <see cref="IXmlSerializable"/> unit of communication between endpoints in a distributed
	/// environment.
	/// </summary>
	public class XmlMessage : IXmlSerializable
	{
		#region IXmlSerializable Members

		/// <summary>
		/// This method is reserved and should not be used. When implementing the <see cref="IXmlSerializable"/>
		/// interface, you should return <c>null</c> from this method, and instead, if specifying a custom schema is
		/// required, apply the <see cref="XmlSchemaProviderAttribute"/> to the class.
		/// </summary>
		/// <returns>
		/// An <see cref="XmlSchema"/> that describes the XML representation of the object that is produced by the <see
		/// cref="IXmlSerializable.WriteXml(XmlWriter)"/> method and consumed by the <see
		/// cref="IXmlSerializable.ReadXml(XmlReader)"/> method.
		/// </returns>
		public virtual XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> stream from which the object is deserialized.
		/// </param>
		public virtual void ReadXml(XmlReader reader)
		{
			RawXmlBody = reader.ReadOuterXml();
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">
		/// The <see cref="XmlWriter"/> stream to which the object is serialized.
		/// </param>
		/// <remarks>
		/// Because the intent is to work at the XML level of messages complying to a message contract (by opposition to a
		/// DataContract), where we want to keep control on the whole XML, this method skips the root XML element as the
		/// actual root element surrounding the response <see cref="XmlMessage"/> will be provided by WCF.
		/// </remarks>
		public virtual void WriteXml(XmlWriter writer)
		{
			using (var xmlReader = GetReaderAtContent())
			{
				xmlReader.ReadStartElement();
				// see http://blogs.msdn.com/b/wifry/archive/2007/05/15/wcf-bodywriter-and-raw-xml-problems.aspx
				while (xmlReader.NodeType != XmlNodeType.EndElement)
				{
					writer.WriteNode(xmlReader, true);
				}
			}
		}

		#endregion

		/// <summary>
		/// Gets a description of how the message should be processed.
		/// </summary>
		public virtual string Action
		{
			get { return OperationContext.Current.IncomingMessageHeaders.Action; }
		}

		/// <summary>
		/// Gets the SOAP version of the message.
		/// </summary>
		public virtual MessageVersion Version
		{
			get { return OperationContext.Current.IncomingMessageVersion; }
		}

		protected internal string RawXmlBody { get; protected set; }

		/// <summary>
		/// Gets the <see cref="XmlReader"/> that accesses the body of this message.
		/// </summary>
		/// <returns>
		/// An <see cref="XmlReader"/>.
		/// </returns>
		public virtual XmlReader GetReaderAtContent()
		{
			var xmlReader = XmlReader.Create(new StringReader(RawXmlBody), new XmlReaderSettings { CloseInput = true });
			xmlReader.MoveToContent();
			return xmlReader;
		}
	}
}

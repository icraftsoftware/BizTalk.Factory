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
using System.Xml.Serialization;

namespace Be.Stateless.ServiceModel.Channels
{
	/// <summary>
	/// Represents an empty <see cref="IXmlSerializable"/> unit of communication between endpoints in a distributed
	/// environment.
	/// </summary>
	public class EmptyXmlMessage : XmlMessage
	{
		#region Base Class Member Overrides

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">
		/// The <see cref="XmlReader"/> stream from which the object is deserialized.
		/// </param>
		public override void ReadXml(XmlReader reader) { }

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
		public override void WriteXml(XmlWriter writer) { }

		#endregion
	}
}

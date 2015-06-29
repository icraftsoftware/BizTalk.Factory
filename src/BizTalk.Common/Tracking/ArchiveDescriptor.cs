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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Extensions;

namespace Be.Stateless.BizTalk.Tracking
{
	/// <summary>
	/// Describes how and where a message payload stream will be archived.
	/// </summary>
	public class ArchiveDescriptor : IXmlSerializable
	{
		public static ArchiveDescriptor Create(XmlReader reader)
		{
			var descriptor = new ArchiveDescriptor();
			descriptor.ReadXml(reader);
			return descriptor;
		}

		private ArchiveDescriptor() { }

		public ArchiveDescriptor(string source, string target)
		{
			if (source.IsNullOrEmpty()) throw new ArgumentNullException("source");
			if (target.IsNullOrEmpty()) throw new ArgumentNullException("target");
			Source = source;
			Target = target;
		}

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			throw new NotSupportedException();
		}

		public void ReadXml(XmlReader reader)
		{
			reader.AssertStartElement(typeof(ArchiveDescriptor).Name);
			Source = reader.GetMandatoryAttribute("source");
			Target = reader.GetMandatoryAttribute("target");
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement(typeof(ArchiveDescriptor).Name);
			writer.WriteAttributeString("source", Source);
			writer.WriteAttributeString("target", Target);
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
		}

		#endregion

		/// <summary>
		/// Source location of the archive payload, typically a entry in the central claim store, that is a capture of message payload.
		/// </summary>
		public string Source { get; private set; }

		/// <summary>
		/// Target location of the archive, that is where to bring the payload found at <see cref="Source"/> to.
		/// </summary>
		public string Target { get; private set; }
	}
}

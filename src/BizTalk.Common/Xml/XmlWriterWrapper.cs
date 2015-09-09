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

using System;
using System.Xml;

namespace Be.Stateless.BizTalk.Xml
{
	/// <summary>
	/// <see cref="XmlWriter"/> wrapper that delegates all operations to the wrapped <see cref="XmlWriter"/>-derived
	/// instance.
	/// </summary>
	public abstract class XmlWriterWrapper : XmlWriter
	{
		protected XmlWriterWrapper(XmlWriter writer)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			_writer = writer;
		}

		#region Base Class Member Overrides

		public override void Flush()
		{
			_writer.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			return _writer.LookupPrefix(ns);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			_writer.WriteBase64(buffer, index, count);
		}

		public override void WriteCData(string text)
		{
			_writer.WriteCData(text);
		}

		public override void WriteCharEntity(char ch)
		{
			_writer.WriteCharEntity(ch);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			_writer.WriteChars(buffer, index, count);
		}

		public override void WriteComment(string text)
		{
			_writer.WriteComment(text);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			_writer.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteEndAttribute()
		{
			_writer.WriteEndAttribute();
		}

		public override void WriteEndDocument()
		{
			_writer.WriteEndDocument();
		}

		public override void WriteEndElement()
		{
			_writer.WriteEndElement();
		}

		public override void WriteEntityRef(string name)
		{
			_writer.WriteEntityRef(name);
		}

		public override void WriteFullEndElement()
		{
			_writer.WriteFullEndElement();
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			_writer.WriteProcessingInstruction(name, text);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			_writer.WriteRaw(buffer, index, count);
		}

		public override void WriteRaw(string data)
		{
			_writer.WriteRaw(data);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			_writer.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteStartDocument(bool standalone)
		{
			_writer.WriteStartDocument(standalone);
		}

		public override void WriteStartDocument()
		{
			_writer.WriteStartDocument();
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			_writer.WriteStartElement(prefix, localName, ns);
		}

		public override WriteState WriteState
		{
			get { return _writer.WriteState; }
		}

		public override void WriteString(string text)
		{
			_writer.WriteString(text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			_writer.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteWhitespace(string ws)
		{
			_writer.WriteWhitespace(ws);
		}

		#endregion

		public XmlWriter InnerWriter
		{
			get { return _writer; }
		}

		private readonly XmlWriter _writer;
	}
}

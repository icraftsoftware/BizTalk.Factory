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
using System.Text;
using System.Xml;

namespace Be.Stateless.Xml.Builder
{
	/// <summary>
	/// Spy that intercepts and records all the invocations of the <see cref="XmlReader"/>'s overridden abstract methods
	/// so as to be able to compare them to another recording.
	/// </summary>
	public class XmlReaderSpy : XmlReader
	{
		public XmlReaderSpy(XmlReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			_reader = reader;
		}

		#region Base Class Member Overrides

		public override int AttributeCount
		{
			get
			{
				var result = _reader.AttributeCount;
				_invocations.AppendFormat("get_AttributeCount() = {0}\r\n", result);
				return result;
			}
		}

		public override string BaseURI
		{
			get
			{
				var result = _reader.BaseURI;
				_invocations.AppendFormat("get_BaseURI() = {0}\r\n", result);
				return result;
			}
		}

		public override int Depth
		{
			get
			{
				var result = _reader.Depth;
				_invocations.AppendFormat("get_Depth() = {0}\r\n", result);
				return result;
			}
		}

		public override bool EOF
		{
			get
			{
				var result = _reader.EOF;
				_invocations.AppendFormat("get_EOF() = {0}\r\n", result);
				return result;
			}
		}

		public override bool HasValue
		{
			get
			{
				var result = _reader.HasValue;
				_invocations.AppendFormat("get_HasValue() = {0}\r\n", result);
				return result;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				var result = _reader.IsEmptyElement;
				_invocations.AppendFormat("get_IsEmptyElement() = {0}\r\n", result);
				return result;
			}
		}

		public override string LocalName
		{
			get
			{
				var result = _reader.LocalName;
				_invocations.AppendFormat("get_LocalName() = {0}\r\n", result);
				return result;
			}
		}

		public override XmlNameTable NameTable
		{
			get { return _reader.NameTable; }
		}

		public override string NamespaceURI
		{
			get
			{
				var result = _reader.NamespaceURI;
				_invocations.AppendFormat("get_NamespaceURI() = {0}\r\n", result);
				return result;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				var result = _reader.NodeType;
				_invocations.AppendFormat("get_NodeType() = {0}\r\n", result);
				return result;
			}
		}

		public override string Prefix
		{
			get
			{
				var result = _reader.Prefix;
				_invocations.AppendFormat("get_Prefix() = {0}\r\n", result);
				return result;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				var result = _reader.ReadState;
				_invocations.AppendFormat("get_ReadState() = {0}\r\n", result);
				return result;
			}
		}

		public override string Value
		{
			get
			{
				var result = _reader.Value;
				_invocations.AppendFormat("get_Value() = {0}\r\n", result);
				return result;
			}
		}

		public override void Close()
		{
			_invocations.Append("Close()\r\n");
			_reader.Close();
		}

		public override string GetAttribute(string name)
		{
			var result = _reader.GetAttribute(name);
			_invocations.AppendFormat("GetAttribute(name:={0}) = {1}\r\n", name, result);
			return result;
		}

		public override string GetAttribute(string name, string ns)
		{
			var result = _reader.GetAttribute(name, ns);
			_invocations.AppendFormat("GetAttribute(name:={0}, ns:={1}) = {2}\r\n", name, ns, result);
			return result;
		}

		public override string GetAttribute(int i)
		{
			var result = _reader.GetAttribute(i);
			_invocations.AppendFormat("GetAttribute(i:={0}) = {1}\r\n", i, result);
			return result;
		}

		public override string LookupNamespace(string prefix)
		{
			var result = _reader.LookupNamespace(prefix);
			_invocations.AppendFormat("LookupNamespace(prefix:={0}) = {1}\r\n", prefix, result);
			return result;
		}

		public override bool MoveToAttribute(string name)
		{
			var result = _reader.MoveToAttribute(name);
			_invocations.AppendFormat("MoveToAttribute(name:={0}) = {1}\r\n", name, result);
			return result;
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			var result = _reader.MoveToAttribute(name, ns);
			_invocations.AppendFormat("MoveToAttribute(name:={0}, ns:={1}) = {2}\r\n", name, ns, result);
			return result;
		}

		public override bool MoveToElement()
		{
			var result = _reader.MoveToElement();
			_invocations.AppendFormat("MoveToElement() = {0}\r\n", result);
			return result;
		}

		public override bool MoveToFirstAttribute()
		{
			var result = _reader.MoveToFirstAttribute();
			_invocations.AppendFormat("MoveToFirstAttribute() = {0}\r\n", result);
			return result;
		}

		public override bool MoveToNextAttribute()
		{
			var result = _reader.MoveToNextAttribute();
			_invocations.AppendFormat("MoveToNextAttribute() = {0}\r\n", result);
			return result;
		}

		public override bool Read()
		{
			var result = _reader.Read();
			_invocations.AppendFormat("Read() = {0}\r\n", result);
			return result;
		}

		public override bool ReadAttributeValue()
		{
			var result = _reader.ReadAttributeValue();
			_invocations.AppendFormat("ReadAttributeValue() = {0}\r\n", result);
			return result;
		}

		public override void ResolveEntity()
		{
			_invocations.Append("ResolveEntity()\r\n");
			_reader.ResolveEntity();
		}

		#endregion

		/// <summary>
		/// All of the invocations that have been performed against the <see cref="XmlReaderSpy"/> since it has been
		/// created.
		/// </summary>
		public string Invocations
		{
			get { return _invocations.ToString(); }
		}

		private readonly StringBuilder _invocations = new StringBuilder();
		private readonly XmlReader _reader;
	}
}
